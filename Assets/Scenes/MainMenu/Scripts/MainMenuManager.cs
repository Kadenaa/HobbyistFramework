using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class MainMenuManager : MonoBehaviour {
	public GameObject mainMenu;
	public GameObject newGameMenu;
	public GameObject loadGameMenu;
	public Button loadButtonPrefab;
	public TMPro.TMP_InputField newGameInput;
	public string newGameScene;

	private string[] gameSaves;
	private string selectedSave;

	private void Start() {
		GenerateLoadButtons();

		mainMenu.SetActive(true);
		newGameMenu.SetActive(false);
		loadGameMenu.SetActive(false);
	}

	private void FindAllSaves() {
		gameSaves = Directory.GetFiles(GameManager.gameSaveFolder, "*.save", SearchOption.AllDirectories);
		for (int i = 0; i < gameSaves.Length; ++i) {
			gameSaves[i] = gameSaves[i].Replace('\\', '/');
		}
	}

	private void GenerateLoadButtons() {
		FindAllSaves();

		foreach (Transform child in loadGameMenu.transform.GetChild(0)) {
			Destroy(child.gameObject);
		}

		foreach (string save in gameSaves) {
			Button loadButton = Button.Instantiate(loadButtonPrefab);
			loadButton.GetComponentInChildren<Text>().text = save;
			loadButton.transform.SetParent(loadGameMenu.transform.GetChild(0));
			loadButton.onClick.AddListener(() => { HandleLoadButton(save); });
		}
	}

	private void HandleLoadButton(string save) {
		selectedSave = save;
	}

	public void NewButton() {
		mainMenu.SetActive(false);
		newGameMenu.SetActive(true);
	}

	public void LoadButton() {
		loadGameMenu.SetActive(true);
		mainMenu.SetActive(false);
	}

	public void LoadSelectedButton() {
		if (!string.IsNullOrEmpty(selectedSave)) {
			GameManager.Instance.Load(selectedSave);
			SceneManager.LoadScene(newGameScene);
		}
	}

	public void DeleteSelectedButton() {
		if (!string.IsNullOrEmpty(selectedSave)) {
			string[] split = selectedSave.Split(new char[] { '/' });
			string folder = string.Join("/", split.Take(split.Length - 1).ToArray());
			Directory.Delete(folder, true);
			GenerateLoadButtons();
			selectedSave = null;
		}
	}

	public void LoadCancelButton() {
		mainMenu.SetActive(true);
		loadGameMenu.SetActive(false);
	}

	public void CreateNewGameButton() {
		GameManager.Create(newGameInput.text);
		GameManager.Instance.Load(GameManager.gameSaveFolder + "/" + newGameInput.text + "/" + newGameInput + ".save");
		SceneManager.LoadScene(newGameScene);
	}

	public void CancelNewGameButton() {
		mainMenu.SetActive(true);
		newGameMenu.SetActive(false);
	}

	public void ExitButton() {
		Application.Quit();
	}
}