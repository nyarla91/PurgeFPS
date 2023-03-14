using UnityEngine;

namespace Gameplay.Weapons
{
    public abstract class WeaponBehaviour : MonoBehaviour
    {
        [SerializeField] private float _shootPeriod;
        
        public void Unequip()
        {
            gameObject.SetActive(false);
        }

        public void Equip()
        {
            gameObject.SetActive(true);
        }

        public abstract void TryShoot(Ray ray);
    }
}