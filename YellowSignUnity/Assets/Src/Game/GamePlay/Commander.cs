using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class Commander : TrueSyncBehaviour
{
    public enum CommandType
    {
        NO_ACTION = 0,
        BUILD_TOWER = 1,
        UPGRADE_TOWER,
        DESTROY_TOWER,
        SPAWN_CREEP,
        CAST_SPELL
    }

    public void Start()
    {
        Debug.Log("Start");
    }
    public override void OnSyncedStart()
    {
        base.OnSyncedStart();
    }
    /**
     *  @brief Get local player data.
     *  
     *  Called once every lockstepped frame.
     */
    public override void OnSyncedInput()
    {
        float randNum = Random.Range(0.0f, 1.0f);
        if (randNum > 0.9f)
        {
            TrueSyncInput.SetInt(0, (int)CommandType.BUILD_TOWER);
        }
        else if(randNum > 0.7f)
        {
            TrueSyncInput.SetInt(0, (int)CommandType.SPAWN_CREEP);
        }
        else if (randNum > 0.5f)
        {
            TrueSyncInput.SetInt(0, (int)CommandType.CAST_SPELL);
        }

        TrueSyncInput.SetByte(1, localOwner.Id);
    }

    public override void OnSyncedUpdate()
    {
        int action = TrueSyncInput.GetInt(0);
        byte ownerId = TrueSyncInput.GetByte(1);

        Debug.LogError("Player: " + ownerId + " did action:" + action);
    }

}
