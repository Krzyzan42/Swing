using EasyHighScore;
using UI.MainMenu;
using UnityEngine;

namespace UI.MainGame
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject scorePrefab;
        [SerializeField] private Transform scoresParent;

        public void DisplayHighScores(HighScore[] highScores)
        {
            var children = scoresParent.GetComponentsInChildren<Score>();
            foreach (var child in children) Destroy(child.gameObject);

            foreach (var highScore in highScores)
            {
                var scoreGameObject = Instantiate(scorePrefab, scoresParent);
                scoreGameObject.GetComponent<Score>().Initialize(highScore);
            }
        }
    }
}