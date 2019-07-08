using System.Collections.Generic;
using System.Collections.ObjectModel;
using GhostGen;
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
    private GameState _gameState;
    
    private string _currentTowerId;
    private CameraMovement _camera;
    private PlayerSpawn _playerSpawn;
    private PlayerHudController _hudController;
    private ActorSelector _actorSelector;
    private int _selectionMask;
    private Tower _selectedTower;
    

    private Vector3 _dragStartPos;
    private Vector3 _cameraStartPos;
    private bool _isDragging;
    private bool _canBuildTowerQuick;
    private Plane[] _selectionFrustumPlanes;
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
        GameplayResources gameplayResources,
        GameState gameState)
    {
        _waveSpawnerSystem = waveSpawnerSystem;
        _towerSystem = towerSystem;
        _towerDictionary = towerDictionary;
        _gameplayResources = gameplayResources;
        _gameState = gameState;
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

        _selectionFrustumPlanes = new Plane[6];
        
        _highlighter = Instantiate<TowerHighlighter>(_gameplayResources.highlighterPrefab);
        _highlighter.gameObject.SetActive(false);
        
        SetCurrentTower("basic_tower");
    }

    public void CleanUp()
    {
        if(_commander != null)
        {
            _commander.onCommandExecute -= OnCommandExecute;
            _commander.onSyncedStep -= OnSyncStep;
            _commander.onSyncStartLocalPlayer -= OnSyncStartLocalPlayer;
        }
        
        if (_hudController != null)
        {
            _hudController.RemoveView(true);
        }

        if (_actorSelector != null)
        {
            _actorSelector.onPrimarySelect -= onPrimarySelect;
            _actorSelector.onSecondarySelect -= onSecondarySelect;
            _actorSelector.onPrimaryDoubleSelect -= onPrimaryDoubleSelect;
            _actorSelector.onDragBegin -= onDragBegin;
            _actorSelector.onDragEnd -= onDragEnd;
            _actorSelector = null;
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
      
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        if (_actorSelector != null)
        {
            _actorSelector.Tick();
        }
//        GeometryUtility.CalculateFrustumPlanes()
        
        if (_isDragging)
        {
            Vector3 screenSpaceStart = _dragStartPos;
            Vector3 screenSpaceCurrent = Input.mousePosition;
            
            if(_hudController != null)
            {
                _hudController.SetDragPoints(screenSpaceStart, screenSpaceCurrent);
            }

            _handleMarqueSelection(screenSpaceStart, screenSpaceCurrent);
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
            GameObject gView = Instantiate(def.view.gameObject, _highlighter.root);
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
        _camera = camObj.GetComponent<CameraMovement>();
        _camera.Setup(playerSpawn);

        _hudController = new PlayerHudController(this);
        _hudController.Start(null);
        
        _actorSelector = new ActorSelector(owner.Id, 25);
        _actorSelector.onPrimarySelect += onPrimarySelect;
        _actorSelector.onPrimaryDoubleSelect += onPrimaryDoubleSelect;
        _actorSelector.onSecondarySelect += onSecondarySelect;
        _actorSelector.onDragBegin += onDragBegin;
        _actorSelector.onDragEnd += onDragEnd;
        _actorSelector.onSelectionChanged += onSelectionChanged;

    }

    private void onPrimarySelect(Vector3 mousePosition)
    {
        Vector3 mousePos = mousePosition;
        mousePos.z = 1.0f;

        Ray ray = _camera != null ? _camera.camera.ScreenPointToRay(mousePos) : default(Ray);

        bool hasFloorPosition = _myGrid.GetFloorPosition(ray, out var floorPosition);
        
        GridPosition gridPos = GridPosition.Create(-1, -1);
        bool canBuildTower = hasFloorPosition && controlState == PlayerControlState.TOWER_BUILDER && _myGrid.CanBuildTowerFromPosition(floorPosition, true, out gridPos);

        //bool canBuildTowerDetailed = _grid.CanBuildTower(ray, true, out pos);
        if(canBuildTower && !_towerBlocker.Contains(gridPos))
        {
            ICommand command = new BuildTowerCommand(_currentTowerId, gridPos);
            _commander.AddCommand(command);
            _towerBlocker.Add(gridPos); // Prevents trying to add multiple towers to the same spot before next sync Update
        }
        else
        {
            ITowerView towerView;
            bool foundTower = _actorSelector.PickSelector<ITowerView>(ray, _selectionMask, out towerView);
            if (foundTower)
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    _actorSelector.ClearSelectedActors();
                }

                if (!towerView.isSelected)
                {
                    _actorSelector.SelectActor(towerView);
                }
            }
        }
    }

    private void onSecondarySelect(Vector3 mousePosition)
    {
        _actorSelector.ClearSelectedActors();
//        _commander.AddCommand(new SpawnCreepCommand("basic_creep", 20));
    }

    private void onPrimaryDoubleSelect(Vector3 mousePosition)
    {
        Vector3 mousePos = mousePosition;
        mousePos.z = 1.0f;

        Ray ray = (_camera != null) ? _camera.camera.ScreenPointToRay(mousePos) : default(Ray);
        bool hasFloorPosition = _myGrid.GetFloorPosition(ray, out var floorPosition);
        
        if (hasFloorPosition)
        {
            ITowerView towerView;
            bool foundTower = _actorSelector.PickSelector<ITowerView>(ray, _selectionMask, out towerView);
            if (foundTower)
            {
                for (int i = 0; i < _gameState.towerList.Count; ++i)
                {
                    Tower t = _gameState.towerList[i];
                    if (towerView.tower.type == t.type)
                    {
                        _actorSelector.SelectActor(t.view);
                    }
                }
            }
        }
    }
    
    private void onDragBegin(Vector3 mousePosition)
    {
        _isDragging = true;
        _dragStartPos = mousePosition;
            
        if (_hudController != null)
        {
            _hudController.isSelectionActive = true;
            Debug.Log("Selection Active");
        }

        if (_camera != null)
        {
            _camera.isLocked = true;
        }

        controlState = PlayerControlState.NONE;
    }
   
    private void onDragEnd(DragEndEventData dragEndData )
    {
        _isDragging = false;
        
        if (_hudController != null)
        {
            _hudController.isSelectionActive = false;
            Debug.Log("Selection De-active");
        }

        if (_camera != null)
        {
            _camera.isLocked = false;
        }
        // Select a poop ton of stuff
        
    }

    private void onSelectionChanged(ReadOnlyCollection<IActor> selectedActors)
    {
        if (_hudController != null)
        {
            _hudController.UpdateSelectedActors(selectedActors);
        }
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

        Ray ray = (_camera != null) ? _camera.camera.ScreenPointToRay(mousePos) : default(Ray);
        bool canBuildTower = _myGrid.CanBuildTowerFromRay(ray, true, out var gridPos);

        if(_highlighter)
        {
            if(canBuildTower)
            {
                _highlighter.gameObject.SetActive(true);
                _highlighter.transform.position = gridPos.ToVector3();
            }
            else
            {
                _highlighter.gameObject.SetActive(false);
            }
        }
    }

    private void setupGrid()
    {
        _enemyGridList = new List<Grid>(NetworkManager.kMaxPlayers);

        Grid[] gridList = FindObjectsOfType<Grid>();
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
        if(_myGrid.CanBuildTowerAtPosition(command.position))
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

    private bool generateFrustum(Camera camera, Vector3 screenSpacePoint_1, Vector3 screenSpacePoint_2, ref Plane[] frustumPlanes)
    {
        if (frustumPlanes == null)
        {
            Debug.LogError("FrustumPlanes array is null, can't generate frustum");
            return false;
        }

        if (camera == null)
        {
            Debug.LogError("Camera is null1");
            return false;
        }

        Vector3 viewSpacePoint_1 = camera.ScreenToViewportPoint(screenSpacePoint_1);
        Vector3 viewSpacePoint_2 = camera.ScreenToViewportPoint(screenSpacePoint_2);
        
        Vector3 min = Vector3.Min(viewSpacePoint_1, viewSpacePoint_2);
        Vector3 max = Vector3.Max(viewSpacePoint_1, viewSpacePoint_2);
            
        Vector3 size = max - min;
        
        // TODO: Create these earlier and re-use them
        Rect viewport = new Rect(min, size);

//        Debug.Log(viewport);
        
        Vector3[] nearVerts = new Vector3[4];
        Vector3[] farVerts = new Vector3[4];
        
        camera.CalculateFrustumCorners(viewport, camera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearVerts);
        camera.CalculateFrustumCorners(viewport, camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, farVerts);

        for (int i = 0; i < 4; ++i)
        {
            nearVerts[i] = camera.transform.TransformPoint(nearVerts[i]);
            farVerts[i] = camera.transform.TransformPoint(farVerts[i]);

            Debug.DrawLine(camera.transform.position, nearVerts[i], Color.red);
            Debug.DrawLine(camera.transform.position, farVerts[i], Color.green);
        }

        //Near
        /* red 0*/       Debug.DrawRay(nearVerts[0], Vector3.down + Vector3.left,   Color.red, 1.0f);
        /* blue 1*/      Debug.DrawRay(nearVerts[1], Vector3.up + Vector3.left,     Color.blue, 1.0f);
        /* magenta 2*/   Debug.DrawRay(nearVerts[2], Vector3.up + Vector3.right,    Color.magenta, 1.0f);
        /* yellow 3*/    Debug.DrawRay(nearVerts[3], Vector3.down + Vector3.right,  Color.yellow, 1.0f);
       
        //Far
         /* red 0*/       Debug.DrawRay(farVerts[0], Vector3.down + Vector3.left,  Color.red, 1.0f);
         /* blue 1*/      Debug.DrawRay(farVerts[1], Vector3.up + Vector3.left,    Color.blue, 1.0f);
         /* magenta 2*/   Debug.DrawRay(farVerts[2], Vector3.up + Vector3.right,   Color.magenta, 1.0f);
         /* yellow3 */    Debug.DrawRay(farVerts[3], Vector3.down + Vector3.right, Color.yellow, 1.0f);
        
        frustumPlanes[0].Set3Points(nearVerts[0], nearVerts[1], nearVerts[2]);
        frustumPlanes[1].Set3Points(farVerts[1], nearVerts[1], nearVerts[0]);
        frustumPlanes[2].Set3Points(nearVerts[2], nearVerts[1], farVerts[1]);
        frustumPlanes[3].Set3Points(nearVerts[2], farVerts[2], farVerts[3]);
        frustumPlanes[4].Set3Points(nearVerts[3], farVerts[3], farVerts[0]);
        frustumPlanes[5].Set3Points(farVerts[2], farVerts[1], farVerts[0]);

        frustumPlanes[0].Flip();
        frustumPlanes[1].Flip();
        frustumPlanes[2].Flip();
        frustumPlanes[3].Flip();
        frustumPlanes[4].Flip();
        frustumPlanes[5].Flip();
        // Debugging
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MeshFilter filter = p.GetComponent<MeshFilter>();
            Mesh m = filter.mesh;
            Vector3[] posList = m.vertices;
            Vector3[] normalList = m.normals;
            int[] triangleList = m.triangles;

            triangleList[0] = 0;
            triangleList[1] = 1;
            triangleList[2] = 2;
            triangleList[3] = 2;
            triangleList[4] = 3;
            triangleList[5] = 0;
            posList[0]  = nearVerts[0];
            posList[1]  = nearVerts[1];
            posList[2]  = nearVerts[2];
            posList[3]  = nearVerts[3];
            normalList[0] = frustumPlanes[0].normal;
            normalList[1] = frustumPlanes[0].normal;
            normalList[2] = frustumPlanes[0].normal;
            normalList[3] = frustumPlanes[0].normal;
                
                
            triangleList[6] = 4;
            triangleList[7] = 5;
            triangleList[8] = 6;
            triangleList[9] = 6;
            triangleList[10] = 7;
            triangleList[11] = 4;
            posList[4]  = farVerts[1];
            posList[5]  = nearVerts[1];
            posList[6]  = nearVerts[0];
            posList[7]  = farVerts[0];
            normalList[4] = frustumPlanes[1].normal;
            normalList[5] = frustumPlanes[1].normal;
            normalList[6] = frustumPlanes[1].normal;
            normalList[7] = frustumPlanes[1].normal;
            
            triangleList[12] = 8;
            triangleList[13] = 9;
            triangleList[14] = 10;
            triangleList[15] = 10;
            triangleList[16] = 11;
            triangleList[17] = 8;
            posList[8]  = nearVerts[2];
            posList[9]  = nearVerts[1];
            posList[10] = farVerts[1];
            posList[11] = farVerts[2];
            normalList[8] = frustumPlanes[2].normal;
            normalList[9] = frustumPlanes[2].normal;
            normalList[10] = frustumPlanes[2].normal;
            normalList[11] = frustumPlanes[2].normal;

            posList[12] = nearVerts[2];
            posList[13] = farVerts[2];
            posList[14] = farVerts[3];
            posList[15] = nearVerts[3];
            normalList[12] = frustumPlanes[3].normal;
            normalList[13] = frustumPlanes[3].normal;
            normalList[14] = frustumPlanes[3].normal;
            normalList[15] = frustumPlanes[3].normal;

            posList[16] = nearVerts[3];
            posList[17] = farVerts[3];
            posList[18] = farVerts[0];
            posList[19] = nearVerts[0];
            normalList[16] = frustumPlanes[4].normal;
            normalList[17] = frustumPlanes[4].normal;
            normalList[18] = frustumPlanes[4].normal;
            normalList[19] = frustumPlanes[4].normal;

            posList[20] = farVerts[2];
            posList[21] = farVerts[1];
            posList[22] = farVerts[0];
            posList[23] = farVerts[3];
            normalList[20] = frustumPlanes[5].normal;
            normalList[21] = frustumPlanes[5].normal;
            normalList[22] = frustumPlanes[5].normal;
            normalList[23] = frustumPlanes[5].normal;

            m.vertices = posList;
            m.normals = normalList;
            m.triangles = triangleList;
        }
        return true;
    }

    private void _handleMarqueSelection(Vector3 screenSpaceStart, Vector3 screenSpaceCurrent)
    {
        bool hasFrustum = generateFrustum(_camera.camera, screenSpaceStart, screenSpaceCurrent, ref _selectionFrustumPlanes);

        if (!hasFrustum)
        {
            return;
        }

        for (int i = 0; i < _gameState.towerList.Count; ++i)
        {
            Tower t = _gameState.towerList[i];
            if (t == null)
            {
                continue;
            }

            IActor view = t.view;
            if (GeometryUtility.TestPlanesAABB(_selectionFrustumPlanes, view.bounds))
            {
                _actorSelector.SelectActor(view);
            }
            else
            {
                _actorSelector.DeselectActor(view);
            }
        }
    }
}
