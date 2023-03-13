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
        private DeviceWatcher _deviceWatcher;
        private GameplayActions _actions;
        public SettingsConfig Config { get; private set; }

        public Vector2 CameraDelta
        {
            get
            {
                Vector2 input = _actions.Player.Camera.ReadValue<Vector2>();

                if (_deviceWatcher.CurrentInputScheme == InputScheme.KeyboardMouse)
                {
                    input *= new Vector2(Config.KeyboardMouse.GetSettingValue("sensitivity x"), Config.KeyboardMouse.GetSettingValue("sensitivity y"));
                    return input;
                }

                input = ApplyDeadzone(input);
                input = InvertAxes(input);
                input = ApplySensivity(input);
                //input = ApplyAimAssist(input);
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

                Vector2 ApplyAimAssist(Vector2 originInput)
                {
                    if (Config.Gamepad.IsSettingToggled("gyro enabled"))
                        return originInput;
                    
                    VisionRaycast raycast = Lazy.Vision.Raycast;
                    float hitDistance = raycast.Raycast.distance;
                    if (raycast.Layer == 9 && hitDistance > 0)
                    {
                        float sensivityReduction = 0.6f * Config.Gamepad.GetSettingValue("aim assist");
                        const float MinDistance = 30;
                        const float MaxDistance = 150;
                        sensivityReduction *= Mathf.Clamp((hitDistance - MinDistance) / (MaxDistance - MinDistance), 0, 1);
                        originInput *= 1 - sensivityReduction;
                    }

                    return originInput;
                }
                
                Vector2 ApplyGyro(Vector2 originInput)
                {
                    if (!Config.Gyro.IsSettingToggled("enabled"))
                        return originInput;
                    
                    Vector2 gyroAxes = GetScreenGyroDelta(Config.Gyro.IsSettingToggled("world space"));
                    if (gyroAxes.magnitude < Config.Gyro.GetSettingValue("deadzone"))
                        return originInput;
                    
                    gyroAxes.x *= Config.Gyro.GetSettingPercent("scale x");
                    originInput += gyroAxes * 270 * Config.Gyro.GetSettingPercent("scale y");
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

        public event Action OnJumo;
        public event Action OnCharge;

        [Inject]
        private void Construct(GameplayActions actions, DeviceWatcher deviceWatcher, Settings.Settings settings)
        {
            _deviceWatcher = deviceWatcher;
            _actions = actions;
            _actions.Enable();
            
            _actions.Player.Jump.started += OnJumoInvoke;
            _actions.Player.Charge.started += OnChargeInvoke;

            Config = settings.Config;
        }

        private void OnJumoInvoke(InputAction.CallbackContext _) => OnJumo?.Invoke();
        private void OnChargeInvoke(InputAction.CallbackContext _) => OnCharge?.Invoke();

        private void OnDestroy()
        {
            _actions.Player.Jump.started += OnJumoInvoke;
            _actions.Player.Charge.started += OnChargeInvoke;
        }
        
        private Vector2 GetScreenGyroDelta(bool worldSpace)
        {
            Vector3 gyro = DualshockMotion.RawGyro;
            Debug.Log(gyro);
            //gyro += new Vector3(GetDriftCompensation("x"), GetDriftCompensation("y"), GetDriftCompensation("z"));
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