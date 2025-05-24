using System;
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

        public Action<float> OnDamageTaken;
        public Action OnDeath;

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

            OnDamageTaken?.Invoke(damage);
            onDamageTakenUnityEvent?.Invoke(damage);

            currentHealth -= damage;

            if (!(currentHealth <= 0)) return;

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
            currentHealth = initialHealth;

            if (!(currentHealth > maxHealth)) return;

            Debug.LogError("Initial health is greater than max health");
            currentHealth = maxHealth;
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