using Player;
using UnityEngine;

namespace Statics
{
    [RequireComponent(typeof(Collider2D))]
    public class GravityBlock : MonoBehaviour
    {
        private Collider2D _collider;
        private float _formerGravityScale;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log(collision.CompareTag("Player"));

            SwingBody body = collision.gameObject.GetComponent<SwingBody>();
            if (body)
            {
                body.GrapplePossible = false;
                body.BreakGrapple();
                body.GravityScale = 0;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            SwingBody body = collision.gameObject.GetComponent<SwingBody>();
            if (body)
            {
                body.GrapplePossible = true;
                body.GravityScale = 1;
            }
        }
    }
}