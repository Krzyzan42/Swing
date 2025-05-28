using Events.FlagReached;
using Other.Reset;
using Player;
using UnityEngine;
using Zenject;

namespace Other.FinishFlag
{
    [RequireComponent(typeof(Reset.Reset))]
    public class Flag : MonoBehaviour, IResettable
    {
        private BoxCollider2D _boxCollider;

        [Inject] private FlagReachedEventChannel _flagReachedEventChannel;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var character = collision.GetComponent<Character>();

            if (!character) return;

            _flagReachedEventChannel.RaiseEvent(new FlagReachedData(character));
            _renderer.color = Color.green;
        }

        public void Reset()
        {
            _renderer.color = Color.white;
        }
    }
}