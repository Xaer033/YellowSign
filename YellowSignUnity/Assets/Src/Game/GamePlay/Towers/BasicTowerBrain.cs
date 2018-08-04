using TrueSync;
using UnityEngine;
using GhostGen;

[CreateAssetMenu(menuName = "YellowSign/TowerBrain")]
public class BasicTowerBrain : AbstractTowerBrain
{
    private const string CREEP_LAYER_NAME = "creep";
    
    private int _creepLayer = -666;
    private Collider[] _hitList;
    private IEventDispatcher _notificationDispatcher;
    

    public override void FixedStep(Tower tower, FP fixedDeltaTime)
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
        tower.state.idleTimer = 0;
        tower.behaviorState = Tower.BehaviorState.IDLE;
    }

    private void _doIdling(Tower tower, FP fixedDeltaTime)
    {
        // Idle 
        FP idleTimer = tower.state.idleTimer - fixedDeltaTime;
        if(idleTimer <= 0)
        {
            tower.state.idleTimer = tower.stats.idleTime;
            tower.behaviorState = Tower.BehaviorState.TARGETING;
        }
        else
        {
            tower.state.idleTimer = idleTimer;
        }
    }

    private void _doTargeting(Tower tower, FP fixedDeltaTime)
    {
        // Target enemy based off of some criteria (lowest health/closest range, etc.)
        var colliderList = Physics.OverlapSphere(tower.view.position.ToVector(), tower.state.range.AsFloat(), creepLayer, QueryTriggerInteraction.Collide);
        if(colliderList != null && colliderList.Length > 0)
        {
            ICreepView cView = null;
            for(int i = 0; i < colliderList.Length; ++i)
            {
                cView = colliderList[i].gameObject.GetComponent<ICreepView>();
                if(cView != null && !cView.creep.isDead)
                {
                    tower.targetCreep = cView.creep;
                    tower.behaviorState = Tower.BehaviorState.ATTACKING;
                    break;
                }
            }
            
            if(cView == null)
            {
                tower.behaviorState = Tower.BehaviorState.IDLE;
            }
        }
        else
        {
            tower.behaviorState = Tower.BehaviorState.IDLE;
        }
    }

    private void _doAttacking(Tower tower, FP fixedDeltaTime)
    {
        // attack target creep    
        IAttackTarget victim = tower.targetCreep;
        
        AttackData attackData = tower.CreateAttackData();
        AttackResult attackResult = victim.TakeDamage(attackData);

        FP visualAttackDuration = tower.view.VisualAttack(tower.targetCreep.view);

        notificationDispatcher.DispatchEvent(GameplayEventType.CREEP_DAMAGED, false, attackResult);

        //Debug.Log("Killed Target: " + attackResult.hasKilledTarget);
        tower.behaviorState = Tower.BehaviorState.RECOVERING;
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
            if(tower.targetCreep.isDead)
            {
                tower.behaviorState = Tower.BehaviorState.TARGETING;
            }
            else if(TSVector.Distance(tower.view.position, tower.targetCreep.view.position) <= tower.state.range)
            {
                tower.behaviorState = Tower.BehaviorState.ATTACKING;
            }
            else
            {
                tower.behaviorState = Tower.BehaviorState.IDLE;
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
}
