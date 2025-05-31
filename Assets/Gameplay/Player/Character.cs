using System;
using System.Collections;
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
        [SerializeField] [CanBeNull] private GameObject grappleIndicatorPrefab;

        [SerializeField] private CharacterInput characterInput;
        [SerializeField][CanBeNull] private GameObject deathEffect;

        private Damageable _damageable;

        [CanBeNull] private GameObject _grappleIndicator;

        private GrappleManager _grappleManager;

        [Inject] private PlayerDeathEventChannel _playerDeathEventChannel;

        private RopeAnimation _rope;
        private SwingBody _swingBody;
        public Vector2 Velocity => _swingBody.Velocity;
        public float shieldCooldown = 1.5f;
        public float shieldDuration = 0.2f;

        public UnityEvent grappled = new UnityEvent(); // For disabling starting platform
        public Animator shieldAnimator;

        private void Start()
        {
            _grappleManager = FindAnyObjectByType<GrappleManager>();
            _swingBody = GetComponent<SwingBody>();
            _rope = GetComponentInChildren<RopeAnimation>();
            _damageable = GetComponent<Damageable>();

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

            if (characterInput.IsSecondaryActionDown) StartCoroutine(Shield());

            UpdateGrappleIndicator(target);
        }

        private void PlayDeathEffect()
        {
            if (deathEffect == null) return;

            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }


        private float lastShieldTime = 0;
        private IEnumerator Shield()
        {
            if (Mathf.Abs(Time.time - lastShieldTime) > shieldCooldown)
            {
                lastShieldTime = Time.time;
                _damageable.IsInvulnerable = true;
                print("triggering");
                shieldAnimator.SetTrigger("Play");
                yield return new WaitForSeconds(shieldDuration);
                _damageable.IsInvulnerable = false;
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
            print(_playerDeathEventChannel);
            _playerDeathEventChannel.RaiseEvent(new DeathData(this));
            PlayDeathEffect();
            Destroy(gameObject);
        }
    }
}