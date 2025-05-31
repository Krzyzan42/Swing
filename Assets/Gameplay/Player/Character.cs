using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Events.PlayerDeath;
using Gameplay.Grappables;
using JetBrains.Annotations;
using LM;
using UnityEngine;
using Zenject;

namespace Gameplay.Player
{
    public class Character : MonoBehaviour
    {
        [SerializeField] [CanBeNull] private GameObject grappleIndicatorPrefab;

        [SerializeField] private CharacterInput characterInput;
        [SerializeField][CanBeNull] private GameObject deathEffect;

        private readonly SimpleTimer _protectActivityTimer = new(1);
        private readonly SimpleTimer _protectCooldownTimer = new(.1f);

        private Damageable _damageable;

        [CanBeNull] private GameObject _grappleIndicator;

        private GrappleManager _grappleManager;

        [Inject] private PlayerDeathEventChannel _playerDeathEventChannel;

        private CancellationTokenSource _protectActionCts;

        private RopeAnimation _rope;
        private SwingBody _swingBody;
        public Vector2 Velocity => _swingBody.Velocity;

        private void Start()
        {
            _grappleManager = FindAnyObjectByType<GrappleManager>();
            _swingBody = GetComponent<SwingBody>();
            _rope = GetComponentInChildren<RopeAnimation>();
            _damageable = GetComponent<Damageable>();

            if (grappleIndicatorPrefab)
                _grappleIndicator = Instantiate(grappleIndicatorPrefab, transform.position,
                    grappleIndicatorPrefab.transform.rotation);

            _protectActionCts = new CancellationTokenSource();
            HandleProtectAction(_protectActionCts.Token).Forget();
        }

        private void Update()
        {
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

        private void PlayDeathEffect()
        {
            if (deathEffect == null) return;

            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        private async UniTaskVoid HandleProtectAction(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (characterInput.IsSecondaryActionDown && _protectCooldownTimer.IsFinished(true))
                    {
                        _damageable.IsInvulnerable = true;
                        await _protectActivityTimer.WaitAsync();
                        _protectCooldownTimer.Reset();
                        _damageable.IsInvulnerable = false;
                    }

                    await UniTask.Yield();
                }
            }
            catch (Exception) { }
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
            print(_playerDeathEventChannel);
            _playerDeathEventChannel.RaiseEvent(new DeathData(this));
            PlayDeathEffect();
            Destroy(gameObject);
        }
    }
}