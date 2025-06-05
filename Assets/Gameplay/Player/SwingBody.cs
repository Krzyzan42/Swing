using Events.PlayerDeath;
using Gameplay.Grappables;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Gameplay.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SwingBody : MonoBehaviour
    {
        [Range(0f, 1f)] [SerializeField] private float fallAccelerationTime;

        [SerializeField] private float maxFallSpeed;

        [Range(0f, 0.1f)] [SerializeField] private float fallDrag;

        [Range(0, 0.2f)] [SerializeField] private float horizontalDrag = 0.03f;

        [Range(0, 20)] [SerializeField] private float maxHorizontalVel = 5;

        public UnityEvent<Grappable> GrappleConnected = new();
        public UnityEvent<Grappable> GrappleDisconnected = new();

        [field: SerializeField] public float FallDragScale { get; set; } = 1f;

        private Grappable _currentGrapple;
        private GrappleManager _grappleManager;

        [Inject] private PlayerDeathEventChannel _playerDeathEventChannel;

        private Rigidbody2D _rb;

        public bool EnableGravity { get; set; } = true;
        public float GravityScale { get; set; } = 1f;
        public bool EnableFallDrag { get; set; } = true;
        public bool GrapplePossible { get; set; } = true;
        public bool InsideGravityBlock { get; set; } = false;
        public bool IsGrappled => _currentGrapple != null;
        public Vector2 Velocity => _rb.linearVelocity;
        public Vector2 Position2D => new(transform.position.x, transform.position.y);

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _grappleManager = FindAnyObjectByType<GrappleManager>();
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

        public bool Grapple(Grappable point)
        {
            if (!CanGrapple(point))
                return false;
            var success = point.Grab(this);
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
            return false;
        }

        public bool BreakGrapple()
        {
            if (_currentGrapple == null) return false;

            var success = _currentGrapple.Release();

            if (!success) return false;

            var temp = _currentGrapple;
            _currentGrapple = null;
            GrappleDisconnected.Invoke(temp);

            return true;
        }

        private void ApplyDrag()
        {
            var velocity = _rb.linearVelocity;
            var speedDiff = Mathf.Abs(velocity.y - maxFallSpeed);

            var slowdown = speedDiff * fallDrag * FallDragScale;
            velocity.y *= 1 - slowdown;
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