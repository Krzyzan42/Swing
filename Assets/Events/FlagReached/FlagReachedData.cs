using System;
using Gameplay.Player;

namespace Events.FlagReached
{
    [Serializable]
    public struct FlagReachedData
    {
        public Character character;

        public FlagReachedData(Character character)
        {
            this.character = character;
        }
    }
}