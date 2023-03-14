using Gameplay.Entity.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Entity.Targets
{
    public class ChargeRestoreTarget : MonoBehaviour, IHittable
    {
        [Inject] private PlayerMovement PlayerMovement { get; set; }
        
        public void TakeHit()
        {
            PlayerMovement.ReadyCharge();
        }
    }
}