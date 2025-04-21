using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SerializableDictionary<T, B> : Dictionary<T, B>, ISerializationCallbackReceiver
{
    [SerializeField] private List<T> keys = new List<T>();
    [SerializeField] private List<B> values = new List<B>();
    public void OnAfterDeserialize()
    {
        this.Clear();
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        keys.Clear(); values.Clear();
        foreach (KeyValuePair<T, B> kvp in this)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

}