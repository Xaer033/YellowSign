using TrueSync;
using UnityEngine;

[System.Flags]
public enum CreepType
{
    NONE = 0,
    GROUND = 1,
    AIR = 2,
    BOTH = GROUND | AIR
}


[CreateAssetMenu(menuName = "YellowSign/CreepStats")]
public class CreepStats : ScriptableObject
{
    public CreepType creepType;
    public float baseSpeed;
    public int maxHealth;
    public int baseArmor;
}
