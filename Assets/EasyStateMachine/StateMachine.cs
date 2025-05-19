using System;

namespace EasyStateMachine
{
    public abstract class StateMachine<TSelf, TStateBase>
        where TSelf : StateMachine<TSelf, TStateBase>
        where TStateBase : State<TSelf, TStateBase>
    {
        public TStateBase CurrentState { get; private set; }

        public void Initialize(TStateBase startingState)
        {
            CurrentState = startingState ?? throw new ArgumentNullException(nameof(startingState));
            CurrentState.Enter();
        }

        public void ChangeState(TStateBase newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}