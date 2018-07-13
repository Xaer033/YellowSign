using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ContextInstaller", menuName = "Installers/ContextInstaller")]
public class ContextInstaller : ScriptableObjectInstaller<ContextInstaller>
{
    public override void InstallBindings()
    {
    }
}