using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using UnityEditor;

public class SkipReactiveSpeechNode : DialogueNode {
	public string speakerText;
	public string dialogueText;
	public int uninterruptedNode;
	public int interruptedNode;
	public float delayPerCharacter;

	private bool interrupted;

	public SkipReactiveSpeechNode() {
		speakerText = "Speaker";
		dialogueText = "Dialogue";
		uninterruptedNode = -1;
		interruptedNode = -1;
		delayPerCharacter = 0.1f;
	}

	public override void Execute(DialogueManager manager) {
		if (!manager.dialoguePanel.activeSelf) {
			manager.dialoguePanel.SetActive(true);
		}

		interrupted = false;
		manager.dialogueText.text = "";
		manager.isWaiting = true;

		manager.speakerText.text = speakerText;
		manager.StartCoroutine(AnimateText(manager));
	}

	private IEnumerator AnimateText(DialogueManager manager) {
		string replacedText = ReplaceTextVariables(dialogueText);

		for (int i = 0; i < replacedText.Length; ++i) {
			if (manager.isWaiting) {
				if (replacedText[i] == '\\') {
					++i;
					string command = "";
					while (replacedText[i] != ' ') {
						command += replacedText[i];
						++i;
					}
					command = command.ToLower();

					Debug.Log(command);
				} else {
					manager.dialogueText.text += replacedText[i];
					yield return new WaitForSeconds(delayPerCharacter);
				}
			} else {
				interrupted = true;
				break;
			}
		}

		manager.dialogueText.text = replacedText;
		manager.isWaiting = false;

		if (interrupted) {
			JumpToNode(manager, interruptedNode);
		} else {
			JumpToNode(manager, uninterruptedNode);
		}

		yield return null;
	}

	private string ReplaceTextVariables(string originalText) {
		string result = "";

		for (int i = 0; i < originalText.Length; ++i) {
			if (originalText[i] == '@') {
				++i;
				string variableSource = "";
				for (int j = i; j < originalText.Length; ++j) {
					++i;
					if (originalText[j] == '[') {
						break;
					}
					variableSource += originalText[j];
				}

				string variableName = "";
				for (int j = i; j < originalText.Length; ++j) {
					++i;
					if (originalText[j] == ']') {
						break;
					}
					variableName += originalText[j];
				}

				result += GetVariableString(variableSource, variableName);
			} else {
				result += originalText[i];
			}
		}

		return result;
	}

	private string GetVariableString(string source, string name) {
		source = source.ToLower();
		name = name.ToLower();

		switch (source) {
			default:
				Debug.LogError("[Speech Node] - Unknown Variable Source");
				return "";
			case "blackboard":
				object blackboardVariable = Blackboard.Instance[name];
				if (blackboardVariable != null) {
					return blackboardVariable.ToString();
				} else {
					return string.Format("@Blackboard[{0}]", name);
				}
		}
	}

	public override void DetailEditorGUI() {
		speakerText = EditorGUILayout.TextField("Speaker", speakerText);
		dialogueText = EditorGUILayout.TextField("Text", dialogueText);
		uninterruptedNode = EditorGUILayout.IntField("Un-Interrupted Target", uninterruptedNode);
		interruptedNode = EditorGUILayout.IntField("Interrupted Target", interruptedNode);
		delayPerCharacter = EditorGUILayout.FloatField("Default Delay", delayPerCharacter);
	}
}