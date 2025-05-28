using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Events.PlayerDeath;
using Grappables;
using JetBrains.Annotations;
using LM;
using Other.Rope;
using UnityEngine;
using Zenject;

namespace Player
{
    public class Character : MonoBehaviour
    {
        // will this be used?
        [Range(0, 1)] public float grabGravityScale;

        [SerializeField] [CanBeNull] private GameObject grappleIndicatorPrefab;

        [SerializeField] private CharacterInput characterInput;

        private readonly SimpleTimer _protectActivityTimer = new(1);
        private readonly SimpleTimer _protectCooldownTimer = new(.1f);

        [CanBeNull] private GameObject _grappleIndicator;

        private GrappleManager _grappleManager;

        [Inject] private PlayerDeathEventChannel _playerDeathEventChannel;

        private CancellationTokenSource _protectActionCts;

        private RopeAnimation _rope;
        private SwingBody _swingBody;
        public Vector2 Position2D => new(transform.position.x, transform.position.y);

        private void Start()
        {
            _grappleManager = FindAnyObjectByType<GrappleManager>();
            _swingBody = GetComponent<SwingBody>();
            _rope = GetComponentInChildren<RopeAnimation>();

            if (grappleIndicatorPrefab)
                _grappleIndicator = Instantiate(grappleIndicatorPrefab, transform.position,
                    grappleIndicatorPrefab.transform.rotation);

            _protectActionCts = new CancellationTokenSource();
            HandleProtectAction(_protectActionCts.Token).Forget();
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

        private void OnDestroy()
        {
            _protectActionCts?.Cancel();
            _protectActionCts?.Dispose();
        }

        private async UniTaskVoid HandleProtectAction(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (characterInput.IsSecondaryActionDown && _protectCooldownTimer.IsFinished(true))
                    {
                        await _protectActivityTimer.WaitAsync();
                        _protectCooldownTimer.Reset();
                    }

                    await UniTask.Yield();
                }
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private void UpdateGrappleIndicator([CanBeNull] Grappable target)
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
            _playerDeathEventChannel.RaiseEvent(new DeathData(this));
            Destroy(gameObject);
        }
    }
}