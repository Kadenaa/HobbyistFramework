using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class DialogueDatabase : Database<Dialogue> {
	public static string databasePath = "Data/Dialogue";

	private static DialogueDatabase _instance;
	public static DialogueDatabase Instance {
		get {
			if (_instance == null) {
				_instance = new DialogueDatabase();
			}
			return _instance;
		}
	}

	private DialogueDatabase() {
		Load();
	}

	public override void Load() {
		data.Clear();

		string folderPath = Application.persistentDataPath + "/" + databasePath; 
		if (Directory.Exists(folderPath)) {
			string[] files = Directory.GetFiles(folderPath, "*.xml", SearchOption.AllDirectories);
			foreach (string file in files) {
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(Dialogue));
				using (StreamReader reader = new StreamReader(file)) {
					Dialogue dialogue = (Dialogue)xmlSerializer.Deserialize(reader);
					data.Add(dialogue);
				}
			}
		} else {
			string[] splitPath = databasePath.Split(new char[] { '/' });
			for (int i = 0; i < splitPath.Length; ++i) {
				string currentPath = Application.persistentDataPath + "/" + string.Join("/", splitPath.Take(1 + i).ToArray());

				if (!Directory.Exists(currentPath)) {
					Directory.CreateDirectory(currentPath);
				}
			}
		}
	}

	public override void Save() {
		if (_instance != null) {
			foreach (Dialogue dialogue in data) {
				XmlSerializer serializer = new XmlSerializer(typeof(Dialogue));
				using (StreamWriter writer = new StreamWriter(dialogue.filePath)) {
					try {
						serializer.Serialize(writer, dialogue);
					} catch (System.Exception e) {
						Debug.LogError(string.Format("[Dialogue Database] - {0}", e.ToString()));
					}
				}
			}
		}
	}
}