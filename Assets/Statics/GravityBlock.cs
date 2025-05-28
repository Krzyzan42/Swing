using Player;
using UnityEngine;

namespace Statics
{
    [RequireComponent(typeof(Collider2D))]
    public class GravityBlock : MonoBehaviour
    {
        private float _formerGravityScale;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var body = collision.gameObject.GetComponent<SwingBody>();

            if (!body) return;

            body.GrapplePossible = false;
            body.BreakGrapple();
            body.GravityScale = 0;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var body = collision.gameObject.GetComponent<SwingBody>();

            if (!body) return;

            body.GrapplePossible = true;
            body.GravityScale = 1;
        }
    }
}