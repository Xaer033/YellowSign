using TrueSync;

public class CreepState
{
    public int          health;
    //public TSVector     position;
    //public TSQuaternion rotation;

    public bool         isDead
    {
        get { return health <= 0; }
    }

    public CreepStats stats { get; private set; }

    private CreepState(CreepStats pStats)
    {
        stats = pStats;
        health = pStats.maxHealth;
    }

    public static CreepState CreateFromStats(CreepStats stats)
    {
        return new CreepState(stats);
    }
}

