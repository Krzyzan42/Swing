using Events.FlagReached;
using Events.PlayerDeath;
using LM;
using UnityEngine;
using Zenject;

namespace Injection
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private PlayerDeathEventChannel playerDeathEventChannel;
        [SerializeField] private FlagReachedEventChannel flagReachedEventChannel;

        [SerializeField] private SoundManager soundManager;

        public override void InstallBindings()
        {
            Container.Bind<PlayerDeathEventChannel>().FromInstance(playerDeathEventChannel).AsSingle();

            Container.Bind<FlagReachedEventChannel>().FromInstance(flagReachedEventChannel).AsSingle();

            Container.Bind<SoundManager>().FromInstance(soundManager).AsSingle();
        }
    }
}