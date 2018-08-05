using UnityEngine;
using TrueSync;

[CreateAssetMenu(menuName = "YellowSign/TowerStats")]
public class TowerStats : ScriptableObject
{
    public DamageType   damageType;
    public AttackType   attackType;
    public CreepType    targetableCreepTypes;

    public int  maxHealth;
    public int  baseDamage;
    public int  baseRange;
    public float   reloadTime;
    public float   idleTime;

    public static TowerStats FromJson(string json)
    {
        return JsonUtility.FromJson<TowerStats>(json);
    }

    public static string ToJson(TowerStats stats)
    {
        return JsonUtility.ToJson(stats);
    }
}
