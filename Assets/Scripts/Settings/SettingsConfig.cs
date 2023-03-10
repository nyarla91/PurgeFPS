using System;
using UnityEngine;

namespace Settings
{
    [Serializable]
    public struct SettingsConfig
    {
        [SerializeField] private SettingsSection _video;
        [SerializeField] private SettingsSection _audio;
        [SerializeField] private SettingsSection _game;
        [SerializeField] private SettingsSection _keyboardMouse;
        [SerializeField] private SettingsSection _gamepad;
        
        public SettingsSection Video => _video;
        public SettingsSection Audio => _audio;
        public SettingsSection Game => _game;
        public SettingsSection Gamepad => _gamepad;
        public SettingsSection KeyboardMouse => _keyboardMouse;
        
        public SettingsSection GetSection(SettingsSectionLabel label)
        {
            return label switch
            {
                SettingsSectionLabel.Video => Video,
                SettingsSectionLabel.Audio => Audio,
                SettingsSectionLabel.Game => Game,
                SettingsSectionLabel.KeyboardMouse => KeyboardMouse,
                SettingsSectionLabel.Gamepad => Gamepad,
                _ => throw new ArgumentOutOfRangeException(nameof(label), label, null)
            };
        }
    }

    public enum SettingsSectionLabel
    {
        Video,
        Audio,
        Game,
        KeyboardMouse,
        Gamepad,
    }
}