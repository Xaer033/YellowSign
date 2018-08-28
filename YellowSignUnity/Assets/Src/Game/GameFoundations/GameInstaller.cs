using GhostGen;
using TrueSync;
using UnityEngine;
using Zenject;


[CreateAssetMenu(menuName = "YellowSign/Game Installer")]
public class GameInstaller : ScriptableObjectInstaller
{
    public const string GLOBAL_DISPATCHER = "global_dispatcher";

    public TowerDictionary towerDictionary;
    public CreepDictionary creepDictionary;
    public GameplayResources gameplayResources;
    public GameConfig gameConfig;
    public GameObject syncCommanderPrefab;

    public override void InstallBindings()
    {
        towerDictionary.Initialize();
        creepDictionary.Initialize();
       
        YellowSignStateFactory gameStateInstaller = Container.Instantiate<YellowSignStateFactory>();
        gameStateInstaller.InstallBindings();

        Container.Bind<IEventDispatcher>().WithId(GLOBAL_DISPATCHER).To<EventDispatcher>().AsSingle();

        Container.Bind<GameState>().AsSingle();
        Container.Bind<GameTimerManager>().AsSingle();
        Container.Bind<WaveAISystem>().AsSingle();
        Container.Bind<CreepViewSystem>().AsSingle();
        Container.Bind<WaveSpawnerSystem>().AsSingle();
        Container.Bind<GameSystemManager>().AsSingle();
        Container.Bind<CreepSystem>().AsSingle();
        Container.Bind<TowerSystem>().AsSingle();
        Container.Bind<TowerDictionary>().FromInstance(towerDictionary).AsSingle();
        Container.Bind<CreepDictionary>().FromInstance(creepDictionary).AsSingle();
        Container.Bind<CreepHealthUISystem>().FromNewComponentOnNewGameObject().AsSingle();
        Container.BindFactory<string, TowerSpawnInfo, Tower, Tower.Factory>();
        Container.BindFactory<string, CreepSpawnInfo, Creep, Creep.Factory>();
        Container.BindFactory<SyncStepper, SyncStepper.Factory>().WithFactoryArguments<GameObject>(syncCommanderPrefab);

        Container.Bind<SessionFlags>().AsSingle();
        Container.Bind<GameConfig>().FromInstance(gameConfig).AsSingle();
        Container.Bind<GameplayResources>().FromInstance(gameplayResources).AsSingle();
        Container.BindInterfacesAndSelfTo<GuiManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameStateMachine<YellowSignStateType>>().AsSingle().WithArguments(gameStateInstaller);
        Container.BindInterfacesAndSelfTo<NetworkManager>().FromNewComponentOnNewGameObject().AsSingle();
        Container.BindInterfacesAndSelfTo<Singleton>().AsSingle();
     }
}
