using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ContextInstaller", menuName = "Installers/ContextInstaller")]
public class ContextInstaller : ScriptableObjectInstaller<ContextInstaller>
{
    public TowerFactory towerFactory;
    public GameplayResources gameplayResources;

    public override void InstallBindings()
    {
        Container.Bind<CreepSystem>().AsSingle();
        Container.Bind<TowerSystem>().AsSingle();
        Container.Bind<TowerFactory>().FromInstance(towerFactory).AsSingle();
        Container.Bind<GameplayResources>().FromInstance(gameplayResources).AsSingle();

        Container.BindFactory<ITowerBrain, TowerStats, ITowerView, Tower, Tower.Factory>();
    }
}