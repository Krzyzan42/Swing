namespace EasyHighScore
{
    public interface IHighScoreDisplay
    {
        void DisplayHighScoresLocal();
        void DisplayNetworkError();
        void HandleHighScoresDownloaded(HighScore[] scores);
    }
}
