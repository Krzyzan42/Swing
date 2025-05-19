using System;
using System.Collections;
using Other.Reset;
using UnityEngine;

namespace Grappables.SwingPoint
{
    public class MovingBlock : Grappable, IResettable
    {
        public float correctionRange = 0.2f;
        public float correctionStrength = 3f;
        public float speedScale = 20;
        public AnimationCurve speedCurve;
        public PathCreation.PathCreator path;

        public Transform player;

        private Coroutine _currentRoutine;

        [NonSerialized] private Rigidbody2D _rigidbody;

        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Reset()
        {
            if (_currentRoutine != null)
                StopCoroutine(_currentRoutine);
        }

        protected override void OnGrab()
        {
            _currentRoutine = StartCoroutine(MoveCoroutine());
        }

        private IEnumerator MoveCoroutine()
        {
            var t = 0f;
            while (t < 1f)
            {
                t = path.path.GetClosestTimeOnPath(transform.position);
                var vel = path.path.GetDirection(t) * speedScale * speedCurve.Evaluate(t);

                var perpendicularness =
                    1 - Mathf.Abs(Vector3.Dot((player.position - transform.position).normalized, vel.normalized));
                perpendicularness = Mathf.Clamp(perpendicularness, 1 - correctionRange, 1);

                var strength = Reframe(perpendicularness, 1 - correctionRange, 1, 1 - correctionStrength, 1f);

                _rigidbody.linearVelocity = vel * strength;
                yield return null;
            }

            _rigidbody.linearVelocity = Vector2.zero;
        }

        private float Reframe(float x, float startX, float endX, float startY, float endY)
        {
            var percentage = (x - startX) / (endX - startX);
            return startY + percentage * (endY - startY);
        }
    }
}