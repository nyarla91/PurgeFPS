using Extentions;
using Gameplay.Entity;
using UnityEngine;

namespace Gameplay.Weapons
{
    public class HitscanWeaponBehaviour : WeaponBehaviour
    {
        public override void TryShoot(Ray ray)
        {
            LayerMask mask = LayerMask.GetMask("Obstacle", "Enemy", "NeutralTarget");
            if (Physics.Raycast(ray, out RaycastHit raycast, 900, mask))
            {
                if (raycast.collider == null)
                    return;
                IHittable[] hittables = raycast.collider.GetComponents<IHittable>();
                if (hittables.Length == 0)
                    return;
                hittables.Foreach(hittable => hittable.TakeHit());
            }
        }
    }
}