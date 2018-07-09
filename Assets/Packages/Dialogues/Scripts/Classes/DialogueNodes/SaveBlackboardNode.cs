using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SaveBlackboardNode : DialogueNode, IExecutableNode {
	public int targetNode;

	public void Execute(DialogueController manager) {
		GameManager.Instance.blackboard.Save(GameManager.Instance.saveFolder + "/Blackboard.xml");
		JumpToNode(manager, targetNode);
	}

#if UNITY_EDITOR
	public override void DetailEditorGUI() {
		targetNode = EditorGUILayout.IntField("Target Node", targetNode);
	}
#endif
}
