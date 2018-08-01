using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AttackData 
{
    public readonly int potentialDamage;
    public readonly DamageType damageType;

    public AttackData(DamageType type, int damage)
    {
        damageType = type;
        potentialDamage = damage;
    }

}
