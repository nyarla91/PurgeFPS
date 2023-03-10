using System.Collections.Generic;
using Extentions;
using Localization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Settings.UI
{
    public class SliderDictionaryView : LazyGetComponent<TMP_Text>
    {
        [FormerlySerializedAs("_value")] [SerializeField] private List<LocalizedString> _labels;

        [Inject] private Settings Settings { get; set; }

        public void ApplyValue(float value)
        {
            LocalizedString localizedLabel = _labels[Mathf.RoundToInt(value)];
            int language = Settings.Config.Game.GetSettingValue("language");
            Lazy.text = localizedLabel.GetTranslation(language);
        }
    }
}