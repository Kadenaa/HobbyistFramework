using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using UnityEditor;

public class SetStringVariableNode : DialogueNode {
	public string targetSource;
	public string targetName;
	public string value;
	public int targetNode;

	public override void Execute(DialogueManager manager) {
		string actualValue = value;

		if (value[0] == '@') {
			int i = 1;
			string variableSource = "";
			for (int j = i; j < value.Length; ++j) {
				++i;
				if (value[j] == '[') {
					break;
				}
				variableSource += value[j];
			}

			string variableName = "";
			for (int j = i; j < value.Length; ++j) {
				if (value[j] == ']') {
					break;
				}
				++i;
				variableName += value[j];
			}

			actualValue = GetVariableString(variableSource, variableName);
		}

		switch (targetSource.ToLower()) {
			default:
				Debug.LogError("[SetVariable Node] - Unknown Variable Source");
				break;
			case "blackboard":
				Blackboard.Instance[targetName] = actualValue;
				break;
		}

		JumpToNode(manager, targetNode);
	}

	public override void DetailEditorGUI() {
		targetSource = EditorGUILayout.TextField("Variable Source", targetSource);
		targetName = EditorGUILayout.TextField("Variable Name", targetName);
		value = EditorGUILayout.TextField("Variable Value", value);
		targetNode = EditorGUILayout.IntField("Target Node", targetNode);
	}
}
