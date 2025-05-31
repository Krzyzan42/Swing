using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Misc
{
    public class CameraFollow : MonoBehaviour
    {
        [FormerlySerializedAs("player")] public Transform followed;

        public float smoothTime = 0.1f;
        private Vector3 _velocity = Vector3.zero;

        private void FixedUpdate()
        {
            Vector2 pos = followed.position;
            var target = new Vector3(pos.x, pos.y, -10);
            transform.position = Vector3.SmoothDamp(transform.position, target, ref _velocity, smoothTime);
        }
    }
}