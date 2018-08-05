using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandFactory
{
    public static ICommand CreateCommand (CommandType type, params object[] args)
    {
        ICommand command = null;
        switch(type)
        {
            case CommandType.BUILD_TOWER:
                command = new BuildTowerCommand(args[0] as string, (GridPosition)args[1]);
                break;
            case CommandType.SPAWN_CREEP:
                command = new SpawnCreepCommand(args[0] as string, (int)args[1]);
                break;
            default:
                Debug.LogError("Don't have Command for Type: " + type);
                break;
        }
        return command;
    }

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
