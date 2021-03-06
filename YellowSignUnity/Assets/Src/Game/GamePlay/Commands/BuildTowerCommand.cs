﻿
[System.Serializable]
public struct BuildTowerCommand : ICommand
{
    public GridPosition position;
    public string type;

    public BuildTowerCommand(string towerType, GridPosition pos)
    {
        position = pos;
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
