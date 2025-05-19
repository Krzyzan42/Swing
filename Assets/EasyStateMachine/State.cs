namespace EasyStateMachine
{
    public abstract class State<TStateMachine, TStateBase>
        where TStateMachine :
        StateMachine<TStateMachine, TStateBase>
        where TStateBase : State<TStateMachine, TStateBase>
    {
        protected readonly TStateMachine Machine;

        protected State(TStateMachine machine)
        {
            Machine = machine;
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }
    }
}