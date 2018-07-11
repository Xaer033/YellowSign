using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class PlayerController : TrueSyncBehaviour
{
    public GameObject highlighterPrefab; // Move this to external resources

    private Commander _commander;
    private Grid _grid;
    private CreepSystem _creepSystem;
    private TowerSystem _towerSystem;
    private GameObject _highlighter;
    private HashSet<GridPosition> _towerBlocker = new HashSet<GridPosition>();

    public void Setup(CreepSystem creepSystem, TowerSystem towerSystem)
    {
        _creepSystem = creepSystem;
        _towerSystem = towerSystem;
    }

    public void Start()
    {
        GameObject gridObj = GameObject.FindGameObjectWithTag("grid_p1");
        _grid = gridObj.GetComponent<Grid>();

        _commander = GetComponent<Commander>();
        _commander.onCommandExecute += OnCommandExecute;

        _highlighter = GameObject.Instantiate<GameObject>(highlighterPrefab);
        Debug.Log("Owner: " + localOwner.Id);
    }

    public void CleanUp()
    {
        _commander.onCommandExecute -= OnCommandExecute;
    }


    public void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 1.0f;

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        GridPosition pos;
        bool canBuildTower = _grid.CanBuildTower(ray, out pos);

        if(_highlighter && canBuildTower)
        {
            _highlighter.SetActive(true);
            _highlighter.transform.position = pos.ToVector3();
        }
        else
        {
            _highlighter.SetActive(false);
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(canBuildTower && !_towerBlocker.Contains(pos))
            {
                ICommand command = CommandFactory.CreateCommand(CommandType.BUILD_TOWER, new object[] { pos, "poop_tower" });
                _commander.AddCommand(command);
                _towerBlocker.Add(pos); // Prevents trying to add multiple towers to the same spot before next sync Update
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
           _commander.AddCommand(new SpawnCreepCommand("poop_creep"));
        }

        if(_creepSystem != null)
        {
            _creepSystem.Step(Time.deltaTime);
        }
    }
    
    private void OnCommandExecute(byte ownerId, CommandType type, ICommand command)
    {
        GameplayResources gResources = Singleton.instance.gameplayResources;

        switch(type)
        {
            case CommandType.SPAWN_CREEP:
                {
                    SpawnCreepCommand scc = (SpawnCreepCommand)command;
                    for (int s = 0; s < 5; ++s)
                    {
                        Transform spawnPoint = _grid.spawnPoints[TSRandom.Range(0, _grid.spawnPoints.Length)];
                        TSVector pos = spawnPoint.position.ToTSVector();
                        GameObject creepObj = TrueSyncManager.SyncedInstantiate(gResources.basicCreep, pos, TSQuaternion.identity);
                        Creep creep = new Creep(ownerId, creepObj.GetComponent<TSTransform>());
                        creep.Start((byte)(ownerId == 0 ? 1 : 0), _grid.target.transform.position);
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
                        Tower tower = gResources.towerFactory.Create("basic_tower", btc.position.ToTSVector(), TSQuaternion.identity);
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

    override public void OnSyncedUpdate()
    {
        if(_creepSystem != null)
        {
            _creepSystem.FixedStep(TrueSyncManager.DeltaTime);
        }
    }

    
}
