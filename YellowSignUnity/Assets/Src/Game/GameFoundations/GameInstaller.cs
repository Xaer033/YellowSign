using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using TrueSync;
using Zenject;


[CreateAssetMenu(menuName = "YellowSign/Game Installer")]
public class GameInstaller : ScriptableObjectInstaller
{
    public const string GLOBAL_DISPATCHER = "global_dispatcher";

    public TowerDictionary towerDictionary;
    public GameplayResources gameplayResources;
    public GameConfig gameConfig;

    public override void InstallBindings()
    {
        towerDictionary.Initialize();
        
        YellowSignStateFactory gameStateInstaller = Container.Instantiate<YellowSignStateFactory>();
        gameStateInstaller.InstallBindings();

        Container.Bind<IEventDispatcher>().WithId(GLOBAL_DISPATCHER).To<EventDispatcher>().AsSingle();
        Container.Bind<CreepSystem>().AsSingle();
        Container.Bind<TowerSystem>().AsSingle();
        Container.Bind<SessionFlags>().AsSingle();
        Container.Bind<GameConfig>().FromInstance(gameConfig).AsSingle();
        Container.Bind<TowerDictionary>().FromInstance(towerDictionary).AsSingle();
        Container.Bind<GameplayResources>().FromInstance(gameplayResources).AsSingle();

        Container.BindFactory<string, TSVector, TSQuaternion, Tower, Tower.Factory>();

        Container.BindInterfacesAndSelfTo<GuiManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameStateMachine<YellowSignStateType>>().AsSingle().WithArguments(gameStateInstaller);
        Container.BindInterfacesAndSelfTo<NetworkManager>().FromNewComponentOnNewGameObject().AsSingle();
        Container.BindInterfacesAndSelfTo<Singleton>().AsSingle();
     }
    
}
