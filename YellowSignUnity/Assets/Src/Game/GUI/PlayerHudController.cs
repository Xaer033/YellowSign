using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;

public class PlayerHudController : BaseController
{
    private PlayerController _controller;
    private PlayerHudView _hudView;
    
    public PlayerHudController(PlayerController controller)
    {
        _controller = controller;
    }

    public void Start(Action onViewCreated)
    {
        viewFactory.CreateAsync<PlayerHudView>("GUI/Gameplay/PlayerHud", (x) =>
        {
            _hudView = x as PlayerHudView;
            _hudView.AddListener(HoverDispatcher.EVENT_HOVER, onHoverUI);

            if(onViewCreated != null)
            {
                onViewCreated();
            }
        });
    }

    public override void RemoveView(bool immediately = false)
    {
        _hudView.RemoveListener(HoverDispatcher.EVENT_HOVER, onHoverUI);
        base.RemoveView(immediately);
    }

    private void onHoverUI(GeneralEvent e)
    {
        bool isHovering = (bool)e.data;
        Debug.Log("IsHOvering: " + isHovering);
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
}
