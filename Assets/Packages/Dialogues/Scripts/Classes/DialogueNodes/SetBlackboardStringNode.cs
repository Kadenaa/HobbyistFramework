using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using UnityEditor;

public class SetBlackboardStringNode : DialogueNode, IExecutableNode {
	public string variableName;
	public DynamicInput<string> input;
	public int targetNode;

	public SetBlackboardStringNode() {
		variableName = "variable_name";
		input = new DynamicInput<string>("Value");
		targetNode = -1;
	}

	public void Execute(DialogueManager manager) {
		Blackboard.Instance[variableName] = input.GetValue(manager);

		JumpToNode(manager, targetNode);
	}

	public override void DetailEditorGUI() {
		variableName = EditorGUILayout.TextField("Variable Name", variableName);

		EditorGUILayout.BeginHorizontal();
		input.dynamic = EditorGUILayout.Toggle("Dynamic", input.dynamic);
		if (input.dynamic) {
			input.inputNode = EditorGUILayout.IntField("Input Node", input.inputNode);
		} else {
			input.defaultData = EditorGUILayout.TextField("Text", input.defaultData);
		}
		EditorGUILayout.EndHorizontal();

		targetNode = EditorGUILayout.IntField("Target Node", targetNode);
	}
}