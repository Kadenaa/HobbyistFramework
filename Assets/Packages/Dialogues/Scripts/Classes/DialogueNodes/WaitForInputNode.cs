using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml.Serialization;


public class WaitForInputNode : DialogueNode, IExecutableNode {
	public int targetNode;

	public void Execute(DialogueManager manager) {
		manager.isWaiting = true;
		manager.StartCoroutine(WaitCoroutine(manager));
	}

	private IEnumerator WaitCoroutine(DialogueManager manager) {
		while (manager.isWaiting) {
			yield return null;
		}

		manager.isWaiting = false;
		JumpToNode(manager, targetNode);
		yield return null;
	}

	public override void DetailEditorGUI() {
		targetNode = EditorGUILayout.IntField("Target Node", targetNode);
	}
}
