using Enemies.StateMachine;
using Player;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class Monster : MonoBehaviour
    {
        [SerializeField] private float sightRange;

        private EnemyStateMachine _machine;

        [Inject]
        public Character Player { get; private set; }

        public EnemyState WaitingForSightState => new WaitingForSightState(_machine, this, sightRange);
        public EnemyState AimingAtPlayer => new WaitingForSightState(_machine, this, sightRange);

        private void Start()
        {
            _machine = new EnemyStateMachine();
            _machine.Initialize(WaitingForSightState);
        }
    }
}