using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedDodoServer;

namespace GamePad
{
    /// <summary>
    /// Key identificators
    /// </summary>
    public enum JoystickKeyCode { START, BACK, GUIDE, TURBO, L_BUMPER, R_BUMPER, A, B, X, Y, LEFT, RIGHT, DOWN, UP }
    /// <summary>
    /// Class handling joystick input, similar to UnityEngine.Input. Has funcionalities like GetKey, GetKeyDown, GetKeyUp
    /// </summary>
    public class JoystickInput : Singleton<JoystickInput>
    {
        VariableJoystick leftStick;
        VariableJoystick rightStick;
        VariableJoystick directionalPad;
        InputButton start;
        InputButton back;
        InputButton guide;
        InputButton turbo;//nie ma tego na XboxPadzie, ale jest na naszych
        InputButton leftTrigger; //trigger jest floatem, ale dajemy button który przypisuje 0f lub 1f
        InputButton rightTrigger; //trigger jest floatem, ale dajemy button który przypisuje 0f lub 1f
        InputButton leftBumper;
        InputButton rightBumper;
        InputButton AButton;
        InputButton BButton;
        InputButton XButton;
        InputButton YButton;
        InputButton leftButton;
        InputButton downButton;
        InputButton upButton;
        InputButton rightButton;

        ButtonsLayout buttonsLayout;

        InputButton[] buttons;
        public static VariableJoystick LeftStick { get { return instance.leftStick; } }
        public static VariableJoystick RightStick { get { return instance.rightStick; } }
        public static VariableJoystick DirectionalPad { get { return instance.directionalPad; } }     
        public static float LeftTrigger { get { return instance.leftTrigger.Pressed ? 1f : 0f; } }
        public static float RightTrigger { get { return instance.rightTrigger.Pressed ? 1f : 0f; } }
        int n_buttons;
        bool[] prevState;
        bool[] currentState;
        bool buttonsEnabled = false;
        /// <summary>
        /// Sets a buttonsLayout
        /// </summary>
        /// <param name="_buttonsLayout">Layout</param>
        public void SetButtonsLayout(ButtonsLayout _buttonsLayout)
        {
            buttonsLayout = _buttonsLayout;

            leftStick = buttonsLayout.leftStick;
            rightStick = buttonsLayout.rightStick;
            directionalPad = buttonsLayout.directionalPad;
            start = buttonsLayout.start;
            back = buttonsLayout.back;
            guide = buttonsLayout.guide;
            turbo = buttonsLayout.turbo;
            leftTrigger = buttonsLayout.leftTrigger;
            rightTrigger = buttonsLayout.rightTrigger;
            leftBumper = buttonsLayout.leftBumper;
            rightBumper = buttonsLayout.rightBumper;
            AButton = buttonsLayout.AButton;
            BButton = buttonsLayout.BButton;
            XButton = buttonsLayout.XButton;
            YButton = buttonsLayout.YButton;
            leftButton = buttonsLayout.leftButton;
            rightButton = buttonsLayout.rightButton;
            upButton = buttonsLayout.upButton;
            downButton = buttonsLayout.downButton;

            //buttons = new InputButton[] { start, back, guide, turbo, leftTrigger, rightTrigger, leftBumper, rightBumper, AButton, BButton, XButton, YButton, leftButton, rightButton, downButton, upButton };
            // Buttons in the same order as in JoystickKeyCode
            buttons = new InputButton[] { start, back, guide, turbo, leftBumper, rightBumper, AButton, BButton, XButton, YButton, leftButton, rightButton, downButton, upButton };
            n_buttons = buttons.Length;
            prevState = new bool[n_buttons];
            currentState = new bool[n_buttons];
            for (int i = 0; i < n_buttons; i++)
            {
                if (buttons[i] == null)
                {
                    Debug.LogError("Button " + i + " is a null. Assign the button and set to inactive if you don't need it");
                    continue;
                }
                prevState[i] = false;
                currentState[i] = false;

            }
            buttonsEnabled = true;
        }
        protected override void Awake()
        {
            base.Awake();
            //buttons = new InputButton[] { start, back, guide, turbo, leftTrigger, rightTrigger, leftBumper, rightBumper, AButton, BButton, XButton, YButton, leftButton, rightButton, downButton, upButton };
            //n_buttons = buttons.Length;
            //prevState = new bool[n_buttons];
            //currentState = new bool[n_buttons];
            //for (int i = 0; i < n_buttons; i++)
            //{
            //    if (buttons[i] == null)
            //    {
            //        Debug.LogError("Button " + i + " is a null. Assign the button and set to inactive if you don't need it");
            //        continue;
            //    }
            //    prevState[i] = false;
            //    currentState[i] = false;

            //}
        }
        /// <summary>
        /// Returns true every frame the button given by an int ID is pressed
        /// </summary>
        /// <param name="buttonID"></param>
        /// <returns></returns>
        public static bool GetKey(int buttonID)
        {
            return instance.buttons[buttonID].Pressed;
        }
        /// <summary>
        /// Returns true every frame the button given by an enum is pressed
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static bool GetKey(JoystickKeyCode keyCode)
        {
            return instance.buttons[(int)keyCode].Pressed;
        }
        /// <summary>
        /// Returns true right after the button given by an int ID was pressed
        /// </summary>
        /// <param name="buttonID"></param>
        /// <returns></returns>
        public static bool GetKeyDown(int buttonID)
        {
            return (instance.prevState[buttonID] == false && instance.currentState[buttonID] == true);
        }
        /// <summary>
        /// Returns true right after the button given by an enum was pressed
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static bool GetKeyDown(JoystickKeyCode keyCode)
        {
            int buttonID = (int)keyCode;
            return (instance.prevState[buttonID] == false && instance.currentState[buttonID] == true);
        }
        /// <summary>
        /// Returns true right after the button given by an int ID was released
        /// </summary>
        /// <param name="buttonID"></param>
        /// <returns></returns>
        public static bool GetKeyUp(int buttonID)
        {
            return (instance.prevState[buttonID] == true && instance.currentState[buttonID] == false);
        }
        /// <summary>
        /// Returns true right after the button given by an enum was released
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static bool GetKeyUp(JoystickKeyCode keyCode)
        {
            int buttonID = (int)keyCode;
            return (instance.prevState[buttonID] == true && instance.currentState[buttonID] == false);
        }
        private void Update()
        {
            if (!buttonsEnabled) return;

            for (int i = 0; i < prevState.Length; i++)
            {                
                prevState[i] = currentState[i];
                currentState[i] = GetKey(i);
            }
        }
    }
}