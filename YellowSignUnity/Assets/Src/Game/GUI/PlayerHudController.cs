using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;

public class PlayerHudController : BaseController
{
    private PlayerController _controller;
    private CreepDictionary _creepDictionary;
    private PlayerHudView _hudView;
    private CreepSpawnerView _creepSpawnerView;
    private HashSet<IEventDispatcher> _hoverItems = new HashSet<IEventDispatcher>();

    private CreepDef _currentCreepDef;
    private TowerDef _currentTowerDef;

    public PlayerHudController(PlayerController controller)
    {
        _controller = controller;

        _creepDictionary = Singleton.instance.diContainer.Resolve<CreepDictionary>();
    }

    public void Start(Action onViewCreated)
    {
        viewFactory.CreateAsync<PlayerHudView>("GUI/Gameplay/PlayerHudView", (x) =>
        {
            _hudView = x as PlayerHudView;
            _hudView.AddListener(HoverDispatcher.EVENT_HOVER, onHoverUI);
            _hudView.AddListener(PlayerUIEventType.TOGGLE_CREEP_VIEW, onToggleCreepView);
            _hudView.AddListener(PlayerUIEventType.TOGGLE_TOWER_VIEW, onToggleTowerView);
            
            if(onViewCreated != null)
            {
                onViewCreated();
            }
        });

        viewFactory.CreateAsync<CreepSpawnerView>("GUI/Gameplay/CreepSpawnerView", (x) =>
        {
            _creepSpawnerView = x as CreepSpawnerView;

            var creepList = _getCreepList();
            _creepSpawnerView.SetCreepList(creepList);
            _creepSpawnerView.AddListener(HoverDispatcher.EVENT_HOVER, onHoverUI);
            _creepSpawnerView.AddListener(PlayerUIEventType.DISMISS_VIEW, onDismissCreepSpawner);
            _creepSpawnerView.AddListener(PlayerUIEventType.SPAWN_CREEPS, onSpawnCreeps);
            _creepSpawnerView.AddListener(PlayerUIEventType.SELECT_CREEP_TYPE, onCreepSelected);

        });
    }

    public override void RemoveView(bool immediately = false)
    {
        _hudView.RemoveListener(HoverDispatcher.EVENT_HOVER, onHoverUI);
        base.RemoveView(immediately);
    }

    private bool hasHoverItems
    {
        get { return _hoverItems.Count > 0; }
    }

    private void setHoverItem(IEventDispatcher dispatcher, bool isHovering)
    {
        if(isHovering)
        {
            _hoverItems.Add(dispatcher);
        }
        else
        {
            _hoverItems.Remove(dispatcher);
        }
    }

    private void onHoverUI(GeneralEvent e)
    {
        setHoverItem(e.target as IEventDispatcher, (bool)e.data);

        bool isHovering = (bool)e.data;

        Debug.Log("IsHovering: " + isHovering);
        if(isHovering)
        {
            _controller.controlState = PlayerController.PlayerControlState.NONE;
        }
        else
        {
            _controller.controlState = PlayerController.PlayerControlState.TOWER_BUILDER;
        }
        //_controller.set
    }
    private void onSpawnCreeps(GeneralEvent e)
    {
        SpawnCreepCommand command = (SpawnCreepCommand)e.data;
        _controller.AddCommand(command);
    }

    private void onDismissCreepSpawner(GeneralEvent e)
    {
        _creepSpawnerView.Hide(null);
    }

    private void onToggleCreepView(GeneralEvent e)
    {
        _creepSpawnerView.Show(null);
    }
    
    private void onToggleTowerView(GeneralEvent e)
    {

    }

    private void onCreepSelected(GeneralEvent e)
    {
        CreepDef def = e.data as CreepDef; // So much Def :O
        _currentCreepDef = def;
        _hudView.currentCreepDef = def;
        _creepSpawnerView.currentCreepDef = def;
    }

    private List<CreepDef> _getCreepList()
    {
        var creeps = _creepDictionary.GetValues();
        return creeps;
    }
}
