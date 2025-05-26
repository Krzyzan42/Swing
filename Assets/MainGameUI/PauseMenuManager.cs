using System.Globalization;
using Other;
using TMPro;
using UnityEngine;

namespace MainGameUI
{
    public class PauseMenuManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private GameObject pauseMenu;

        private void Update()
        {
            timeText.SetText(Time.time.ToString(CultureInfo.InvariantCulture));
        }

        public void EnablePauseMenu()
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }

        public void DisablePauseMenu()
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }

        public void LoadMainMenu()
        {
            SceneLoader.LoadMainMenu();
        }
    }
}