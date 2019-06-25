using System.Collections.Generic;
using ModestTree;
using TrueSync;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    public enum PlayerControlState
    {
        NONE = 0,
        TOWER_BUILDER,
        STATS_LOOKUP
    }

    private HashSet<GridPosition> _towerBlocker = new HashSet<GridPosition>();

    private Commander _commander;
    private Grid _myGrid;
    private List<Grid> _enemyGridList;

    private TowerHighlighter _highlighter;
    private WaveSpawnerSystem _waveSpawnerSystem;
    private TowerSystem _towerSystem;
    private TowerDictionary _towerDictionary;
    private GameplayResources _gameplayResources;

    private string _currentTowerId;
    private Camera _camera;
    private PlayerSpawn _playerSpawn;
    private PlayerHudController _hudController;
    private ActorSelector _actorSelector;
    private int _selectionMask;
    private Tower _selectedTower;

    private Vector3 _dragStartPos;
    private Vector3 _cameraStartPos;
    private bool _isDragging;
    private bool _canBuildTowerQuick;
    public PlayerControlState controlState { get; set; }
    
    public PlayerSpawn playerSpawn
    {
        get
        {
            return _playerSpawn;
        }
        set
        {
            _playerSpawn = value;
            setupGrid();
        }
    }

    public TSPlayerInfo owner
    {
        get { return _commander.owner; }
    }

    [Inject]
    public void Construct(
        WaveSpawnerSystem waveSpawnerSystem,
        TowerSystem towerSystem,
        TowerDictionary towerDictionary,
        GameplayResources gameplayResources)
    {
        _waveSpawnerSystem = waveSpawnerSystem;
        _towerSystem = towerSystem;
        _towerDictionary = towerDictionary;
        _gameplayResources = gameplayResources;
        
    }

    public void Awake()
    {
        Singleton.instance.diContainer.InjectGameObject(gameObject);
        controlState = PlayerControlState.TOWER_BUILDER;

        _commander = GetComponent<Commander>();
        _commander.onCommandExecute += OnCommandExecute;
        _commander.onSyncedStep += OnSyncStep;
        _commander.onSyncStartLocalPlayer += OnSyncStartLocalPlayer;

        _selectionMask = LayerMask.GetMask(new []{"tower"});
    }

    public void CleanUp()
    {
        if(_commander != null)
        {
            _commander.onCommandExecute -= OnCommandExecute;
            _commander.onSyncedStep -= OnSyncStep;
            _commander.onSyncStartLocalPlayer -= OnSyncStartLocalPlayer;
        }

        if (_actorSelector != null)
        {
            _actorSelector.RemoveListener(PlayerUIEventType.PRIMARY_SELECT, onPrimarySelect);
            _actorSelector.RemoveListener(PlayerUIEventType.SECONDARY_SELECT, onSecondarySelect);
            _actorSelector.RemoveListener(PlayerUIEventType.DRAG_BEGIN, onDragBegin);
            _actorSelector.RemoveListener(PlayerUIEventType.DRAG_END, onDragEnd);
        }

        if (_hudController != null)
        {
            _hudController.RemoveView(true);
        }
    }

    public void Update()
    {
        if(_myGrid == null)
        {
            return;
        }

        if(playerSpawn != null)
        {
            if((byte)playerSpawn.playerNumber != TrueSyncManager.LocalPlayer.Id)
            {
                return;
            }
        }

        switch(controlState)
        {
            case PlayerControlState.NONE:           _noneState();           break;
            case PlayerControlState.TOWER_BUILDER:  _towerBuilderState();   break;
        }
    }

    public void AddCommand(ICommand command)
    {
        Debug.Log("Base commander: " + _commander.ownerIndex);
        Debug.Log("PlayerIndex: " + _playerSpawn.playerNumber);
        _commander.AddCommand(command);
    }

    public void SetCurrentTower(string towerId)
    {
        if(towerId != _currentTowerId)
        {
            _currentTowerId = towerId;

            GameObjectUtilities.DestroyAllChildren(_highlighter.root);

            TowerDef def = _towerDictionary.GetDef(towerId);
            GameObject gView = GameObject.Instantiate(def.view.gameObject, _highlighter.root);
            ITowerView towerView = gView.GetComponent<ITowerView>();
            _highlighter.SetTower(towerView);
        }
    }

    private void OnCommandExecute(byte ownerId, CommandType type, ICommand command)
    {
        switch(type)
        {
            case CommandType.SPAWN_CREEP:
            {
                SpawnCreepCommand scc = (SpawnCreepCommand)command;
                spawnCreeps(ownerId, scc);
                break;
            }
            case CommandType.BUILD_TOWER:
            {
                BuildTowerCommand btc = (BuildTowerCommand)command;
                buildTower(ownerId, btc);
                break;
            }
        }
    }

    private void OnSyncStep(FP fixedDeltaTime)
    {
        //if(_creepSystem != null)
        //{
        //    _creepSystem.FixedStep(fixedDeltaTime);
        //}

        //if(_towerSystem != null)
        //{
        //    _towerSystem.FixedStep(fixedDeltaTime);
        //}   
    }

    private void OnSyncStartLocalPlayer()
    {
        Debug.Log("Sync Start");
        GameObject camObj = GameObject.FindWithTag("MainCamera");
        CameraMovement camMovement = camObj.GetComponent<CameraMovement>();
        camMovement.Setup(playerSpawn);
        _camera = camMovement.camera;

        _actorSelector = new ActorSelector(owner.Id, _camera, 25);
        _actorSelector.AddListener(PlayerUIEventType.PRIMARY_SELECT, onPrimarySelect);
        _actorSelector.AddListener(PlayerUIEventType.SECONDARY_SELECT, onSecondarySelect);
        _actorSelector.AddListener(PlayerUIEventType.DRAG_BEGIN, onDragBegin);
        _actorSelector.AddListener(PlayerUIEventType.DRAG_END, onDragEnd);
        
        _hudController = new PlayerHudController(this);
        _hudController.Start(null);
        
        
        _highlighter = GameObject.Instantiate<TowerHighlighter>(_gameplayResources.highlighterPrefab);
        _highlighter.gameObject.SetActive(false);
        
        SetCurrentTower("basic_tower");
    }

    private void onPrimarySelect(GhostGen.GeneralEvent e)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 1.0f;

        Ray ray = (_camera != null) ? _camera.ScreenPointToRay(mousePos) : default(Ray);
        GridPosition pos;
        bool canBuildTowerQuick = _myGrid.CanBuildTower(ray, true, out pos);
     

            //bool canBuildTowerDetailed = _grid.CanBuildTower(ray, true, out pos);
        if(canBuildTowerQuick && !_towerBlocker.Contains(pos))
        {
            ICommand command = new BuildTowerCommand(_currentTowerId, pos);
            _commander.AddCommand(command);
            _towerBlocker.Add(pos); // Prevents trying to add multiple towers to the same spot before next sync Update
        }
        else
        {
            ITowerView towerView;
            bool foundTower = _actorSelector.PickSelector<ITowerView>(ray, _selectionMask, out towerView);
            if (foundTower)
            {
                if (_selectedTower != null)
                {
                    _selectedTower.view.shouldShowRange = false;
                }
                _selectedTower = towerView.tower;
                Debug.Log("TowerView" + towerView.tower.ownerId);
                towerView.shouldShowRange = true;
            }
        }
    }

    private void onSecondarySelect(GhostGen.GeneralEvent e)
    {
        _commander.AddCommand(new SpawnCreepCommand("basic_creep", 20));
    }
    private void onDragBegin(GhostGen.GeneralEvent e )
    {
        _dragStartPos = (Vector3)e.data;
        _cameraStartPos = _camera.transform.position;
        _isDragging = true;
        if (_hudController != null)
        {
            _hudController.isSelectionActive = true;
            Debug.Log("Selection Active");
        }
    }
   
    private void onDragEnd(GhostGen.GeneralEvent e )
    {
        _isDragging = false;
        if (_hudController != null)
        {
            _hudController.isSelectionActive = false;
            Debug.Log("Selection De-active");
        }
        // Select a poop ton of stuff
        
    }

    private void _noneState()
    {
        if(_highlighter)
        {
            _highlighter.gameObject.SetActive(false);
        }
    }

    private void _towerBuilderState()
    {
        _canBuildTowerQuick = false;
        
       
        
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 1.0f;

        Ray ray = (_camera != null) ? _camera.ScreenPointToRay(mousePos) : default(Ray);
        GridPosition pos;
        _canBuildTowerQuick = _myGrid.CanBuildTower(ray, true, out pos);
        
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        if (_actorSelector != null)
        {
            _actorSelector.Tick();
        }
        
        if(_highlighter)
        {
            if(_canBuildTowerQuick)
            {
                _highlighter.gameObject.SetActive(true);
                _highlighter.transform.position = pos.ToVector3();
            }
            else
            {
                _highlighter.gameObject.SetActive(false);
            }
        }
        
        if (Input.GetMouseButton(0))
        {
            if (_isDragging && _hudController != null)
            {
                Vector3 camStart = _cameraStartPos;
                camStart.y = camStart.z;
                camStart = _camera.WorldToScreenPoint(camStart);
                
                Vector3 camPos = _camera.transform.position;
                camPos.y = camPos.z;
                camPos = _camera.WorldToScreenPoint(camPos);

                Vector3 camDelta = (camStart - camPos);// / Singleton.instance.gui.mainCanvas.scaleFactor;
                Debug.Log("CamPos: " + camPos + ", " + _cameraStartPos);
                camDelta.z = 0;
                _hudController.SetDragPoints(_dragStartPos + camDelta, Input.mousePosition);
            }
        }
    }

    private void setupGrid()
    {
        _enemyGridList = new List<Grid>(NetworkManager.kMaxPlayers);

        Grid[] gridList = GameObject.FindObjectsOfType<Grid>();
        List<TSPlayerInfo> playerList = TrueSyncManager.Players;

        foreach(Grid g in gridList)
        {
            if(g.playerNumber == playerSpawn.playerNumber)
            {
                _myGrid = g;
            }
            else
            {
                foreach(TSPlayerInfo info in playerList)
                {
                    if(info.Id == (byte)g.playerNumber)
                    {
                        _enemyGridList.Add(g);
                        break;
                    }
                }
            }
        }
    }

    private void spawnCreeps(byte ownerId, SpawnCreepCommand command)
    {
        // TODO: do this better...
        if (PhotonNetwork.countOfPlayers == 1)
        {
            addWave(_myGrid, ownerId, command);
        }
        else
        {
            for(var i = 0; i < _enemyGridList.Count; ++i)
            {
                Grid enemyGrid = _enemyGridList[i];

                addWave(enemyGrid, ownerId, command);
            }
        }
    }

    private void addWave(Grid grid, byte ownerId, SpawnCreepCommand command)
    {
        TSQuaternion rotation = TSQuaternion.Euler(0, 180, 0);

        Transform spawnPoint = grid.spawnPoint;
        TSVector startingPos = spawnPoint.position.ToTSVector();
        TSVector targetPos = grid.target.position.ToTSVector();

        byte targetOwnerId = (byte)grid.playerNumber;

        CreepSpawnInfo spawnInfo = CreepSpawnInfo.Create(
            ownerId,
            startingPos,
            rotation,
            targetOwnerId,
            targetPos);

        SpawnWaveInfo wave = new SpawnWaveInfo();
        wave.betweenSpawnDelay = 0.5;
        wave.spawnCommand = command;
        wave.spawnInfo = spawnInfo;

        _waveSpawnerSystem.AddWave(wave);
        Debug.Log("Adding Wave from: " + ownerId + ", to " + targetOwnerId);
    }
    
    private void buildTower(byte ownerId, BuildTowerCommand command)
    {
        if(_myGrid.CanBuildTowerAtPos(command.position))
        {
            TSVector pos = command.position.ToTSVector();
            TSQuaternion rotation = TSQuaternion.identity;

            TowerSpawnInfo spawnInfo = TowerSpawnInfo.Create(ownerId, pos, rotation);
            Tower tower = _towerSystem.AddTower(command.type, spawnInfo);

            _myGrid.UpdateGridPosition(tower.view.bounds);
            _towerBlocker.Remove(command.position);
        }
        else
        {
            Debug.Log("Tower Denied");
        }
    }
}
