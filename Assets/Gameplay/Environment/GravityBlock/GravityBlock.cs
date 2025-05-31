using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Environment.GravityBlock
{
    [RequireComponent(typeof(Collider2D))]
    public class GravityBlock : MonoBehaviour
    {
        private float _formerGravityScale;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var body = collision.gameObject.GetComponent<SwingBody>();

            if (!body) return;

            _formerGravityScale = body.GravityScale;
            body.GrapplePossible = false;
            body.BreakGrapple();
            body.GravityScale = 0;
            print("hehexd");
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var body = collision.gameObject.GetComponent<SwingBody>();

            if (!body) return;

            body.GrapplePossible = true;
            body.GravityScale = _formerGravityScale;
            print("ahhh");
        }
    }
}