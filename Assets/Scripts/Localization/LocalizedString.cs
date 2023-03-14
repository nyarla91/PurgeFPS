using System;
using UnityEngine;

namespace Localization
{
    [Serializable]
    public class LocalizedString
    {
        [SerializeField] [TextArea(1, 15)] private string _english;
        [Tooltip("Leave empty to inherit from English")]
        [SerializeField] [TextArea(1, 15)] private string _russian;

        public string Russian => _russian.Equals("") ? _english : _russian;
        public string English => _english;

        public string GetTranslation(int language) => language switch
        {
            0 => English,
            1 => Russian,
            _ => ""
        };
    }
}