using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class Commander : TrueSyncBehaviour
{
    public GameObject testPrefab;
    private Queue<ICommand> _commandQueue = new Queue<ICommand>();


    private event Action<byte, CommandType, ICommand> _onCommandExecute;
    private event Action<FP> _onSyncedStep;


    public event Action<byte, CommandType, ICommand> onCommandExecute
    {
        add { _onCommandExecute += value; }
        remove { _onCommandExecute -= value; }
    }

    public event Action<FP> onSyncedStep
    {
        add { _onSyncedStep += value; }
        remove { _onSyncedStep -= value; }
    }

    public void AddCommand(ICommand command)
    {
        _commandQueue.Enqueue(command);
    }
    
    public override void OnSyncedStart()
    {
        TSRandom.instance.Initialize(42);
        if(Singleton.instance)
        {
            Singleton.instance.notificationDispatcher.DispatchEvent("GameStart");
        }
    }
    /**
     *  @brief Get local player data.
     *  
     *  Called once every lockstepped frame.
     */
    public override void OnSyncedInput()
    {
        byte iterKey = 0;
        int commandCount = _commandQueue.Count;
        TrueSyncInput.SetByte(iterKey++, localOwner.Id);
        TrueSyncInput.SetInt(iterKey++, commandCount);

        while (_commandQueue.Count > 0)
        {
            ICommand ct = _commandQueue.Dequeue();
            TrueSyncInput.SetByte(iterKey++, (byte)ct.commandType);
            string jsonCommand = JsonUtility.ToJson(ct);
            byte[] byteCommand = System.Text.Encoding.UTF8.GetBytes(jsonCommand);
            TrueSyncInput.SetByteArray(iterKey++, byteCommand);
        }
    }

    public override void OnSyncedUpdate()
    {
        byte iterKey = 0;
        byte ownerId = TrueSyncInput.GetByte(iterKey++);
        int commandCount = TrueSyncInput.GetInt(ownerId, iterKey++);

        for (int i = 0; i < commandCount; ++i)
        {
            CommandType type = (CommandType)TrueSyncInput.GetByte(ownerId, iterKey++);
            byte[] byteCommand = TrueSyncInput.GetByteArray(ownerId, iterKey++);
            string jsonCommand = System.Text.Encoding.UTF8.GetString(byteCommand);

            ICommand command = null;
            
            switch(type)
            {
                case CommandType.BUILD_TOWER:
                    command = JsonUtility.FromJson<BuildTowerCommand>(jsonCommand);
                    break;
                case CommandType.SPAWN_CREEP:
                    command = JsonUtility.FromJson<SpawnCreepCommand>(jsonCommand); 
                    break;
            }

            if(_onCommandExecute != null && type > 0 && command != null)
            {
                _onCommandExecute(ownerId, type, command);
            }

            Debug.LogError(command.commandType);
        }

        if(_onSyncedStep != null)
        {
            _onSyncedStep(TrueSyncManager.DeltaTime);
        }
    }
}
