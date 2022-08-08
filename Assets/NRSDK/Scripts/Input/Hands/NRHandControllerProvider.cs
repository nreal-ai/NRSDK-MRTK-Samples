/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/         
* 
*****************************************************************************/

using UnityEngine;

namespace NRKernal
{
    public class NRHandControllerProvider : ControllerProviderBase
    {
        public class NRHandControllerStateParser
        {
            /// <summary> The buttons down. </summary>
            private bool[] m_ButtonsDown = new bool[3];
            /// <summary> The buttons up. </summary>
            private bool[] m_ButtonsUp = new bool[3];
            /// <summary> The buttons. </summary>
            private bool[] m_Buttons = new bool[3];
            /// <summary> The down. </summary>
            private bool[] m_Down = new bool[3];

            /// <summary> Parser controller state. </summary>
            /// <param name="state"> The state.</param>
            public void ParserControllerState(ControllerState state, HandState handState)
            {
                lock (m_Buttons)
                {
                    lock (m_Down)
                    {
                        for (int i = 0; i < m_Buttons.Length; ++i)
                        {
                            m_Down[i] = m_Buttons[i];
                        }
                    }

                    m_Buttons[0] = handState.pointerPoseValid && handState.isPinching;  //Trigger
                    m_Buttons[1] = false;  //App
                    m_Buttons[2] = false;  //Home

                    lock (m_ButtonsUp)
                    {
                        lock (m_ButtonsDown)
                        {
                            for (int i = 0; i < m_Buttons.Length; ++i)
                            {
                                m_ButtonsUp[i] = (m_Down[i] & !m_Buttons[i]);
                                m_ButtonsDown[i] = (!m_Down[i] & m_Buttons[i]);
                            }
                        }
                    }
                }
                state.buttonsState =
                    (m_Buttons[0] ? ControllerButton.TRIGGER : 0)
                    | (m_Buttons[1] ? ControllerButton.APP : 0)
                    | (m_Buttons[2] ? ControllerButton.HOME : 0);
                state.buttonsDown =
                    (m_ButtonsDown[0] ? ControllerButton.TRIGGER : 0)
                    | (m_ButtonsDown[1] ? ControllerButton.APP : 0)
                    | (m_ButtonsDown[2] ? ControllerButton.HOME : 0);
                state.buttonsUp =
                    (m_ButtonsUp[0] ? ControllerButton.TRIGGER : 0)
                    | (m_ButtonsUp[1] ? ControllerButton.APP : 0)
                    | (m_ButtonsUp[2] ? ControllerButton.HOME : 0);
            }
        }

        /// <summary> The processed frame. </summary>
        private int m_ProcessedFrame;
        private NRHandControllerStateParser[] m_StateParsers = new NRHandControllerStateParser[NRInput.MAX_CONTROLLER_STATE_COUNT];

        /// <summary> Constructor. </summary>
        /// <param name="states"> The states.</param>
        public NRHandControllerProvider(ControllerState[] states) : base(states)
        {
            for (int i = 0; i < m_StateParsers.Length; i++)
            {
                m_StateParsers[i] = new NRHandControllerStateParser();
            }
            Inited = true;
        }

        public override int ControllerCount { get { return 2; } }

        public override void OnDestroy()
        {
            
        }

        public override void OnPause()
        {
            for (int i = 0; i < states.Length; i++)
            {
                states[i].Reset();
            }
        }

        public override void OnResume()
        {
            
        }

        public override void Update()
        {
            if (m_ProcessedFrame == Time.frameCount)
                return;
            m_ProcessedFrame = Time.frameCount;
            for (int i = 0; i < states.Length; i++)
            {
                UpdateControllerState(i, GetHandState(i));
            }
        }

        private NRHandControllerStateParser GetNRHandControllerStateParser(int index)
        {
            if(index < m_StateParsers.Length)
            {
                return m_StateParsers[index];
            }
            return null;
        }

        private HandState GetHandState(int index)
        {
            return NRInput.Hands.GetHandState(index == 0 ? HandEnum.RightHand : HandEnum.LeftHand);
        }

        private void UpdateControllerState(int index, HandState handState)
        {
            states[index].controllerType = ControllerType.CONTROLLER_TYPE_HAND;
            states[index].availableFeature = ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_ROTATION | ControllerAvailableFeature.CONTROLLER_AVAILABLE_FEATURE_POSITION;
            states[index].connectionState = ControllerConnectionState.CONTROLLER_CONNECTION_STATE_CONNECTED;
            states[index].rotation = handState.pointerPose.rotation;
            states[index].position = handState.pointerPose.position;
            states[index].gyro = Vector3.zero;
            states[index].accel = Vector3.zero;
            states[index].mag = Vector3.zero;
            states[index].touchPos = Vector3.zero;
            states[index].isTouching = false;
            states[index].recentered = false;
            states[index].isCharging = false;
            states[index].batteryLevel = 0;

            var stateParser = GetNRHandControllerStateParser(index);
            if (stateParser != null)
            {
                stateParser.ParserControllerState(states[index], handState);
            }
        }
    }
}
