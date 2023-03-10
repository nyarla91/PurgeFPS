using SceneManagement;
using Extentions;
using Input;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _sceneLoaderPrefab;
        [SerializeField] private GameObject _deviceWatcherPrefab;
        [SerializeField] private GameObject _settingsPrefab;
        [SerializeField] private GameObject _pausePrefab;
        
        public override void InstallBindings()
        {
            BindFromPrefab<SceneLoader>(_sceneLoaderPrefab);
            BindFromPrefab<DeviceWatcher>(_deviceWatcherPrefab);
            BindFromPrefab<Settings.Settings>(_settingsPrefab);
            BindFromPrefab<Pause>(_pausePrefab);
        }

        private void BindFromPrefab<T>(GameObject prefab)
        {
            GameObject instance = Container.InstantiatePrefab(prefab, transform);
            Container.Bind<T>().FromInstance(instance.GetComponent<T>()).AsSingle();
        }
    }
}