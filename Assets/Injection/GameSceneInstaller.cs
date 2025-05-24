using Player;
using UnityEngine;
using Zenject;

namespace Other
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private Character character;
        
        public override void InstallBindings()
        {
            Container.Bind<Character>().FromInstance(character).AsSingle();
        }
    }
}
