using System;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Zenject;
using GhostGen;

public class SyncStepper  : TrueSyncBehaviour
{
    public class Factory : PlaceholderFactory<SyncStepper>, IValidatable
    {
        private DiContainer _container;
        private GameObject _syncPrefab;

        public Factory(GameObject syncPrefab, DiContainer container)
        {
            _syncPrefab = syncPrefab;
            _container = container;
        }

        public override SyncStepper Create()
        {
            GameObject gObject = TrueSyncManager.SyncedInstantiate(_syncPrefab);
            _container.InjectGameObject(gObject);
            SyncStepper commander = gObject.GetComponent<SyncStepper>();
            return commander;
        }

        public override void Validate()
        {
            _container.Instantiate<SyncStepper>();
        }
    }

    private IEventDispatcher _eventDispatcher;

    [Inject]
    public void Construct(
        [Inject(Id = GameInstaller.GLOBAL_DISPATCHER)]
        IEventDispatcher eventDispatcher)
    {
        _eventDispatcher = eventDispatcher;
    }
    
    private event Action _onSyncStart;
    private event Action _onSyncStartLocalPlayer;
    private event Action<FP> _onSyncedStep;
    private event Action<float> _onFrameUpdate;


    public event Action onSyncStart
    {
        add { _onSyncStart += value; }
        remove { _onSyncStart -= value; }
    }

    public event Action onSyncStartLocalPlayer
    {
        add { _onSyncStartLocalPlayer += value; }
        remove { _onSyncStartLocalPlayer -= value; }
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
    
    public override void OnSyncedStart()
    {
        if(_onSyncStart != null)
        {
            _onSyncStart();
        }
    }

    public override void OnSyncedStartLocalPlayer()
    {
        if(_onSyncStartLocalPlayer != null)
        {
            _onSyncStartLocalPlayer();
        }
    }

    public override void OnSyncedUpdate()
    {
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
