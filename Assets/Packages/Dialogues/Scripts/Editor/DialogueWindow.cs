using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

public class DialogueWindow : EditorWindow {
	private Vector2 listScrollPosition;
	private Vector2 detailScrollPosition;
	private string filter;
	private Dialogue selected = null;
	private bool addingNode;

	private string choiceText;
	private int choiceTarget;

	private List<Type> nodeTypes;

	[MenuItem("Databases/Dialogue")]
	private static void Init() {
		DialogueWindow window = GetWindow<DialogueWindow>();
		window.Show();
	}

	private void OnEnable() {
		nodeTypes = Assembly.GetAssembly(typeof(DialogueNode)).GetTypes().Where((type) => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(DialogueNode))).ToList();
	}

	private void OnGUI() {
		EditorGUILayout.BeginHorizontal();
		SelectionGUI();
		DetailsGUI();
		EditorGUILayout.EndHorizontal();
	}

	private void SelectionGUI() {
		EditorGUILayout.BeginVertical("box", GUILayout.Width(150), GUILayout.ExpandHeight(true));

		filter = EditorGUILayout.TextField(filter);
		listScrollPosition = EditorGUILayout.BeginScrollView(listScrollPosition);

		for (int i = 0; i < DialogueDatabase.Instance.data.Count; ++i) {
			Dialogue dialogue = DialogueDatabase.Instance.data[i];
			GUI.color = dialogue == selected ? Color.green : Color.white;
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(dialogue.name, EditorStyles.toolbarButton)) {
				selected = dialogue;
			}

			GUI.color = Color.red;
			if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(20))) {
				if (dialogue == selected) {
					selected = null;
				}
				DialogueDatabase.Instance.DeleteItem(dialogue);
				i--;
			}
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
		}

		EditorGUILayout.EndScrollView();

		GUILayout.FlexibleSpace();

		if (GUILayout.Button("Add Item")) {
			selected = DialogueDatabase.Instance.AddItem();
			string path = Application.persistentDataPath + "/" + DialogueDatabase.databasePath;
			selected.filePath = EditorUtility.SaveFilePanel("Dialogue Folder", path, "dialogue", "xml");
			string[] split = selected.filePath.Split(new char[] { '/', '.' });
			selected.name = split[split.Length - 2];
		}
		if (GUILayout.Button("Save")) {
			DialogueDatabase.Instance.Save();
		}
		if (GUILayout.Button("Reload")) {
			DialogueDatabase.Instance.Load();
			selected = null;
		}

		EditorGUILayout.EndVertical();
	}

	private void DetailsGUI() {
		EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
		detailScrollPosition = EditorGUILayout.BeginScrollView(detailScrollPosition);
		if (selected != null) {
			if (GUILayout.Button(selected.filePath)) {
				string path = Application.persistentDataPath + "/" + DialogueDatabase.databasePath;
				string newPath = EditorUtility.SaveFilePanel("Dialogue Folder", path, "dialogue", "xml");
				if (!string.IsNullOrEmpty(newPath)) {
					selected.filePath = newPath;
					string[] split = selected.filePath.Split(new char[] { '/', '.' });
					selected.name = split[split.Length - 2];
				}
			}

			EditorGUI.indentLevel++;
			for (int i = 0; i < selected.nodes.Count; ++i) {
				EditorGUILayout.BeginVertical("box");
				DialogueNode node = selected.nodes[i];
				EditorGUILayout.BeginHorizontal();
				node.nodeID = EditorGUILayout.IntField("ID", node.nodeID);
				if (GUILayout.Button("X", GUILayout.Width(20))) {
					selected.DeleteNode(node);
					i--;
				}
				EditorGUILayout.EndHorizontal();

				node.DetailEditorGUI();

				EditorGUILayout.EndVertical();
			}
			EditorGUI.indentLevel--;

			GUILayout.FlexibleSpace();
			if (addingNode) {
				AddingNodeGUI();
			} else {
				if (GUILayout.Button("Add Node")) {
					addingNode = true;
				}
			}
		}
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}

	private void AddingNodeGUI() {
		EditorGUILayout.BeginVertical();

		foreach (Type type in nodeTypes) {
			if (GUILayout.Button(type.ToString())) {
				DialogueNode node = (DialogueNode)Activator.CreateInstance(type);
				selected.AddNode(node);
				addingNode = false;
			}
		}

		if (GUILayout.Button("Cancel")) {
			addingNode = false;
		}
		EditorGUILayout.EndVertical();
	}
}
