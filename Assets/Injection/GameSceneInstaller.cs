using Enemies;
using Events.PlayerDeath;
using Player;
using UnityEngine;
using Zenject;

namespace Injection
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private Character character;
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private PlayerDeathEventChannelSO playerDeathEventChannel;

        public override void InstallBindings()
        {
            Container.Bind<Character>().FromInstance(character).AsSingle();

            Container.Bind<Projectile>().FromInstance(projectilePrefab).AsSingle();

            Container.Bind<PlayerDeathEventChannelSO>().FromInstance(playerDeathEventChannel).AsSingle();
        }
    }
}