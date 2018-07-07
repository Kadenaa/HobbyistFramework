using System.Collections.Generic;
using System.IO;
using System;

[System.Serializable]
public abstract class Database<T> where T : DatabaseItem {
	public List<T> data = new List<T>();

	public bool ContainsID(Guid id) {
		for (int i = 0; i < data.Count; ++i) {
			if (data[i].id == id) {
				return true;
			}
		}
		return false;
	}

	public T GetItem(string name) {
		for (int i = 0; i < data.Count; ++i) {
			if (data[i].name == name) {
				return data[i];
			}
		}
		return null;
	}

	public T AddItem() {
		T item = System.Activator.CreateInstance<T>();
		while (ContainsID(item.id)) {
			item.id = Guid.NewGuid();
		}
		data.Add(item);
		return item;
	}

	public void DeleteItem(T item) {
		data.Remove(item);
		File.Delete(item.filePath);
	}

	public bool ContainsItem(T item) {
		for (int i = 0; i < data.Count; ++i) {
			if (data[i].id == item.id) {
				return true;
			}
		}
		return false;
	}

	public abstract void Save();
	public abstract void Load();
}