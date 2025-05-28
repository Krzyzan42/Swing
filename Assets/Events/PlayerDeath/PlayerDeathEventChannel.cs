using System;
using UnityEngine;
using UnityEngine.Events;

namespace Events.PlayerDeath
{
    [CreateAssetMenu(menuName = "Events/Player Death Event Channel")]
    public class PlayerDeathEventChannel : ScriptableObject
    {
        public UnityEvent<DeathData> onEventRaisedUnityEvent;
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