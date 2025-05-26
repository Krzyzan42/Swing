using System;

namespace EasyHighScore
{
    public interface ILeaderboardProvider
    {
        /// <summary>
        ///     Uploads a new highScore.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="score">The score.</param>
        /// <param name="onSuccess">Callback on successful upload.</param>
        /// <param name="onError">Callback on error, with an error message.</param>
        void UploadNewHighScore(string username, int score, Action onSuccess, Action<string> onError);

        /// <summary>
        ///     Downloads a list of highScores.
        /// </summary>
        /// <param name="onSuccess">Callback with the list of highScores.</param>
        /// <param name="onError">Callback on error, with an error message.</param>
        void DownloadHighScores(Action<HighScore[]> onSuccess, Action<string> onError);
    }
}