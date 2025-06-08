using EasyHighScore;
using TMPro;
using UnityEngine;

namespace UI.MainMenu
{
    public class Score : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI scoreText;

        public void Initialize(string playerName, string score)
        {
            playerNameText.text = playerName;
            scoreText.text = score;
        }

        public void Initialize(HighScore highScore)
        {
            playerNameText.text = highScore.username;
            scoreText.text = highScore.score.ToString();
        }
    }
}