using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ClampedStat : Stat {
	protected double _currentValue;
	public double CurrentValue {
		get {
			return _currentValue;
		}
		set {
			if (_currentValue != value) {
				_currentValue = value;

				ClampCurrentValue();

				if (CurrentValueChanged != null) {
					CurrentValueChanged(this, System.EventArgs.Empty);
				}
			}
		}
	}

	public event StatEventHandler CurrentValueChanged;
	public event StatEventHandler CurrentValueMinimized;
	public event StatEventHandler CurrentValueMaximized;

	public ClampedStat(StatCollection owner) : base(owner) {
		CurrentValue = 0.0;
	}

	public ClampedStat(StatCollection owner, double baseValue) : base(owner, baseValue) {
		CurrentValue = baseValue;
	}

	public ClampedStat(StatCollection owner, double baseValue, double currentValue) : base(owner, baseValue) {
		CurrentValue = currentValue;
	}

	protected override double CalculateValue() {
		double result = base.CalculateValue();

		ClampCurrentValue();

		return result;
	}

	protected void ClampCurrentValue() {
		if (CurrentValue > Value) {
			CurrentValue = Value;
			if (CurrentValueMaximized != null) {
				CurrentValueMaximized(this, System.EventArgs.Empty);
			}
		} else if (CurrentValue < 0) {
			CurrentValue = 0;
			if (CurrentValueMinimized != null) {
				CurrentValueMinimized(this, System.EventArgs.Empty);
			}
		}
	}
}