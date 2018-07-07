using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System;
using System.Xml.Schema;

public class Dialogue : DatabaseItem, IXmlSerializable {
	public List<DialogueNode> nodes;

	public Dialogue() {
		nodes = new List<DialogueNode>();
	}

	public bool ContainsID(int id) {
		for (int i = 0; i < nodes.Count; ++i) {
			if (nodes[i].nodeID == id) {
				return true;
			}
		}
		return false;
	}

	public DialogueNode GetNode(int id) {
		for (int i = 0; i < nodes.Count; ++i) {
			if (nodes[i].nodeID == id) {
				return nodes[i];
			}
		}
		return null;
	}

	public void AddNode(DialogueNode node) {
		while (ContainsID(node.nodeID)) {
			node.nodeID++;
		}
		nodes.Add(node);
	}

	public void DeleteNode(DialogueNode node) {
		nodes.Remove(node);
	}

	public XmlSchema GetSchema() {
		return null;
	}

	public void ReadXml(XmlReader reader) {
		id = new Guid(reader.GetAttribute("id"));
		name = reader.GetAttribute("name");
		filePath = reader.GetAttribute("filePath");

		reader.ReadStartElement();
		int count = int.Parse(reader.GetAttribute("count"));
		if (count > 0) {
			reader.ReadStartElement();

			for (int i = 0; i < count; ++i) {
				string typeAttrib = reader.GetAttribute("type");
				Type type = Type.GetType(typeAttrib);
				reader.ReadStartElement();
				DialogueNode item = (DialogueNode)new XmlSerializer(type).Deserialize(reader);
				nodes.Add(item);
				reader.ReadEndElement();
			}

			reader.ReadEndElement();
			reader.ReadEndElement();
		}
	}

	public void WriteXml(XmlWriter writer) {
		writer.WriteAttributeString("id", id.ToString());
		writer.WriteAttributeString("name", name);
		writer.WriteAttributeString("filePath", filePath);

		writer.WriteStartElement("Nodes");
		writer.WriteAttributeString("count", nodes.Count.ToString());
		for (int i = 0; i < nodes.Count; ++i) {
			writer.WriteStartElement("Node");
			Type type = nodes[i].GetType();
			writer.WriteAttributeString("type", type.AssemblyQualifiedName);
			new XmlSerializer(type).Serialize(writer, nodes[i]);
			writer.WriteEndElement();
		}
		writer.WriteEndElement();
	}
}