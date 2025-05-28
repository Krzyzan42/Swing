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
            grappleIndicatorPrefab = Instantiate(grappleIndicatorPrefab, transform.position,
                grappleIndicatorPrefab.transform.rotation);
            _grappleIndicator = grappleIndicatorPrefab;
        }

        private void Update()
        {
            // I wanted to include a new input system, but it is broken xd
            var target = _grappleManager.FindClosestGrappablePoint(transform.position, _swingBody);
            if (characterInput.IsGrabDown && target)
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

            UpdateGrappleIndicator(target);
        }

        private void UpdateGrappleIndicator(Grappable target)
        {
            if (_grappleIndicator && target)
            {
                _grappleIndicator.SetActive(true);
                _grappleIndicator.transform.position = target.transform.position;
            }
            else
            {
                _grappleIndicator?.SetActive(false);
            }
        }

        public void HandleDeath()
        {
            // todo get level
            SceneLoader.ReloadScene();
        }
    }
}