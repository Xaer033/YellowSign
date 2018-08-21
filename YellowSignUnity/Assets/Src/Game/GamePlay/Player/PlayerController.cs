using System.Collections.Generic;
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
    private Grid _grid;
    private TowerHighlighter _highlighter;
    private WaveSpawnerSystem _waveSpawnerSystem;
    private TowerSystem _towerSystem;
    private TowerDictionary _towerDictionary;
    private GameplayResources _gameplayResources;

    private string _currentTowerId;
    private Camera _camera;
    private PlayerSpawn _playerSpawn;

    
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
        
        _highlighter = GameObject.Instantiate<TowerHighlighter>(_gameplayResources.highlighterPrefab);
        _highlighter.gameObject.SetActive(false);
    }

    public void Awake()
    {
        controlState = PlayerControlState.TOWER_BUILDER;
        
        _commander = GetComponent<Commander>();
        _commander.onCommandExecute += OnCommandExecute;
        _commander.onSyncedStep += OnSyncStep;
        _commander.onSyncStartLocalPlayer += OnSyncStartLocalPlayer;
        
        Debug.Log("Owner: " + _commander.localOwner.Id);
    }

    public void CleanUp()
    {
        _commander.onCommandExecute -= OnCommandExecute;
        _commander.onSyncedStep -= OnSyncStep;
    }

    public void Update()
    {
        if(_grid == null)
        {
            return;
        }

        switch(controlState)
        {
            case PlayerControlState.NONE:           _noneState();           break;
            case PlayerControlState.TOWER_BUILDER:  _towerBuilderState();   break;
        }
    }

    public void AddCommand(ICommand command)
    {
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
                    //for (int s = 0; s < scc.count; ++s)
                    //{
                        Transform spawnPoint = _grid.spawnPoint;
                        TSVector startingPos = spawnPoint.position.ToTSVector();
                        TSVector targetPos = _grid.target.transform.position.ToTSVector();

                        byte targetOwnerId = ownerId == (byte)1 ? (byte)2 : (byte)1;
                        TSQuaternion rotation = ownerId == (byte)1 ? TSQuaternion.Euler(0, 180, 0) : TSQuaternion.identity;

                        CreepSpawnInfo spawnInfo = CreepSpawnInfo.Create(
                            ownerId,
                            startingPos, 
                            rotation,
                            targetOwnerId,
                            targetPos);

                    SpawnWaveInfo wave = new SpawnWaveInfo();
                    wave.betweenSpawnDelay = 0.5;
                    wave.spawnCommand = scc;
                    wave.spawnInfo = spawnInfo;

                    _waveSpawnerSystem.AddWave(wave);
                        //_creepSystem.AddCreep(scc.type, spawnInfo);
                    //}
                    break;
                }
            case CommandType.BUILD_TOWER:
                {
                    BuildTowerCommand btc = (BuildTowerCommand)command;

                    if(_grid.CanBuildTowerAtPos(btc.position))
                    {
                        TSVector pos = btc.position.ToTSVector();
                        TSQuaternion rotation = ownerId == (byte)1 ? TSQuaternion.identity : TSQuaternion.Euler(0, 180, 0);

                        TowerSpawnInfo spawnInfo = TowerSpawnInfo.Create(ownerId, pos, rotation);
                        Tower tower = _towerSystem.AddTower(btc.type, spawnInfo);

                        _grid.UpdateGridPosition(tower.view.bounds);
                        _towerBlocker.Remove(btc.position);
                    }
                    else
                    {
                        Debug.Log("Tower Denied");
                    }

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

        //GameObject cameraObj = GameObject.Instantiate<GameObject>(
        //        _gameplayResources.gameplayCamera, _playerSpawn.cameraHook);

        //_camera = cameraObj.GetComponent<Camera>();
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
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 1.0f;

        Ray ray = (_camera != null) ? _camera.ScreenPointToRay(mousePos) : default(Ray);
        GridPosition pos;
        bool canBuildTowerQuick = _grid.CanBuildTower(ray, true, out pos);
        if(_highlighter)
        {
            if(canBuildTowerQuick)
            {
                _highlighter.gameObject.SetActive(true);
                _highlighter.transform.position = pos.ToVector3();
            }
            else
            {
                _highlighter.gameObject.SetActive(false);
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            //bool canBuildTowerDetailed = _grid.CanBuildTower(ray, true, out pos);
            if(canBuildTowerQuick && !_towerBlocker.Contains(pos))
            {
                ICommand command = CommandFactory.CreateCommand(CommandType.BUILD_TOWER, new object[] { _currentTowerId, pos });
                _commander.AddCommand(command);
                _towerBlocker.Add(pos); // Prevents trying to add multiple towers to the same spot before next sync Update
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            _commander.AddCommand(new SpawnCreepCommand("basic_creep", 20));
        }
    }

    private void setupGrid()
    {
        GameObject gridObj = GameObject.FindGameObjectWithTag("grid_p1");
        Grid[] gridList = gridObj.GetComponents<Grid>();
        foreach(Grid g in gridList)
        {
            if(g.playerNumber == playerSpawn.playerNumber)
            {
                _grid = g;
                break;
            }
        }
    }
}
