using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateStringNode : DialogueNode, IValueNode<string> {
	public string value;

	public override void DetailEditorGUI() {
		value = EditorGUILayout.TextField("Value", value);
	}

	public string GetValue(DialogueManager manager) {
		return value;
	}
}
