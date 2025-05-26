using UnityEngine;

namespace MainMenu
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private GameObject settings;

        public void SetMusicVolume(float volume)
        {
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }

        public void SetSoundEffectsVolume(float volume)
        {
            PlayerPrefs.SetFloat("EffectsVolume", volume);
        }

        public void EnableSettingsMenu()
        {
            settings.SetActive(true);
        }

        public void DisableSettingsMenu()
        {
            settings.SetActive(false);
        }
    }
}