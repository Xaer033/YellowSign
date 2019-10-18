using UnityEngine;

public class CommandFactory
{
	public static ICommand CreateFromJson(CommandType type, string jsonString)
    {
        ICommand command = null;

        switch(type)
        {
            case CommandType.BUILD_TOWER:
                command = JsonUtility.FromJson<BuildTowerCommand>(jsonString);
                break;
            case CommandType.SPAWN_CREEP:
                command = JsonUtility.FromJson<SpawnCreepCommand>(jsonString);
                break;
            case CommandType.UPGRADE_TOWER:
                command = JsonUtility.FromJson<UpgradeTowerCommand>(jsonString);
                break;
            case CommandType.SELL_TOWER:
                command = JsonUtility.FromJson<SellTowerCommand>(jsonString);
                break;
            default:
                Debug.LogError("Don't have Command for Type: " + type + ", jsonString: " + jsonString);
                break;
        }

        return command;
    }

    public static ICommand CreateFromByteArray(CommandType type, byte[] commandBytes)
    {
        string jsonCommand = System.Text.Encoding.UTF8.GetString(commandBytes);
        return CreateFromJson(type, jsonCommand);
    }

    public static byte[] ToByteArray(ICommand command)
    {
        string jsonCommand = JsonUtility.ToJson(command);
        return System.Text.Encoding.UTF8.GetBytes(jsonCommand);
    }
}
