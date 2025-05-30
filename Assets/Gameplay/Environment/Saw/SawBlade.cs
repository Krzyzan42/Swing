using UnityEngine;

namespace Gameplay.Environment.Saw
{
    public class SawBlade : MonoBehaviour
    {
        public float speed = 1f;

        private void Update()
        {
            var zRot = transform.rotation.eulerAngles.z;
            transform.rotation = Quaternion.Euler(0, 0, zRot + speed * 360 * Time.deltaTime);
        }
    }
}