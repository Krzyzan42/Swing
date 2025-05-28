using UnityEngine;

namespace Player
{
    public class CharacterInput : MonoBehaviour
    {
        [SerializeField] private int playerIndex = 1;

        public bool IsGrabDown => Input.GetButtonDown("Grab" + playerIndex);
        public bool IsGrab => Input.GetButton("Grab" + playerIndex);
        public bool IsGrabUp => Input.GetButtonUp("Grab" + playerIndex);

        public bool IsSecondaryActionDown => Input.GetButtonDown("SecondaryAction" + playerIndex);
        public bool IsSecondaryAction => Input.GetButton("SecondaryAction" + playerIndex);
        public bool IsSecondaryActionGrabUp => Input.GetButtonUp("SecondaryAction" + playerIndex);
    }
}