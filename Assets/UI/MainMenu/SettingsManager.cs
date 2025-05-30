using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private GameObject settings;

        [SerializeField] private TextMeshProUGUI musicText;
        [SerializeField] private TextMeshProUGUI effectsText;

        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider effectsSlider;

        public void HandleMusicVolumeChanged()
        {
            var volume = musicSlider.value;
            musicText.SetText($"Music Volume: {volume.ToString("0.00", CultureInfo.InvariantCulture)}");
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }

        public void HandleSoundEffectsVolumeChanged()
        {
            var volume = effectsSlider.value;
            effectsText.SetText($"Sound Effects Volume: {volume.ToString("0.00", CultureInfo.InvariantCulture)}");
            PlayerPrefs.SetFloat("EffectsVolume", volume);
        }

        public void EnableSettingsMenu()
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            effectsSlider.value = PlayerPrefs.GetFloat("EffectsVolume", 1f);

            HandleMusicVolumeChanged();
            HandleSoundEffectsVolumeChanged();

            settings.SetActive(true);
        }

        public void DisableSettingsMenu()
        {
            PlayerPrefs.Save();
            settings.SetActive(false);
        }
    }
}