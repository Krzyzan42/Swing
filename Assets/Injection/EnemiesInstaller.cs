using Gameplay.Monster;
using UnityEngine;
using Zenject;

namespace Injection
{
    public class EnemiesInstaller : MonoInstaller
    {
        [SerializeField] private Projectile projectilePrefab;

        public override void InstallBindings()
        {
            Container.Bind<Projectile>().FromInstance(projectilePrefab).AsSingle();
        }
    }
}