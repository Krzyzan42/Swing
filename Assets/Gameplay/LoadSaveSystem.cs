using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace Gameplay
{
    public static class LoadSaveSystem
    {
        // C:/Users/janta/AppData/LocalLow/DefaultCompany/Swing\levels.json
        private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "levels.json");

        public static List<LevelInfo> GetLevels()
        {
            if (!File.Exists(SavePath))
                return GenerateDefaultLevels();

            var json = File.ReadAllText(SavePath);
            var wrapper = JsonUtility.FromJson<LevelInfoListWrapper>(json);
            return wrapper?.levels ?? GenerateDefaultLevels();
        }

        public static void SetLevelAsCompleted(string levelId, [CanBeNull] string nextLevelId, int bestTimeMilliseconds)
        {
            var levels = GetLevels();
            var level = levels.Find(l => l.levelId == levelId);
            if (level == null)
                return;

            level.unlocked = true;
            if (!level.BestTimeMilliseconds.HasValue || bestTimeMilliseconds < level.BestTimeMilliseconds.Value)
                level.BestTimeMilliseconds = bestTimeMilliseconds;

            var nextLevel = levels.Find(l => l.levelId == nextLevelId);
            if (nextLevel != null)
                nextLevel.unlocked = true;

            SaveLevels(levels);
        }

        private static void SaveLevels(List<LevelInfo> levels)
        {
            var wrapper = new LevelInfoListWrapper { levels = levels };
            var json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(SavePath, json);
        }

        private static List<LevelInfo> GenerateDefaultLevels()
        {
            var levels = new List<LevelInfo>();
            for (var i = 1; i <= 10; i++)
                levels.Add(new LevelInfo
                {
                    levelId = i.ToString(),
                    unlocked = i == 1,
                    BestTimeMilliseconds = null
                });
            SaveLevels(levels);
            return levels;
        }

        [Serializable]
        public class LevelInfo
        {
            public string levelId;
            public bool unlocked;
            public int? BestTimeMilliseconds;
        }

        [Serializable]
        private class LevelInfoListWrapper
        {
            public List<LevelInfo> levels;
        }
    }
}