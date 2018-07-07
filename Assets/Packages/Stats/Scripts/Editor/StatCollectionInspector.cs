using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StatCollection))]
public class StatCollectionInspector : Editor {
	private string _statKey;
	private StatCollection collection;

	private bool _showAttributes;
	private bool _showStats;

	private bool _addNewStat;
	private bool _removeStat;

	private string _dependencyKey;
	private double _modifierValue;
	private int _modifierType;

	public void OnEnable() {
		collection = target as StatCollection;
	}

	public override void OnInspectorGUI() {
		_showAttributes = EditorGUILayout.Foldout(_showAttributes, "Attributes");
		if (_showAttributes) {
			EditorGUI.indentLevel++;
			foreach (KeyValuePair<string, Stat> kvp in collection.attributes) {
				kvp.Value._showFoldout = EditorGUILayout.Foldout(kvp.Value._showFoldout, kvp.Key.ToUpper() + (kvp.Value._showFoldout ? "" : ("  \t" + kvp.Value.Value.ToString())));
				if (kvp.Value._showFoldout) {
					EditorGUI.indentLevel++;
					kvp.Value.BaseValue = EditorGUILayout.DelayedDoubleField("BaseValue", kvp.Value.BaseValue);

					StatModifierGUI(kvp.Value);
					StatDependencyGUI(kvp.Value);

					EditorGUILayout.LabelField("Value", kvp.Value.Value.ToString());
					EditorGUI.indentLevel--;
				}
			}
			EditorGUI.indentLevel--;
		}

		_showStats = EditorGUILayout.Foldout(_showStats, "Stats");
		if (_showStats) {
			EditorGUI.indentLevel++;
			foreach (KeyValuePair<string, ClampedStat> kvp in collection.stats) {
				kvp.Value._showFoldout = EditorGUILayout.Foldout(kvp.Value._showFoldout, kvp.Key.ToUpper() + (kvp.Value._showFoldout ? "" : string.Format("({0}/{1})", kvp.Value.CurrentValue, kvp.Value.Value)));
				if (kvp.Value._showFoldout) {
					EditorGUI.indentLevel++;
					kvp.Value.BaseValue = EditorGUILayout.DelayedDoubleField("BaseValue", kvp.Value.BaseValue);

					StatModifierGUI(kvp.Value);
					kvp.Value._showDependencies = EditorGUILayout.Foldout(kvp.Value._showDependencies, "Dependencies");

					kvp.Value.CurrentValue = EditorGUILayout.DelayedDoubleField("CurrentValue", kvp.Value.CurrentValue);
					EditorGUILayout.LabelField("MaximumValue", "\t" + kvp.Value.Value.ToString());
					EditorGUI.indentLevel--;
				}
			}
			EditorGUI.indentLevel--;
		}

		if (!_addNewStat) {
			if (GUILayout.Button("Add")) {
				_addNewStat = true;
			}
		} else {
			_statKey = EditorGUILayout.TextField(_statKey);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Attribute")) {
				collection.AddAttribute(_statKey, new Stat(collection));
				_addNewStat = false;
			}
			if (GUILayout.Button("Add Stat")) {
				collection.AddStat(_statKey, new ClampedStat(collection));
				_addNewStat = false;
			}
			if (GUILayout.Button("Cancel")) {
				_addNewStat = false;
			}
			EditorGUILayout.EndHorizontal();
		}

		if (!_removeStat) {
			if (GUILayout.Button("Remove")) {
				_removeStat = true;
			}
		} else {
			_statKey = EditorGUILayout.TextField(_statKey);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Remove Attribute")) {
				collection.RemoveAttribute(_statKey);
				_removeStat = false;
			}
			if (GUILayout.Button("Remove Stat")) {
				collection.RemoveStat(_statKey);
				_removeStat = false;
			}
			if (GUILayout.Button("Cancel")) {
				_removeStat = false;
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	private void StatModifierGUI(Stat stat) {
		stat._showModifiers = EditorGUILayout.Foldout(stat._showModifiers, "Modifiers");
		if (stat._showModifiers) {
			EditorGUI.indentLevel++;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Value");
			EditorGUILayout.LabelField("Type");
			EditorGUILayout.EndHorizontal();

			for (int i = 0; i < stat.Modifiers.Count; ++i) {
				StatModifier modifier = stat.Modifiers[i];

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(modifier.value.ToString(), modifier.type.ToString());
				if (GUILayout.Button("X")) {
					i--;
					stat.RemoveModifier(modifier);
				}
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.BeginHorizontal();
			_modifierValue = EditorGUILayout.DoubleField(_modifierValue);
			_modifierType = EditorGUILayout.Popup(_modifierType, System.Enum.GetNames(typeof(StatModifierType)));
			if (GUILayout.Button("Add")) {
				stat.AddModifier(new StatModifier(_modifierValue, (StatModifierType)_modifierType));
			}
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel--;
		}
	}

	private void StatDependencyGUI(Stat stat) {
		stat._showDependencies = EditorGUILayout.Foldout(stat._showDependencies, "Dependencies");
		if (stat._showDependencies) {
			EditorGUI.indentLevel++;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Stat", "Weight");
			EditorGUILayout.LabelField("Type");
			EditorGUILayout.EndHorizontal();

			for (int i = 0; i < stat.Dependencies.Count; ++i) {
				StatDependency dependency = stat.Dependencies[i];

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(dependency.stat, dependency.weight.ToString());
				EditorGUILayout.LabelField(dependency.type.ToString());
				if (GUILayout.Button("X")) {
					i--;
					stat.RemoveDependency(dependency);
				}
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.BeginHorizontal();
			_dependencyKey = EditorGUILayout.TextField(_dependencyKey);
			_modifierValue = EditorGUILayout.DoubleField(_modifierValue);
			_modifierType = EditorGUILayout.Popup(_modifierType, System.Enum.GetNames(typeof(StatModifierType)));
			if (GUILayout.Button("Add")) {
				stat.AddDependency(new StatDependency(_dependencyKey, _modifierValue, (StatModifierType)_modifierType));
			}
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel--;
		}
	}
}