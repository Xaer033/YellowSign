using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class PlayerController : TrueSyncBehaviour
{
    private Commander _commander;
    private Grid _grid;

    private CreepSystem _creepSystem;
    private TowerSystem _towerSystem;

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
    }

    public void CleanUp()
    {
        _commander.onCommandExecute -= OnCommandExecute;
    }


    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 1.0f;

            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            GridPosition pos;
            bool canBuildTower = _grid.CanBuildTower(ray, out pos);
            if(canBuildTower)
            {
                _commander.AddCommand(new BuildTowerCommand(pos.x, pos.z, "poop_tower"));
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
                        GameObject creepObj = TrueSyncManager.SyncedInstantiate(_commander.testPrefab, pos, TSQuaternion.identity);
                        Creep creep = new Creep(creepObj.GetComponent<TSTransform>());
                        creep.Start(_grid.target.transform.position);
                        _creepSystem.AddCreep(ownerId, creep);
                    }
                    break;
                }
            case CommandType.BUILD_TOWER:
                {
                    BuildTowerCommand btc = (BuildTowerCommand)command;
                    TSVector pos = btc.position.ToTSVector();
                    GameObject towerObj = TrueSyncManager.SyncedInstantiate(gResources.basicTower, pos, TSQuaternion.identity);
                    Collider towerCollider = towerObj.GetComponent<Collider>();
                    _grid.UpdateGridPosition(towerCollider.bounds);
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
