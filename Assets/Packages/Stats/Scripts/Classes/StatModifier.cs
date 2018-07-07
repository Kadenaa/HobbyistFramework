using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatModifier {
	public double value;
	public StatModifierType type;

	public StatModifier(double value, StatModifierType type) {
		this.value = value;
		this.type = type;
	}
}

public enum StatModifierType {
	BaseAdditive,
	BaseScaling,
	TotalAdditive,
	TotalScaling
}