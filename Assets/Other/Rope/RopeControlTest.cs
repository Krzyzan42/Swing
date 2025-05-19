using UnityEngine;

namespace Other.Rope
{
    public class RopeControlTest : MonoBehaviour
    {
        public Transform target;
        public RopeAnimation rope;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) rope.Attach(transform, target);
            if (Input.GetKeyUp(KeyCode.Space)) rope.Deattach();
        }
    }
}