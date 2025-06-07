using LM;
using UnityEngine;
using Zenject;

namespace Injection
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] private SoundManager soundManager;

        public override void InstallBindings()
        {
            Container.Bind<SoundManager>().FromInstance(soundManager).AsSingle();
        }
    }
}