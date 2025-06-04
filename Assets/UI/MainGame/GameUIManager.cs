using System;
using System.Globalization;
using Gameplay.Misc;
using TMPro;
using UnityEngine;

namespace UI.MainGame
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject gameUI;
        [SerializeField] private GameObject victoryMenu;
        [SerializeField] private TextMeshProUGUI victoryText;
        private float _lastDisplayedTime = -1f;
        private Action _onNextLevelClicked;

        private void Update()
        {
            var currentTime = Time.time;
            if (!(Mathf.Abs(currentTime - _lastDisplayedTime) >= 0.01f)) return;

            timeText.SetText($"Time {currentTime.ToString("0.00", CultureInfo.InvariantCulture)}");
            _lastDisplayedTime = currentTime;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }

        private void EnableGameUI()
        {
            gameUI.SetActive(true);
        }

        private void DisableGameUI()
        {
            gameUI.SetActive(false);
        }

        public void EnablePauseMenu()
        {
            DisableGameUI();
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }

        public void DisablePauseMenu()
        {
            EnableGameUI();
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }

        public void EnableVictoryMenu(Action onNextLevelClicked)
        {
            DisableGameUI();
            Time.timeScale = 0f;
            victoryMenu.SetActive(true);

            var currentTime = Time.time;

            victoryText.SetText(victoryText.text.Replace("{time}",
                currentTime.ToString("0.00", CultureInfo.InvariantCulture)));
            _onNextLevelClicked = onNextLevelClicked;
        }

        public void NextLevelClicked()
        {
            _onNextLevelClicked?.Invoke();
        }

        public void RestartClicked()
        {
            SceneLoader.ReloadScene();
        }

        public void LoadMainMenu()
        {
            SceneLoader.LoadMainMenu();
        }
    }
}