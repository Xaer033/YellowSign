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
    private SyncStepper _syncStepper;
    private SyncStepper.Factory _syncFactory;

    
    public GameSystemManager(
        TowerSystem towerSystem,
        CreepSystem creepSystem,
        SyncStepper.Factory syncFactory)
    {
        _towerSystem = towerSystem;
        _creepSystem = creepSystem;
        _syncFactory = syncFactory;
    }

    public void Initialize()
    {
        _syncStepper = _syncFactory.Create();
        _syncStepper.onSyncedStep += onSyncStep;
        _syncStepper.onFrameUpdate += onFrameUpdate;

        _towerSystem.AddListener(GameplayEventType.TOWER_BUILT, onTowerBuilt);
    }

    public void CleanUp()
    {
        _syncStepper.onSyncedStep -= onSyncStep;
        _syncStepper.onFrameUpdate -= onFrameUpdate;

        _towerSystem.RemoveListener(GameplayEventType.TOWER_BUILT, onTowerBuilt);
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
    }
    
    private void onTowerBuilt(GeneralEvent e)
    {
        _creepSystem.recalculatePaths = true;
    }
}
