using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Player
{
    public class Damageable : MonoBehaviour
    {
        public UnityEvent onDeathUnityEvent;
        public Action OnDeath;

        public bool IsInvulnerable { get; set; }

        private void OnEnable()
        {
            OnDeath += PlayHitSound;
        }

        private void OnDisable()
        {
            OnDeath -= PlayHitSound;
        }

        private void PlayHitSound()
        {
            //_soundManager.Play("hit");
        }

        public void TakeDamage()
        {
            if (IsInvulnerable) return;

            OnDeath?.Invoke();
            onDeathUnityEvent?.Invoke();
        }
    }
}