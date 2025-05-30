using EasyStateMachine;

namespace Gameplay.Monster.StateMachine
{
    public abstract class EnemyState : State<EnemyStateMachine, EnemyState>
    {
        protected readonly Monster Monster;
        
        protected EnemyState(EnemyStateMachine machine, Monster monster) : base(machine)
        {
            Monster = monster;
        }
        
        public abstract void LogicUpdate();
    }
}