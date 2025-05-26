using Other;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        public Button exitBtn;

        private void Start()
        {
            exitBtn.onClick.AddListener(ExitGame);
        }

        public void SelectLevel(int levelIndex)
        {
            SceneLoader.LoadLevel(levelIndex);
        }

        private static void ExitGame()
        {
            Application.Quit();
            EditorApplication.ExitPlaymode();
        }
    }
}