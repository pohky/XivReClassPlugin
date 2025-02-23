using ReClassNET.CodeGenerator;
using ReClassNET.Logger;
using ReClassNET.Nodes;

namespace XivReClassPlugin.Nodes; 

public class XivNodeGenerator : CustomCppCodeGenerator {
	public override bool CanHandle(BaseNode node) {
		return node switch {
			Utf8StringNode => true,
			StdVectorNode => true,
			AtkValueNode => true,
			_ => false
		};
	}
		
	public override string GetTypeDefinition(BaseNode node, GetTypeDefinitionFunc defaultGetTypeDefinitionFunc, ResolveWrappedTypeFunc defaultResolveWrappedTypeFunc, ILogger logger) {
		return node switch {
			Utf8StringNode => "Client::System::String::Utf8String",
			StdVectorNode vn => $"std::vector<{vn.InnerNode.Name}>",
			AtkValueNode => "Component::GUI::AtkValue",
			_ => $"/* Invalid Node Type ({node}) */ void"
		};
	}
}