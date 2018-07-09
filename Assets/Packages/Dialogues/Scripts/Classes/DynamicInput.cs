using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

public class DynamicInput<T> {
	public bool dynamic;
	public T defaultData;
	public int inputNode;

	public DynamicInput() {
		inputNode = -1;
		this.defaultData = default(T);
	}

	public DynamicInput(T defaultData) {
		inputNode = -1;
		this.defaultData = defaultData;
	}

	public T GetValue(DialogueController manager) {
		if (dynamic) {
			IValueNode<T> node = manager.currentDialogue.GetNode(inputNode) as IValueNode<T>;
			return node == null ? defaultData : node.GetValue(manager);
		} else {
			return defaultData;
		}
	}
}
