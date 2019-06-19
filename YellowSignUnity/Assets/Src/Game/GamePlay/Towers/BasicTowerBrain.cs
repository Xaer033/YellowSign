using System.Collections.Generic;
using TrueSync;
using UnityEngine;
using GhostGen;
using Zenject;

[CreateAssetMenu(menuName = "YellowSign/Towers/Basic Tower Brain")]
public class BasicTowerBrain : AbstractTowerBrain
{
    private const string CREEP_LAYER_NAME = "creep";
    
    private int _creepLayer = -666;
    private IEventDispatcher _notificationDispatcher;
    
    private CreepSystem _creepSystem;
    

    public override void FixedStep(Tower tower, FP fixedDeltaTime)
    {
        switch(tower.state.behaviorMode)
        {
            case TowerState.BehaviorMode.SPAWNING:      _doSpawning(tower, fixedDeltaTime);     break;
            case TowerState.BehaviorMode.IDLE:          _doIdling(tower, fixedDeltaTime);       break;
            case TowerState.BehaviorMode.TARGETING:     _doTargeting(tower, fixedDeltaTime);    break;
            case TowerState.BehaviorMode.VISUAL_ATTACK: _doVisualAttack(tower, fixedDeltaTime); break;
            case TowerState.BehaviorMode.ATTACK:        _doAttack(tower, fixedDeltaTime);       break;
            case TowerState.BehaviorMode.RECOVERING:    _doRecovering(tower, fixedDeltaTime);   break;
        }
    }

    private void _doSpawning(Tower tower, FP fixedDeltaTime)
    {
        // Have tower view animate its spawning animation 
        tower.state.idleTimer = 0;
        tower.state.behaviorMode = TowerState.BehaviorMode.IDLE;
    }

    private void _doIdling(Tower tower, FP fixedDeltaTime)
    {
        // Idle 
        FP idleTimer = tower.state.idleTimer - fixedDeltaTime;
        if(idleTimer <= 0)
        {
            tower.state.idleTimer = tower.stats.idleTime;
            tower.state.behaviorMode = TowerState.BehaviorMode.TARGETING;
        }
        else
        {
            tower.state.idleTimer = idleTimer;
        }
    }

    private void _doTargeting(Tower tower, FP fixedDeltaTime)
    {
        // Target enemy based off of some criteria (lowest health/closest range, etc.)
        Creep target = getClosestCreep(tower);
        if(target != null && target.isValid)
        {
            tower.targetCreep = target;
            tower.state.behaviorMode = TowerState.BehaviorMode.VISUAL_ATTACK;
        }
        else
        {
            tower.state.behaviorMode = TowerState.BehaviorMode.IDLE;
        }
    }

    private void _doVisualAttack(Tower tower, FP fixedDeltaTime)
    {
        if(tower.targetCreep != null && !tower.targetCreep.isValid)
        {
            tower.state.behaviorMode = TowerState.BehaviorMode.IDLE;
            return;
        }

        // attack target creep    
        tower.state.attackTimer = tower.view.VisualAttack(tower.targetCreep.view);
        tower.state.behaviorMode = TowerState.BehaviorMode.ATTACK;
        
    }

    private void _doAttack(Tower tower, FP fixedDeltaTime)
    {
        FP attackTimer = tower.state.attackTimer;
        FP delta = tower.state.reloadTimer - attackTimer;
        tower.state.reloadTimer = delta > 0 ? delta : 0;
        //if(attackTimer > 0.0f)
        //{
        //    tower.state.attackTimer = attackTimer - fixedDeltaTime;
        //}
        //else
        {
            IAttackTarget victim = tower.targetCreep;

            AttackData attackData = tower.CreateAttackData();
            AttackResult attackResult = victim.TakeDamage(attackData);
            
            tower.state.behaviorMode = TowerState.BehaviorMode.RECOVERING;
        }
    }

    private void _doRecovering(Tower tower, FP fixedDeltaTime)
    {
        // recover from shot
        TowerState state = tower.state;
        TowerStats stats = tower.stats;

        FP cooldownTimer = state.reloadTimer - fixedDeltaTime;
        if(cooldownTimer < 0)
        {
            cooldownTimer = stats.reloadTime;
            if(!tower.targetCreep.isValid)
            {
                tower.state.behaviorMode = TowerState.BehaviorMode.TARGETING;
            }
            else if(TSVector.Distance(tower.view.position, tower.targetCreep.view.position) <= tower.state.range)
            {
                tower.state.behaviorMode = TowerState.BehaviorMode.VISUAL_ATTACK;
            }
            else
            {
                tower.state.behaviorMode = TowerState.BehaviorMode.IDLE;
            }
        }
        state.reloadTimer = cooldownTimer;
    }

    private IEventDispatcher notificationDispatcher
    {
        get
        {
            if(_notificationDispatcher == null)
            {
                _notificationDispatcher = Singleton.instance.notificationDispatcher;
            }
            return _notificationDispatcher;
        }
    }

    private int creepLayer
    {
        get
        {
            //if(_creepLayer < 0)
            {
                _creepLayer = LayerMask.GetMask(CREEP_LAYER_NAME);
            }
            return _creepLayer;
        }
    }

    private CreepSystem creepSystem
    {
        get
        {
            if(_creepSystem == null)
            {
                _creepSystem = Singleton.instance.diContainer.Resolve<CreepSystem>();
            }
            return _creepSystem;
        }
    }


    private Creep getClosestCreep(Tower tower)
    {
        FP minDistance = tower.stats.baseRange + 9999999;
        Creep target = null;
        List<Creep> creepList = creepSystem.GetCreepList();

        for(int i = 0; i < creepList.Count; ++i)
        {
            Creep c = creepList[i];
            // Check for valid Creep State
            if(c != null && !c.isValid)
            {
                continue;
            }

            // Self explanatory
            bool isPlayerOwnedCreep = PhotonNetwork.countOfPlayers != 1 && c.ownerId == tower.ownerId;
            if(isPlayerOwnedCreep)
            {
                continue;
            }

            FP dist = TSVector.Distance(tower.view.position, c.view.position);
            if(dist > tower.state.range)
            {
                continue;
            }

            // Find closest creep
            if(dist < minDistance)
            {
                minDistance = dist;
                target = c;
            }
        }
        return target;
    }
}
