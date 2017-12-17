using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class CreepSystem
{
    private const int kMaxCreeps = 200;

    private List<List<Creep>> _creeps;

    public bool recalculatePaths { set; get; }
    
    public CreepSystem()
    {
        int maxPlayers = NetworkManager.kMaxPlayers;
        _creeps = new List<List<Creep>>(maxPlayers);

        for(int i = 0; i < maxPlayers; ++i)
        {
            _creeps.Add(new List<Creep>(200));
        }
    }

    public void AddCreep(int owner, Creep creep)
    {
        List<Creep> creepList = GetCreepList(owner);
        creepList.Add(creep);
    }

    public List<Creep> GetCreepList(int owner)
    {
        if(owner < 0 || owner >= NetworkManager.kMaxPlayers)
        {
            Debug.LogError("Owner is out of range!");
            return null;
        }
        return _creeps[owner];
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
        for (int o = 0; o < _creeps.Count; ++o)
        {
            int count = _creeps[o].Count;
            for (int i = count - 1; i >= 0; --i)
            {
                Creep c = _creeps[o][i];
                if (c.flagForRemoval)
                {
                    //TrueSyncManager.SyncedDestroy(c.transform.gameObject);
                    GameObject.Destroy(c.transform.gameObject);
                    _creeps[o].RemoveAt(i);
                    continue;
                }

                if (recalculatePaths)
                {
                    c.RecalculatePath();
                }
                c.FixedStep(fixedDeltaTime);
            }
        }

        recalculatePaths = false;
    }
}
