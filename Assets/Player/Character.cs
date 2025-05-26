using System.Collections;
using Grappables;
using Other.Rope;
using UnityEngine;

namespace Player
{
    public class Character : MonoBehaviour
    {
        [Range(0, 1)] public float grabGravityScale;

        private GrappleManager _grappleManager;
        private SwingBody _swingBody;

        private RopeAnimation _rope;

        public Vector2 Position2D => new(transform.position.x, transform.position.y);

        private void Start()
        {
            _grappleManager = FindAnyObjectByType<GrappleManager>();
            _swingBody = GetComponent<SwingBody>();
            _rope = GetComponentInChildren<RopeAnimation>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Grappable _target = _grappleManager.FindClosestGrapplePoint(transform.position);
                if (_swingBody.Grapple(_target))
                {
                    _rope.Attach(transform, _target.transform);
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                _rope.Deattach();
                _swingBody.BreakGrapple();
            } 
        }
    }
}