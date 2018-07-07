using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatCollection : MonoBehaviour, ISerializationCallbackReceiver {
	[HideInInspector]
	public List<string> attributeKeys;
	[HideInInspector]
	public List<Stat> attributeValues;
	public Dictionary<string, Stat> attributes;

	[HideInInspector]
	public List<string> statKeys;
	[HideInInspector]
	public List<ClampedStat> statValues;
	public Dictionary<string, ClampedStat> stats;

	public delegate void StatCollectionHandler(object sender, System.EventArgs e);
	public event StatCollectionHandler StatAdded;
	public event StatCollectionHandler StatRemoved;

	public StatCollection() {
		attributeKeys = new List<string>();
		attributeValues = new List<Stat>();
		attributes = new Dictionary<string, Stat>();

		statKeys = new List<string>();
		statValues = new List<ClampedStat>();
		stats = new Dictionary<string, ClampedStat>();
	}

	public void AddAttribute(string key, Stat value) {
		key = key.ToLower();
		if (!ContainsAttribute(key)) {
			attributes.Add(key, value);
			if (StatAdded != null) {
				StatAdded(this, System.EventArgs.Empty);
			}
		}
	}

	public Stat GetAttribute(string key) {
		key = key.ToLower();
		if (ContainsAttribute(key)) {
			return attributes[key];
		}
		return null;
	}

	public void RemoveAttribute(string key) {
		key = key.ToLower();
		if (ContainsAttribute(key)) {
			attributes.Remove(key);
			if (StatRemoved != null) {
				StatRemoved(this, System.EventArgs.Empty);
			}
		}
	}

	public void AddStat(string key, ClampedStat value) {
		key = key.ToLower();
		if (!ContainsStat(key)) {
			stats.Add(key, value);
			if (StatAdded != null) {
				StatAdded(this, System.EventArgs.Empty);
			}
		}
	}

	public ClampedStat GetStat(string key) {
		key = key.ToLower();
		if (ContainsStat(key)) {
			return stats[key];
		}
		return null;
	}

	public void RemoveStat(string key) {
		key = key.ToLower();
		if (ContainsStat(key)) {
			stats.Remove(key);
			if (StatRemoved != null) {
				StatRemoved(this, System.EventArgs.Empty);
			}
		}
	}

	public bool ContainsAttribute(string key) {
		key = key.ToLower();
		return attributes.ContainsKey(key);
	}

	public bool ContainsStat(string key) {
		key = key.ToLower();
		return stats.ContainsKey(key);
	}

	public void OnBeforeSerialize() {
		attributeKeys.Clear();
		attributeValues.Clear();

		statKeys.Clear();
		statValues.Clear();

		foreach (var kvp in attributes) {
			attributeKeys.Add(kvp.Key);
			attributeValues.Add(kvp.Value);
		}

		foreach (var kvp in stats) {
			statKeys.Add(kvp.Key);
			statValues.Add(kvp.Value);
		}
	}

	public void OnAfterDeserialize() {
		attributes = new Dictionary<string, Stat>();
		stats = new Dictionary<string, ClampedStat>();

		for (int i = 0; i != Mathf.Min(attributeKeys.Count, attributeValues.Count); i++)
			attributes.Add(attributeKeys[i], attributeValues[i]);

		for (int i = 0; i != Mathf.Min(statKeys.Count, statValues.Count); i++)
			stats.Add(statKeys[i], statValues[i]);
	}
}