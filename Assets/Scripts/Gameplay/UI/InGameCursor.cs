using System;
using Extentions;
using UnityEngine;
using Zenject;

namespace Gameplay.UI
{
    public class InGameCursor : MonoBehaviour
    {
        [Inject] private Pause Pause { get; set; }
        
        private void Start()
        {
            Cursor.lockState = Pause ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}