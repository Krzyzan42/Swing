using Other;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        public Button startBtn, exitBtn;

        private void Start()
        {
            startBtn.onClick.AddListener(StartGame);
            exitBtn.onClick.AddListener(ExitGame);
        }

        private static void StartGame()
        {
            SceneLoader.LoadLevel(1);
        }

        private static void ExitGame()
        {
            Application.Quit();
            EditorApplication.ExitPlaymode();
        }
    }
}