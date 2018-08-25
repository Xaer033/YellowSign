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


[CreateAssetMenu(menuName = "YellowSign/Creeps/Creep Stats")]
public class CreepStats : ScriptableObject
{
    public CreepType creepType;
    public float baseSpeed;
    public int maxHealth;
    public int baseArmor;

    public int cost;
    public int income;

    public static CreepStats FromJson(string json)
    {
        return JsonUtility.FromJson<CreepStats>(json);
    }

    public static string ToJson(CreepStats stats)
    {
        return CreepStats.ToJson(stats);
    }
}
