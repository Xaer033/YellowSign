using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ShowOdinSerializedPropertiesInInspector]
[System.Serializable]
public class DefinitionDictionary<T> : SerializedScriptableObject
{
    [System.Serializable]
    public class DefDisplay
    {
        public string id;
        public T def;
    }

    [SerializeField]
    private Dictionary<string, T> _defMap;

    //[ShowInInspector]
    //[SerializeField]
    //private DefDisplay[] _defList;

    public void Initialize()
    {
        //Debug.Log("Enabling");
        //_defMap = new Dictionary<string, T>();

        //if(_defList != null)
        //{
        //    for(int i = 0; i < _defList.Length; ++i)
        //    {
        //        _defMap[_defList[i].id] = _defList[i].def;
        //    }
        //}
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
}
