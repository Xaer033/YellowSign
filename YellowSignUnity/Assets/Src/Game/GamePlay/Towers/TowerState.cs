using TrueSync;
using UnityEngine;


public class TowerState
{
    public enum BehaviorMode
    {
        MOCK,
        SPAWNING,
        IDLE,
        TARGETING,
        VISUAL_ATTACK,
        ATTACK,
        RECOVERING
    }


    public int  health;
    public int  attackDamage;
    public int  upgradeTier;

    public FP   reloadTimer;
    public FP   idleTimer;
    public FP   attackTimer;

    public TSVector position;
    public TSQuaternion rotation;
    
    public BehaviorMode behaviorMode;

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

        position = TSVector.zero;
        rotation = TSQuaternion.identity;

        behaviorMode = BehaviorMode.SPAWNING;
        upgradeTier = 0;
    }

    public static TowerState CreateFromStats(TowerStats stats)
    {
        return new TowerState(stats);
    }
}
