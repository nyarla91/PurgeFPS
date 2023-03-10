using Extentions;
using UnityEngine;

namespace Gameplay.Entity.Player
{
    public class PlayerComposition : Transformable
    {
        private PlayerControls _controls;
        private PlayerVision _vision;
        private PlayerMovement _movement;

        public PlayerControls Controls => _controls ??= GetComponent<PlayerControls>();
        public PlayerVision Vision => _vision ??= GetComponent<PlayerVision>();
        public PlayerMovement Movement => _movement ??= GetComponent<PlayerMovement>();
    }
}