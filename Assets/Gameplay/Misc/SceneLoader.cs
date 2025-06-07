using UnityEngine.SceneManagement;

namespace Gameplay.Misc
{
    internal enum Scene
    {
    }

    public static class SceneLoader
    {
        public static void Load(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        public static void LoadLevel(string level)
        {
            SceneManager.LoadScene($"Scenes/Levels/{level}");
        }

        public static void LoadCutscene(string cutscene)
        {
            SceneManager.LoadScene($"Scenes/Cutscenes/{cutscene}");
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