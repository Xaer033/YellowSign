using System;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using Zenject;

public class Commander : TrueSyncBehaviour
{
    private Queue<ICommand> _commandQueue = new Queue<ICommand>();

    private event Action _onStart;
    private event Action<byte, CommandType, ICommand> _onCommandExecute;
    private event Action<FP> _onSyncedStep;
    private event Action<float> _onFrameUpdate;


    public event Action onStart
    {
        add { _onStart += value; }
        remove { _onStart -= value; }
    }

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

    public event Action<float> onFrameUpdate
    {
        add { _onFrameUpdate += value; }
        remove { _onFrameUpdate -= value; }
    }

    public void AddCommand(ICommand command)
    {
        _commandQueue.Enqueue(command);
    }

    public override void OnSyncedStart()
    {
        TSRandom.instance.Initialize(42);
        if(Singleton.instance != null)
        {
            Singleton.instance.notificationDispatcher.DispatchEvent(GameplayEventType.GAME_START);
        }

        if(_onStart != null)
        {
            _onStart();
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
            byte[] byteCommand = CommandFactory.ToByteArray(ct);
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
            ICommand command = CommandFactory.CreateFromByteArray(type, byteCommand);

            Debug.LogWarningFormat("Owner: {0}, Type: {1}", ownerId, command.commandType);

            if(_onCommandExecute != null && type > 0 && command != null)
            {
                _onCommandExecute(ownerId, type, command);
            }
        }

        if(_onSyncedStep != null)
        {
            _onSyncedStep(TrueSyncManager.DeltaTime);
        }
    }

    public void Update()
    {
        if(_onFrameUpdate != null)
        {
            _onFrameUpdate(Time.deltaTime);
        }
    }
}
