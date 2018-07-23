using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using TrueSync;
using Zenject;


[CreateAssetMenu(menuName = "YellowSign/Game Installer")]
public class GameInstaller : ScriptableObjectInstaller
{   
    public TowerDictionary towerDictionary;
    public GameplayResources gameplayResources;

    public override void InstallBindings()
    {
        towerDictionary.Initialize();
        
        YellowSignStateFactory gameStateInstaller = Container.Instantiate<YellowSignStateFactory>();
        gameStateInstaller.InstallBindings();

        Container.Bind<CreepSystem>().AsSingle();
        Container.Bind<TowerSystem>().AsSingle();
        Container.Bind<TowerDictionary>().FromInstance(towerDictionary).AsSingle().NonLazy();
        Container.Bind<GameplayResources>().FromInstance(gameplayResources).AsSingle();
        Container.Bind<GameStateMachine<YellowSignState>>().AsSingle().WithArguments(gameStateInstaller);
        Container.BindFactory<string, TSVector, TSQuaternion, Tower, Tower.Factory>().FromFactory<Tower.DynamicFactory>();

        Container.BindInterfacesAndSelfTo<Singleton>().AsSingle();
        Debug.Log("Poop");
     }
    
}
