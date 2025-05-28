using Enemies.StateMachine;
using Player;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class Monster : MonoBehaviour
    {
        [SerializeField] private float sightRange;
        [SerializeField] private float aimingTime;

        [SerializeField] private LayerMask playerLayerMask;
        [SerializeField] private LayerMask sightBlockerLayerMask;

        [SerializeField] private bool usePredictiveAiming;

        private EnemyStateMachine _machine;

        [Inject] private Projectile _projectilePrefab;

        [Inject]
        public Character Player { get; private set; }

        public EnemyState WaitingForSightState => new WaitingForSightState(_machine, this);
        public EnemyState AimingAtPlayer => new AimingAtPlayerState(_machine, this, aimingTime);

        private void Start()
        {
            _machine = new EnemyStateMachine();
            _machine.Initialize(WaitingForSightState);
        }

        private void Update()
        {
            _machine.CurrentState.LogicUpdate();
        }

        public void ShootAtPlayer()
        {
            var projectile = Instantiate(_projectilePrefab.gameObject, transform.position, Quaternion.identity);

            var directionToPlayer = Player.transform.position - transform.position;
            projectile.GetComponent<Projectile>().ShootAt(directionToPlayer);
        }

        public bool SeesPlayer()
        {
            var player = Player;

            if (!player) return false;

            var distance = Vector2.Distance(player.transform.position, transform.position);

            if (distance > sightRange) return false;

            var directionToPlayer = (player.transform.position - transform.position).normalized;

            var hit = Physics2D.Raycast(transform.position, directionToPlayer, sightRange, sightBlockerLayerMask);

            return hit && hit.transform.CompareTag("Player");
        }
    }
}