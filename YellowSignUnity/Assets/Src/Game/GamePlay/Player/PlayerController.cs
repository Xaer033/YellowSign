using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    private Commander _commander;
    private Grid _grid;
    private GameObject _highlighter;
    private HashSet<GridPosition> _towerBlocker = new HashSet<GridPosition>();
    private CreepSystem _creepSystem;
    private TowerSystem _towerSystem;
    private Tower.Factory _towerFactory;
    private GameplayResources _gameplayResources;
    private PlayerSpawn _playerSpawn;

    private Camera _camera;
    
    public PlayerNumber playerNumber { get; set; }

    public void Initialize(
        PlayerSpawn playerSpawn,
        CreepSystem creepSystem, 
        TowerSystem towerSystem, 
        Tower.Factory towerFactory,
        GameplayResources gameplayResources)
    {
        _playerSpawn = playerSpawn;
        _creepSystem = creepSystem;
        _towerSystem = towerSystem;
        _towerFactory = towerFactory;
        _gameplayResources = gameplayResources;

        _highlighter = GameObject.Instantiate<GameObject>(_gameplayResources.highlighterPrefab);
        playerNumber = playerSpawn.playerNumber;

        if(TrueSyncManager.LocalPlayer.Id == owner.Id)
        {
            GameObject cameraObj = GameObject.Instantiate<GameObject>(
                    _gameplayResources.gameplayCamera, _playerSpawn.cameraHook);

            CameraMovement camMovement = cameraObj.GetComponent<CameraMovement>();
            camMovement.playerNumber = playerNumber;
            _camera = camMovement.camera;
        }
    }

    public void Start()
    {        
        GameObject gridObj = GameObject.FindGameObjectWithTag("grid_p1");
        _grid = gridObj.GetComponent<Grid>();

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
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 1.0f;

        Ray ray = (_camera != null) ? _camera.ScreenPointToRay(mousePos) : default(Ray);
        GridPosition pos;
        bool canBuildTower = _grid.CanBuildTower(ray, out pos);

        if(_highlighter)
        {
            if(canBuildTower)
            {
                _highlighter.SetActive(true);
                _highlighter.transform.position = pos.ToVector3();
            }
            else
            {
                _highlighter.SetActive(false);
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(canBuildTower && !_towerBlocker.Contains(pos))
            {
                ICommand command = CommandFactory.CreateCommand(CommandType.BUILD_TOWER, new object[] { "basic_tower", pos });
                _commander.AddCommand(command);
                _towerBlocker.Add(pos); // Prevents trying to add multiple towers to the same spot before next sync Update
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
           _commander.AddCommand(new SpawnCreepCommand("poop_creep"));
        }

       
    }

    public TSPlayerInfo owner
    {
        get { return _commander.owner; }
    }
    
    private void OnCommandExecute(byte ownerId, CommandType type, ICommand command)
    {
        switch(type)
        {
            case CommandType.SPAWN_CREEP:
                {
                    SpawnCreepCommand scc = (SpawnCreepCommand)command;
                    for (int s = 0; s < 5; ++s)
                    {
                        Transform spawnPoint = _grid.spawnPoints[TSRandom.Range(0, _grid.spawnPoints.Length)];
                        TSVector pos = spawnPoint.position.ToTSVector();
                        GameObject creepObj = TrueSyncManager.SyncedInstantiate(_gameplayResources.basicCreep, pos, TSQuaternion.identity);
                        ICreepView creepView = creepObj.GetComponent<ICreepView>();
                        
                        Creep creep = new Creep(ownerId, _gameplayResources.basicCreepStats, creepView);         
                        creep.Start(ownerId, _grid.target.transform.position);
                       
                        _creepSystem.AddCreep(ownerId, creep);
                    }
                    break;
                }
            case CommandType.BUILD_TOWER:
                {
                    BuildTowerCommand btc = (BuildTowerCommand)command;

                    if(_grid.CanBuildTowerAtPos(btc.position))
                    {
                        TSVector pos = btc.position.ToTSVector();
                        Tower tower = _towerFactory.Create(btc.type, pos, TSQuaternion.identity);
                        _towerSystem.AddTower(tower);

                        _grid.UpdateGridPosition(tower.view.bounds);
                        _towerBlocker.Remove(btc.position);

                        Debug.Log("Tower BUilt");
                    }
                    else
                    {
                        Debug.Log("Tower Denied");
                    }

                    _creepSystem.recalculatePaths = true;

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
        //GameObject cameraObj = GameObject.Instantiate<GameObject>(
        //        _gameplayResources.gameplayCamera, _playerSpawn.cameraHook);

        //_camera = cameraObj.GetComponent<Camera>();
    }
}
