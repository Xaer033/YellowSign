﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TowerState
{
    public int health;
    public int attackDamage;

    public TowerStats stats { get; private set; }

    private TowerState(TowerStats _stats)
    {
        stats = _stats;
        health = stats.maxHealth;
        attackDamage = stats.baseDamage;
    }

    public static TowerState CreateFromStats(TowerStats stats)
    {
        TowerState state = new TowerState(stats);
        return state;
    }

}