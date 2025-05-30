using Gameplay;
using TMPro;
using UnityEngine;

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
                levelGameObject.GetComponentInChildren<TextMeshProUGUI>().SetText(level.levelIndex.ToString());
            }
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