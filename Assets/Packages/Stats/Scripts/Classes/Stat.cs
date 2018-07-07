using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Stat {
#if UNITY_EDITOR
	public bool _showFoldout;
	public bool _showModifiers;
	public bool _showDependencies;
#endif
	public StatCollection owner;

	[SerializeField]
	protected double _baseValue;
	public double BaseValue {
		get { return _baseValue; }
		set {
			_baseValue = value;
			Value = CalculateValue();
		}
	}

	[SerializeField]
	protected List<StatModifier> _modifiers;
	public ReadOnlyCollection<StatModifier> Modifiers {
		get {
			return _modifiers.AsReadOnly();
		}
	}

	[SerializeField]
	protected List<StatDependency> _dependencies;
	public ReadOnlyCollection<StatDependency> Dependencies {
		get {
			return _dependencies.AsReadOnly();
		}
	}

	[SerializeField]
	protected double _value;
	public double Value {
		get {
			return _value;
		}
		set {
			if (_value != value) {
				_value = value;
				if (ValueChanged != null) {
					ValueChanged.Invoke(this, System.EventArgs.Empty);
				}
			}
		}
	}

	public delegate void StatEventHandler(object sender, System.EventArgs e);
	public event StatEventHandler ValueChanged;

	public Stat(StatCollection owner) {
		this.owner = owner;
		_baseValue = 0.0;
		_value = 0.0;
		_modifiers = new List<StatModifier>();
		_dependencies = new List<StatDependency>();
	}

	public Stat(StatCollection owner, double value) {
		this.owner = owner;
		_baseValue = value;
		_value = value;
		_modifiers = new List<StatModifier>();
		_dependencies = new List<StatDependency>();
	}

	public void AddModifier(StatModifier modifier) {
		_modifiers.Add(modifier);
		Value = CalculateValue();
	}

	public void RemoveModifier(StatModifier modifier) {
		_modifiers.Remove(modifier);
		Value = CalculateValue();
	}

	public void AddDependency(StatDependency dependency) {
		if (owner.ContainsAttribute(dependency.stat)) {
			Stat attribute = owner.GetAttribute(dependency.stat);
			if (attribute != this) {
				_dependencies.Add(dependency);
				owner.GetAttribute(dependency.stat).ValueChanged += DependencyValueChanged;
				Value = CalculateValue();
			}
		}
	}

	public void RemoveDependency(StatDependency dependency) {
		_dependencies.Remove(dependency);
		owner.GetAttribute(dependency.stat).ValueChanged -= DependencyValueChanged;
		Value = CalculateValue();
	}

	protected virtual double CalculateValue() {
		double baseAdditives = 0.0;
		double baseScaling = 1.0;
		double totalAdditives = 0.0;
		double totalScaling = 1.0;
		
		foreach (StatModifier modifier in Modifiers) {
			switch (modifier.type) {
				default:
					break;
				case StatModifierType.BaseAdditive:
					baseAdditives += modifier.value;
					break;
				case StatModifierType.BaseScaling:
					baseScaling += modifier.value;
					break;
				case StatModifierType.TotalAdditive:
					totalAdditives += modifier.value;
					break;
				case StatModifierType.TotalScaling:
					totalScaling += modifier.value;
					break;
			}
		}
		
		for (int i = 0; i < Dependencies.Count; ++i) {
			StatDependency dependency = Dependencies[i];
			Stat attribute = owner.GetAttribute(dependency.stat);
			if (attribute != null) {
				double value = owner.GetAttribute(dependency.stat).Value * dependency.weight;
				switch (dependency.type) {
					default:
						break;
					case StatModifierType.BaseAdditive:
						baseAdditives += value;
						break;
					case StatModifierType.BaseScaling:
						baseScaling += value;
						break;
					case StatModifierType.TotalAdditive:
						totalAdditives += value;
						break;
					case StatModifierType.TotalScaling:
						totalScaling += value;
						break;
				}
			} else {
				_dependencies.Remove(dependency);
				i--;
			}
		}

		return (((BaseValue + baseAdditives) * baseScaling) + totalAdditives) * totalScaling;
	}

	private void DependencyValueChanged(object sender, System.EventArgs e) {
		Value = CalculateValue();
	}
}