using System;
using Extentions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputBuffer
    {
        private readonly Timer _timer;
        private float _timeWindow;
        
        public float TimeWindow
        {
            get => _timeWindow;
            set
            {
                _timeWindow = value;
                _timer.Length = value;
            }
        }

        public bool PerformAllowed { get; set; }

        public event Action OnPerformed;

        public InputBuffer(MonoBehaviour container, float timeWindow)
        {
            _timeWindow = timeWindow;
            _timer = new Timer(container, timeWindow);
            _timer.Ticked += CheckAvialability;
        }

        public void SendInput(InputAction.CallbackContext context) => SendInput();
        
        public void SendInput()
        {
            _timer.Stop();
            if (PerformAllowed)
            {
                OnPerformed?.Invoke();
                return;
            }
            _timer.Restart();
        }

        public void InterruptBuffering() => _timer.Stop();

        private void CheckAvialability(float irrelevant)
        {
            if (!PerformAllowed)
                return;
            
            OnPerformed?.Invoke();
            _timer.Stop();
        }
    }
}