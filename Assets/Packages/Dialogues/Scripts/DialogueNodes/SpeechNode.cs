using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class SpeechNode : DialogueNode {
	public string speakerText;
	public string dialogueText;
	public int targetNode;
	public float delayPerCharacter;

	private float currentDelay;

	public SpeechNode() {
		speakerText = "Speaker";
		dialogueText = "Dialogue";
		targetNode = -1;
		delayPerCharacter = 0.1f;
	}

	public override void Execute(DialogueManager manager) {
		if (!manager.dialoguePanel.activeSelf) {
			manager.dialoguePanel.SetActive(true);
		}

		currentDelay = delayPerCharacter;
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
					string command = ParseCommand(replacedText.Substring(i));
					i += command.Length + 1;

					string[] parameters = ParseParameters(replacedText.Substring(i));
					for (int j = 0; j < parameters.Length; ++j) {
						i += parameters[j].Length;
					}

					HandleCommand(command, parameters);
				} else {
					yield return new WaitForSeconds(currentDelay);
					manager.dialogueText.text += replacedText[i];
				}
			} else {
				break;
			}
		}

		manager.dialogueText.text = replacedText;
		manager.isWaiting = false;

		JumpToNode(manager, targetNode);

		yield return null;
	}

	protected string ParseCommand(string value) {
		string command = "";

		for (int i = 0; i < value.Length; ++i) {
			if (value[i] == '(') {
				break;
			}
			command += value[i];
		}

		return command.ToLower();
	}

	protected string[] ParseParameters(string value) {
		string parameterString = "";

		for (int i = 0; i < value.Length; ++i) {
			if (value[i] == ')') {
				break;
			}
			parameterString += value[i];
		}

		return parameterString.ToLower().Split(new string[] { ", " }, StringSplitOptions.None);
	}

	protected void HandleCommand(string command, string[] parameters) {
		switch (command) {
			default:
				Debug.LogError("[Speech Node] - Command [{0}] is unrecognized.");
				throw new NotImplementedException();
			case "delay":
				if (parameters.Length == 1) {
					if (!float.TryParse(parameters[0], out currentDelay)) {
						Debug.LogError("[Speech Node] - Command [delay] argument must Parse as Float.");
					}
				} else {
					Debug.LogError("[Speech Node] - Command [delay] only takes 1 argument.");
				}
				break;
		}
	}

	protected string ReplaceTextVariables(string originalText) {
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
					if (originalText[j] == ']') {
						break;
					}
					++i;
					variableName += originalText[j];
				}

				result += GetVariableString(variableSource, variableName);
			} else {
				result += originalText[i];
			}
		}

		return result;
	}

	public override void DetailEditorGUI() {
		speakerText = EditorGUILayout.TextField("Speaker", speakerText);
		dialogueText = EditorGUILayout.TextField("Text", dialogueText);
		targetNode = EditorGUILayout.IntField("Target", targetNode);
		delayPerCharacter = EditorGUILayout.FloatField("Default Delay", delayPerCharacter);
	}
}
