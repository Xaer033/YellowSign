using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using Zenject;


[CreateAssetMenu(menuName = "YellowSign/Game Installer")]
public class GameInstaller : ScriptableObjectInstaller
{
    public TowerFactory towerFactory;
    public GameplayResources gameplayResources;

    public override void InstallBindings()
    {
        Container.Bind<CreepSystem>().AsSingle();
        Container.Bind<TowerSystem>().AsSingle();
        Container.Bind<TowerFactory>().FromInstance(towerFactory).AsSingle();
        Container.Bind<GameplayResources>().FromInstance(gameplayResources).AsSingle();
        Container.Bind<GameStateMachine<YellowSignState>>().AsSingle();
        Container.Bind<IStateFactory<YellowSignState>>().To<YellowSignStateFactory>().AsTransient();
        Container.BindFactory<ITowerBrain, TowerStats, ITowerView, Tower, Tower.Factory>();
        Debug.Log("Poop");
    }
}
