using UnityEngine;

namespace Grappables
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Grappable : MonoBehaviour
    {
        private GrappleManager _grappleManager;
        private Rigidbody2D _rigidbody;

        public Vector2 Position2D => new(transform.position.x, transform.position.y);

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        protected virtual void Start()
        {
            _grappleManager = FindAnyObjectByType<GrappleManager>();
            _grappleManager.AddGrappable(this);
        }

        public void Grab(Joint2D joint)
        {
            joint.connectedBody = _rigidbody;
            joint.enabled = true;

            OnGrab();
        }

        public void Release(Joint2D joint)
        {
            joint.enabled = false;

            OnRelease();
        }

        protected virtual void OnGrab()
        {
        }

        protected virtual void OnRelease()
        {
        }
    }
}