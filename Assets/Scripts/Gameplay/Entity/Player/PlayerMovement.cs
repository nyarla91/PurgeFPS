using System.Collections;
using Extentions;
using UnityEngine;
using Zenject;

namespace Gameplay.Entity.Player
{
    public class PlayerMovement : LazyGetComponent<PlayerComposition>
    {
        private const string RegularState = "Regular";
        private const string DashState = "Dash";
        private const string ChargeState = "Charge";
        
        [SerializeField] private StateMachine _movementStates;
        [SerializeField] private Movable _movable;
        [SerializeField] private float _maxSpeed;
        [SerializeField] private float _acceleration;
        [Space]
        [SerializeField] [Range(0, 1)] private float _airAccelerationMultiplier;
        [SerializeField] private float _jumpForce;
        [Space]
        [SerializeField] private float _dashDuration;
        [SerializeField] private AnimationCurve _dashSpeedCurve;
        [SerializeField] private float _dashSpeed;
        [Space]
        [SerializeField] private float _chargeDuration;
        [SerializeField] private AnimationCurve _chargeSpeedCurve;
        [SerializeField] private float _chargeSpeed;
        [SerializeField] private float _chargeCooldownDuration;
        
        private bool _dashReady = true;
        private Timer _chargeCooldown;

        public bool ChargeAllowed => ! _chargeCooldown.IsOn;
        public float ChargeCooldownPercent => _chargeCooldown.TimeElapsed / _chargeCooldown.Length;
        public bool DashReady => _dashReady;
        public bool IsGrounded => _movable.IsGrounded;

        private Vector3 InputDirectionToWorld => Transform.forward * Lazy.Controls.MovementDelta.y + Transform.right * Lazy.Controls.MovementDelta.x;
        
        [Inject] private Pause Pause { get; set; }

        public void ReadyCharge() => _chargeCooldown.Reset();
        
        private void Move()
        {
            if (Pause.IsPaused)
                return;
            if (_movementStates.IsCurrentStateNoneOf(RegularState))
                return;
            
            Vector3 targetVelocity = InputDirectionToWorld;
            targetVelocity *= _maxSpeed;
            float maxVelocityDelta = _movable.IsGrounded ? _acceleration : (_acceleration * _airAccelerationMultiplier);

            _movable.SetHorizontalVelocity(Vector3.MoveTowards(_movable.Velocity, targetVelocity, maxVelocityDelta));
        }

        private void TryCharge()
        {
            if (! ChargeAllowed || ! _movementStates.TryEnterState(ChargeState))
                return;

            StartCoroutine(Charge());
        }

        private IEnumerator Charge()
        {
            _chargeCooldown.Restart();
            Vector3 direction = Lazy.Vision.CameraTransform.forward;
            for (float i = 0; i < 1; i += Time.fixedDeltaTime / _chargeDuration)
            {
                _movable.SetVelocity(direction * _chargeSpeed * _chargeSpeedCurve.Evaluate(i));
                yield return new WaitForFixedUpdate();
            }
            _movementStates.TryExitState(ChargeState);
        }

        private IEnumerator Dash()
        {
            _dashReady = false;
            Vector3 direction = InputDirectionToWorld.normalized;
            for (float i = 0; i < 1; i += Time.fixedDeltaTime / _dashDuration)
            {
                _movable.SetVelocity(direction * _dashSpeed * _dashSpeedCurve.Evaluate(i));
                yield return new WaitForFixedUpdate();
            }
            _movementStates.TryExitState(DashState);
        }

        private void JumpOrDash()
        {
            if (Pause.IsPaused)
                return;
            
            if (_movable.IsGrounded)
                _movable.Jump(_jumpForce);
            else if (_dashReady && _movementStates.TryEnterState(DashState))
                StartCoroutine(Dash());
        }

        private void Awake()
        {
            Lazy.Controls.Jumped += JumpOrDash;
            Lazy.Controls.Charged += TryCharge;

            _movable.Grounded += () => _dashReady = true;
            _chargeCooldown = new Timer(this, _chargeCooldownDuration, Pause);
        }

        private void FixedUpdate()
        {
            Move();
        }
    }
}