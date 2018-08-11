using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour
{
    private HashSet<GridPosition> _towerBlocker = new HashSet<GridPosition>();

    private Commander _commander;
    private Grid _grid;
    private TowerHighlighter _highlighter;
    private CreepSystem _creepSystem;
    private TowerSystem _towerSystem;
    private TowerDictionary _towerDictionary;
    private GameplayResources _gameplayResources;

    private string _currentTowerId;
    private Camera _camera;
    
    public PlayerSpawn playerSpawn { get; set; }

    [Inject]
    public void Construct(
        CreepSystem creepSystem,
        TowerSystem towerSystem,
        TowerDictionary towerDictionary,
        GameplayResources gameplayResources)
    {
        _creepSystem = creepSystem;
        _towerSystem = towerSystem;
        _towerDictionary = towerDictionary;
        _gameplayResources = gameplayResources;
        
        _highlighter = GameObject.Instantiate<TowerHighlighter>(_gameplayResources.highlighterPrefab);

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
            if(canBuildTower && !_towerBlocker.Contains(pos))
            {
                ICommand command = CommandFactory.CreateCommand(CommandType.BUILD_TOWER, new object[] { "basic_tower", pos });
                _commander.AddCommand(command);
                _towerBlocker.Add(pos); // Prevents trying to add multiple towers to the same spot before next sync Update
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
           _commander.AddCommand(new SpawnCreepCommand("basic_creep", 20));
        }

       
    }

    public TSPlayerInfo owner
    {
        get { return _commander.owner; }
    }

    public void SetCurrentTower(string towerId)
    {
        if(towerId != _currentTowerId)
        {
            _currentTowerId = towerId;

            _destroyAllChildren(_highlighter.root);

            TowerDef def = _towerDictionary.GetDef(towerId);
            GameObject gView = GameObject.Instantiate(def.view.gameObject, _highlighter.root);
            gView.transform.localPosition = Vector3.zero;
            gView.transform.rotation = Quaternion.identity;
            //GameObject.Destroy(towerView);
            ITowerView towerView = gView.GetComponent<ITowerView>();
            GameObject.Destroy(towerView.transformTS);
        }
    }

    private void _destroyAllChildren(Transform root)
    {
        int count = root.childCount;
        for(int i = count - 1; i >= 0; --i)
        {
            GameObject.Destroy(root.GetChild(i).gameObject);
        }
    }
    private void OnCommandExecute(byte ownerId, CommandType type, ICommand command)
    {
        switch(type)
        {
            case CommandType.SPAWN_CREEP:
                {
                    SpawnCreepCommand scc = (SpawnCreepCommand)command;
                    for (int s = 0; s < scc.count; ++s)
                    {
                        Transform spawnPoint = _grid.spawnPoints[s % _grid.spawnPoints.Length];
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
                        
                        _creepSystem.AddCreep(scc.type, spawnInfo);
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
}
