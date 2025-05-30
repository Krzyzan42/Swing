using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Grappables
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Grappable : MonoBehaviour
    {
        public Vector2 Position2D => new(transform.position.x, transform.position.y);

        protected virtual void Awake()
        {
            var grappleManager = FindAnyObjectByType<GrappleManager>();
            grappleManager.AddGrappable(this);
        }

        public abstract bool Grab(SwingBody body);

        public abstract bool CanGrab(SwingBody body);

        public abstract bool Release();
    }
}