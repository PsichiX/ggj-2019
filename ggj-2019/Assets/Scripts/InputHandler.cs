using UnityEngine;

namespace GaryMoveOut
{
    public class InputHandler : MonoBehaviour
    {
        public enum Layout
        {
            None,
            Wsad,
            Arrows,
            Gamepad
        }

        public bool Left { get; private set; }
        public bool Right { get; private set; }
        public bool Up { get; private set; }
        public bool Down { get; private set; }
        public bool Action { get; private set; }

        [SerializeField]
        private Layout m_layout = Layout.None;

        private void Update()
        {
            if (m_layout == Layout.Wsad)
            {
                Up = Input.GetKey(KeyCode.W);
                Down = Input.GetKey(KeyCode.S);
                Left = Input.GetKey(KeyCode.A);
                Right = Input.GetKey(KeyCode.D);
                Action = Input.GetKey(KeyCode.LeftControl);
            }
            else if (m_layout == Layout.Arrows)
            {
                Up = Input.GetKey(KeyCode.UpArrow);
                Down = Input.GetKey(KeyCode.DownArrow);
                Left = Input.GetKey(KeyCode.LeftArrow);
                Right = Input.GetKey(KeyCode.RightArrow);
                Action = Input.GetKey(KeyCode.KeypadEnter);
            }
            else if (m_layout == Layout.Gamepad)
            {
                Up = Input.GetAxis("GamepadVertical") > 0.5;
                Down = Input.GetAxis("GamepadVertical") < -0.5;
                Left = Input.GetAxis("GamepadHorizontal") > 0.5;
                Right = Input.GetAxis("GamepadHorizontal") > 0.5;
                Action = Input.GetButton("GamepadAction");
            }
        }
    }
}
