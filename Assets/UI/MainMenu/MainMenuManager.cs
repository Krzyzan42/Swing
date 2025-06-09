using Gameplay.Misc;
using UnityEngine;

namespace UI.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject credits;
        [SerializeField] private string raceLevelName;

        public void LoadRaceLevel()
        {
            SceneLoader.LoadLevel(raceLevelName);
        }

        public void ShowCredits()
        {
            credits.SetActive(true);
        }

        public void HideCredits()
        {
            credits.SetActive(false);
        }

        public void ExitGame()
        {
            Application.Quit();
#if UNITY_ANDROID || UNITY_IOS
#else
            EditorApplication.ExitPlaymode();
#endif
        }
    }
}