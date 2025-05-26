using System.Collections;
using Grappables;
using Grappables.JumpPad;
using Grappables.SwingPoint;
using Other;
using Other.Rope;
using UnityEngine;

namespace Player
{
    public class Character : MonoBehaviour
    {
        [Range(0, 1)] public float grabGravityScale;

        [SerializeField] private GameObject grappleIndicator;

        private GrappleManager _grappleManager;
        private CharacterPhysics _gravity;

        private float _gravityMultiplier = 1;
        private DistanceJoint2D _joint;
        private JumpPadPhysics _padPhysics;

        private Rigidbody2D _rb;
        private RopeAnimation _rope;
        private Grappable _target;

        public Vector2 Position2D => new(transform.position.x, transform.position.y);

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _joint = GetComponent<DistanceJoint2D>();
            _grappleManager = FindAnyObjectByType<GrappleManager>();
            _gravity = GetComponent<CharacterPhysics>();
            _padPhysics = GetComponent<JumpPadPhysics>();
            _rope = GetComponentInChildren<RopeAnimation>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _target = _grappleManager.FindClosestGrapplePoint(_rb.position);

                switch (_target)
                {
                    case MovingBlock block:
                        StartCoroutine(GrabSwingPoint(block));
                        break;
                    case JumpPad pad:
                        _padPhysics.JumpOff(pad);
                        break;
                }

                _rope.Attach(transform, _target.transform);

                return;
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                _rope.Deattach();
                return;
            }

            _target = _grappleManager.FindClosestGrapplePoint(_rb.position);
            grappleIndicator.transform.position = _target.transform.position;
        }

        private void FixedUpdate()
        {
            if (_rb.linearVelocity.y > 0 && IsGrabbed() && !IsAboveGrabbedObject())
                _gravity.GravityScale = grabGravityScale * _gravityMultiplier;
            else
                _gravity.GravityScale = 1 * _gravityMultiplier;
        }

        public void SetGravityMultiplier(float multiplier)
        {
            _gravityMultiplier = multiplier;
        }

        private bool IsGrabbed()
        {
            return _joint.connectedBody && _joint.isActiveAndEnabled;
        }

        private bool IsAboveGrabbedObject()
        {
            var dist = _joint.connectedBody.transform.position - transform.position;
            return Vector2.Dot(new Vector2(dist.x, dist.y), Vector2.up) < 0;
        }

        private IEnumerator GrabSwingPoint(MovingBlock point)
        {
            point.Grab(_joint);
            do
            {
                yield return new WaitForFixedUpdate();
            } while (Input.GetKey(KeyCode.Space));

            point.Release(_joint);
            _target = null;
        }

        public void HandleDeath()
        {
            // todo get level
            SceneLoader.LoadLevel(1);
        }
    }
}