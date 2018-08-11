using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            if(onViewCreated != null)
            {
                onViewCreated();
            }
        });
    }
    
}
