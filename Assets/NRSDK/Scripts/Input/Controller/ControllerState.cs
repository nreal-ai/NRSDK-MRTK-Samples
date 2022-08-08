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
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> Values that represent controller types. </summary>
    public enum ControllerType
    {
        /// <summary> An enum constant representing the controller type editor option. </summary>
        CONTROLLER_TYPE_EDITOR = 1001,
        /// <summary> An enum constant representing the controller type unknown option. </summary>
        CONTROLLER_TYPE_UNKNOWN = -1,
        /// <summary> An enum constant representing the controller type nreallight option. </summary>
        CONTROLLER_TYPE_NREALLIGHT = 0,
        /// <summary> An enum constant representing the controller type phone option. </summary>
        CONTROLLER_TYPE_PHONE = 1,
        /// <summary> An enum constant representing the controller type hand option. </summary>
        CONTROLLER_TYPE_HAND = 2
    }

    /// <summary> Values that represent controller available features. </summary>
    public enum ControllerAvailableFeature
    {
        /// <summary> The position is avaliable. </summary>
        CONTROLLER_AVAILABLE_FEATURE_POSITION = (1 << 0),
        /// <summary> The rotation is avaliable. </summary>
        CONTROLLER_AVAILABLE_FEATURE_ROTATION = (1 << 1),
        /// <summary>
        /// An enum constant representing the controller available feature gyro option. </summary>
        CONTROLLER_AVAILABLE_FEATURE_GYRO = (1 << 2),
        /// <summary>
        /// An enum constant representing the controller available feature accel option. </summary>
        CONTROLLER_AVAILABLE_FEATURE_ACCEL = (1 << 3),
        /// <summary>
        /// An enum constant representing the controller available feature Magnitude option. </summary>
        CONTROLLER_AVAILABLE_FEATURE_MAG = (1 << 4),
        /// <summary>
        /// An enum constant representing the controller available feature battery option. </summary>
        CONTROLLER_AVAILABLE_FEATURE_BATTERY = (1 << 5),
        /// <summary>
        /// An enum constant representing the controller available feature charging option. </summary>
        CONTROLLER_AVAILABLE_FEATURE_CHARGING = (1 << 6),
        /// <summary>
        /// An enum constant representing the controller available feature recenter option. </summary>
        CONTROLLER_AVAILABLE_FEATURE_RECENTER = (1 << 7),
        /// <summary>
        /// An enum constant representing the controller available feature haptic vibrate option. </summary>
        CONTROLLER_AVAILABLE_FEATURE_HAPTIC_VIBRATE = (1 << 8)
    }

    /// <summary> Values that represent controller buttons. </summary>
    public enum ControllerButton
    {
        /// <summary> An enum constant representing the trigger option. </summary>
        TRIGGER = 1 << 0,
        /// <summary> An enum constant representing the Application option. </summary>
        APP = 1 << 1,
        /// <summary> An enum constant representing the home option. </summary>
        HOME = 1 << 2,
        /// <summary> An enum constant representing the grip option. </summary>
        GRIP = 1 << 3,
        /// <summary> An enum constant representing the touchpad button option. </summary>
        TOUCHPAD_BUTTON = 1 << 4
    }

    /// <summary> Values that represent controller connection states. </summary>
    public enum ControllerConnectionState
    {
        /// <summary>
        /// An enum constant representing the controller connection state error option. </summary>
        CONTROLLER_CONNECTION_STATE_ERROR = -1,
        /// <summary>
        /// An enum constant representing the controller connection state not initialized option. </summary>
        CONTROLLER_CONNECTION_STATE_NOT_INITIALIZED = 0,
        /// <summary>
        /// An enum constant representing the controller connection state disconnected option. </summary>
        CONTROLLER_CONNECTION_STATE_DISCONNECTED = 1,
        /// <summary>
        /// An enum constant representing the controller connection state connecting option. </summary>
        CONTROLLER_CONNECTION_STATE_CONNECTING = 2,
        /// <summary>
        /// An enum constant representing the controller connection state connected option. </summary>
        CONTROLLER_CONNECTION_STATE_CONNECTED = 3
    }

    internal enum HandednessType
    {
        LEFT_HANDEDNESS = 1,
        RIGHT_HANDEDNESS,
    }

    /// <summary> Values that represent button event types. </summary>
    internal enum ButtonEventType
    {
        /// <summary> An enum constant representing the down option. </summary>
        Down = 0,
        /// <summary> An enum constant representing the pressing option. </summary>
        Pressing,
        /// <summary> An enum constant representing the up option. </summary>
        Up,
        /// <summary> An enum constant representing the click option. </summary>
        Click,
    }

    /// <summary> A controller state. </summary>
    public class ControllerState
    {
        /// <summary> Type of the controller. </summary>
        internal ControllerType controllerType;
        /// <summary> State of the connection. </summary>
        internal ControllerConnectionState connectionState;
        /// <summary> The rotation. </summary>
        internal Quaternion rotation;
        /// <summary> The position. </summary>
        internal Vector3 position;
        /// <summary> The gyro. </summary>
        internal Vector3 gyro;
        /// <summary> The accel. </summary>
        internal Vector3 accel;
        /// <summary> The magnitude. </summary>
        internal Vector3 mag;
        /// <summary> The touch position. </summary>
        internal Vector2 touchPos;
        /// <summary> The delta touch. </summary>
        internal Vector2 deltaTouch;
        /// <summary> True if is touching, false if not. </summary>
        internal bool isTouching;
        /// <summary> True if recentered. </summary>
        internal bool recentered;
        /// <summary> State of the buttons. </summary>
        internal ControllerButton buttonsState;
        /// <summary> The buttons down. </summary>
        internal ControllerButton buttonsDown;
        /// <summary> The buttons up. </summary>
        internal ControllerButton buttonsUp;
        /// <summary> True if is charging, false if not. </summary>
        internal bool isCharging;
        /// <summary> The battery level. </summary>
        internal int batteryLevel;
        /// <summary> The available feature. </summary>
        internal ControllerAvailableFeature availableFeature;

        /// <summary> The last touch position. </summary>
        private Vector2 m_LastTouchPos;
        /// <summary> Dictionary of last down times. </summary>
        private Dictionary<ControllerButton, float> m_LastDownTimeDict = new Dictionary<ControllerButton, float>();
        /// <summary> Array of listeners. </summary>
        private Dictionary<ControllerButton, Action>[] m_ListenersArr = new Dictionary<ControllerButton, Action>[Enum.GetValues(typeof(ButtonEventType)).Length];

        /// <summary> Default constructor. </summary>
        public ControllerState()
        {
            Reset();
        }

        /// <summary> Gets a value indicating whether this object is 6dof. </summary>
        /// <value> True if this object is 6dof, false if not. </value>
        public bool Is6dof
        {
            get
            {
                return IsFeatureAvailable(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_POSITION) && IsFeatureAvailable(ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION);
            }
        }

        /// <summary> Queries if a feature is available. </summary>
        /// <param name="feature"> The feature.</param>
        /// <returns> True if the feature is available, false if not. </returns>
        public bool IsFeatureAvailable(ControllerAvailableFeature feature)
        {
            return (availableFeature & feature) != 0;
        }

        /// <summary> Gets a button. </summary>
        /// <param name="button"> The button.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetButton(ControllerButton button)
        {
            return (buttonsState & button) != 0;
        }

        /// <summary> Gets button down. </summary>
        /// <param name="button"> The button.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetButtonDown(ControllerButton button)
        {
            return (buttonsDown & button) != 0;
        }

        /// <summary> Gets button up. </summary>
        /// <param name="button"> The button.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetButtonUp(ControllerButton button)
        {
            return (buttonsUp & button) != 0;
        }

        /// <summary> Resets this object. </summary>
        public void Reset()
        {
            controllerType = ControllerType.CONTROLLER_TYPE_UNKNOWN;
            connectionState = ControllerConnectionState.CONTROLLER_CONNECTION_STATE_NOT_INITIALIZED;
            rotation = Quaternion.identity;
            position = Vector3.zero;
            gyro = Vector3.zero;
            accel = Vector3.zero;
            mag = Vector3.zero;
            touchPos = Vector2.zero;
            isTouching = false;
            recentered = false;
            buttonsState = 0;
            buttonsDown = 0;
            buttonsUp = 0;
            isCharging = false;
            batteryLevel = 0;
            availableFeature = 0;
        }

        /// <summary> Adds a button listener. </summary>
        /// <param name="buttonEventType"> Type of the button event.</param>
        /// <param name="button">          The button.</param>
        /// <param name="action">          The action.</param>
        internal void AddButtonListener(ButtonEventType buttonEventType, ControllerButton button, Action action)
        {
            int buttonEventID = (int)buttonEventType;
            if (m_ListenersArr[buttonEventID] == null)
            {
                m_ListenersArr[buttonEventID] = new Dictionary<ControllerButton, Action>();
            }
            if (m_ListenersArr[buttonEventID].ContainsKey(button))
            {
                m_ListenersArr[buttonEventID][button] += action;
            }
            else
            {
                m_ListenersArr[buttonEventID].Add(button, action);
            }
        }

        /// <summary> Removes the button listener. </summary>
        /// <param name="buttonEventType"> Type of the button event.</param>
        /// <param name="button">          The button.</param>
        /// <param name="action">          The action.</param>
        internal void RemoveButtonListener(ButtonEventType buttonEventType, ControllerButton button, Action action)
        {
            int buttonEventID = (int)buttonEventType;
            if (m_ListenersArr[buttonEventID] == null)
            {
                m_ListenersArr[buttonEventID] = new Dictionary<ControllerButton, Action>();
            }
            if (m_ListenersArr[buttonEventID].ContainsKey(button) && m_ListenersArr[buttonEventID][button] != null)
            {
                m_ListenersArr[buttonEventID][button] -= action;
                if (m_ListenersArr[buttonEventID][button] == null)
                {
                    m_ListenersArr[buttonEventID].Remove(button);
                }
            }
        }

        /// <summary> Updates the delta touch. </summary>
        internal void UpdateDeltaTouch()
        {
            if (m_LastTouchPos.Equals(Vector2.zero) || touchPos.Equals(Vector2.zero))
            {
                deltaTouch = Vector2.zero;
            }
            else
            {
                deltaTouch = touchPos - m_LastTouchPos;
            }
            m_LastTouchPos = touchPos;
        }

        /// <summary> Check button events. </summary>
        internal void CheckButtonEvents()
        {
            UpdateDeltaTouch();
            foreach (ControllerButton button in Enum.GetValues(typeof(ControllerButton)))
            {
                if (GetButtonDown(button))
                {
                    if (m_LastDownTimeDict.ContainsKey(button))
                    {
                        m_LastDownTimeDict[button] = Time.unscaledTime;
                    }
                    else
                    {
                        m_LastDownTimeDict.Add(button, Time.unscaledTime);
                    }
                    TryInvokeListener(ButtonEventType.Down, button);
                }
                if (GetButton(button))
                {
                    TryInvokeListener(ButtonEventType.Pressing, button);
                }
                else if (GetButtonUp(button))
                {
                    TryInvokeListener(ButtonEventType.Up, button);
                    float lastDownTime;
                    if (m_LastDownTimeDict.TryGetValue(button, out lastDownTime) && Time.unscaledTime - lastDownTime < NRInput.ClickInterval)
                    {
                        TryInvokeListener(ButtonEventType.Click, button);
                    }
                }
            }
        }

        /// <summary> Try invoke listener. </summary>
        /// <param name="buttonEventType"> Type of the button event.</param>
        /// <param name="button">          The button.</param>
        private void TryInvokeListener(ButtonEventType buttonEventType, ControllerButton button)
        {
            int buttonEventID = (int)buttonEventType;
            if (m_ListenersArr[buttonEventID] == null)
                return;
            if (m_ListenersArr[buttonEventID].ContainsKey(button) && m_ListenersArr[buttonEventID][button] != null)
            {
                m_ListenersArr[buttonEventID][button].Invoke();
            }
        }
    }

}