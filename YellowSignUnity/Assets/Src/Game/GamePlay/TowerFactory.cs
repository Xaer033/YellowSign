using Sirenix.OdinInspector;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using System;
using System.Reflection;
using System.Reflection.Emit;
using Sirenix.Serialization;
using Zenject;

[ShowOdinSerializedPropertiesInInspector]
[System.Serializable]
public class TowerDef
{
    [OdinSerialize]
    public ITowerBrain  brain;

    public TowerStats   stats;

    [OdinSerialize]
    public ITowerView   view;
}

[ShowOdinSerializedPropertiesInInspector]
[CreateAssetMenu(menuName = "YellowSign/TowerFactory")]
public class TowerFactory : SerializedScriptableObject
{
    //[OdinSerialize]
    [SerializeField]
    private Dictionary<string, TowerDef> _towerDefMap;

    [Inject]
    private Tower.Factory _towerFactory;
    //public void Awake()
    //{
    //    if(_towerList != null)
    //    {
    //        for(int i = 0; i < _towerList.Count; ++i)
    //        {
    //            _towerDefMap[_towerList[i].towerId] = _towerList[i];
    //        }        
    //    }
    //}


    public Tower Create(string towerId, TSVector position, TSQuaternion rotation)
    {
        TowerDef def = _towerDefMap[towerId];
        GameObject towerGameObject = TrueSyncManager.SyncedInstantiate(def.view.gameObject, position, rotation);
        ITowerView towerView = towerGameObject.GetComponent<ITowerView>();
        
        Tower tower = _towerFactory.Create(def.brain, def.stats, towerView);
        return tower;
    }

    [Button]
    private void buildTowerIdClass()
    {
        Debug.LogWarning("Building list");
    }
}
