
[System.Serializable]
public struct SellTowerCommand : ICommand
{
    public GridPosition position;
    
    public SellTowerCommand(GridPosition pos)
    {
        this.position = pos;
    }
    
    public CommandType commandType
    {
        get
        {
            return CommandType.SELL_TOWER;
        }
    }
}
