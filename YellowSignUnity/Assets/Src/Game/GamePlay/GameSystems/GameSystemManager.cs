using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using Zenject;
using TrueSync;

public class GameSystemManager : EventDispatcher
{
    private TowerSystem _towerSystem;
    private CreepSystem _creepSystem;
    private CreepHealthUISystem _creepHealthUISystem;
    private CreepViewSystem _creepViewSystem;
    private SyncStepper _syncStepper;
    private SyncStepper.Factory _syncFactory;

    
    public GameSystemManager(
        TowerSystem towerSystem,
        CreepSystem creepSystem,
        CreepViewSystem creepViewSystem,
        CreepHealthUISystem creepHealthUISystem,
        SyncStepper.Factory syncFactory)
    {
        _towerSystem = towerSystem;
        _creepSystem = creepSystem;
        _creepViewSystem = creepViewSystem;
        _creepHealthUISystem = creepHealthUISystem;
        _syncFactory = syncFactory;
    }

    public void Initialize()
    {
        _syncStepper = _syncFactory.Create();
        _syncStepper.onSyncedStep += onSyncStep;
        _syncStepper.onFrameUpdate += onFrameUpdate;

        _towerSystem.AddListener(GameplayEventType.TOWER_BUILT, onTowerBuilt);

        _creepSystem.AddListener(GameplayEventType.CREEP_REACHED_GOAL, onCreepGoalReached);
        _creepSystem.AddListener(GameplayEventType.CREEP_DAMAGED, onCreepDamaged);
        _creepSystem.AddListener(GameplayEventType.CREEP_KILLED, onCreepKilled);
        _creepSystem.AddListener(GameplayEventType.CREEP_SPAWNED, onCreepSpawned);
        
    }

    public void CleanUp()
    {
        _syncStepper.onSyncedStep -= onSyncStep;
        _syncStepper.onFrameUpdate -= onFrameUpdate;

        _towerSystem.RemoveListener(GameplayEventType.TOWER_BUILT, onTowerBuilt);

        _creepSystem.RemoveListener(GameplayEventType.CREEP_REACHED_GOAL, onCreepGoalReached);
        _creepSystem.RemoveListener(GameplayEventType.CREEP_DAMAGED, onCreepDamaged);
        _creepSystem.RemoveListener(GameplayEventType.CREEP_KILLED, onCreepKilled);
        _creepSystem.RemoveListener(GameplayEventType.CREEP_SPAWNED, onCreepSpawned);
    }


    private void onSyncStep(FP fixedDeltaTime)
    {
        _creepSystem.FixedStep(fixedDeltaTime);
        _towerSystem.FixedStep(fixedDeltaTime);
    }

    private void onFrameUpdate(float deltaTime)
    {
        _creepSystem.Step(deltaTime);
        _towerSystem.Step(deltaTime);

        _creepViewSystem.Step(deltaTime);
    }
    
    private void onTowerBuilt(GeneralEvent e)
    {
        _creepSystem.recalculatePaths = true;
    }


    private void onCreepGoalReached(GhostGen.GeneralEvent e)
    {
        Creep c = e.data as Creep;
        //Debug.Log("Creep Goal reached: " + c.targetOwnerId + " loses life" );
    }

    private void onCreepKilled(GhostGen.GeneralEvent e)
    {

        Creep c = e.data as Creep;
        //Debug.Log("Creep Death: " + c.ownerId + "'s creep died");
        _creepHealthUISystem.RemoveCreep(c);
    }

    private void onCreepSpawned(GhostGen.GeneralEvent e)
    {

        Creep c = e.data as Creep;
        //Debug.Log("Creep Spawned: " + c.stats.creepType.ToString() + " on playfield");
        _creepViewSystem.AddCreep(c);
        _creepHealthUISystem.AddCreep(c);
    }

    private void onCreepDamaged(GhostGen.GeneralEvent e)
    {

        AttackResult ar = (AttackResult)e.data;
        _creepHealthUISystem.ShowHealthOnCreep(ar.target as Creep);
        //Debug.LogFormat("Creep took '{0}' and has '{1}' remaining health!", ar.totalDamageDelt, ar.targetHealthRemaining);
    }
}
