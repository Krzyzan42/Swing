using Grappables;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SwingBody : MonoBehaviour
    {
        [Range(0f, 1f)] [SerializeField] private float fallAccelerationTime;

        [SerializeField] private float maxFallSpeed;

        [Range(0f, 0.1f)] [SerializeField] private float fallDrag;

        [Range(0, 0.2f)] [SerializeField] private float horizontalDrag = 0.03f;

        [Range(0, 20)] [SerializeField] private float maxHorizontalVel = 5;

        private Rigidbody2D _rb;
        private GrappleManager _grappleManager;
        private Grappable _currentGrapple = null;

        public bool EnableGravity { get; set; } = true;
        public float GravityScale { get; set; } = 1f;
        public bool EnableFallDrag { get; set; } = true;
        public float FallDragScale { get; set; } = 1f;
        public bool GrapplePossible { get; set; } = true;
        public bool IsGrappled => _currentGrapple != null;
        public Vector2 Position2D => new(transform.position.x, transform.position.y);

        public UnityEvent<Grappable> GrappleConnected = new();
        public UnityEvent<Grappable> GrappleDisconnected = new();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _grappleManager = FindAnyObjectByType<GrappleManager>();
        }

        public bool Grapple(Grappable point)
        {
            if (!CanGrapple(point))
                return false;
            bool success = point.Grab(this);
            if (success)
            {
                _currentGrapple = point;
                GrappleConnected.Invoke(_currentGrapple);
            }
            return success;
        }

        public bool CanGrapple(Grappable point)
        {
            if (!IsGrappled && GrapplePossible)
                return point.CanGrab(this);
            else
                return false;
        }

        public bool BreakGrapple()
        {
            bool success = false;
            if (_currentGrapple != null)
            {
                success = _currentGrapple.Release();
                if (success)
                {
                    Grappable temp = _currentGrapple;
                    _currentGrapple = null;
                    GrappleDisconnected.Invoke(temp);
                }
            }
            return success;
        }


        private void FixedUpdate()
        {
            if (EnableFallDrag && _rb.linearVelocity.y < -maxFallSpeed)
                ApplyDrag();

            if (EnableGravity && _rb.linearVelocity.y > -maxFallSpeed)
                ApplyGravity();

            var velocity = _rb.linearVelocity;
            if (Mathf.Abs(velocity.x) > maxHorizontalVel)
                velocity.x *= 1 - horizontalDrag;
            _rb.linearVelocity = velocity;
        }

        private void ApplyDrag()
        {
            var velocity = _rb.linearVelocity;
            var speedDiff = Mathf.Abs(velocity.y - maxFallSpeed);

            var slowdown = speedDiff * fallDrag * FallDragScale;
            velocity.y -= slowdown;
            _rb.linearVelocity = velocity;
        }

        private void ApplyGravity()
        {
            var downForce = maxFallSpeed / fallAccelerationTime;
            var downAcceleration = downForce * Time.fixedDeltaTime * GravityScale;

            var velocity = _rb.linearVelocity;
            velocity.y -= downAcceleration;
            if (velocity.y < -maxFallSpeed)
                velocity.y = -maxFallSpeed;

            _rb.linearVelocity = velocity;
        }
    }
}