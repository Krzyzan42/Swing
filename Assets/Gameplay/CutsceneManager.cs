using System;
using Cysharp.Threading.Tasks;
using Gameplay.Misc;
using UnityEngine;

namespace Gameplay
{
    public class CutsceneManager : MonoBehaviour
    {
        [SerializeField] private float animationTime;

        [SerializeField] private string nextScene;

        private void Start()
        {
            AnimateCutscene().Forget();
        }

        private async UniTaskVoid AnimateCutscene()
        {
            var token = this.GetCancellationTokenOnDestroy();
            await UniTask.Delay(TimeSpan.FromSeconds(animationTime), cancellationToken: token);

            SceneLoader.Load(nextScene);
        }
    }
}