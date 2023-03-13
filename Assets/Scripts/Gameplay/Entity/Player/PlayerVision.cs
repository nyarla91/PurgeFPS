using System;
using System.Collections;
using Extentions;
using UnityEngine;
using Zenject;

namespace Gameplay.Entity.Player
{
    public class PlayerVision : PlayerComposition
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _turnAroundSpeed;
        [SerializeField] private float _standartFOV;
        [SerializeField] private float _zoomFOV;

        private Coroutine _turnAroundCoroutine;
        private Transform _cameraTransform;
        private bool _aiming;
        
        public Camera Camera => _camera;
        public Transform CameraTransform => _cameraTransform;

        public VisionRaycast Raycast
        {
            get
            {
                Ray ray = Camera.ScreenPointToRay(new Vector2(Screen.width, Screen.height) / 2);
                LayerMask mask = LayerMask.GetMask("Enemy", "Obstacle");
                Physics.Raycast(ray, out RaycastHit raycast, 500, mask);
                return new VisionRaycast(raycast);
            }
        }
        
        [Inject] private Pause Pause { get; set; }

        private void TryTurnAround()
        {
            if (_turnAroundCoroutine != null)
                return;
            _turnAroundCoroutine = StartCoroutine(TurnAround(1));
        }

        private IEnumerator TurnAround(int direction)
        {
            for (float i = 0; i < 180; i += Time.deltaTime * _turnAroundSpeed)
            {
                RotateCamera(new Vector2(direction * Time.deltaTime * _turnAroundSpeed, 0));
                yield return null;
            }
            _turnAroundCoroutine = null;
        }

        private void RotateCamera(Vector2 delta)
        {
            if (Pause.IsPaused)
                return;
            delta = delta / _standartFOV * _camera.fieldOfView;
            float verticalAngle = _cameraTransform.localRotation.eulerAngles.x;
            verticalAngle -= delta.y;
            verticalAngle = verticalAngle.ClampAngle(275, 85);
            _cameraTransform.localRotation = Quaternion.Euler(_cameraTransform.localRotation.eulerAngles.WithX(verticalAngle));
            Transform.Rotate(0, delta.x, 0);
        }

        private void Awake()
        {
            _cameraTransform = _camera.transform;
            Controls.ToggledAim += () => _aiming = !_aiming;
        }

        private void FixedUpdate()
        {
            float targetFOV = _aiming ? _zoomFOV : _standartFOV;
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, 0.3f);
        }

        private void Update()
        {
            RotateCamera(Controls.CameraDelta * Time.deltaTime);
        }
    }

    public class VisionRaycast
    {
        public bool Hit { get; }
        public RaycastHit Raycast { get; }
        public int Layer { get; }

        public VisionRaycast(RaycastHit raycast)
        {
            Raycast = raycast;
            Hit = raycast.collider != null;
            if (Hit)
                Layer = raycast.collider.gameObject.layer;
        }
    }
}