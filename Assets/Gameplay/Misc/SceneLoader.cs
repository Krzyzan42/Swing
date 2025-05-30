using UnityEngine.SceneManagement;

namespace Gameplay.Misc
{
    internal enum Scene
    {
    }

    public static class SceneLoader
    {
        public static void LoadLevel(int level)
        {
            SceneManager.LoadScene($"Scenes/Levels/{level.ToString()}");
        }

        public static void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public static void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}