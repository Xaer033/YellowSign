using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[ShowOdinSerializedPropertiesInInspector]
[Serializable]
public class DefinitionDictionary<T> : SerializedScriptableObject
{
    [Serializable]
    public class DefDisplay
    {
        public string id;
        public T def;
    }

    [SerializeField]
    protected Dictionary<string, T> _defMap;

    //[ShowInInspector]
    //[SerializeField]
    //private DefDisplay[] _defList;

    public virtual void Initialize()
    {
    }


    public T GetDef(string id)
    {
        T def = default(T);
        if(!_defMap.TryGetValue(id, out def))
        {
            Debug.LogError("Could not find definition for id: " + id);
        }
        return def;        
    }

    public List<string> GetKeys()
    {
        List<string> keys = new List<string>();
        foreach(var pair in _defMap)
        {
            keys.Add(pair.Key);
        }
        return keys;
    }

    public List<T> GetValues()
    {
        List<T> values = new List<T>();
        foreach(var pair in _defMap)
        {
            values.Add(pair.Value);
        }
        return values;
    }
}
