using System;
using Gameplay.Player;

namespace Events.PlayerDeath
{
    [Serializable]
    public struct DeathData
    {
        public Character character;

        public DeathData(Character character)
        {
            this.character = character;
        }
    }
}