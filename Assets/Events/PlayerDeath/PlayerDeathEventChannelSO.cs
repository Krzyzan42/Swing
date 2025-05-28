using System;
using UnityEngine;
using UnityEngine.Events;

namespace Events.PlayerDeath
{
    [Serializable]
    public class UnityPlayerDeathEvent : UnityEvent<DeathData>
    {
    }

    [CreateAssetMenu(menuName = "Events/Player Death Event Channel")]
    public class PlayerDeathEventChannelSO : ScriptableObject
    {
        public UnityPlayerDeathEvent onEventRaisedUnityEvent;
        private event Action<DeathData> OnEventRaised;

        public void RaiseEvent(DeathData data)
        {
            OnEventRaised?.Invoke(data);
            onEventRaisedUnityEvent?.Invoke(data);
        }

        public void RegisterListener(Action<DeathData> listener)
        {
            OnEventRaised += listener;
        }

        public void UnregisterListener(Action<DeathData> listener)
        {
            OnEventRaised -= listener;
        }
    }
}