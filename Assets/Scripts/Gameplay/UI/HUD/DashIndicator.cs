using Gameplay.Entity.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.UI.HUD
{
    public class DashIndicator : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] [Range(0, 1)] private float _spentAlpha;
        [SerializeField] [Range(0, 1)] private float _groundedAlpha;
        
        [Inject] private PlayerMovement PlayerMovement { get; set; }

        private void Update()
        {
            if (PlayerMovement.IsGrounded)
                _canvasGroup.alpha = _groundedAlpha;
            else if (PlayerMovement.DashReady)
                _canvasGroup.alpha = 1;
            else
                _canvasGroup.alpha = _spentAlpha;
        }
    }
}