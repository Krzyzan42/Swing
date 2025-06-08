using System.Linq;
using UnityEngine;

namespace Gameplay.Player
{
    public class CameraFollow : MonoBehaviour
    {
        public Vector3 offset = new(0, 0, -10);
        public float smoothTime = 0.25f;

        public Transform[] targets;

        private Camera _cameraMain;
        private Vector3 _velocity = Vector3.zero;

        private void Start()
        {
            _cameraMain = Camera.main;

            targets = FindObjectsByType<Character>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Select(c => c.transform).ToArray();
        }

        private void Update()
        {
            switch (targets.Length)
            {
                case 0:
                    return;
                case 1:
                {
                    var target = targets[0];

                    if (!target) return;

                    var targetPosition = target.position + offset;
                    transform.position =
                        Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);
                    break;
                }
                case 2:
                {
                    if (!targets[0] || !targets[1])
                        break;

                    var target1 = targets[0].position;
                    var target2 = targets[1].position;
                    var center = (target1 + target2) / 2f;
                    var targetPosition = center + offset;
                    transform.position =
                        Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);

                    var distance = Mathf.Max(Vector3.Distance(target1, target2), 0.1f);
                    if (_cameraMain.orthographic)
                    {
                        const float minSize = 5f;
                        const float maxSize = 20f;
                        var targetSize = Mathf.Clamp(distance, minSize, maxSize);
                        _cameraMain.orthographicSize =
                            Mathf.Lerp(_cameraMain.orthographicSize, targetSize, Time.deltaTime * 2f);
                    }

                    break;
                }
            }
        }
    }
}