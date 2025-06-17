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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) SceneLoader.Load(nextScene);
        }

        private async UniTaskVoid AnimateCutscene()
        {
            var token = this.GetCancellationTokenOnDestroy();
            await UniTask.Delay(TimeSpan.FromSeconds(animationTime), cancellationToken: token);

            SceneLoader.Load(nextScene);
        }
    }
}