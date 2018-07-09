using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

[System.Serializable]
public class TowerDef
{
    public string       towerId;

    [SerializeField]
    public AbstractTowerBrain  brain;

    public TowerStats   stats;
    public GameObject   view;
}

[CreateAssetMenu(menuName = "YellowSign/TowerFactory")]
public class TowerFactory : ScriptableObject
{
    public List<TowerDef> _towerList;

    private Dictionary<string, TowerDef> _towerDefMap = new Dictionary<string, TowerDef>();

    public void Awake()
    {
        if(_towerList != null)
        {
            for(int i = 0; i < _towerList.Count; ++i)
            {
                _towerDefMap[_towerList[i].towerId] = _towerList[i];
            }        
        }
    }


    public Tower Create(string towerId, TSVector position, TSQuaternion rotation)
    {
        TowerDef def = _towerDefMap[towerId];
        GameObject towerView = TrueSyncManager.SyncedInstantiate(def.view, position, rotation);
        //Collider towerCollider = towerObj.GetComponent<Collider>();

        Tower tower = new Tower(def.brain, def.stats, towerView, null);
        return tower;
    }

}
