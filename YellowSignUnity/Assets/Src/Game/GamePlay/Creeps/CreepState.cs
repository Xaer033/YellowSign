using TrueSync;

public class CreepState
{
    public int      health;
    public TSVector position;
    public TSQuaternion rotation;
    
    private CreepState(CreepStats pStats)
    {
        health = pStats.maxHealth;

        position = TSVector.zero;
        rotation = TSQuaternion.identity;
    }

    public static CreepState CreateFromStats(CreepStats stats)
    {
        return new CreepState(stats);
    }
}

