using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateStringNode : DialogueNode, IValueNode<string> {
	public string value;

#if UNITY_EDITOR
	public override void DetailEditorGUI() {
		value = EditorGUILayout.TextField("Value", value);
	}
#endif

	public string GetValue(DialogueController manager) {
		return value;
	}
}