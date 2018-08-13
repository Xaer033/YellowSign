using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

//[ShowOdinSerializedPropertiesInInspector]
[System.Serializable]
public class TowerDef
{
    [HideInInspector]
    public string id;

    public AbstractTowerBrain  brain;
    public TowerStats   stats;
    public ITowerView   view;
}

//[ShowOdinSerializedPropertiesInInspector]
[System.Serializable]
[CreateAssetMenu(menuName = "YellowSign/Tower Dictionary")]
public class TowerDictionary : DefinitionDictionary<TowerDef>
{
    public override void Initialize()
    {
        foreach(var pair in _defMap)
        {
            pair.Value.id = pair.Key;
        }
    }
    //[Button]
    private void buildTowerIdClass()
    {
        Debug.LogWarning("Building list");
    }
}
