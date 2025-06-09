using Events.FlagReached;
using Events.PlayerDeath;
using Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Injection
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private PlayerDeathEventChannel playerDeathEventChannel;
        [SerializeField] private FlagReachedEventChannel flagReachedEventChannel;

        [SerializeField] private Character character;

        public override void InstallBindings()
        {
            Container.Bind<PlayerDeathEventChannel>().FromInstance(playerDeathEventChannel).AsSingle();

            Container.Bind<FlagReachedEventChannel>().FromInstance(flagReachedEventChannel).AsSingle();

            Container.Bind<Character>().FromInstance(character).AsSingle();
        }
    }
}