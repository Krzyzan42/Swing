using Gameplay.Misc;
using UnityEditor;
using UnityEngine;

namespace UI.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject credits;

        public void ShowCredits()
        {
            credits.SetActive(true);
        }

        public void HideCredits()
        {
            credits.SetActive(false);
        }

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