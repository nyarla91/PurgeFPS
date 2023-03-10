using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class GameplayInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameplayActions>().AsSingle();
        }
    }
}