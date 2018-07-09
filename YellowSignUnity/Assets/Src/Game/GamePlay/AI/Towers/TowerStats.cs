using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{

}

[CreateAssetMenu(menuName = "YellowSign/TowerStats")]
public class TowerStats : ScriptableObject
{
    public int maxHealth;
    public int baseDamage;
    public int baseRange;
    public DamageType damageType;
    public AttackType attackType;
    public CreepType targetableCreepTypes;
}
