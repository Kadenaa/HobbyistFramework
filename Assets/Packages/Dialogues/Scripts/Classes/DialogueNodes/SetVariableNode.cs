using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using UnityEditor;

public class SetBlackboardStringNode : DialogueNode, IExecutableNode {
	public string variableName;
	public string defaultValue;
	public int inputNode;
	public int targetNode;

	public void Execute(DialogueManager manager) {
		IValueNode<string> valueNode = manager.currentDialogue.GetNode(inputNode) as IValueNode<string>;
		string actualValue = (valueNode == null ? defaultValue : valueNode.GetValue());
		Blackboard.Instance[variableName] = actualValue;

		JumpToNode(manager, targetNode);
	}

	public override void DetailEditorGUI() {
		variableName = EditorGUILayout.TextField("Variable Name", variableName);
		defaultValue = EditorGUILayout.TextField("Default Value", defaultValue);
		inputNode = EditorGUILayout.IntField("Input Node", targetNode);
		targetNode = EditorGUILayout.IntField("Target Node", targetNode);
	}
}
