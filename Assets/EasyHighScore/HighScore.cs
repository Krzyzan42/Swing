using System;

namespace EasyHighScore
{
    [Serializable]
    public class HighScore
    {
        public int score;
        public string username;

        public HighScore(string username, int score)
        {
            this.username = username;
            this.score = score;
        }
    }
}