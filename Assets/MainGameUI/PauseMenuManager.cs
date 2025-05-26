using System.Globalization;
using Other;
using TMPro;
using UnityEngine;

namespace MainGameUI
{
    public class PauseMenuManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private GameObject pauseMenu;
        private float _lastDisplayedTime = -1f;

        private void Update()
        {
            var currentTime = Time.time;
            if (!(Mathf.Abs(currentTime - _lastDisplayedTime) >= 0.01f)) return;

            timeText.SetText($"Time {currentTime.ToString("0.00", CultureInfo.InvariantCulture)}");
            _lastDisplayedTime = currentTime;
        }

        public void EnablePauseMenu()
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }

        public void DisablePauseMenu()
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }

        public void LoadMainMenu()
        {
            SceneLoader.LoadMainMenu();
        }
    }
}