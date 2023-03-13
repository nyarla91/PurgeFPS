using System;
using Extentions;
using Input;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Gameplay.UI
{
    public class GamepadDisconnectMessage : LazyGetComponent<Menu>
    {
        private bool _gamepadConnected;

        private bool GamepadConnected
        {
            get => _gamepadConnected;
            set
            {
                if (value == _gamepadConnected)
                    return;
                _gamepadConnected = value;
                if ( ! _gamepadConnected && DeviceWatcher.CurrentInputScheme == InputScheme.Gamepad)
                    Lazy.Open();
            }
        }
        
        [Inject] private DeviceWatcher DeviceWatcher { get; set; }

        private void Update()
        {
            GamepadConnected = Gamepad.current != null;
        }
    }
}