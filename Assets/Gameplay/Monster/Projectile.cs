using Player;
using UnityEngine;

namespace Enemies
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float damage;
        [SerializeField] private float speed;

        public float Speed => speed;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Monster")) return;

            if (other.CompareTag("Player")) other.GetComponent<Damageable>().TakeDamage(damage);

            Destroy(gameObject);
        }

        public void ShootAt(Vector2 direction)
        {
            GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * speed;
        }
    }
}