using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curly.InputWrapper
{
    [CreateAssetMenu(fileName = "InputMapping", menuName = "Curly/Input Mapping")]
    public class InputMapping : ScriptableObject
    {
        private static InputMapping _defaultLoad;

        public static InputMapping Default
        {
            get
            {
                if (_defaultLoad == null)
                {
                    _defaultLoad = Resources.Load<InputMapping>("InputMapping");
                }
                return _defaultLoad;
            }
        }

        [System.Serializable]
        public class AxisInputMap
        {
            public KeyCode UpKey = KeyCode.W;
            public KeyCode DownKey = KeyCode.S;
            public KeyCode LeftKey = KeyCode.A;
            public KeyCode RightKey = KeyCode.D;

            public Vector2 GetAxis()
            {
                Vector2 axis = Vector2.zero;
                if (Input.GetKey(UpKey))
                {
                    axis.y = 1;
                }
                if (Input.GetKey(DownKey))
                {
                    axis.y = -1;
                }
                if (Input.GetKey(LeftKey))
                {
                    axis.x = -1;
                }
                if (Input.GetKey(RightKey))
                {
                    axis.x = 1;
                }
                return axis;
            }

            public Vector2 GetAxisNormalized() => GetAxis().normalized;
        }
        
        [System.Serializable]
        public class ButtonInputMap
        {
            public KeyCode ButtonKey;
            public bool GetButtonDown() => Input.GetKeyDown(ButtonKey);
            public bool GetButton() => Input.GetKey(ButtonKey);
            public bool GetButtonUp() => Input.GetKeyUp(ButtonKey);
        }

        public AxisInputMap Movement = new AxisInputMap();
        public ButtonInputMap Jump = new ButtonInputMap();
        public ButtonInputMap Fire = new ButtonInputMap();
    }
}
