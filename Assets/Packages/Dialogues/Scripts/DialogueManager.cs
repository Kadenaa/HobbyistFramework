using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
	public GameObject dialoguePanel;
	public GameObject actorPrefab;
	public GameObject actorContainer;
	public Text speakerText;
	public Text dialogueText;

	public GameObject choicePanel;
	public Button choicePrefab;

	public GameObject stringInputPanel;
	public InputField stringInputField;
	public Button stringInputButton;

	//Turn this into a class that we can enable, and get data from.
	public GameObject integerInputPanel;
	public Button integerInputButton;

	public List<string> actorKeys;
	public List<GameObject> actorValues;
	public Dictionary<string, GameObject> actors;

	public Dialogue currentDialogue;
	public DialogueNode currentNode;

	public bool isWaiting;

	private void Awake() {
		if (dialoguePanel == null) {
			Debug.LogError("[Dialogue Manager] - DialoguePanel is Null");
			enabled = false;
			return;
		}

		if (actorPrefab == null) {
			Debug.LogError("[Dialogue Manager] - DialogePanel/ActorPrefab is Null");
			enabled = false;
			return;
		}

		if (actorContainer == null) {
			Debug.LogError("[Dialogue Manager] - DialogePanel/ActorContainer is Null");
			enabled = false;
			return;
		}

		if (speakerText == null) {
			Debug.LogError("[Dialogue Manager] - DialogePanel/SpeakerText is Null");
			enabled = false;
			return;
		}

		if (dialogueText == null) {
			Debug.LogError("[Dialogue Manager] - DialogePanel/DialogueText is Null");
			enabled = false;
			return;
		}

		if (choicePanel == null) {
			Debug.LogError("[Dialogue Manager] - ChoicePanel is Null");
			enabled = false;
			return;
		}

		if (choicePrefab == null) {
			Debug.LogError("[Dialogue Manager] - ChoicePanel/ChoicePrefab is Null");
			enabled = false;
			return;
		}

		if (stringInputPanel == null) {
			Debug.LogError("[Dialogue Manager] - StringInputPanel is Null");
			enabled = false;
			return;
		}

		if (stringInputField == null) {
			Debug.LogError("[Dialogue Manager] - StringInputField is Null");
			enabled = false;
			return;
		}

		if (stringInputButton == null) {
			Debug.LogError("[Dialogue Manager] - StringInputButton is Null");
			enabled = false;
			return;
		}

		if (integerInputPanel == null) {
			Debug.LogError("[Dialogue Manager] - IntegerInputPanel is Null");
			enabled = false;
			return;
		}

		if (integerInputButton == null) {
			Debug.LogError("[Dialogue Manager] - IntegerInputButton is Null");
			enabled = false;
			return;
		}
	}

	private void Start() {
		Blackboard.Initialize();
		StartDialogue(DialogueDatabase.Instance.GetItem("test"));
	}
	private void Update() {
		if (Input.GetMouseButtonDown(0)) {
			isWaiting = false;
		}
	}

	public void StartDialogue(Dialogue dialogue) {
		currentDialogue = dialogue;
		currentNode = dialogue.GetNode(0);
		if (currentNode == null) {
			Debug.LogError("[Dialogue Manager] - Error, the Dialogue Object has no starting node.");
		} else {
			Play();
		}
	}

	public void StopDialogue() {
		currentDialogue = null;
		currentNode = null;
		dialoguePanel.SetActive(false);
	}

	public void Play() {
		if (currentNode != null && currentDialogue != null) {
			currentNode.Execute(this);
		}
	}
}