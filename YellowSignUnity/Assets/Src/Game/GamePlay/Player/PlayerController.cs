using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    private HashSet<GridPosition> _towerBlocker = new HashSet<GridPosition>();

    private Commander _commander;
    private Grid _grid;
    private GameObject _highlighter;
    private Creep.Factory _creepFactory;
    private Tower.Factory _towerFactory;
    private GameplayResources _gameplayResources;
    private PlayerSpawn _playerSpawn;

    private Camera _camera;
    
    public PlayerNumber playerNumber { get; set; }

    public void Initialize(
        PlayerSpawn playerSpawn,
        Creep.Factory creepFactory, 
        Tower.Factory towerFactory,
        GameplayResources gameplayResources)
    {
        _playerSpawn = playerSpawn;
        _creepFactory = creepFactory;
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
           _commander.AddCommand(new SpawnCreepCommand("basic_creep"));
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
                        TSVector startingPos = spawnPoint.position.ToTSVector();
                        TSVector targetPos = _grid.target.transform.position.ToTSVector();

                        byte targetOwnerId = ownerId == (byte)1 ? (byte)2 : (byte)1;
                        TSQuaternion rotation = ownerId == (byte)1 ? TSQuaternion.identity : TSQuaternion.Euler(0, 180, 0);

                        CreepSpawnInfo spawnInfo = CreepSpawnInfo.Create(
                            ownerId,
                            startingPos, 
                            rotation,
                            targetOwnerId,
                            targetPos);
                        
                        Creep creep = _creepFactory.Create(scc.type, spawnInfo);
                    }
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
                        Tower tower = _towerFactory.Create(btc.type, spawnInfo);

                        _grid.UpdateGridPosition(tower.view.bounds);
                        _towerBlocker.Remove(btc.position);

                        Debug.Log("Tower BUilt");
                    }
                    else
                    {
                        Debug.Log("Tower Denied");
                    }

                    //_creepSystem.recalculatePaths = true;

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
