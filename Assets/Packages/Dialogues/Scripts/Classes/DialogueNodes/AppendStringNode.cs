using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AppendStringNode : DialogueNode, IValueNode<string> {
	public int nodeA;
	public int nodeB;
	public string defaultA;
	public string defaultB;

	private bool dynamicA;
	private bool dynamicB;

	public AppendStringNode() {
		nodeA = -1;
		nodeB = -1;
	}

	public string GetValue(DialogueManager manager) {
		IValueNode<string> a = manager.currentDialogue.GetNode(nodeA) as IValueNode<string>;
		string valueA = a == null ? defaultA : a.GetValue(manager);

		IValueNode<string> b = manager.currentDialogue.GetNode(nodeB) as IValueNode<string>;
		string valueB = b == null ? defaultB : b.GetValue(manager);

		return valueA + valueB;
	}

	public override void DetailEditorGUI() {
		EditorGUILayout.BeginHorizontal();
		dynamicA = EditorGUILayout.Toggle("Dynamic", dynamicA);
		if (dynamicA) {
			nodeA = EditorGUILayout.IntField("Input Node", nodeA);
		} else {
			defaultA = EditorGUILayout.TextField("Default Value", defaultA);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		dynamicB = EditorGUILayout.Toggle("Dynamic", dynamicB);
		if (dynamicB) {
			nodeB = EditorGUILayout.IntField("Input Node", nodeB);
		} else {
			defaultB = EditorGUILayout.TextField("Default Value", defaultB);
		}
		EditorGUILayout.EndHorizontal();
	}
}
