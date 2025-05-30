using Gameplay.Misc;
using UnityEditor;
using UnityEngine;

namespace MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        public void SelectLevel(int levelIndex)
        {
            SceneLoader.LoadLevel(levelIndex);
        }

        public void ExitGame()
        {
            Application.Quit();
            EditorApplication.ExitPlaymode();
        }
    }
}