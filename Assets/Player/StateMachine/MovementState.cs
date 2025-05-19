using EasyStateMachine;

namespace Player.StateMachine
{
    public abstract class MovementState : State<MovementStateMachine, MovementState>
    {
        protected MovementState(MovementStateMachine machine) : base(machine)
        {
        }
    }
}