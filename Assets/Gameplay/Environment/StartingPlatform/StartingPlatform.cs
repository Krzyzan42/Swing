using Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Environment.StartingPlatform
{
    public class StartingPlatform : MonoBehaviour
    {
        public new Collider2D collider;
        public new Animator animation;

        [Inject] private Character _character;

        private bool _hidden;

        private void Awake()
        {
            animation.enabled = false;
            if (_character)
                _character.grappled.AddListener(HidePlatform);
        }

        private void HidePlatform()
        {
            if (_hidden) return;

            animation.enabled = true;
            collider.enabled = false;
            _hidden = true;
        }
    }
}