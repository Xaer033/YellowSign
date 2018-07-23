using Sirenix.OdinInspector;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using System;
using System.Reflection;
using System.Reflection.Emit;
using Sirenix.Serialization;
using Zenject;

//[ShowOdinSerializedPropertiesInInspector]
[System.Serializable]
public class TowerDef
{
    //[OdinSerialize]
    public AbstractTowerBrain  brain;

    public TowerStats   stats;

    //[OdinSerialize]
    public GameObject   view;
}

//[ShowOdinSerializedPropertiesInInspector]
[System.Serializable]
[CreateAssetMenu(menuName = "YellowSign/TowerFactory")]
public class TowerDictionary : ScriptableObject
{
    //[OdinSerialize]
    [System.Serializable]
    public class TowerDefDisplay
    {
        public string towerId;
        public TowerDef towerDef;
    }

    //[SerializeField]
    private Dictionary<string, TowerDef> _towerDefMap;

    [ShowInInspector]
    [SerializeField]
    private TowerDefDisplay[] _towerDefList;

    public void Initialize()
    {
        Debug.Log("Enabling");
        _towerDefMap = new Dictionary<string, TowerDef>();

        if(_towerDefList != null)
        {
            for(int i = 0; i < _towerDefList.Length; ++i)
            {
                _towerDefMap[_towerDefList[i].towerId] = _towerDefList[i].towerDef;
            }
        }
    }


    public TowerDef GetDef(string towerId)
    {
        TowerDef def = null;
        if( _towerDefMap.TryGetValue(towerId, out def))
        {
            return def;
        }
        Debug.LogError("Could not find tower definition for towerId: " + towerId);
        return null;
    }

    //[Button]
    private void buildTowerIdClass()
    {
        Debug.LogWarning("Building list");
    }
}
