using CandyCoded.env;
using EasyHighScore.Dino;
using UnityEngine;
using UnityEngine.Events;

namespace EasyHighScore
{
    public class HighScores : MonoBehaviour
    {
        private static HighScores _instance;

        public string dinoServerEndpoint = "https://exploitavoid.com/leaderboards/v1/api";

        public bool displayHighScoresOnDownload = true;

        public int scoresToFetchPerLeaderboard = 50;

        public UnityEvent uploadingHighScoreSuccess = new();
        public UnityEvent uploadingHighScoreError = new();

        private IHighScoreDisplay _highScoreDisplay;
        private ILeaderboardProvider _leaderboardProvider;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            if (_highScoreDisplay == null)
                Debug.LogWarning(
                    "DisplayHighScores component not found on this GameObject. HighScore display might not work.");

            var provider = new GameObject("LeaderboardProvider_Dino");
            provider.transform.SetParent(transform);

            env.TryParseEnvironmentVariable("LEADERBOARD_ID", out int leaderboardID);
            env.TryParseEnvironmentVariable("LEADERBOARD_SECRET", out string leaderboardSecret);

            var adapter = provider.AddComponent<DinoLeaderboardAdapter>();
            adapter.Initialize(
                leaderboardID,
                leaderboardSecret,
                dinoServerEndpoint,
                scoresToFetchPerLeaderboard
            );
            _leaderboardProvider = adapter;
        }

        public static void AddNewHighScore(string username, int score)
        {
            var provider = _instance._leaderboardProvider;
            if (provider == null)
            {
                Debug.LogError("Leaderboard provider is not configured.");
                _instance.uploadingHighScoreError.Invoke();
                return;
            }

            provider.UploadNewHighScore(username, score,
                () =>
                {
                    _instance.uploadingHighScoreSuccess.Invoke();
                    Debug.Log($"Successfully uploaded score for {username}. Refreshing scores.");
                    _instance.DownloadHighScores();
                },
                errorMsg =>
                {
                    _instance.uploadingHighScoreError.Invoke();
                    Debug.LogError($"Error uploading highScore: {errorMsg}");
                    _instance.Invoke(nameof(DownloadHighScores), 10f);
                });
        }

        public void DownloadHighScores()
        {
            if (_leaderboardProvider == null)
            {
                Debug.LogError("Leaderboard provider is not initialized.");
                return;
            }

            _leaderboardProvider.DownloadHighScores(
                scores =>
                {
                    if (displayHighScoresOnDownload && _highScoreDisplay != null)
                        _highScoreDisplay.HandleHighScoresDownloaded(scores);
                    Debug.Log($"HighScores downloaded. Count: {scores.Length}");
                },
                errorMsg =>
                {
                    Debug.LogError($"Error Downloading HighScores: {errorMsg}");
                    if (_highScoreDisplay != null) _highScoreDisplay.DisplayNetworkError();
                    Invoke(nameof(DownloadHighScores), 5f);
                });
        }

        public void ClearScoreForUser(string username)
        {
            Debug.LogWarning($"Attempting to clear score for {username} by setting score to 0.");
            AddNewHighScore(username, 0);
        }

        public static void DisplayLocalHighScores()
        {
            if (_instance == null || _instance._highScoreDisplay == null)
            {
                Debug.LogWarning("HighScores or DisplayHighScores not initialized for local display.");
                return;
            }

            _instance._highScoreDisplay.DisplayHighScoresLocal();
        }
    }
}