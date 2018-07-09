using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;


[CreateAssetMenu(menuName = "YellowSign/TowerBrain")]
public class BasicTowerBrain : AbstractTowerBrain
{
    public override void FixedStep(Tower tower, FP fixedDeltaTime, CreepSystem creepSystem)
    {
        switch(tower.behaviorState)
        {
            case Tower.BehaviorState.SPAWNING:    _doSpawning(tower, fixedDeltaTime);      break;
            case Tower.BehaviorState.IDLE:        _doIdling(tower, fixedDeltaTime);        break;
            case Tower.BehaviorState.TARGETING:   _doTargeting(tower, fixedDeltaTime);     break;
            case Tower.BehaviorState.ATTACKING:   _doAttacking(tower, fixedDeltaTime);     break;
            case Tower.BehaviorState.RECOVERING:  _doRecovering(tower, fixedDeltaTime);    break;
        }
    }

    private void _doSpawning(Tower tower, FP fixedDeltaTime)
    {
        // Have tower view animate its spawning animation 
    }

    private void _doIdling(Tower tower, FP fixedDeltaTime)
    {
        // Idle 
    }

    private void _doTargeting(Tower tower, FP fixedDeltaTime)
    {
        // Target enemy based off of some criteria (lowest health/closest range, etc.)
        var colliderList = Physics.OverlapSphere(tower.view.transform.position, tower.stats.baseRange, ~LayerMask.NameToLayer("creep"), QueryTriggerInteraction.Collide);
        if(colliderList != null && colliderList.Length > 0)
        {

        }
    }

    private void _doAttacking(Tower tower, FP fixedDeltaTime)
    {
        // attack target creep
    }

    private void _doRecovering(Tower tower, FP fixedDeltaTime)
    {
        // recover from shot
    }
}
