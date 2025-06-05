using UnityEngine;

namespace Gameplay.Player.Animation
{
    public class CharacterAnimationController : MonoBehaviour
    {
        public Rigidbody2D rb;

        private void Update()
        {
            if (rb.linearVelocityX > 0.01f)
                transform.eulerAngles = new Vector3(0, 0, 0);
            else if (rb.linearVelocityX < -0.01f) transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}