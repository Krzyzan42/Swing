using System;
using System.Collections;
using Other.Reset;
using Player;
using UnityEngine;

namespace Grappables.SwingPoint
{
    public class MovingBlock : Grappable, IResettable
    {
        public float correctionRange = 0.2f;
        public float correctionStrength = 3f;
        public float speedScale = 20;
        [Range(0, 1)] public float SwingUpGravityScale = 0.8f;
        public float maxDistance = 6;
        public AnimationCurve speedCurve;
        public PathCreation.PathCreator path;
        public Transform sprite;
        public Transform startSprite;
        public Transform endSprite;

        private Coroutine _currentRoutine;
        private SwingBody _attachedBody;
        private bool _used = false;
        private Rigidbody2D _rigidbody;

        protected override void Awake()
        {
            base.Awake();
            _rigidbody = GetComponent<Rigidbody2D>();
            sprite.transform.up = Vector2.down;
        }

		public void Start()
		{
            startSprite.transform.position = path.path.GetPointAtTime(0);
            endSprite.transform.position = path.path.GetPointAtTime(1, PathCreation.EndOfPathInstruction.Stop);
		}

		public void Reset()
        {
            if (_currentRoutine != null)
                StopCoroutine(_currentRoutine);
            _used = false;
        }

        public override bool Grab(SwingBody body)
        {
            if (!CanGrab(body))
                return false;
            _attachedBody = body;
            DistanceJoint2D joint = body.GetComponent<DistanceJoint2D>();
            joint.connectedBody = GetComponent<Rigidbody2D>();
            joint.enabled = true;

            _currentRoutine = StartCoroutine(MoveCoroutine());
            return true;
        }

		public override bool Release()
		{
            DistanceJoint2D joint = _attachedBody.GetComponent<DistanceJoint2D>();
            joint.connectedBody = null;
            joint.enabled = false;
            _attachedBody.GravityScale = 1;
            _attachedBody = null;
            return true;
		}

		void Update()
		{
            float t = path.path.GetClosestTimeOnPath(transform.position);
            Vector3 dir = path.path.GetDirection(t, PathCreation.EndOfPathInstruction.Stop);
            sprite.transform.up = dir;
		}

        void FixedUpdate()
        {
            if (!_attachedBody) return;

            Rigidbody2D rb = _attachedBody.GetComponent<Rigidbody2D>();
            if (rb.linearVelocity.y > 0 && !IsAboveSwinger(_attachedBody))
                _attachedBody.GravityScale = SwingUpGravityScale;
            else
                _attachedBody.GravityScale = 1;
		}

		private IEnumerator MoveCoroutine()
        {
            var t = 0f;
            while (t < 0.95f)
            {
                t = path.path.GetClosestTimeOnPath(transform.position);

                transform.position = path.path.GetPointAtTime(t, PathCreation.EndOfPathInstruction.Stop);
                var vel = path.path.GetDirection(t, PathCreation.EndOfPathInstruction.Stop) * speedScale * speedCurve.Evaluate(t);

                float perpendicularness = 1;
                if(_attachedBody != null)
                    perpendicularness = 1 - Mathf.Abs(Vector3.Dot((_attachedBody.transform.position - transform.position).normalized, vel.normalized));
                perpendicularness = Mathf.Clamp(perpendicularness, 1 - correctionRange, 1);

                var strength = MapValue(perpendicularness, 1 - correctionRange, 1, 1 - correctionStrength, 1f);

                _rigidbody.linearVelocity = vel * strength;
                yield return new WaitForFixedUpdate();
            }
            _used = true;

            _rigidbody.linearVelocity = Vector2.zero;
        }

        private float MapValue(float x, float startX, float endX, float startY, float endY)
        {
            var percentage = (x - startX) / (endX - startX);
            return startY + percentage * (endY - startY);
        }

		public override bool CanGrab(SwingBody body)
		{
            if ((transform.position - body.transform.position).magnitude > maxDistance)
                return false;
            return !_used;
		}

        private bool IsAboveSwinger(SwingBody body)
        {
            var dist = body.transform.position - transform.position;
            return Vector2.Dot(dist, Vector2.up) < 0;
        }
	}
}