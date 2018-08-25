using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreepDef
{
    [HideInInspector]
    public string id;

    public CreepStats stats;
    public ICreepView view;
    public Sprite icon;
}

[System.Serializable]
[CreateAssetMenu(menuName = "YellowSign/Creeps/Creep Dictionary")]
public class CreepDictionary : DefinitionDictionary<CreepDef>
{
    public override void Initialize()
    {
        foreach(var pair in _defMap)
        {
            pair.Value.id = pair.Key;
        }
    }
}
