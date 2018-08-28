using System.Collections;
using System.Collections.Generic;
using TrueSync;
using GhostGen;
using UnityEngine;

public class CreepSystem : EventDispatcher
{
    private const int kMaxCreeps = 200;

    //private Dictionary<byte, List<Creep>> _creeps;
    private List<Pathfinding.Path> _generatingPath;
    private Creep.Factory _creepFactory;
    private GameState _gameState;

    public bool recalculatePaths { set; get; }
    
    public CreepSystem(GameState gameState, Creep.Factory creepFactory)
    {
        _creepFactory = creepFactory;
        _gameState = gameState;

        int maxPlayers = NetworkManager.kMaxPlayers;
        //_creeps = new Dictionary<byte, List<Creep>>(maxPlayers);

        //for(int i = 0; i < maxPlayers; ++i)
        //{
        //    _creeps.Add((byte)(i + 1), new List<Creep>(kMaxCreeps));
        //}

        _generatingPath = new List<Pathfinding.Path>(kMaxCreeps);
    }

    public Creep AddCreep(string creepId, CreepSpawnInfo spawnInfo)
    {
        Creep creep = _creepFactory.Create(creepId, spawnInfo);
        if(creep != null)
        {
            List<Creep> creepList = GetCreepList(spawnInfo.ownerId);

            creepList.Add(creep);
            creep.AddListener(GameplayEventType.CREEP_DAMAGED, onCreepDamaged);

            DispatchEvent(GameplayEventType.CREEP_SPAWNED, false, creep);
        }

        return creep;
    }

    //public void RemoveCreep
    public List<Creep> GetCreepList(byte ownerId)
    {
        return _gameState.creepList;
        //if(ownerId < 0 || ownerId > NetworkManager.kMaxPlayers)
        //{
        //    Debug.LogError("Owner is out of range!");
        //    return null;
        //}
        //return _creeps[ownerId];
    }

    public void Step(float deltaTime)
    {
        //float lerpFactory = deltaTime * 10.0f;

        //for(byte o = 1; o <= _creeps.Count; ++o)
        //{
        //    int count = _creeps[o].Count;
        //    for (int i = 0; i < count; ++i)
        //    {
        //        //TSTransform tsTransform = _creepList[i];
        //        //Transform t = tsTransform.transform;
        //        //tsTransform.UpdatePlayMode();
        //        //t.position = Vector3.Lerp(t.position, tsTransform.position.ToVector(), deltaTime * 10f);
        //        //t.position = Vector3.Lerp(t.position, tsTransform.position.ToVector(), lerpFactory);
        //    }
        //}
    }

    public void FixedStep(FP fixedDeltaTime)
    {
        _generatingPath.Clear();

        var creepList = GetCreepList(0);
        
        int count = creepList.Count;
        for (int i = count - 1; i >= 0; --i)
        {
            Creep c = creepList[i];
            c.FixedStep(fixedDeltaTime);

            if(c.reachedTarget)
            {
                DispatchEvent(GameplayEventType.CREEP_REACHED_GOAL, false, c);
            }
            
            if (c.flagForRemoval)
            {
                DispatchEvent(GameplayEventType.CREEP_KILLED, false, c);

                c.RemoveListener(GameplayEventType.CREEP_DAMAGED, onCreepDamaged);
                //GameObject.Destroy(c.view.gameObject);
                creepList[i] = null;
                creepList.RemoveAt(i);
                continue;
            }

            if (recalculatePaths)
            {
                _generatingPath.Add(c.RecalculatePath());
            }
        }

        for(int i = 0; i < _generatingPath.Count; ++i)
        {
            AstarPath.BlockUntilCalculated(_generatingPath[i]);
        }

        recalculatePaths = false;
    }

    private void onCreepDamaged(GhostGen.GeneralEvent e)
    {
        DispatchEvent(e); // Redispatch
    }
}
