using System.Collections;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Grappables.JumpPad
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class JumpPad : Grappable
    {
        public Transform target;

        [FormerlySerializedAs("jumppadSpeedChange")]
        public AnimationCurve jumpPadSpeedChange;

        [FormerlySerializedAs("distTriggerTreshold")]
        public float distTriggerThreshold = 0.04f;

        public float maxSpeed = 30;
        public AnimationCurve speedCurve;
        public float maxDist;
        public float retreatSpeed = 2f;
        public float maxApproachSpeed = 20;
        public float velocitySaveDist = 1f;
        public float maxGrappleDistance = 3f;
        private bool _canBeGrabbed = true;

        private bool _canBeReleased;

        private SwingBody _current;
        private Rigidbody2D _rb;

        public Vector2 TargetPosition => new(target.position.x, target.position.y);
        public Vector2 JumpDirection => new(transform.up.x, transform.up.y);

        protected override void Awake()
        {
            base.Awake();
            _rb = GetComponent<Rigidbody2D>();
        }

        public bool IsTargetReached(Vector2 position)
        {
            return Vector2.Distance(position, TargetPosition) < distTriggerThreshold;
        }

        public void Activate()
        {
            StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            Vector2 initialPosition = transform.position;
            Vector2 direction = transform.up;
            Vector2 currentPosition = transform.position;

            while (Vector2.Distance(initialPosition, currentPosition) < maxDist)
            {
                _rb.linearVelocity = direction * maxSpeed;

                yield return new WaitForFixedUpdate();
                currentPosition = transform.position;
            }

            _rb.linearVelocity = Vector2.zero;

            // return to the original position
            _rb.linearVelocity = -direction * retreatSpeed;
            while (true)
            {
                if (Vector2.Dot(direction, _rb.position - initialPosition) < 0)
                {
                    _rb.linearVelocity = Vector2.zero;
                    _rb.position = initialPosition;
                    _canBeGrabbed = true;
                    yield break;
                }

                yield return null;
            }
        }

        public override bool Grab(SwingBody body)
        {
            if (!CanGrab(body))
                return false;
            _canBeReleased = false;
            _canBeGrabbed = false;
            StartCoroutine(JumpOffCoroutine(body));
            return true;
        }

        public override bool CanGrab(SwingBody body)
        {
            if (!_canBeGrabbed) return false;

            if ((transform.position - body.transform.position).magnitude > maxGrappleDistance)
                return false;

            var bodyDir = (body.transform.position - transform.position).normalized;
            var dot = Vector3.Dot(transform.up, bodyDir);
            print(dot);
            return dot > 0.2f;
        }

        public override bool Release()
        {
            return _canBeReleased;
        }

        private IEnumerator JumpOffCoroutine(SwingBody swingBody)
        {
            var rb = swingBody.GetComponent<Rigidbody2D>();
            Vector2 startPosition = swingBody.Position2D;
            var initialVelocity = rb.linearVelocity; // Save for perpendicular launch
    
            // Take full control
            rb.isKinematic = true;
            rb.linearVelocity = Vector2.zero;

            float duration = 0.5f; // Control how long the pull-in takes
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                // Move the player's position along a curve from their start to the target
                Vector2 newPos = Vector2.Lerp(startPosition, TargetPosition, t / duration);
                rb.MovePosition(newPos);
        
                if (IsTargetReached(rb.position))
                    break; // Exit if we arrive early
            
                yield return null; // Use normal update for positional lerp
            }

            // Ensure final position is exact
            rb.position = TargetPosition;

            // Give control back to physics and launch
            rb.isKinematic = false;
    
            var perp = Vector2.Perpendicular(JumpDirection).normalized;
            var savedVelocity = Vector2.Dot(perp, initialVelocity) * perp;

            _canBeReleased = true;
            swingBody.BreakGrapple();
            _current = null;
            Activate();
            rb.linearVelocity = maxSpeed * JumpDirection + savedVelocity;
        }
    }
}