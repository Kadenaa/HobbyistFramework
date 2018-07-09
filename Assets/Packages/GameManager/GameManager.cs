using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class GameManager : MonoBehaviour {
	private static GameManager _instance;
	public static GameManager Instance {
		get {
			return _instance;
		}
	}

	public static string gameSaveFolder {
		get {
			return Application.persistentDataPath + "/Saves";
		}
	}

	public Blackboard blackboard;
	public string saveFolder;

	private void Awake() {
		if (_instance == null) {
			_instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(this);
		}

		if (!Directory.Exists(gameSaveFolder)) {
			Directory.CreateDirectory(gameSaveFolder);
		}
	}

	public static void Create(string gameName) {
		Directory.CreateDirectory(gameSaveFolder + "/" + gameName);
		File.Create(gameSaveFolder + "/" + gameName + "/" + gameName + ".save");
	}

	public void Load(string filePath) {
		string[] split = filePath.Split(new char[] { '/' });
		saveFolder = string.Join("/", split.Take(split.Length - 1).ToArray());
		blackboard = new Blackboard(saveFolder + "/Blackboard.xml");
	}

	public void Save(string filePath) {
		//save gamedata to filePath
		string[] split = filePath.Split(new char[] { '/' });
		string saveFolder = string.Join("/", split.Take(split.Length - 1).ToArray());
		blackboard.Save(saveFolder + "/Blackboard.xml");
	}

	public void Save() {
		blackboard.Save(saveFolder + "/Blackboard.xml");
	}
}