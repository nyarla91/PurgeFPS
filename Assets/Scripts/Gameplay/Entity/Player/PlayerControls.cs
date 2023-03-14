using Extentions;
using Input;
using Settings;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Action = System.Action;

namespace Gameplay.Entity.Player
{
    public class PlayerControls : LazyGetComponent<PlayerComposition>
    {
        private GameplayActions _actions;
        public SettingsConfig Config { get; private set; }

        public Vector2 CameraDelta
        {
            get
            {
                Vector2 input = _actions.Player.Camera.ReadValue<Vector2>();

                if (DeviceWatcher.CurrentInputScheme == InputScheme.KeyboardMouse)
                {
                    input *= new Vector2(Config.KeyboardMouse.GetSettingValue("sensitivity x"), Config.KeyboardMouse.GetSettingValue("sensitivity y"));
                    return input;
                }

                input = ApplyDeadzone(input);
                input = InvertAxes(input);
                input = ApplySensivity(input);
                input = ApplyGyro(input);
                
                return input;

                
                Vector2 ApplyDeadzone(Vector2 originInput)
                {
                    if (originInput.magnitude * 100 < Config.Gamepad.GetSettingValue("deadzone"))
                        originInput = Vector2.zero;
                    return originInput;
                }

                Vector2 InvertAxes(Vector2 originInput)
                {
                    if (Config.Gamepad.IsSettingToggled("invert y"))
                        originInput = originInput.WithY(-originInput.y);
                    if (Config.Gamepad.IsSettingToggled("invert x"))
                        originInput = originInput.WithX(-originInput.x);
                    return originInput;
                }

                Vector2 ApplySensivity(Vector2 originInput)
                {
                    originInput *= new Vector2(Config.Gamepad.GetSettingValue("sensitivity x"), Config.Gamepad.GetSettingValue("sensitivity y")) * 6;
                    return originInput;
                }
                
                Vector2 ApplyGyro(Vector2 originInput)
                {
                    if (!Config.Gyro.IsSettingToggled("enabled"))
                        return originInput;
                    
                    Vector2 gyroAxes = GetScreenGyroDelta(Config.Gyro.IsSettingToggled("world space"));
                    if (gyroAxes.magnitude < Config.Gyro.GetSettingValue("deadzone") * 0.015f)
                        return originInput;
                    
                    gyroAxes *= new Vector2(Config.Gyro.GetSettingPercent("scale x"), Config.Gyro.GetSettingPercent("scale y"));
                    originInput += gyroAxes * 270;
                    print(gyroAxes);
                    return originInput;
                }
            }
        }

        public Vector2 MovementDelta
        {
            get
            {
                Vector2 input = _actions.Player.Move.ReadValue<Vector2>();
                return Vector2.ClampMagnitude(input, 1);
            }
        }

        public event Action Jumped;
        public event Action Charged;
        public event Action ToggledAim;
        public event Action ChangedWeapon;
        public event Action StartFire;
        public event Action EndFire;
        
        [Inject] private DeviceWatcher DeviceWatcher { get; set; }
        [Inject] private Pause Pause { get; set; }

        [Inject]
        private void Construct(GameplayActions actions, Settings.Settings settings)
        {
            _actions = actions;
            _actions.Enable();
            
            _actions.Player.Jump.started += JumoedInvoke;
            _actions.Player.Charge.started += ChargedInvoke;
            _actions.Player.Aim.started += ToggledAimInvoke;
            _actions.Player.Aim.canceled += ToggledAimInvoke;
            _actions.Player.ChangeWeapon.started += ChangedWeaponInvoke;
            _actions.Player.Fire.started += StartFireInvoke;
            _actions.Player.Fire.canceled += EndFireInvoke;

            Config = settings.Config;
        }

        private void ChangedWeaponInvoke(InputAction.CallbackContext obj)
        {
            if (Pause.IsPaused)
                return;
            ChangedWeapon?.Invoke();
        }

        private void JumoedInvoke(InputAction.CallbackContext _)
        {
            if (Pause.IsPaused)
                return;
            Jumped?.Invoke();
        }

        private void ChargedInvoke(InputAction.CallbackContext _)
        {
            if (Pause.IsPaused)
                return;
            Charged?.Invoke();
        }

        private void ToggledAimInvoke(InputAction.CallbackContext _) => ToggledAim?.Invoke();
        private void StartFireInvoke(InputAction.CallbackContext _)
        {
            if (Pause.IsPaused)
                return;
            StartFire?.Invoke();
        }

        private void EndFireInvoke(InputAction.CallbackContext _) => EndFire?.Invoke();

        private void OnDestroy()
        {
            _actions.Player.Jump.started -= JumoedInvoke;
            _actions.Player.Charge.started -= ChargedInvoke;
            _actions.Player.Aim.started -= ToggledAimInvoke;
            _actions.Player.Aim.canceled -= ToggledAimInvoke;
            _actions.Player.ChangeWeapon.started -= ChangedWeaponInvoke;
            _actions.Player.Fire.started -= StartFireInvoke;
            _actions.Player.Fire.canceled -= EndFireInvoke;
        }
        
        private Vector2 GetScreenGyroDelta(bool worldSpace)
        {
            Vector3 gyro = DualshockMotion.RawGyro;
            float xAxisInfluence = DualshockMotion.Accelerometer.y;
            bool invertZ = DualshockMotion.Accelerometer.z > 0;
            
            Vector2 delta = new Vector2(-gyro.y, gyro.x);
            if (worldSpace)
            {
                delta.x = -gyro.y * xAxisInfluence + (gyro.z * (1 - xAxisInfluence)) * (invertZ ? -1 : 1);
            }
            return delta;

            float GetDriftCompensation(string axis) => Config.Gamepad.GetSettingValue($"gyro drift compensation {axis}");
        }
    }
}