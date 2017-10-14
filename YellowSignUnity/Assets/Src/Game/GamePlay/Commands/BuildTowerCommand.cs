
[System.Serializable]
public struct BuildTowerCommand : ICommand
{
    public int gridX;
    public int gridY;
    public string type;

    public BuildTowerCommand(int x, int y, string towerType)
    {
        gridX = x;
        gridY = y;
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
