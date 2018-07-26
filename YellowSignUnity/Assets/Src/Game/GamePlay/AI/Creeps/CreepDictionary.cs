using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreepDef
{
    public CreepStats stats;
    public GameObject view;
}

[System.Serializable]
[CreateAssetMenu(menuName = "YellowSign/Creep Dictionary")]
public class CreepDictionary : DefinitionDictionary<CreepDef>
{
    
}
