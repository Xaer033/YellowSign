
[System.Serializable]
public struct SpawnCreepCommand : ICommand
{
    public string type;
    public int count;

    public SpawnCreepCommand(string creepType, int pCount)
    {
        type = creepType;
        count = pCount;
    }
    
    public CommandType commandType
    {
        get
        {
            return CommandType.SPAWN_CREEP;
        }
    }
}
