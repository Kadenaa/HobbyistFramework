using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InputIntegerNode : DialogueNode, IExecutableNode, IValueNode<int> {
	public int maximumValue;
	public int targetNode;
	private int inputData;

	public void Execute(DialogueManager manager) {
		manager.integerInputPanel.gameObject.SetActive(true);

		manager.integerInputPanel.CurrentValue = 0;
		manager.integerInputPanel.MaximumValue = maximumValue;
		manager.integerInputButton.onClick.RemoveAllListeners();
		manager.integerInputButton.onClick.AddListener(() => { HandleSubmitClick(manager); });
	}

	public int GetValue(DialogueManager manager) {
		return inputData;
	}

	public override void DetailEditorGUI() {
		maximumValue = EditorGUILayout.IntField("Maximum Value", maximumValue);
		targetNode = EditorGUILayout.IntField("Target Node", targetNode);
	}

	private void HandleSubmitClick(DialogueManager manager) {
		inputData = manager.integerInputPanel.CurrentValue;
		JumpToNode(manager, targetNode);

		manager.integerInputPanel.gameObject.SetActive(false);
	}
} 