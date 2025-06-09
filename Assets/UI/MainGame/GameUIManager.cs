using System;
using System.Globalization;
using EasyHighScore.Dino;
using Gameplay.Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainGame
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject gameUI;
        [SerializeField] private GameObject victoryMenu;
        [SerializeField] private TextMeshProUGUI victoryText;
        [SerializeField] private GameObject newRecord;
        [SerializeField] private GameObject globalScores;
        [SerializeField] private ScoreDisplay scoreDisplay;
        [SerializeField] private Button uploadScoreButton;
        [SerializeField] private bool showGlobalScores = true;

        [SerializeField] private DinoLeaderboardAdapter leaderboardProvider;

        private float _lastDisplayedTime = -1f;
        private Action _onNextLevelClicked;

        private float _startTime;

        private void Start()
        {
            _startTime = Time.time;
        }

        private void Update()
        {
            var currentTime = Time.time - _startTime;
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

        public void EnableVictoryMenu(Action onNextLevelClicked, bool showNewRecord, string formattedTime,
            string username, int milliseconds)
        {
            DisableGameUI();
            Time.timeScale = 0f;
            victoryMenu.SetActive(true);

            newRecord.SetActive(showNewRecord);

            victoryText.SetText(victoryText.text.Replace("{time}", formattedTime));
            _onNextLevelClicked = onNextLevelClicked;

            globalScores.SetActive(false);

            if (!showGlobalScores) return;

            leaderboardProvider.DownloadHighScores(
                scores =>
                {
                    globalScores.SetActive(true);
                    scoreDisplay.DisplayHighScores(scores);
                },
                errorMsg => { Debug.LogError($"Failed to download high scores: {errorMsg}"); }
            );
            uploadScoreButton.onClick.AddListener(() =>
            {
                UploadNewHighScore(username, milliseconds);
                uploadScoreButton.interactable = false;
            });
        }

        private void RefreshGlobalScores()
        {
            leaderboardProvider.DownloadHighScores(
                scores => { scoreDisplay.DisplayHighScores(scores); },
                errorMsg => { Debug.LogError($"Failed to download high scores: {errorMsg}"); }
            );
        }

        private void UploadNewHighScore(string username, int milliseconds)
        {
            leaderboardProvider.UploadNewHighScore(username, milliseconds, RefreshGlobalScores,
                _ => { });
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