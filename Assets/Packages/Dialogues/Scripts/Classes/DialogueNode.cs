using System.Xml.Serialization;
using UnityEngine;
using System;

public abstract class DialogueNode {
	public int nodeID;

	public DialogueNode() { }

#if UNITY_EDITOR
	public abstract void DetailEditorGUI();
#endif

	protected void JumpToNode(DialogueController manager, int targetNode) {
		manager.currentNode = manager.currentDialogue.GetNode(targetNode);
		if (manager.currentNode != null) {
			IExecutableNode node = manager.currentNode as IExecutableNode;
			if (node != null) {
				node.Execute(manager);
			}
		} else {
			manager.StopDialogue();
		}
	}

	protected string GetVariableString(string source, string name) {
		source = source.ToLower();
		name = name.ToLower();

		switch (source) {
			default:
				Debug.LogError("[Dialogue Node] - Unknown Variable Source");
				return "";
			case "blackboard":
				object blackboardVariable = GameManager.Instance.blackboard[name];
				if (blackboardVariable != null) {
					return blackboardVariable.ToString();
				} else {
					return string.Format("@Blackboard[{0}]", name);
				}
		}
	}

	public virtual void OnEnter(DialogueController manager) { }
	public virtual void OnLeave(DialogueController manager) { }
	public virtual void OnSkip(DialogueController manager) { }
	public virtual void OnQuit(DialogueController manager) { }
}