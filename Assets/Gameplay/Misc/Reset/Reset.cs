using UnityEngine;

namespace Gameplay.Misc.Reset
{
    public class Reset : MonoBehaviour
    {
        private Vector2 _initialPosition;
        private Quaternion _initialRotation;
        private Vector2 _initialVelocity;
        private Rigidbody2D _rb;
        private IResettable[] _resettableComponents;

        private void Start()
        {
            _resettableComponents = GetComponents<IResettable>();
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
            _rb = GetComponent<Rigidbody2D>();
            if (_rb)
                _initialVelocity = _rb.linearVelocity;
        }

        private void Update()
        {
            if (!Input.GetKey(KeyCode.R)) return;

            transform.position = _initialPosition;
            transform.rotation = _initialRotation;
            if (_rb)
                _rb.linearVelocity = _initialVelocity;

            foreach (var resettable in _resettableComponents) resettable.Reset();
        }
    }
}