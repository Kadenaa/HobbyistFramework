using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InputStringNode : DialogueNode, IExecutableNode, IValueNode<string> {
	public int targetNode;
	private string inputData;

	public InputStringNode() {
		targetNode = -1;
	}

	public void Execute(DialogueController manager) {
		manager.stringInputPanel.SetActive(true);

		manager.stringInputField.text = "";
		manager.stringInputButton.onClick.RemoveAllListeners();
		manager.stringInputButton.onClick.AddListener( () => { HandleSubmitClick(manager); });
	}

	public string GetValue(DialogueController manager) {
		return inputData;
	}

#if UNITY_EDITOR
	public override void DetailEditorGUI() {
		targetNode = EditorGUILayout.IntField("Target Node", targetNode);
	}
#endif

	private void HandleSubmitClick(DialogueController manager) {
		inputData = manager.stringInputField.text;
		JumpToNode(manager, targetNode);

		manager.stringInputPanel.SetActive(false);
	}
}