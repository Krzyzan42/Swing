using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Grappables.JumpPad
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class JumpPad : Grappable
    {
        public Transform target;

        [FormerlySerializedAs("distTriggerTreshold")]
        public float distTriggerThreshold = 0.04f;

        public float maxSpeed = 30;
        public AnimationCurve speedCurve;
        public float maxDist;
        public float retreatSpeed = 2f;
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
                var t = Vector2.Distance(initialPosition, currentPosition) / maxDist;
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
                    yield break;
                }

                yield return null;
            }
        }
    }
}