using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GetBlackboardStringNode : DialogueNode, IValueNode<string> {
	public string variableName;

	public override void DetailEditorGUI() {
		variableName = EditorGUILayout.TextField("Variable Name", variableName);
	}

	public string GetValue(DialogueManager manager) {
		object varaible = Blackboard.Instance[variableName];
		if (varaible != null) {
			return varaible.ToString();
		} else {
			return null;
		}
	}
}