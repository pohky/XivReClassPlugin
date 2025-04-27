using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ReClassNET.DataExchange.ReClass;
using ReClassNET.Logger;
using ReClassNET.Nodes;

namespace XivReClassPlugin.Nodes;

public class XivNodeSerializer : ICustomNodeSerializer {
    private const string Utf8StringType = "FFXIV::Utf8String";
    private const string StdDequeType = "FFXIV::StdDeque";
    private const string StdListType = "FFXIV::StdList";
    private const string StdSetType = "FFXIV::StdSet";
    private const string StdVectorType = "FFXIV::StdVector";
    private const string AtkValueType = "FFXIV::AtkValue";

    public bool CanHandleNode(BaseNode node) {
        return node switch {
            Utf8StringNode => true,
            StdDequeNode => true,
            StdListNode => true,
            StdSetNode => true,
            StdVectorNode => true,
            AtkValueNode => true,
            _ => false
        };
    }

    public bool CanHandleElement(XElement element) {
        return element.Attribute(ReClassNetFile.XmlTypeAttribute)?.Value switch {
            Utf8StringType => true,
            StdDequeType => true,
            StdListType => true,
            StdSetType => true,
            StdVectorType => true,
            AtkValueType => true,
            _ => false
        };
    }

    public BaseNode CreateNodeFromElement(XElement element, BaseNode parent, IEnumerable<ClassNode> classes, ILogger logger, CreateNodeFromElementHandler defaultHandler) {
        return element.Attribute(ReClassNetFile.XmlTypeAttribute)?.Value switch {
            Utf8StringType => new Utf8StringNode(),
            StdDequeType => new StdDequeNode(),
            StdListType => new StdListNode(),
            StdSetType => new StdSetNode(),
            StdVectorType => new StdVectorNode(),
            AtkValueType => new AtkValueNode(),
            _ => throw new InvalidOperationException("Invalid XML Element Type")
        };
    }

    public XElement CreateElementFromNode(BaseNode node, ILogger logger, CreateElementFromNodeHandler defaultHandler) {
        return node switch {
            Utf8StringNode => new XElement(ReClassNetFile.XmlNodeElement, new XAttribute(ReClassNetFile.XmlTypeAttribute, Utf8StringType)),
            StdDequeNode => new XElement(ReClassNetFile.XmlNodeElement, new XAttribute(ReClassNetFile.XmlTypeAttribute, StdDequeType)),
            StdListNode => new XElement(ReClassNetFile.XmlNodeElement, new XAttribute(ReClassNetFile.XmlTypeAttribute, StdListType)),
            StdSetNode => new XElement(ReClassNetFile.XmlNodeElement, new XAttribute(ReClassNetFile.XmlTypeAttribute, StdSetType)),
            StdVectorNode => new XElement(ReClassNetFile.XmlNodeElement, new XAttribute(ReClassNetFile.XmlTypeAttribute, StdVectorType)),
            AtkValueNode => new XElement(ReClassNetFile.XmlNodeElement, new XAttribute(ReClassNetFile.XmlTypeAttribute, AtkValueType)),
            _ => throw new InvalidOperationException("Invalid Node Type")
        };
    }
}
