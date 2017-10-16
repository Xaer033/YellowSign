﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class Commander : TrueSyncBehaviour
{
    
    public GameObject testPrefab;
    private Queue<ICommand> _commandQueue = new Queue<ICommand>();

    private CreepController _creepController;

    public void AddCommand(ICommand command)
    {
        _commandQueue.Enqueue(command);
    }
    
    public void Start()
    {
        Debug.Log("Start");
        _creepController = new CreepController();
    }

    public void Update()
    {
        bool lmd = Input.GetMouseButtonDown(0);
        bool rmd = Input.GetMouseButtonDown(1);
        bool mmd = Input.GetMouseButtonDown(2);

        //bool commandSent = true;

        if (lmd)
        {
            AddCommand(new BuildTowerCommand(2, 1, "poop_tower"));
        }
        if (rmd)
        {
            AddCommand(new SpawnCreepCommand("poop_creep"));
        }
        if(mmd)
        {
            AddCommand(new BuildTowerCommand(9, 9, "poop_tower"));
        }

        //float lerpFactor = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
        _creepController.Step(Time.deltaTime);
    }

    public override void OnSyncedStart()
    {
        TSRandom.instance.Initialize(42);
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

            int range = 5;
            switch(type)
            {
                case CommandType.BUILD_TOWER:
                    command = JsonUtility.FromJson<BuildTowerCommand>(jsonCommand);
                    break;
                case CommandType.SPAWN_CREEP:
                    command = JsonUtility.FromJson<SpawnCreepCommand>(jsonCommand);
                    for(int s = 0; s < 10; ++s)

                    {
                        TSVector pos = new TSVector(TSRandom.Range(-range, range), 0, TSRandom.Range(-range, range));
                        GameObject creep = TrueSyncManager.SyncedInstantiate(testPrefab, pos, TSQuaternion.identity);
                        _creepController.AddCreep(creep.GetComponent<TSTransform>());

                    }
                    break;
            }
            Debug.LogError(command.commandType);
            //Execute commands!
            //if (action > 0)
            //{
            //    Debug.LogError("Player: " + ownerId + " did action: " + action);
            //}

            //if(action == CommandType.BUILD_TOWER)
            //{
            //    TrueSyncManager.SyncedInstantiate()
            //}
        }

        _creepController.FixedStep(TrueSyncManager.DeltaTime);
    }

}
