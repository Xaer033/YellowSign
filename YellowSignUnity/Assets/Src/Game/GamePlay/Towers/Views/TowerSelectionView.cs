using System.Collections;
using System.Collections.Generic;
using GhostGen;
using UnityEngine;
using Zenject;

public class TowerSelectionView : UIView
{

    public CanvasGroup _canvasGroup;
    private GameplayResources _gameplayResources;
    private ViewFactory _viewFactory;

    [Inject]
    private void Construct(
        GameplayResources gameplayResources)
    {
        _gameplayResources = gameplayResources;
    }

    public void Awake()
    {
        
    }
    

}
