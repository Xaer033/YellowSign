
[System.Serializable]
public struct SpawnCreepCommand : ICommand
{
    public string type;

    public SpawnCreepCommand( string creepType)
    {
        type = creepType;
    }

    public CommandType commandType
    {
        get
        {
            return CommandType.SPAWN_CREEP;
        }
    }
}
