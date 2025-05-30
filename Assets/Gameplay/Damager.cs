using Player;
using UnityEngine;

namespace Statics
{
    [RequireComponent(typeof(Collider2D))]
    public class Damager : MonoBehaviour
    {
        [SerializeField] private LayerMask damageableLayers = ~0;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & damageableLayers) == 0) return;

            var damageable = other.GetComponent<Damageable>();
            if (damageable) damageable.TakeDamage();
        }
    }
}