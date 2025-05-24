using UnityEngine;

namespace Enemies.StateMachine
{
    public class AimingAtPlayer : EnemyState
    {
        private readonly float _minDistanceToPlayer;
        private readonly float _timeToShoot;

        private float _timeLeftToShoot;

        public AimingAtPlayer(EnemyStateMachine machine, Monster monster, float minDistanceToPlayer,
            float timeToShoot) : base(machine, monster)
        {
            _minDistanceToPlayer = minDistanceToPlayer;
            _timeToShoot = timeToShoot;
        }

        public override void Enter()
        {
            base.Enter();
            _timeLeftToShoot = _timeToShoot;
        }

        public override void LogicUpdate()
        {
            var player = Monster.Player;

            if (!player) return;

            var distance = Vector3.Distance(player.transform.position, Monster.transform.position);

            if (distance > _minDistanceToPlayer) return;

            var directionToPLayer = (player.transform.position - Monster.transform.position).normalized;

            var hit = Physics2D.Raycast(Monster.transform.position, directionToPLayer);

            if (!hit) return;

            if (!hit.transform.CompareTag("Player")) Machine.ChangeState(Monster.WaitingForSightState);

            _timeLeftToShoot -= Time.deltaTime;

            if (_timeLeftToShoot > 0) return;

            _timeLeftToShoot = _timeToShoot;
            Monster.ShootAtPlayer();
        }
    }
}