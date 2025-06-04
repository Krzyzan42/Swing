using System;
using UnityEngine;
using UnityEngine.Events;

namespace Events.FlagReached
{
    [CreateAssetMenu(menuName = "Events/Flag Reached Event Channel")]
    public class FlagReachedEventChannel : ScriptableObject
    {
        public UnityEvent<FlagReachedData> onEventRaisedUnityEvent;
        private event Action<FlagReachedData> OnEventRaised;

        public void RaiseEvent(FlagReachedData data)
        {
            OnEventRaised?.Invoke(data);
            onEventRaisedUnityEvent?.Invoke(data);
        }

        public void RegisterListener(Action<FlagReachedData> listener)
        {
            OnEventRaised += listener;
        }

        public void UnregisterListener(Action<FlagReachedData> listener)
        {
            OnEventRaised -= listener;
        }
    }
}