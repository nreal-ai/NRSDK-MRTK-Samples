/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using UnityEngine;

    /// <summary>
    /// The class is to emulate controller with mouse and keyboard input in unity editor mode. </summary>
    public class EditorControllerProvider : ControllerProviderBase
    {
        /// <summary> Gets the number of controllers. </summary>
        /// <value> The number of controllers. </value>
        public override int ControllerCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary> Constructor. </summary>
        /// <param name="states"> The states.</param>
        public EditorControllerProvider(ControllerState[] states) : base(states)
        {
            Inited = true;
        }

        /// <summary> Executes the 'pause' action. </summary>
        public override void OnPause()
        {

        }

        /// <summary> Executes the 'resume' action. </summary>
        public override void OnResume()
        {

        }

        /// <summary> Updates this object. </summary>
        public override void Update()
        {
            if (!Inited)
                return;
            UpdateControllerState(0);
        }

        public override void Recenter()
        {
            base.Recenter();
            needRecenter = true;
        }

        /// <summary> Executes the 'destroy' action. </summary>
        public override void OnDestroy()
        {

        }

        /// <summary> Updates the controller state described by index. </summary>
        /// <param name="index"> Zero-based index of the.</param>
        private void UpdateControllerState(int index)
        {
            states[index].controllerType = ControllerType.CONTROLLER_TYPE_EDITOR;
            states[index].availableFeature = ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION;
            states[index].connectionState = ControllerConnectionState.CONTROLLER_CONNECTION_STATE_CONNECTED;
            states[index].rotation = rotation;

            states[index].touchPos = touchPos;
            states[index].isTouching = isTouching;
            states[index].recentered = false;
            states[index].isCharging = false;
            states[index].batteryLevel = 100;

            if (NRInput.EmulateVirtualDisplayInEditor)
            {
                IControllerStateParser stateParser = ControllerStateParseUtility.GetControllerStateParser(states[index].controllerType, index);
                if (stateParser != null)
                {
                    stateParser.ParserControllerState(states[index]);
                }
            }
            else
            {
                var trigger_button_state = (states[index].buttonsState) & (ControllerButton.TRIGGER);
                var app_button_state = (states[index].buttonsState) & (ControllerButton.APP);
                var home_button_state = (states[index].buttonsState) & (ControllerButton.HOME);

                states[index].buttonsState =
                    ((buttonState[0] == 1) ? ControllerButton.TRIGGER : 0)
                    | ((buttonState[1] == 1) ? ControllerButton.APP : 0)
                    | ((buttonState[2] == 1) ? ControllerButton.HOME : 0);
                states[index].buttonsDown =
                    (trigger_button_state != ControllerButton.TRIGGER && buttonState[0] == 1 ? ControllerButton.TRIGGER : 0)
                    | (app_button_state != ControllerButton.APP && buttonState[1] == 1 ? ControllerButton.APP : 0)
                    | (home_button_state != ControllerButton.HOME && buttonState[2] == 1 ? ControllerButton.HOME : 0);
                states[index].buttonsUp =
                    (trigger_button_state == ControllerButton.TRIGGER && buttonState[0] == 0 ? ControllerButton.TRIGGER : 0)
                    | (app_button_state == ControllerButton.APP && buttonState[1] == 0 ? ControllerButton.APP : 0)
                    | (home_button_state == ControllerButton.HOME && buttonState[2] == 0 ? ControllerButton.HOME : 0);
            }

            CheckRecenter(index);
            if (needRecenter)
            {
                states[0].recentered = true;
                resetRotation = Quaternion.Inverse(originRotation);
                needRecenter = false;
            }
        }

        bool needRecenter;
        float timeLast = 0;
        private void CheckRecenter(int index)
        {
            if (states[index].GetButton(ControllerButton.HOME))
            {
                timeLast += Time.deltaTime;
                if (timeLast > 2f)
                {
                    needRecenter = true;
                    timeLast = 0f;
                }
            }
            else
            {
                timeLast = 0f;
            }
        }

        // Receive input data.
        private static Vector2 touchPos;
        private static bool isTouching = false;
        private static int[] buttonState = new int[3] { 0, 0, 0 };
        private static Quaternion rotation = Quaternion.identity;
        private static Quaternion originRotation = Quaternion.identity;
        private static Quaternion resetRotation = Quaternion.identity;

        public static void SetControllerTouchPoint(float x, float y)
        {
            touchPos.x = x;
            touchPos.y = y;
        }

        public static void SetControllerIsTouching(bool istouching)
        {
            isTouching = istouching;
        }

        public static void SetControllerButtonState(ControllerButton button, int state)
        {
            switch (button)
            {
                case ControllerButton.TRIGGER:
                    buttonState[0] = state;
                    buttonState[1] = 0;
                    buttonState[2] = 0;
                    break;
                case ControllerButton.APP:
                    buttonState[0] = 0;
                    buttonState[1] = state;
                    buttonState[2] = 0;
                    break;
                case ControllerButton.HOME:
                    buttonState[0] = 0;
                    buttonState[1] = 0;
                    buttonState[2] = state;
                    break;
                default:
                    break;
            }
        }

        public static void SetControllerRotation(Quaternion qua)
        {
            originRotation = qua;
            rotation = resetRotation * originRotation;
        }
    }
}
