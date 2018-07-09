using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class SpeechNode : DialogueNode, IExecutableNode {
	public string speakerText;
	public int targetNode;
	public DynamicInput<string> dynamicInput;
	public float delayPerCharacter;

	private float currentDelay;

	public SpeechNode() {
		speakerText = "Speaker";
		dynamicInput = new DynamicInput<string>("Dialogue");
		targetNode = -1;
		delayPerCharacter = 0.1f;
	}

	public void Execute(DialogueController manager) {
		if (!manager.dialoguePanel.activeSelf) {
			manager.dialoguePanel.SetActive(true);
		}

		currentDelay = delayPerCharacter;
		manager.dialogueText.text = "";
		manager.isWaiting = true;

		manager.speakerText.text = speakerText;
		manager.StartCoroutine(AnimateText(manager));
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

		targetNode = EditorGUILayout.IntField("Target", targetNode);
		delayPerCharacter = EditorGUILayout.FloatField("Default Delay", delayPerCharacter);
	}
#endif

	private IEnumerator AnimateText(DialogueController manager) {
		string text = dynamicInput.GetValue(manager);

		for (int i = 0; i < text.Length; ++i) {
			if (manager.isWaiting) {
				if (text[i] == '\\') {
					++i;
					string command = ParseCommand(text.Substring(i));
					i += command.Length + 1;

					string[] parameters = ParseParameters(text.Substring(i));
					for (int j = 0; j < parameters.Length; ++j) {
						i += parameters[j].Length;
					}

					HandleCommand(command, parameters);
				} else {
					yield return new WaitForSeconds(currentDelay);
					manager.dialogueText.text += text[i];
				}
			} else {
				break;
			}
		}

		string cleanText = RemoveCommands(text);
		manager.dialogueText.text = cleanText;
		manager.isWaiting = false;

		JumpToNode(manager, targetNode);

		yield return null;
	}

	protected string RemoveCommands(string value) {
		string result = "";
		string[] split = value.Split(new char[] { '\\', ')' });
		for (int i = 0; i < split.Length; i += 2) {
			result += split[i];
		}
		return result;
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
}