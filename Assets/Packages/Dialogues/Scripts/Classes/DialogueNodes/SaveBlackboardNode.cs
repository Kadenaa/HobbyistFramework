using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveBlackboardNode : DialogueNode, IExecutableNode {
	public int targetNode;

	public void Execute(DialogueManager manager) {
		Blackboard.Save();

		JumpToNode(manager, targetNode);
	}

	public override void DetailEditorGUI() {
		targetNode = EditorGUILayout.IntField("Target Node", targetNode);
	}
}
