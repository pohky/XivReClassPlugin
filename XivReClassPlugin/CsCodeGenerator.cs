using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReClassNET.CodeGenerator;
using ReClassNET.Logger;
using ReClassNET.Nodes;
using ReClassNET.Project;
using XivReClassPlugin.Nodes;
using ICodeGenerator = ReClassNET.CodeGenerator.ICodeGenerator;

namespace XivReClassPlugin {
    public class CsCodeGenerator : ICodeGenerator {
        private static readonly Dictionary<Type, string> NodeTypeToTypeMap = new() {
            [typeof(DoubleNode)] = "double",
            [typeof(FloatNode)] = "float",
            [typeof(BoolNode)] = "byte",
            [typeof(Int8Node)] = "sbyte",
            [typeof(Int16Node)] = "short",
            [typeof(Int32Node)] = "int",
            [typeof(Int64Node)] = "long",
            [typeof(NIntNode)] = "nint",
            [typeof(UInt8Node)] = "byte",
            [typeof(UInt16Node)] = "ushort",
            [typeof(UInt32Node)] = "uint",
            [typeof(UInt64Node)] = "ulong",
            [typeof(NUIntNode)] = "nuint",

            [typeof(Utf8TextPtrNode)] = "byte*",
            [typeof(Utf16TextPtrNode)] = "char*",
            [typeof(Utf32TextPtrNode)] = "int*",
            [typeof(Utf8TextNode)] = "fixed byte",
            [typeof(Utf16TextNode)] = "fixed char",
            [typeof(Utf32TextNode)] = "fixed int",

            [typeof(FunctionPtrNode)] = "nint",
            [typeof(PointerNode)] = "void*",
            [typeof(VirtualMethodTableNode)] = "void**",

            [typeof(Vector2Node)] = "Vector2",
            [typeof(Vector3Node)] = "Vector3",
            [typeof(Vector4Node)] = "Vector4",
            [typeof(Matrix4x4Node)] = "Matrix44",

            [typeof(Utf8StringNode)] = "Utf8String"
        };

        public Language Language => Language.CSharp;

        public string GenerateCode(IReadOnlyList<ClassNode> classes, IReadOnlyList<EnumDescription> enums, ILogger logger) {
            using var sw = new StringWriter();
            using var iw = new IndentedTextWriter(sw, "\t");

            iw.WriteLine("using System;");
            iw.WriteLine("using System.Numerics;");
            iw.WriteLine("using System.Runtime.InteropServices;");
            iw.WriteLine("using System.Runtime.CompilerServices;");
            iw.WriteLine("using FFXIVClientStructs.FFXIV.Client.System.String;");
            iw.WriteLine();

            foreach (var enumDef in enums) {
                WriteEnum(iw, enumDef);
                iw.WriteLine();
            }

            foreach (var classNode in classes.Where(c => !c.Nodes.Any(n => n is FunctionNode)).Distinct()) {
                WriteClass(iw, classNode, logger);
                iw.WriteLine();
            }

            return sw.ToString();
        }

        private static void WriteEnum(IndentedTextWriter writer, EnumDescription enumDef) {
            writer.Write($"public enum {enumDef.Name} : ");
            switch (enumDef.Size) {
                case EnumDescription.UnderlyingTypeSize.OneByte:
                    writer.Write(NodeTypeToTypeMap[typeof(UInt8Node)]);
                    break;
                case EnumDescription.UnderlyingTypeSize.TwoBytes:
                    writer.Write(NodeTypeToTypeMap[typeof(Int16Node)]);
                    break;
                case EnumDescription.UnderlyingTypeSize.FourBytes:
                    writer.Write(NodeTypeToTypeMap[typeof(Int32Node)]);
                    break;
                case EnumDescription.UnderlyingTypeSize.EightBytes:
                    writer.Write(NodeTypeToTypeMap[typeof(Int64Node)]);
                    break;
                default:
                    writer.Write(NodeTypeToTypeMap[typeof(Int32Node)]);
                    break;
            }

            writer.WriteLine(" {");
            writer.Indent++;
            for (var j = 0; j < enumDef.Values.Count; ++j) {
                var kv = enumDef.Values[j];

                writer.Write(kv.Key);
                writer.Write(" = ");
                writer.Write(kv.Value);
                if (j < enumDef.Values.Count - 1)
                    writer.Write(",");
                writer.WriteLine();
            }

            writer.Indent--;
            writer.WriteLine("}");
        }

        private static void WriteClass(IndentedTextWriter writer, ClassNode classNode, ILogger logger) {
            if (!string.IsNullOrEmpty(classNode.Comment))
                writer.WriteLine($"// {classNode.Comment}");

            writer.WriteLine($"[StructLayout(LayoutKind.Explicit, Size = 0x{classNode.MemorySize:X2})]");
            writer.WriteLine($"public unsafe struct {classNode.Name} {{");
            
            writer.Indent++;

            var nodes = classNode.Nodes.Where(n => n is not FunctionNode and not BaseHexNode);
            foreach (var node in nodes) {
                var (type, attribute) = GetTypeDefinition(node);
                if (type != null) {
                    if (attribute != null)
                        writer.WriteLine(attribute);

                    if (node is BaseTextNode tn && type.Contains("fixed"))
                        writer.Write($"[FieldOffset(0x{node.Offset:X2})] public {type} {node.Name}[{tn.Length}]; // string");
                    else {
                        writer.Write($"[FieldOffset(0x{node.Offset:X2})] public {type} {node.Name};");
                        if (!string.IsNullOrEmpty(node.Comment))
                            writer.Write($" // {node.Comment}");
                    }

                    writer.WriteLine();
                } else if (node is ArrayNode arrNode) {
                    if (arrNode.InnerNode is ClassInstanceNode instanceNode) {
                        writer.Write($"[FieldOffset(0x{node.Offset:X2})] public fixed byte {node.Name}[{arrNode.Count} * 0x{instanceNode.MemorySize:X2}];");
                        writer.Write($" // {arrNode.Count} * {instanceNode.InnerNode.Name}");
                    } else {
                        if (arrNode.InnerNode is Utf8StringNode utf8) {
                            writer.Write($"[FieldOffset(0x{node.Offset:X2})] public fixed byte {node.Name}[{arrNode.Count} * 0x{utf8.MemorySize:X2}];");
                            writer.Write($" // {arrNode.Count} * Utf8String");
                        } else {
                            (type, _) = GetTypeDefinition(arrNode.InnerNode);
                            if (type != null)
                                writer.Write($"[FieldOffset(0x{node.Offset:X2})] public fixed {type} {node.Name}[{arrNode.Count}];");
                        }
                    }
                    writer.WriteLine();
                } else {
                    logger.Log(LogLevel.Warning, $"Skipping node with unhandled type: {node.GetType()}");
                }
            }

            writer.Indent--;
            writer.WriteLine("}");
        }

        private static (string? typeName, string? attribute) GetTypeDefinition(BaseNode node) {
            if (node is BitFieldNode bitFieldNode) {
                var underlayingNode = bitFieldNode.GetUnderlayingNode();
                underlayingNode.CopyFromNode(node);
                node = underlayingNode;
            }

            switch (node) {
                case ClassInstanceNode instanceNode:
                    return ($"{instanceNode.InnerNode.Name}", null);
                case PointerNode {InnerNode: ClassInstanceNode cin}:
                    return ($"{cin.InnerNode.Name}*", null);
            }

            if (NodeTypeToTypeMap.TryGetValue(node.GetType(), out var type))
                return (type, null);

            return node switch {
                EnumNode enumNode => (enumNode.Enum.Name, null),
                _ => (null, null)
            };
        }
    }
}