using System;
using Extentions;
using Gameplay.Weapons;
using UnityEngine;

namespace Gameplay.Entity.Player
{
    public class PlayerWeapons : LazyGetComponent<PlayerComposition>
    {
        [SerializeField] private WeaponBehaviour[] _weapons;

        private int _currentWeaponIndex;

        public WeaponBehaviour CurrentWeapon => _weapons[_currentWeaponIndex];
        
        private void EquipWeapon(int weaponIndex)
        {
            if (weaponIndex < 0 || weaponIndex >= _weapons.Length)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < _weapons.Length; i++)
            {
                if (i != weaponIndex)
                {
                    _weapons[i].Unequip();
                    continue;
                }
                _weapons[i].Equip();
                _currentWeaponIndex = weaponIndex;
            }
        }

        private void EquipNextWeapon()
        {
            EquipWeapon((_currentWeaponIndex + 1).RepeatIndex(_weapons.Length));
        }

        private void StartFire()
        {
            CurrentWeapon.TryShoot(Lazy.Vision.LookRay);
        }

        private void Awake()
        {
            Lazy.Controls.ChangedWeapon += EquipNextWeapon;
            Lazy.Controls.StartFire += StartFire;
            EquipWeapon(0);
        }
    }
}