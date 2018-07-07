using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Serialization;
using UnityEditor;

public class ChoiceNode : DialogueNode {
	public List<DialogueChoice> choices;

	public ChoiceNode() {
		choices = new List<DialogueChoice>();
	}

	public void AddChoice() {
		choices.Add(new DialogueChoice());
	}

	public void DeleteChoice(DialogueChoice choice) {
		choices.Remove(choice);
	}

	public override void Execute(DialogueManager manager) {
		manager.choicePanel.SetActive(true);

		foreach (DialogueChoice choice in choices) {
			Button button = GameObject.Instantiate(manager.choicePrefab);
			button.GetComponentInChildren<Text>().text = choice.choiceText;
			button.transform.SetParent(manager.choicePanel.transform);
			button.onClick.AddListener(() => { HandleChoice(manager, choice); });
		}
	}

	public override void DetailEditorGUI() {
		for (int i = 0; i < choices.Count; ++i) {
			EditorGUILayout.BeginHorizontal();
			choices[i].choiceText = EditorGUILayout.TextField("Choice Text", choices[i].choiceText);
			choices[i].targetNode = EditorGUILayout.IntField("Choice Target", choices[i].targetNode);
			if (GUILayout.Button("x", GUILayout.Width(30))) {
				DeleteChoice(choices[i]);
				--i;
			}
			EditorGUILayout.EndHorizontal();
		}
		if (GUILayout.Button("Add Choice")) {
			AddChoice();
		}
	}

	private void HandleChoice(DialogueManager manager, DialogueChoice choice) {
		manager.choicePanel.SetActive(false);

		foreach (Transform button in manager.choicePanel.transform) {
			GameObject.Destroy(button.gameObject);
		}

		JumpToNode(manager, choice.targetNode);
	}
}

public class DialogueChoice {
	public string choiceText;
	public int targetNode;
}