using GhostGen;
using System.Collections.Generic;
using TrueSync;

public class TowerSystem : EventDispatcher
{
    //private List<Tower> _towerList;
    private Tower.Factory _towerFactory;
    private GameState _gameState;
    
    public TowerSystem(GameState gameState, Tower.Factory towerFactory)
    {
        _towerFactory = towerFactory;
        _gameState = gameState;
        //_towerList = new List<Tower>(200);
    }

    public Tower AddTower(string towerId, TowerSpawnInfo spawnInfo)
    {
        Tower tower = _towerFactory.Create(towerId, spawnInfo);
        if(tower != null)
        {
            _towerList.Add(tower);
            DispatchEvent(GameplayEventType.TOWER_BUILT, false, tower);
        }
        return tower;
    }

    public void Step(float deltaTime)
    {
        //float lerpFactory = deltaTime * 10.0f;
        int count = _towerList.Count;
        for (int i = 0; i < count; ++i)
        {

            //TSTransform tsTransform = _creepList[i];
            //Transform t = tsTransform.transform;
            //tsTransform.UpdatePlayMode();
            //t.position = Vector3.Lerp(t.position, tsTransform.position.ToVector(), deltaTime * 10f);
            //t.position = Vector3.Lerp(t.position, tsTransform.position.ToVector(), lerpFactory);
        }
    }

    public void FixedStep(FP fixedDeltaTime)
    {
        int count = _towerList.Count;
        for (int i = count - 1; i >= 0; --i)
        {
            Tower t = _towerList[i];
            t.FixedStep(fixedDeltaTime);
        }
    }

    private List<Tower> _towerList
    {
        get
        {
            return _gameState.towerList;
        }
    }
}
