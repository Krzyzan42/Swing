using System;
using System.Linq;
using UnityEngine;

namespace EasyHighScore.Dino
{
    public class
        DinoLeaderboardAdapter : MonoBehaviour, ILeaderboardProvider
    {
        private const int MaxNameLength = 10;

        public int scoresToFetch = 50;

        private CoroutineLeaderboardServerBridge _bridge;

        private int _configuredLeaderboardID;
        private string _configuredLeaderboardSecret;
        private string _configuredServerEndpoint;

        private void Awake()
        {
            _bridge = GetComponent<CoroutineLeaderboardServerBridge>();
            if (_bridge == null) _bridge = gameObject.AddComponent<CoroutineLeaderboardServerBridge>();
        }

        public void UploadNewHighScore(string username, int score, Action onSuccess, Action<string> onError)
        {
            if (_bridge == null || _bridge.leaderboardID == 0)
            {
                onError?.Invoke("DinoLeaderboardAdapter or its bridge is not properly initialized.");
                Debug.LogError(
                    $"[DinoLeaderboardAdapter ID:{_configuredLeaderboardID}] UploadNewHighScore called but bridge not ready. Bridge ID: {(_bridge != null ? _bridge.leaderboardID.ToString() : "null")}");
                return;
            }

            if (string.IsNullOrEmpty(username))
            {
                onError?.Invoke("Username cannot be empty.");
                return;
            }

            if (username.Length > MaxNameLength) username = username.Substring(0, MaxNameLength);

            Debug.Log(
                $"[DinoLeaderboardAdapter ID:{_configuredLeaderboardID}] Attempting to upload score for {username}: {score} to bridge ID: {_bridge.leaderboardID}");
            _bridge.SendUserValue(username, score,
                () =>
                {
                    Debug.Log(
                        $"[DinoLeaderboardAdapter ID:{_configuredLeaderboardID}] Score uploaded successfully for {username}: {score}");
                    onSuccess?.Invoke();
                },
                () =>
                {
                    var errorMsg =
                        $"[DinoLeaderboardAdapter ID:{_configuredLeaderboardID}] Error uploading score for {username}.";
                    Debug.LogError(errorMsg);
                    onError?.Invoke(errorMsg);
                });
        }

        public void DownloadHighScores(Action<HighScore[]> onSuccess,
            Action<string> onError)
        {
            if (!_bridge || _bridge.leaderboardID == 0)
            {
                onError?.Invoke("DinoLeaderboardAdapter or its bridge is not properly initialized.");
                Debug.LogError(
                    $"[DinoLeaderboardAdapter ID:{_configuredLeaderboardID}] DownloadHighScores called but bridge not ready. Bridge ID: {(_bridge != null ? _bridge.leaderboardID.ToString() : "null")}");
                onSuccess?.Invoke(Array.Empty<HighScore>());
                return;
            }

            const int startPosition = 1;

            Debug.Log(
                $"[DinoLeaderboardAdapter ID:{_configuredLeaderboardID}] Attempting to download scores. Fetch: {scoresToFetch}. Bridge ID: {_bridge.leaderboardID}");
            _bridge.RequestEntries(startPosition, scoresToFetch,
                dinoEntries =>
                {
                    if (dinoEntries == null)
                    {
                        Debug.LogWarning(
                            $"[DinoLeaderboardAdapter ID:{_configuredLeaderboardID}] Received null entries list from server.");
                        onSuccess?.Invoke(Array.Empty<HighScore>());
                        return;
                    }

                    var highScores = dinoEntries
                        .Select(entry => new HighScore(entry.Name, entry.GetValueAsInt()))
                        .ToArray();
                    Debug.Log(
                        $"[DinoLeaderboardAdapter ID:{_configuredLeaderboardID}] Downloaded {highScores.Length} scores.");
                    onSuccess?.Invoke(highScores);
                },
                () =>
                {
                    var errorMsg =
                        $"[DinoLeaderboardAdapter ID:{_configuredLeaderboardID}] Error downloading highScores.";
                    Debug.LogError(errorMsg);
                    onError?.Invoke(errorMsg);
                });
        }

        public void Initialize(int id, string secret, string endpoint, int fetchCount)
        {
            _configuredLeaderboardID = id;
            _configuredLeaderboardSecret = secret;
            _configuredServerEndpoint = endpoint;
            scoresToFetch = fetchCount;

            if (_bridge == null)
            {
                Debug.LogError(
                    "[DinoLeaderboardAdapter] Bridge was null during Initialize. Attempting to re-acquire/add.");
                _bridge = GetComponent<CoroutineLeaderboardServerBridge>();
                if (_bridge == null) _bridge = gameObject.AddComponent<CoroutineLeaderboardServerBridge>();
            }

            _bridge.leaderboardID = _configuredLeaderboardID;
            _bridge.leaderboardSecret = _configuredLeaderboardSecret;
            _bridge.serverEndpoint = _configuredServerEndpoint;

            Debug.Log($"[DinoLeaderboardAdapter] Initialized and Bridge configured for ID: {_bridge.leaderboardID}");
        }
    }
}