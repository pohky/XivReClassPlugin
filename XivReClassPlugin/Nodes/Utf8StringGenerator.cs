using ReClassNET.CodeGenerator;
using ReClassNET.Logger;
using ReClassNET.Nodes;

namespace XivReClassPlugin.Nodes {
    public class Utf8StringGenerator : CustomCppCodeGenerator {
        public override bool CanHandle(BaseNode node) => node is Utf8StringNode;

        public override BaseNode TransformNode(BaseNode node) => node;

        public override string GetTypeDefinition(BaseNode node, GetTypeDefinitionFunc defaultGetTypeDefinitionFunc, ResolveWrappedTypeFunc defaultResolveWrappedTypeFunc, ILogger logger) {
            return "Client::System::String::Utf8String";
        }
    }
}