using Gameplay;
using Gameplay.Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private GameObject levelSelector;
        [SerializeField] private GameObject levelButtonUnlockedPrefab;
        [SerializeField] private GameObject levelButtonLockedPrefab;
        [SerializeField] private Transform levelsHolder;

        private void Start()
        {
            var levels = LoadSaveSystem.GetLevels();

            foreach (var level in levels)
            {
                var levelGameObject = Instantiate(level.unlocked ? levelButtonUnlockedPrefab : levelButtonLockedPrefab,
                    levelsHolder);
                levelGameObject.GetComponentInChildren<TextMeshProUGUI>().SetText(level.levelId);
                var capturedIndex = level.levelId;
                var button = levelGameObject.GetComponent<Button>();
                button.onClick.AddListener(() => SelectLevel(capturedIndex));
                button.interactable = level.unlocked;
            }
        }

        private static void SelectLevel(string levelId)
        {
            SceneLoader.LoadLevel(levelId);
        }

        public void ShowLevelSelector()
        {
            levelSelector.SetActive(true);
        }

        public void HideLevelSelector()
        {
            levelSelector.SetActive(false);
        }
    }
}