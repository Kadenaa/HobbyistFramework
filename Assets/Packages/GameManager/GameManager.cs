using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	private void Awake() {
		DontDestroyOnLoad(gameObject);
		Blackboard.Initialize();
	}

	private void Start() {
		Blackboard.Instance["player_name"] = null;
		Blackboard.Save();
	}
}
