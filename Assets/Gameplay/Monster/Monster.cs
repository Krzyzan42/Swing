using Gameplay.Monster.StateMachine;
using Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Gameplay.Monster
{
    public class Monster : MonoBehaviour
    {
        [SerializeField] private float sightRange;
        [SerializeField] private float aimingTime;

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
            if (!Player) return;

            var monsterPosition = transform.position;
            var playerPosition = Player.transform.position;

            var directionToShoot = (playerPosition - monsterPosition).normalized;

            if (usePredictiveAiming)
            {
                var playerVelocity = Player.Velocity;
                var projectileSpeed = _projectilePrefab.Speed;

                var interceptPoint =
                    CalculateInterceptPoint(monsterPosition, projectileSpeed, playerPosition, playerVelocity);

                if (interceptPoint.HasValue) directionToShoot = (interceptPoint.Value - monsterPosition).normalized;
            }

            var projectile = Instantiate(_projectilePrefab.gameObject, monsterPosition, Quaternion.identity);
            projectile.GetComponent<Projectile>().ShootAt(directionToShoot);
        }

        private static Vector3? CalculateInterceptPoint(Vector3 shooterPosition, float projectileSpeed,
            Vector3 targetPosition, Vector3 targetVelocity)
        {
            var displacement = targetPosition - shooterPosition;
            var targetSpeedSq = targetVelocity.sqrMagnitude;

            var a = targetSpeedSq - projectileSpeed * projectileSpeed;
            var b = 2f * Vector3.Dot(displacement, targetVelocity);
            var c = displacement.sqrMagnitude;

            if (Mathf.Abs(a) < 0.001f)
            {
                if (Mathf.Abs(b) < 0.001f)
                    return null;
                var t = -c / b;
                return t > 0 ? targetPosition + targetVelocity * t : null;
            }

            var discriminant = b * b - 4f * a * c;

            if (discriminant < 0)
                return null;

            var t1 = (-b + Mathf.Sqrt(discriminant)) / (2f * a);
            var t2 = (-b - Mathf.Sqrt(discriminant)) / (2f * a);

            float timeToIntercept;

            if (t1 > 0 && t2 > 0)
                timeToIntercept = Mathf.Min(t1, t2);
            else
                timeToIntercept = Mathf.Max(t1, t2);

            if (timeToIntercept <= 0)
                return null;

            return targetPosition + targetVelocity * timeToIntercept;
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