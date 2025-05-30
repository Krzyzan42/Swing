using Gameplay.Misc.Reset;
using UnityEngine;

namespace Gameplay.Grappables.StartingPlatform
{
    [RequireComponent(typeof(Reset))]
    public class StartingPlatform : MonoBehaviour, IResettable
    {
        private BoxCollider2D _boxCollider2D;
        private SpriteRenderer _spriteRenderer;

        // Update is called once per frame
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            if (!Input.GetKey(KeyCode.Space)) return;

            _boxCollider2D.enabled = false;
            _spriteRenderer.enabled = false;
        }

        public void Reset()
        {
            _boxCollider2D.enabled = true;
            _spriteRenderer.enabled = true;
        }
    }
}