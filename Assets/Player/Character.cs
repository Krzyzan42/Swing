using Grappables;
using JetBrains.Annotations;
using Other;
using Other.Rope;
using UnityEngine;

namespace Player
{
    public class Character : MonoBehaviour
    {
        [Range(0, 1)] public float grabGravityScale;

        [SerializeField] [CanBeNull] private GameObject grappleIndicatorPrefab;

        [SerializeField] private CharacterInput characterInput;

        [CanBeNull] private GameObject _grappleIndicator;

        private GrappleManager _grappleManager;

        private RopeAnimation _rope;
        private SwingBody _swingBody;

        public Vector2 Position2D => new(transform.position.x, transform.position.y);

        private void Start()
        {
            _grappleManager = FindAnyObjectByType<GrappleManager>();
            _swingBody = GetComponent<SwingBody>();
            _rope = GetComponentInChildren<RopeAnimation>();

            if (!grappleIndicatorPrefab) return;
            grappleIndicatorPrefab = Instantiate(grappleIndicatorPrefab, transform.position, Quaternion.identity);
            _grappleIndicator = grappleIndicatorPrefab;
        }

        private void Update()
        {
            // I wanted to include a new input system but it is broken xd
            var target = _grappleManager.FindClosestGrapplePoint(transform.position);
            if (characterInput.IsGrabDown)
            {
                if (_swingBody.Grapple(target)) _rope.Attach(transform, target.transform);
            }
            else if (characterInput.IsGrabUp)
            {
                _rope.Deattach();
            }

            if (characterInput.IsGrabUp)
            {
                _rope.Deattach();
                _swingBody.BreakGrapple();
            }

            if (_grappleIndicator && target) _grappleIndicator.transform.position = target.transform.position;
        }

        public void HandleDeath()
        {
            // todo get level
            SceneLoader.ReloadScene();
        }
    }
}