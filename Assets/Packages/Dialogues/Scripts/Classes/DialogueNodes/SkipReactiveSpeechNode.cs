using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using UnityEditor;

public class SkipReactiveSpeechNode : DialogueNode, IExecutableNode {
	public string speakerText;
	public DynamicInput<string> dynamicInput;
	public int uninterruptedNode;
	public int interruptedNode;
	public float delayPerCharacter;

	private bool interrupted;

	public SkipReactiveSpeechNode() {
		speakerText = "Speaker";
		dynamicInput = new DynamicInput<string>("Dialogue");
		uninterruptedNode = -1;
		interruptedNode = -1;
		delayPerCharacter = 0.1f;
	}

	public void Execute(DialogueController manager) {
		if (!manager.dialoguePanel.activeSelf) {
			manager.dialoguePanel.SetActive(true);
		}

		interrupted = false;
		manager.dialogueText.text = "";
		manager.isWaiting = true;

		manager.speakerText.text = speakerText;
		manager.StartCoroutine(AnimateText(manager));
	}

	private IEnumerator AnimateText(DialogueController manager) {
		string text = dynamicInput.GetValue(manager);

		for (int i = 0; i < text.Length; ++i) {
			if (manager.isWaiting) {
				if (text[i] == '\\') {
					++i;
					string command = "";
					while (text[i] != ' ') {
						command += text[i];
						++i;
					}
					command = command.ToLower();

					Debug.Log(command);
				} else {
					manager.dialogueText.text += text[i];
					yield return new WaitForSeconds(delayPerCharacter);
				}
			} else {
				interrupted = true;
				break;
			}
		}

		manager.dialogueText.text = text;
		manager.isWaiting = false;

		if (interrupted) {
			JumpToNode(manager, interruptedNode);
		} else {
			JumpToNode(manager, uninterruptedNode);
		}

		yield return null;
	}

#if UNITY_EDITOR
	public override void DetailEditorGUI() {
		speakerText = EditorGUILayout.TextField("Speaker", speakerText);

		EditorGUILayout.BeginHorizontal();
		dynamicInput.dynamic = EditorGUILayout.Toggle("Dynamic", dynamicInput.dynamic);
		if (dynamicInput.dynamic) {
			dynamicInput.inputNode = EditorGUILayout.IntField("Input Node", dynamicInput.inputNode);
		} else {
			dynamicInput.defaultData = EditorGUILayout.TextField("Text", dynamicInput.defaultData);
		}
		EditorGUILayout.EndHorizontal();

		uninterruptedNode = EditorGUILayout.IntField("Un-Interrupted Target", uninterruptedNode);
		interruptedNode = EditorGUILayout.IntField("Interrupted Target", interruptedNode);
		delayPerCharacter = EditorGUILayout.FloatField("Default Delay", delayPerCharacter);
	}
#endif
}