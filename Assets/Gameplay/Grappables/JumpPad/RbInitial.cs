using Gameplay.Misc.Reset;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Grappables.JumpPad
{
    [RequireComponent(typeof(Reset))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class RbInitial : MonoBehaviour, IResettable
    {
        [FormerlySerializedAs("initalVelocity")]
        public Vector2 initialVelocity;

        private Rigidbody2D _rigidbody2D;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            Reset();
        }

        public void Reset()
        {
            _rigidbody2D.linearVelocity = initialVelocity;
        }
    }
}