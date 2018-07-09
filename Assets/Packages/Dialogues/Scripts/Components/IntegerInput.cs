using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class IntegerInput : MonoBehaviour {
	public GameObject[] inputObjects;

	[SerializeField]
	private int _currentValue;
	public int CurrentValue {
		get {
			return _currentValue;
		}
		set {
			_currentValue = Mathf.Clamp(value, 0, _maximumValue);
			UpdateUI();
		}
	}

	[SerializeField]
	private int _maximumValue;
	public int MaximumValue {
		get {
			return _maximumValue;
		}
		set {
			_maximumValue = value;
			CurrentValue = Mathf.Clamp(CurrentValue, 0, _maximumValue);
		}
	}

	public void OnEnable() {
		CurrentValue = 0;
	}

	public void UpdateUI() {
		string value = new string(CurrentValue.ToString().Reverse().ToArray());
		for (int i = 0; i < inputObjects.Length; ++i) {
			if (i < value.Length) {
				inputObjects[i].transform.GetChild(1).GetComponent<Text>().text = "" + value[i];
			} else {
				inputObjects[i].transform.GetChild(1).GetComponent<Text>().text = "0";
			}
		}
	}

	public void AdjustCurrentValue(int amount) {
		CurrentValue += amount;
	}
}
