using UnityEngine;

namespace Enemies.StateMachine
{
    public class AimingAtPlayerState : EnemyState
    {
        private readonly float _timeToShoot;

        private float _timeLeftToShoot;

        public AimingAtPlayerState(EnemyStateMachine machine, Monster monster,
            float timeToShoot) : base(machine, monster)
        {
            _timeToShoot = timeToShoot;
        }

        public override void Enter()
        {
            base.Enter();
            _timeLeftToShoot = _timeToShoot;
        }

        public override void LogicUpdate()
        {
            var seesPlayer = Monster.SeesPlayer();
            if (!seesPlayer)
            {
                Machine.ChangeState(Monster.WaitingForSightState);
                return;
            }

            _timeLeftToShoot -= Time.deltaTime;

            if (_timeLeftToShoot > 0) return;

            _timeLeftToShoot = _timeToShoot;
            Monster.ShootAtPlayer();
        }
    }
}