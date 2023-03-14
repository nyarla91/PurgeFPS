using Gameplay.Entity.Player;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private Transform _playerSpawnPoint;
        
        public override void InstallBindings()
        {
            Container.Bind<GameplayActions>().AsSingle();
            PlayerComposition composition = Container.InstantiatePrefab(_playerPrefab, _playerSpawnPoint)
                .GetComponent<PlayerComposition>();

            Container.Bind<PlayerComposition>().FromInstance(composition).AsSingle();
            Container.Bind<PlayerMovement>().FromInstance(composition.Movement).AsSingle();
        }
    }
}