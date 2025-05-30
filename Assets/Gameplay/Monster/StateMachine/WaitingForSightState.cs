namespace Enemies.StateMachine
{
    public class WaitingForSightState : EnemyState
    {
        public WaitingForSightState(EnemyStateMachine machine, Monster monster) : base(
            machine, monster)
        {
        }

        public override void LogicUpdate()
        {
            var seesPlayer = Monster.SeesPlayer();
            if (seesPlayer) Machine.ChangeState(Monster.AimingAtPlayer);
        }
    }
}