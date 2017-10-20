
[System.Serializable]
public struct BuildTowerCommand : ICommand
{
    public GridPosition position;
    public string type;

    public BuildTowerCommand(int x, int y, string towerType)
    {
        position = GridPosition.Create(x, y);
        type = towerType;
    }

    public CommandType commandType
    {
        get
        {
            return CommandType.BUILD_TOWER;
        }
    }
}
