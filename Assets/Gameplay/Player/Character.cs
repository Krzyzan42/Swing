using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Events.PlayerDeath;
using Gameplay.Grappables;
using JetBrains.Annotations;
using LM;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Gameplay.Player
{
    public class Character : MonoBehaviour
    {
        private static readonly int Play = Animator.StringToHash("Play");
        [SerializeField] [CanBeNull] private GameObject grappleIndicatorPrefab;
        [SerializeField] private CharacterInput characterInput;
        [SerializeField] [CanBeNull] private GameObject deathEffect;
        public float shieldCooldown = 1.5f;
        public float shieldDuration = 0.2f;

        public UnityEvent grappled = new(); // For disabling starting platform
        public Animator shieldAnimator;
        private CancellationTokenSource _cts;

        private Damageable _damageable;

        [CanBeNull] private GameObject _grappleIndicator;

        private GrappleManager _grappleManager;

        private float _lastShieldTime;

        [Inject] private PlayerDeathEventChannel _playerDeathEventChannel;

        private RopeAnimation _rope;

        [Inject] private SoundManager _soundManager;
        private SwingBody _swingBody;
        public Vector2 Velocity => _swingBody.Velocity;

        private void Start()
        {
            _cts = new CancellationTokenSource();

            _grappleManager = FindAnyObjectByType<GrappleManager>();
            _swingBody = GetComponent<SwingBody>();
            _rope = GetComponentInChildren<RopeAnimation>();
            _damageable = GetComponent<Damageable>();

            shieldAnimator.gameObject.SetActive(false);

            _lastShieldTime = -1000f;

            if (grappleIndicatorPrefab)
                _grappleIndicator = Instantiate(grappleIndicatorPrefab, transform.position,
                    grappleIndicatorPrefab.transform.rotation);
        }

        private void Update()
        {
            var target = _grappleManager.FindClosestGrappablePoint(transform.position, _swingBody);
            if (characterInput.IsGrabDown && target)
            {
                if (_swingBody.Grapple(target))
                {
                    _rope.Attach(transform, target.transform);
                    grappled.Invoke();
                    _soundManager.Play("grapple");
                }
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

            if (characterInput.IsSecondaryActionDown && Mathf.Abs(Time.time - _lastShieldTime) > shieldCooldown)
            {
                _soundManager.Play("shield");
                Shield().Forget();
            }

            UpdateGrappleIndicator(target);
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public void OnCollisionEnter2D(Collision2D _)
        {
            if (_swingBody.InsideGravityBlock) HandleDeath();
        }

        private void PlayDeathEffect()
        {
            if (!deathEffect) return;

            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        private async UniTaskVoid Shield()
        {
            _lastShieldTime = Time.time;
            _damageable.IsInvulnerable = true;
            shieldAnimator.gameObject.SetActive(true);
            shieldAnimator.SetTrigger(Play);

            try
            {
                await UniTask.Delay((int)(shieldDuration * 1000), cancellationToken: _cts.Token);
                _damageable.IsInvulnerable = false;
                shieldAnimator.gameObject.SetActive(false);
            }
            catch (OperationCanceledException)
            {
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
            PlayDeathEffect();
            Destroy(gameObject);
        }
    }
}