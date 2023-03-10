using Extentions;
using Localization;
using TMPro;
using UnityEngine;
using Zenject;

namespace Settings.UI
{
    public class SliderValueView : LazyGetComponent<TMP_Text>
    {
        [SerializeField] private LocalizedString _prefix;
        [SerializeField] private LocalizedString _suffix;
        
        [Inject] private Settings Settings { get; set; }
        
        public void ApplyValue(float value)
        {
            int language = Settings.Config.Game.GetSettingValue("language"); 
            Lazy.text = $"{_prefix.GetTranslation(language)}{Mathf.RoundToInt(value).ToString()}{_suffix.GetTranslation(language)}";
        }
    }
}