using TrueSync;
using UnityEngine;

public class TowerState
{
    public int  health;
    public int  attackDamage;

    public FP   reloadTimer;
    public FP   idleTimer;
    public FP   attackTimer;

    public FP   range
    {
        get {  return stats.baseRange; }
    }
    
    public TowerStats stats { get; private set; }

    private TowerState(TowerStats pStats)
    {
        stats = pStats;
        health = pStats.maxHealth;
        attackDamage = pStats.baseDamage;

        reloadTimer = pStats.reloadTime;
        idleTimer = pStats.idleTime;
        attackTimer = 0;
    }

    public static TowerState CreateFromStats(TowerStats stats)
    {
        return new TowerState(stats);
    }
}
