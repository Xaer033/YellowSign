using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

//[ShowOdinSerializedPropertiesInInspector]
[System.Serializable]
public class TowerDef
{
    public AbstractTowerBrain  brain;

    public TowerStats   stats;
    
    public ITowerView   view;
}

//[ShowOdinSerializedPropertiesInInspector]
[System.Serializable]
[CreateAssetMenu(menuName = "YellowSign/Tower Dictionary")]
public class TowerDictionary : DefinitionDictionary<TowerDef>
{ 
    //[Button]
    private void buildTowerIdClass()
    {
        Debug.LogWarning("Building list");
    }
}
