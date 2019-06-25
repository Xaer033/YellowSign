using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using TrueSync;
using Zenject;

public class CreepViewSystem : EventDispatcher
{
    private CreepSystem _creepSystem;
    private List<CreepViewPair> _creepViewList;

    internal class CreepViewPair
    {
        public Creep creep;
        public ICreepView view;
    }

    public CreepViewSystem(CreepSystem creepSystem)
    {
        _creepViewList = new List<CreepViewPair> (200);
    }

    public ICreepView AddCreep(Creep c)
    {
        CreepViewPair pair = new CreepViewPair();
        pair.creep = c;
        pair.view = c.view; // For now...

        _creepViewList.Add(pair);
        return pair.view;
    }
    
    public void Step(float deltaTime)
    {
        int creepCount = _creepViewList.Count;
        for(int i = creepCount - 1; i >= 0; --i)
        {
            CreepViewPair pair = _creepViewList[i];
            ICreepView view = pair.view;
            Creep creep = pair.creep;

            if(creep.flagForRemoval)
            {
                _creepViewList.RemoveAt(i);
                GameObject.Destroy(view.gameObject);
                continue;
            }

            view.transformTS.position = creep.state.position;
            view.transformTS.rotation = creep.state.rotation;
        }
    }
}
