using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class Damageable : MonoBehaviour
    {
        //[Inject] private SoundManager _soundManager;
        public float currentHealth;

        [SerializeField] public bool fullHealthOnStart = true;
        public float maxHealth = 100;
        public UnityEvent<float> onDamageTakenUnityEvent;
        public UnityEvent onDeathUnityEvent;

        private CancellationTokenSource _tempInvulnerabilityCts;

        public Action<float> OnDamageTaken;
        public Action OnDeath;

        public bool IsInvulnerable { get; set; }
        public Func<float, bool> CanTakeDamagePredicate { get; set; }

        private void Start()
        {
            if (fullHealthOnStart) currentHealth = maxHealth;
        }

        private void OnEnable()
        {
            OnDamageTaken += HandleDamageTaken;
            OnDeath += PlayHitSound;
        }

        private void OnDisable()
        {
            OnDamageTaken -= HandleDamageTaken;
            OnDeath -= PlayHitSound;

            _tempInvulnerabilityCts?.Cancel();
            _tempInvulnerabilityCts?.Dispose();
        }

        private void HandleDamageTaken(float damage)
        {
            PlayHitSound();
        }

        private void PlayHitSound()
        {
            //_soundManager.Play("hit");
        }

        public void TakeDamage(float damage)
        {
            if (damage < 0)
            {
                Debug.LogError("Damage taken is negative");
                return;
            }

            if (damage == 0 || currentHealth <= 0) return;

            if (IsInvulnerable) return;

            if (CanTakeDamagePredicate != null && !CanTakeDamagePredicate(damage)) return;

            OnDamageTaken?.Invoke(damage);
            onDamageTakenUnityEvent?.Invoke(damage);

            var previousHealth = currentHealth;
            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0);

            if (!(currentHealth <= 0)) return;

            if (!(previousHealth > 0)) return;

            OnDeath?.Invoke();
            onDeathUnityEvent?.Invoke();
        }

        public void SetHealth(float initialHealth, float? newMaxHealth = null)
        {
            if (initialHealth < 0)
            {
                Debug.LogError("Initial health is negative");
                return;
            }

            maxHealth = newMaxHealth ?? maxHealth;
            currentHealth = Mathf.Clamp(initialHealth, 0, maxHealth);
        }

        public void Heal(int healAmount)
        {
            if (healAmount < 0)
            {
                Debug.LogError("Heal amount is negative");
                return;
            }

            currentHealth = Mathf.Clamp(currentHealth + healAmount, currentHealth, maxHealth);
        }
    }
}