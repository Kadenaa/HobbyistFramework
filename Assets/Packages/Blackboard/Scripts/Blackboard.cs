using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System;

/// <summary>
/// Blackboard is a class that provides Global Access to Primitive Data.
/// Used to store and access data so that scripts can reference them anywhere.
/// Examples: Dialogue and Quests.
/// </summary>
public class Blackboard {
	/// <summary>
	/// Container for all of the data.
	/// All Objects stored must be Primitive or a String.
	/// </summary>
	private Dictionary<string, object> data;

	/// <summary>
	/// Private Constructor to ensure the only instance of Blackboard is static.
	/// </summary>
	public Blackboard() {
		data = new Dictionary<string, object>();
	}

	public Blackboard(string filePath) {
		data = new Dictionary<string, object>();
		Load(filePath);
	}

	/// <summary>
	/// Gets and Sets Values associated with Key
	/// </summary>
	/// <param name="key">Associative Key. Converted to Lower. Must not have spaces.</param>
	/// <returns>Value associated with Key as an object</returns>
	public object this[string key] {
		get {
			key = key.ToLower();
			if (IsValidKey(key)) {
				if (data.ContainsKey(key)) {
					return data[key];
				}
			} else {
				Debug.LogError(string.Format("[Blackboard] - Error: Provided Key: [{0}] is Null, Empty, or Contains Spaces.", key));
			}
			return null;
		}
		set {
			key = key.ToLower();
			if (IsValidKey(key)) {
				if (value != null) {
					if (IsTypeSupported(value.GetType())) {
						if (data.ContainsKey(key)) {
							data[key] = value;
						} else {
							data.Add(key, value);
						}
					} else {
						Debug.LogError(string.Format("[Blackboard] - Error: Type {0} is unsupported.", value.GetType().ToString()));
					}
				} else {
					Delete(key);
				}
			} else {
				Debug.LogError(string.Format("[Blackboard] - Error: Provided Key: [{0}] is Null, Empty, or Contains Spaces.", key));
			}
		}
	}

	/// <summary>
	/// Deletes the Value associated with the provided Key
	/// </summary>
	/// <param name="key">Associative Key</param>
	public void Delete(string key) {
		key = key.ToLower();
		if (IsValidKey(key)) {
			if (data.ContainsKey(key)) {
				data.Remove(key);
			}
		}
	}

	/// <summary>
	/// Determines if the provided key is valid.
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	private bool IsValidKey(string key) {
		key = key.ToLower();
		return !(string.IsNullOrEmpty(key) || key.Contains(" "));
	}

	/// <summary>
	/// Determines if the provided type is supported.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	private bool IsTypeSupported(Type type) {
		return type.IsPrimitive || type == typeof(string) || type == typeof(Vector2);
	}

	/// <summary>
	/// Loads/Reloads the database.
	/// Reverts all unsaved changes.
	/// </summary>
	public void Load(string filePath) {
		data.Clear();

		if (File.Exists(filePath)) {
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<BlackboardEntry>));
			using (StreamReader reader = new StreamReader(filePath)) {
				List<BlackboardEntry> entries = (List<BlackboardEntry>)xmlSerializer.Deserialize(reader);
				foreach (BlackboardEntry entry in entries) {
					data.Add(entry.key, entry.value);
				}
			}
		} else {
			string[] splitPath = filePath.Split(new char[] { '/' });
			for (int i = 0; i < splitPath.Length; ++i) {
				string currentPath = string.Join("/", splitPath.Take(1 + i).ToArray());

				if (i == splitPath.Length - 1) {
					if (!File.Exists(currentPath)) {
						File.Create(currentPath);
					}
				} else {
					if (!Directory.Exists(currentPath)) {
						Directory.CreateDirectory(currentPath);
					}
				}
			}
		}
	}

	/// <summary>
	/// Saves all changes.
	/// </summary>
	public void Save(string filePath) {
		List<BlackboardEntry> entries = new List<BlackboardEntry>();
		foreach (KeyValuePair<string, object> kvp in data) {
			entries.Add(new BlackboardEntry(kvp.Key, kvp.Value));
		}
		XmlSerializer serializer = new XmlSerializer(typeof(List<BlackboardEntry>));
		using (StreamWriter writer = new StreamWriter(filePath)) {
			try {
				serializer.Serialize(writer, entries);
			} catch(System.Exception e) {
				Debug.LogError(string.Format("[Blackboard] - {0}", e.StackTrace));
				Debug.LogError("[Blackboard] - Confirm that Complex Types are XMLIncluded");
			}
		}
	}
}

/// <summary>
/// Struct used to Serialize Entries in Database.
/// </summary>
[XmlInclude(typeof(Vector2))]
public struct BlackboardEntry {
	public string key;
	public object value;

	public BlackboardEntry(string key, object value) {
		this.key = key;
		this.value = value;
	}
}