using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatDependency {
	public string stat;
	public double weight;
	public StatModifierType type;

	public StatDependency(string stat, double weight, StatModifierType type) {
		this.stat = stat;
		this.weight = weight;
		this.type = type;
	}
}