using TrueSync;
using UnityEngine;


public enum CreepType
{
    NONE = 0,
    GROUND,
    AIR
}


[CreateAssetMenu(menuName = "YellowSign/CreepStats")]
public class CreepStats : ScriptableObject
{
    public CreepType creepType;
    public FP speed;
    public int maxHealth;
    public int armor;
}
