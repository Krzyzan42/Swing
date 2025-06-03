using System;
using Gameplay.Player;

namespace Events.FlagReached
{
    [Serializable]
    public struct LevelCompleteData
    {
        public Character character;

        public FlagReachedData(Character character)
        {
            this.character = character;
        }
    }
}