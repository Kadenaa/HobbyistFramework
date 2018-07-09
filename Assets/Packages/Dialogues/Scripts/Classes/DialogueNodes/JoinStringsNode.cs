using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

public class JoinStringsNode : DialogueNode, IValueNode<string> {
	public List<DynamicInput<string>> inputs;

	public JoinStringsNode() {
		inputs = new List<DynamicInput<string>>();
	}

	public void Add() {
		inputs.Add(new DynamicInput<string>());
	}

	public void Delete(DynamicInput<string> input) {
		inputs.Remove(input);
	}

#if UNITY_EDITOR
	public override void DetailEditorGUI() {
		for (int i = 0; i < inputs.Count; ++i) {
			EditorGUILayout.BeginHorizontal();
			inputs[i].dynamic = EditorGUILayout.Toggle("Dynamic", inputs[i].dynamic);
			if (inputs[i].dynamic) {
				inputs[i].inputNode = EditorGUILayout.IntField("Input Node", inputs[i].inputNode);
			} else {
				inputs[i].defaultData = EditorGUILayout.TextField("Text", inputs[i].defaultData);
			}
			if (GUILayout.Button("x", GUILayout.Width(30))) {
				Delete(inputs[i]);
				--i;
			}
			EditorGUILayout.EndHorizontal();
		}

		if (GUILayout.Button("Add Input")) {
			Add();
		}
	}
#endif

	public string GetValue(DialogueController manager) {
		string result = "";

		for (int i = 0; i < inputs.Count; ++i) {
			IValueNode<string> node = manager.currentDialogue.GetNode(inputs[i].inputNode) as IValueNode<string>;
			string text = "";
			if (inputs[i].dynamic) {
				text = node == null ? inputs[i].defaultData : node.GetValue(manager);
			} else {
				text = inputs[i].defaultData;
			}
			result += text;
		}

		return result;
	}
}