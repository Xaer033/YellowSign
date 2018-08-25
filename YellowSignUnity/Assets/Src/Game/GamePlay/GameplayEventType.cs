public sealed class GameplayEventType
{
    public const string GAME_START = "game_start";

    public const string CREEP_REACHED_GOAL = "creep_reached_goal";
    public const string CREEP_DAMAGED = "creep_damage";
    public const string CREEP_KILLED = "creep_killed";
    public const string CREEP_SPAWNED = "creep_spawned";

    public const string TOWER_BUILT = "tower_built";
    public const string TOWER_DESTROYED = "tower_destroyed";

    public const string WAVE_START = "wave_start";
    public const string WAVE_COMPLETE = "wave_complete";
}
