using System.Collections;
using Extentions;
using Input;
using UI.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Settings.UI
{
    public class SliderSettingMenuItem : SettingMenuItem
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private float _holdChangePeriod = 0.5f;

        private UIElement _uiElement;
        private Coroutine _changingValue;

        public void OnValueChanged(float value) => ApplySetting(Mathf.RoundToInt(value));
        
        protected override void SetStartingValue(int value) => _slider.value = value;

        private void Awake()
        {
            _uiElement = GetComponent<UIElement>();
            MenuControls.Actions.Always.MoveLeft.started += StartChangingLeft;
            MenuControls.Actions.Always.MoveLeft.canceled += StopChanging;
            MenuControls.Actions.Always.MoveRight.started += StartChangingRight;
            MenuControls.Actions.Always.MoveRight.canceled += StopChanging;
        }

        private void StartChangingRight(InputAction.CallbackContext _)
        {
            StopChanging(_);
            _changingValue = StartCoroutine(ChangingValue(1));
        }

        private void StartChangingLeft(InputAction.CallbackContext _)
        {
            StopChanging(_);
            _changingValue = StartCoroutine(ChangingValue(-1));
        }

        private void StopChanging(InputAction.CallbackContext _) => _changingValue?.Stop(this);

        private IEnumerator ChangingValue(int sign)
        {
            if ( ! _uiElement.IsSelected || ! gameObject.activeInHierarchy)
                yield break;
            MoveScrollbar(sign);
            yield return new WaitForSeconds(0.5f);
            while (true)
            {
                if ( ! _uiElement.IsSelected || ! gameObject.activeInHierarchy)
                    yield break;
                MoveScrollbar(sign);
                yield return new WaitForSeconds(_holdChangePeriod);
            }
        }

        private void MoveScrollbar(int delta) => _slider.value += delta;

        private void OnDestroy()
        {
            MenuControls.Actions.Always.MoveLeft.started -= StartChangingLeft;
            MenuControls.Actions.Always.MoveLeft.canceled -= StopChanging;
            MenuControls.Actions.Always.MoveRight.started -= StartChangingRight;
            MenuControls.Actions.Always.MoveRight.canceled -= StopChanging;
        }
    }
}