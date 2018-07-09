using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CastToStringNode : DialogueNode, IValueNode<string> {
	public int inputNode;

	public override void DetailEditorGUI() {
		inputNode = EditorGUILayout.IntField("Input", inputNode);
	}

	public string GetValue(DialogueManager manager) {
		DialogueNode valueNode = manager.currentDialogue.GetNode(inputNode);
		Type nodeType = valueNode.GetType();
		object value = nodeType.GetMethod("GetValue").Invoke(valueNode, new object[] { manager });
		return value.ToString();
	}
}
