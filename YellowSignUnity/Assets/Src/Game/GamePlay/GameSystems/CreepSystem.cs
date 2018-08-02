using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class CreepSystem : GhostGen.EventDispatcher
{
    private const int kMaxCreeps = 200;

    private List<List<Creep>> _creeps;
    private List<Pathfinding.Path> _generatingPath;


    public bool recalculatePaths { set; get; }
    
    public CreepSystem()
    {
        int maxPlayers = NetworkManager.kMaxPlayers;
        _creeps = new List<List<Creep>>(maxPlayers);

        for(int i = 0; i < maxPlayers; ++i)
        {
            _creeps.Add(new List<Creep>(200));
        }

        _generatingPath = new List<Pathfinding.Path>(10);
    }

    public void AddCreep(byte ownerId, Creep creep)
    {
        List<Creep> creepList = GetCreepList(ownerId);
        creepList.Add(creep);
        DispatchEvent(GameplayEventType.CREEP_SPAWNED, false, creep);
    }

    public List<Creep> GetCreepList(byte ownerId)
    {
        if(ownerId < 0 || ownerId >= NetworkManager.kMaxPlayers)
        {
            Debug.LogError("Owner is out of range!");
            return null;
        }
        return _creeps[ownerId];
    }

    public void Step(float deltaTime)
    {
        float lerpFactory = deltaTime * 10.0f;

        for(int o = 0; o < _creeps.Count; ++o)
        {
            int count = _creeps[o].Count;
            for (int i = 0; i < count; ++i)
            {
                //TSTransform tsTransform = _creepList[i];
                //Transform t = tsTransform.transform;
                //tsTransform.UpdatePlayMode();
                //t.position = Vector3.Lerp(t.position, tsTransform.position.ToVector(), deltaTime * 10f);
                //t.position = Vector3.Lerp(t.position, tsTransform.position.ToVector(), lerpFactory);
            }
        }
    }

    public void FixedStep(FP fixedDeltaTime)
    {
        _generatingPath.Clear();

        for (int o = 0; o < _creeps.Count; ++o)
        {
            int count = _creeps[o].Count;
            for (int i = count - 1; i >= 0; --i)
            {
                Creep c = _creeps[o][i];

                if(c.reachedTarget)
                {
                    DispatchEvent(GameplayEventType.CREEP_REACHED_GOAL, false, c);
                }

                if (c.flagForRemoval)
                {
                    GameObject.Destroy(c.view.gameObject);
                    _creeps[o][i] = null;
                    _creeps[o].RemoveAt(i);
                    continue;
                }

                if (recalculatePaths)
                {
                    _generatingPath.Add(c.RecalculatePath());
                }
                c.FixedStep(fixedDeltaTime);
            }
        }

        for(int i = 0; i < _generatingPath.Count; ++i)
        {
            AstarPath.BlockUntilCalculated(_generatingPath[i]);
        }

        recalculatePaths = false;
    }
}
