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

        private EnemyStateMachine _machine;

        [Inject] private Projectile _projectilePrefab;

        [Inject]
        public Character Player { get; private set; }

        public EnemyState WaitingForSightState => new WaitingForSightState(_machine, this, sightRange);
        public EnemyState AimingAtPlayer => new AimingAtPlayer(_machine, this, sightRange, aimingTime);

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
    }
}