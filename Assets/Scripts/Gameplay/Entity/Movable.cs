using System;
using Extentions;
using UnityEngine;
using Zenject;

namespace Gameplay.Entity
{
    public class Movable : LazyGetComponent<CharacterController>
    {
        [SerializeField] private float _gravity;
        [SerializeField] private float _coyoteTime;

        private float _timeInAir;
        private Watch<bool> _isGrounded;

        public bool IsGrounded => _isGrounded;
        public Vector3 Velocity { get; private set; }
        public float MaxFallingSpeed { get; set; } = 20;
        
        public event Action Grounded;

        public void SetVelocity(Vector3 velocity)
        {
            if (Pause.IsPaused)
                return;
            Velocity = velocity;
        }

        public void SetHorizontalVelocity(Vector3 velocity)
        {
            if (Pause.IsPaused)
                return;
            Velocity = velocity.WithY(Velocity.y);
        }

        [Inject] private Pause Pause { get; set; }
        
        public void Jump(float force)
        {
            if (Pause.IsPaused)
                return;
            Velocity = Velocity.WithY(force);
            Lazy.Move(Vector3.up * force * Time.fixedDeltaTime);
        }

        private void Awake()
        {
            _isGrounded.OnChanged += (_, newValue) =>
            {
                if (newValue)
                    Grounded?.Invoke();
            };
        }

        private void FixedUpdate()
        {
            if (Pause.IsPaused)
                return;
            _timeInAir = Lazy.isGrounded ? 0 : (_timeInAir + Time.fixedDeltaTime);
            _isGrounded.Value = _timeInAir < _coyoteTime;
            Lazy.Move(Velocity * Time.fixedDeltaTime);
            UpdateFallSpeed();
        }

        private void UpdateFallSpeed()
        {
            if (Pause.IsPaused)
                return;
            if (Lazy.isGrounded)
            {
                Velocity = Velocity.WithY(0);
            }
            Velocity = Velocity.WithY(Mathf.Max(Velocity.y - _gravity * Time.fixedDeltaTime, -MaxFallingSpeed));
        }
    }
}