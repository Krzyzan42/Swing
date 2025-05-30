using Events.FlagReached;
using Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Environment.FinishFlag
{
    public class Flag : MonoBehaviour
    {
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
    }
}