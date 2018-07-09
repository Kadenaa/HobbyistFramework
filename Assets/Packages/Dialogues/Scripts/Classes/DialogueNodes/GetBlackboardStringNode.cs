using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GetBlackboardStringNode : DialogueNode, IValueNode<string> {
	public string variableName;


#if UNITY_EDITOR
	public override void DetailEditorGUI() {
		variableName = EditorGUILayout.TextField("Variable Name", variableName);
	}
#endif

	public string GetValue(DialogueController manager) {
		object variable = GameManager.Instance.blackboard[variableName];
		if (variable != null) {
			return variable.ToString();
		} else {
			return null;
		}
	}
}