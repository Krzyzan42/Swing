using Other.Reset;
using Player;
using UnityEngine;

namespace Other.FinishFlag
{
    [RequireComponent(typeof(Reset.Reset))]
    public class Flag : MonoBehaviour, IResettable
    {
        private BoxCollider2D _boxCollider;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Character>()) _renderer.color = Color.green;
        }

        public void Reset()
        {
            _renderer.color = Color.white;
        }
    }
}