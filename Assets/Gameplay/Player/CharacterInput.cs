using System;
using UnityEngine;

namespace Gameplay.Player
{
    public class CharacterInput : MonoBehaviour
    {
        [SerializeField] private int playerIndex = 1;

        private bool _grabDown, _grab, _grabUp;
        private bool _secondaryDown, _secondary, _secondaryUp;

        public bool IsGrabDown =>
#if UNITY_ANDROID || UNITY_IOS
            _grabDown;
#else
            Input.GetButtonDown("Grab" + playerIndex);
#endif

        public bool IsGrab =>
#if UNITY_ANDROID || UNITY_IOS
            _grab;
#else
            Input.GetButton("Grab" + playerIndex);
#endif

        public bool IsGrabUp =>
#if UNITY_ANDROID || UNITY_IOS
            _grabUp;
#else
            Input.GetButtonUp("Grab" + playerIndex);
#endif

        public bool IsSecondaryActionDown =>
#if UNITY_ANDROID || UNITY_IOS
            _secondaryDown;
#else
            Input.GetButtonDown("SecondaryAction" + playerIndex);
#endif

        public bool IsSecondaryAction =>
#if UNITY_ANDROID || UNITY_IOS
            _secondary;
#else
            Input.GetButton("SecondaryAction" + playerIndex);
#endif

        public bool IsSecondaryActionGrabUp =>
#if UNITY_ANDROID || UNITY_IOS
            _secondaryUp;
#else
            Input.GetButtonUp("SecondaryAction" + playerIndex);
#endif

        private void Update()
        {
#if UNITY_ANDROID || UNITY_IOS
            _grabDown = _grabUp = _secondaryDown = _secondaryUp = false;
            _grab = _secondary = false;

            foreach (var touch in Input.touches)
            {
                var isLeft = touch.position.x < Screen.width / 2f;

                if (isLeft)
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            _grabDown = true;
                            break;
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            _grabUp = true;
                            break;
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            _grab = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                else
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            _secondaryDown = true;
                            break;
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            _secondaryUp = true;
                            break;
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            _secondary = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
            }
#endif
        }
    }
}