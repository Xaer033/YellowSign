
[System.Serializable]
public struct UpgradeTowerCommand : ICommand
{
    public int upgradeTier;
    public GridPosition position;

    public UpgradeTowerCommand(int upgradeTier, GridPosition pos)
    {
        this.upgradeTier = upgradeTier;
        this.position = pos;
    }
    
    public CommandType commandType
    {
        get
        {
            return CommandType.UPGRADE_TOWER;
        }
    }
}
