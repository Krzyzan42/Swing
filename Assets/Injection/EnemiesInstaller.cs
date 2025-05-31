using Gameplay.Monster;
using Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Injection
{
    public class EnemiesInstaller : MonoInstaller
    {
        [SerializeField] private Character character;
        [SerializeField] private Projectile projectilePrefab;

        public override void InstallBindings()
        {
            Container.Bind<Character>().FromInstance(character).AsSingle();

            Container.Bind<Projectile>().FromInstance(projectilePrefab).AsSingle();
        }
    }
}