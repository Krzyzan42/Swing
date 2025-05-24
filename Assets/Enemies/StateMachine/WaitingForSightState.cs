using UnityEngine;

namespace Enemies.StateMachine
{
    public class WaitingForSightState : EnemyState
    {
        private readonly float _minDistanceToPlayer;

        public WaitingForSightState(EnemyStateMachine machine, Monster monster, float minDistanceToPlayer) : base(
            machine, monster)
        {
            _minDistanceToPlayer = minDistanceToPlayer;
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

            if (hit.transform.CompareTag("Player")) Machine.ChangeState(Monster.AimingAtPlayer);
        }
    }
}