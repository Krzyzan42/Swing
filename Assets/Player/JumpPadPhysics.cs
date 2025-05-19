using System.Collections;
using Grappables.JumpPad;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class JumpPadPhysics : MonoBehaviour
    {
        [FormerlySerializedAs("jumppadSpeedChange")]
        public AnimationCurve jumpPadSpeedChange;

        public float maxApproachSpeed = 20;
        public float velocitySaveDist = 1f;

        private Rigidbody2D _rb;
        private Vector2 _savedVelocity;

        public Vector2 Position2D => new(transform.position.x, transform.position.y);


        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void JumpOff(JumpPad pad)
        {
            StartCoroutine(JumpOffCoroutine(pad));
        }

        private IEnumerator JumpOffCoroutine(JumpPad pad)
        {
            float t = 0;
            var initial = _rb.linearVelocity;
            _savedVelocity = Vector2.zero;

            while (t < 1 && !pad.IsTargetReached(_rb.position))
            {
                var dir = (pad.TargetPosition - Position2D).normalized;
                var vel = Vector2.Lerp(initial, dir * maxApproachSpeed, jumpPadSpeedChange.Evaluate(t));

                if (Vector2.Distance(pad.TargetPosition, Position2D) < velocitySaveDist &&
                    _savedVelocity == Vector2.zero)
                {
                    var perp = Vector2.Perpendicular(pad.JumpDirection).normalized;
                    _savedVelocity = Vector2.Dot(perp, vel) * perp;
                }

                _rb.linearVelocity = vel;
                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            Debug.Log(_savedVelocity);
            pad.Activate();
            _rb.linearVelocity = pad.maxSpeed * pad.JumpDirection + _savedVelocity;
        }
    }
}