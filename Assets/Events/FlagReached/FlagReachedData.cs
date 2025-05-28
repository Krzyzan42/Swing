using System;
using Player;

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