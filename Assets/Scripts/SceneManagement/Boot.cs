using UnityEngine;
using Zenject;

namespace SceneManagement
{
    public class Boot : MonoBehaviour
    {
        [Inject] private SceneLoader SceneLoader { get; set; }

        private void Start()
        {
            SceneLoader.LoadMainMenu();
        }
    }
}