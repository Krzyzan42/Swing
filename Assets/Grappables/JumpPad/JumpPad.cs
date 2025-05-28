using System.Collections;
using Other.Reset;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Grappables.JumpPad
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class JumpPad : Grappable, IResettable
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

        public void Reset()
        {
            StopAllCoroutines();
            _canBeGrabbed = true;
            _canBeReleased = true;
            if (_current)
            {
                _current.BreakGrapple();
                _current = null;
            }
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
            float t = 0;
            var rb = swingBody.GetComponent<Rigidbody2D>();
            var initial = rb.linearVelocity;
            var savedVelocity = Vector2.zero;
            _current = swingBody;

            while (t < 1 && !IsTargetReached(rb.position))
            {
                var dir = (TargetPosition - swingBody.Position2D).normalized;
                var vel = Vector2.Lerp(initial, dir * maxApproachSpeed, jumpPadSpeedChange.Evaluate(t));

                if (Vector2.Distance(TargetPosition, swingBody.Position2D) < velocitySaveDist &&
                    savedVelocity == Vector2.zero)
                {
                    var perp = Vector2.Perpendicular(JumpDirection).normalized;
                    savedVelocity = Vector2.Dot(perp, vel) * perp;
                }

                rb.linearVelocity = vel;
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _canBeReleased = true;
            swingBody.BreakGrapple();
            _current = null;
            Activate();
            rb.linearVelocity = maxSpeed * JumpDirection + savedVelocity;
        }
    }
}