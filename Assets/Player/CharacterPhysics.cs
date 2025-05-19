using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class CharacterPhysics : MonoBehaviour
    {
        [Range(0f, 1f)] [SerializeField] private float fallAccelerationTime;

        [SerializeField] private float maxFallSpeed;

        [Range(0f, 0.1f)] [SerializeField] private float fallDrag;

        [Range(0, 0.2f)] [SerializeField] private float horizontalDrag = 0.03f;

        [Range(0, 20)] [SerializeField] private float maxHorizontalVel = 5;

        private Rigidbody2D _rb;

        public bool EnableGravity { get; set; } = true;
        public float GravityScale { get; set; } = 1f;
        public bool EnableFallDrag { get; set; } = true;
        public float FallDragScale { get; set; } = 1f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
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