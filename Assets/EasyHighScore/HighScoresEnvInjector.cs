using CandyCoded.env;
using EasyHighScore.Dino;
using UnityEngine;

namespace EasyHighScore
{
    public class HighScoresEnvInjector : MonoBehaviour
    {
        public string dinoServerEndpoint = "https://exploitavoid.com/leaderboards/v1/api";

        public int scoresToFetchPerLeaderboard = 50;

        [SerializeField] private DinoLeaderboardAdapter dinoLeaderboardAdapter;

        private void Awake()
        {
            env.TryParseEnvironmentVariable("DINO_ID", out int leaderboardID);
            env.TryParseEnvironmentVariable("DINO_SECRET", out string leaderboardSecret);

            dinoLeaderboardAdapter.Initialize(
                leaderboardID,
                leaderboardSecret,
                dinoServerEndpoint,
                scoresToFetchPerLeaderboard
            );
        }
    }
}