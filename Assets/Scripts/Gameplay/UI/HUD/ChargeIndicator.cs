using System;
using Extentions;
using Gameplay.Entity.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.UI.HUD
{
    public class ChargeIndicator : MonoBehaviour
    {
        [SerializeField] private Image _fill;
        [SerializeField] [Range(0, 1)] private float _cooldownAlpha;
        [SerializeField] private float _minFillAmmount;
        [SerializeField] private float _maxFillAmmount;
        
        [Inject] private PlayerMovement PlayerMovement { get; set; }

        private void Update()
        {
            if (PlayerMovement.ChargeAllowed)
            {
                _fill.color = _fill.color.WithA(1);
                _fill.fillAmount = 1;
            }
            else
            {
                _fill.color = _fill.color.WithA(_cooldownAlpha);
                _fill.fillAmount = Mathf.Lerp(_minFillAmmount, _maxFillAmmount, PlayerMovement.ChargeCooldownPercent);
            }
        }
    }
}