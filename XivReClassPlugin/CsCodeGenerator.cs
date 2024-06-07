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

namespace XivReClassPlugin; 

public class CsCodeGenerator : ICodeGenerator {
	private static readonly Dictionary<Type, string> NodeTypeToTypeMap = new() {
		[typeof(Hex8Node)] = "byte",
		[typeof(Hex16Node)] = "ushort",
		[typeof(Hex32Node)] = "uint",
		[typeof(Hex64Node)] = "ulong",

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

		[typeof(FunctionPtrNode)] = "nint",
		[typeof(PointerNode)] = "void*",
		[typeof(VirtualMethodTableNode)] = "void**",

		[typeof(Vector2Node)] = "Vector2",
		[typeof(Vector3Node)] = "Vector3",
		[typeof(Vector4Node)] = "Vector4",
		[typeof(Matrix4x4Node)] = "Matrix44",

		[typeof(Utf8StringNode)] = "Utf8String",
		[typeof(AtkValueNode)] = "AtkValue"
	};

	public Language Language => Language.CSharp;

	public string GenerateCode(IReadOnlyList<ClassNode> classes, IReadOnlyList<EnumDescription> enums, ILogger logger) {
		using var sw = new StringWriter();
        using var iw = new IndentedTextWriter(sw, new string(' ', 4));

        // just to make sure it's using Numerics types if the entire thing is copied
		iw.WriteLine("using System.Numerics;");
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

        if (classNode.Nodes.Any(NodeNeedsInterop))
            writer.WriteLine("[GenerateInterop]");

		writer.WriteLine($"[StructLayout(LayoutKind.Explicit, Size = 0x{classNode.MemorySize:X2})]");
		writer.WriteLine($"public unsafe partial struct {classNode.Name} {{");

		writer.Indent++;

		var nodes = classNode.Nodes.Where(n => n is not FunctionNode and not BaseHexNode);
		foreach (var node in nodes) {
			var (type, attribute) = GetTypeDefinition(node);
			if (type != null) {
				if (attribute != null)
					writer.WriteLine(attribute);

                writer.Write($"[FieldOffset(0x{node.Offset:X2})] public {type} {node.Name};");
                if (!string.IsNullOrEmpty(node.Comment))
                    writer.Write($" // {node.Comment}");

				writer.WriteLine();
				continue;
			}

            if (node is ArrayNode arrNode) {
                (type, _) = GetTypeDefinition(arrNode.InnerNode);
                if (type == null) {
                    writer.WriteLine($"// 0x{node.Offset:X2} - Unhandled Array Node: {arrNode.InnerNode.GetType().Name}[] {arrNode.Name}");
                    continue;
                }
                writer.WriteLine($"[FieldOffset(0x{node.Offset:X2}), FixedSizeArray] internal FixedSizeArray{arrNode.Count}<{type}> {MakeFixedArrayName(arrNode)};");
            } else if (node is Utf8TextNode tn8) {
                writer.Write($"[FieldOffset(0x{node.Offset:X2}), FixedSizeArray(isString: true)] internal ");
                writer.WriteLine($"FixedSizeArray{tn8.Length}<byte> _{MakeFixedArrayName(node)};");
            } else if (node is Utf16TextNode tn16) {
                writer.Write($"[FieldOffset(0x{node.Offset:X2}), FixedSizeArray(isString: true)] internal ");
                writer.WriteLine($"FixedSizeArray{tn16.Length}<char> _{MakeFixedArrayName(node)};");
            } else if (node is Utf32TextNode tn32) {
                writer.Write($"[FieldOffset(0x{node.Offset:X2}), FixedSizeArray(isString: true)] internal ");
                writer.WriteLine($"FixedSizeArray{tn32.Length}<int> _{MakeFixedArrayName(node)};");
            } else {
                logger.Log(LogLevel.Warning, $"Skipping node with unhandled type: 0x{node.Offset:X2} {node.GetType()}");
            }
        }

		writer.Indent--;
		writer.WriteLine("}");
	}

    private static string MakeFixedArrayName(BaseNode node) {
        if (string.IsNullOrWhiteSpace(node.Name)) return $"_node{node.Offset:X2}";
        if (node.Name.Length is >= 1 and <= 3) return $"_{node.Name.ToLower()}";
        return $"_{char.ToLower(node.Name[0])}{node.Name.Substring(1)}";
    }

    private static bool NodeNeedsInterop(BaseNode node) {
        return node is ArrayNode or Utf8TextNode or Utf16TextNode or Utf32TextNode;
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
			case PointerNode { IsWrapped: false} pnRaw:
                if (pnRaw.InnerNode is ClassInstanceNode rawCin)
                    return ($"{rawCin.InnerNode.Name}*", null);
                var rawType = GetTypeDefinition(pnRaw.InnerNode);
                return rawType.typeName == null ? (null, null) : ($"{rawType.typeName}*", null);
            case PointerNode { IsWrapped: true} pnWrap:
                var wrapType = GetTypeDefinition(pnWrap.InnerNode);
                return wrapType.typeName == null ? (null, null) : ($"Pointer<{wrapType.typeName}>", null);
			case VectorNode vector: {
				if (vector.InnerNode is ClassInstanceNode cin)
					return ($"StdVector<{cin.InnerNode.Name}>", null);
                var vectorType = GetTypeDefinition(vector.InnerNode);
                return vectorType.typeName == null ? (null, null) : ($"StdVector<{vectorType.typeName}>", null);
            }
		}

		if (NodeTypeToTypeMap.TryGetValue(node.GetType(), out var type))
			return (type, null);

		return node switch {
			EnumNode enumNode => (enumNode.Enum.Name, null),
			_ => (null, null)
		};
	}
}