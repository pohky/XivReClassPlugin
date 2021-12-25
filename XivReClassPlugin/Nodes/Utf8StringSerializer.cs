using System.Collections.Generic;
using System.Xml.Linq;
using ReClassNET.DataExchange.ReClass;
using ReClassNET.Logger;
using ReClassNET.Nodes;

namespace XivReClassPlugin.Nodes {
    public class Utf8StringSerializer : ICustomNodeSerializer {
        private const string XmlType = "FFXIV::Utf8String";
        
        public bool CanHandleNode(BaseNode node) => node is Utf8StringNode;

        public bool CanHandleElement(XElement element) => element.Attribute(ReClassNetFile.XmlTypeAttribute)?.Value == XmlType;

        public BaseNode CreateNodeFromElement(XElement element, BaseNode parent, IEnumerable<ClassNode> classes, ILogger logger, CreateNodeFromElementHandler defaultHandler) {
            return new Utf8StringNode();
        }

        public XElement CreateElementFromNode(BaseNode node, ILogger logger, CreateElementFromNodeHandler defaultHandler) {
            return new XElement(
                ReClassNetFile.XmlNodeElement,
                new XAttribute(ReClassNetFile.XmlTypeAttribute, XmlType)
            );
        }
    }
}