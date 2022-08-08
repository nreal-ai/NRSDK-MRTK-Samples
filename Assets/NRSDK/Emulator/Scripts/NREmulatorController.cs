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

#if UNITY_EDITOR
    /// <summary> A controller for handling nr emulators. </summary>
    public class NREmulatorController : MonoBehaviour
    {
        /// <summary> regular speed. </summary>
        public float HeadMoveSpeed = 1.0f;
        /// <summary> How sensitive it with mouse. </summary>
        public float HeadRotateSpeed = 1.0f;

        /// <summary> The default controller panel. </summary>
        public GameObject DefaultControllerPanel;
        /// <summary> The image default. </summary>
        public GameObject ImageDefault;
        /// <summary> The image application. </summary>
        public GameObject ImageApp;
        /// <summary> The image confirm. </summary>
        public GameObject ImageConfirm;
        /// <summary> The image home. </summary>
        public GameObject ImageHome;
        /// <summary> The image left. </summary>
        public GameObject ImageLeft;
        /// <summary> The image right. </summary>
        public GameObject ImageRight;
        /// <summary> The image up. </summary>
        public GameObject ImageUp;
        /// <summary> The image down. </summary>
        public GameObject ImageDown;

        /// <summary> The width. </summary>
        private const int kWidth = 2;
        /// <summary> The height. </summary>
        private const int kHeight = 2;

        /// <summary> The touch action. </summary>
        private TouchActionState m_TouchAction;
        /// <summary> The touch action current frame. </summary>
        private int m_TouchActionCurFrame;
        /// <summary> Target for the. </summary>
        private GameObject m_Target;

        /// <summary> Values that represent touch action states. </summary>
        enum TouchActionState
        {
            /// <summary> An enum constant representing the idle option. </summary>
            Idle,
            /// <summary> An enum constant representing the left option. </summary>
            Left,
            /// <summary> An enum constant representing the right option. </summary>
            Right,
            /// <summary> An enum constant representing the up option. </summary>
            Up,
            /// <summary> An enum constant representing the down option. </summary>
            Down,
        };

        void Start()
        {
            DontDestroyOnLoad(this);
            m_Target = new GameObject("NREmulatorControllerTarget");
            m_Target.transform.rotation = Quaternion.identity;
            DontDestroyOnLoad(m_Target);
        }

        void LateUpdate()
        {
            UpdateControllerRotateByInput();

            if (NRInput.EmulateVirtualDisplayInEditor)
            {
                DefaultControllerPanel.SetActive(false);
                UpdateVirtualControllerButtons();
            }
            else
            {
                DefaultControllerPanel.SetActive(true);
                UpdateDefaultControllerButtons();
            }
        }

        private void UpdateDefaultControllerButtons()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetConfirmButton(true);
                ImageConfirm.SetActive(true);
            }
            if (Input.GetMouseButtonUp(0))
            {
                SetConfirmButton(false);
                ImageConfirm.SetActive(false);
            }
            if (Input.GetMouseButtonDown(1))
            {
                SetAppButton(true);
                ImageApp.SetActive(true);
            }
            if (Input.GetMouseButtonUp(1))
            {
                SetAppButton(false);
                ImageApp.SetActive(false);
            }
            if (Input.GetMouseButtonDown(2))
            {
                SetHomeButton(true);
                ImageHome.SetActive(true);
            }
            if (Input.GetMouseButtonUp(2))
            {
                SetHomeButton(false);
                ImageHome.SetActive(false);
            }
            if (m_TouchAction != TouchActionState.Idle)
            {
                UpdateTouchAction();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    ImageLeft.SetActive(true);
                    m_TouchAction = TouchActionState.Left;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    ImageRight.SetActive(true);
                    m_TouchAction = TouchActionState.Right;
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    ImageUp.SetActive(true);
                    m_TouchAction = TouchActionState.Up;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    ImageDown.SetActive(true);
                    m_TouchAction = TouchActionState.Down;
                }
                else if (Input.GetKeyUp(KeyCode.DownArrow)
                    | Input.GetKeyUp(KeyCode.UpArrow)
                    | Input.GetKeyUp(KeyCode.RightArrow)
                    | Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    EditorControllerProvider.SetControllerIsTouching(false);
                }
            }
        }

        private void UpdateVirtualControllerButtons()
        {
            EditorControllerProvider.SetControllerButtonState(ControllerButton.APP, 0);
            EditorControllerProvider.SetControllerButtonState(ControllerButton.HOME, 0);
            EditorControllerProvider.SetControllerButtonState(ControllerButton.TRIGGER, 0);
            EditorControllerProvider.SetControllerIsTouching(true);
            EditorControllerProvider.SetControllerTouchPoint(NRVirtualDisplayer.GetEmulatorScreenTouch().x, NRVirtualDisplayer.GetEmulatorScreenTouch().y);
        }

        private void UpdateTouchAction()
        {
            EditorControllerProvider.SetControllerIsTouching(true);
            const int kActionMaxFrame = 20;
            float touchx = 0;
            float touchy = 0;
            if (m_TouchAction == TouchActionState.Left)
            {
                touchy = kHeight / 2;
                touchx = 0.1f * kWidth + ((float)(kActionMaxFrame - m_TouchActionCurFrame) / kActionMaxFrame) * (0.8f * kWidth);
            }
            else if (m_TouchAction == TouchActionState.Right)
            {
                touchy = kHeight / 2;
                touchx = 0.1f * kWidth + ((float)m_TouchActionCurFrame / kActionMaxFrame) * (0.8f * kWidth);
            }
            else if (m_TouchAction == TouchActionState.Up)
            {
                touchx = kWidth / 2;
                touchy = 0.1f * kHeight + ((float)(kActionMaxFrame - m_TouchActionCurFrame) / kActionMaxFrame) * (0.8f * kHeight);
            }
            else if (m_TouchAction == TouchActionState.Down)
            {
                touchx = kWidth / 2;
                touchy = 0.1f * kHeight + ((float)m_TouchActionCurFrame / kActionMaxFrame) * (0.8f * kHeight);
            }

            EditorControllerProvider.SetControllerTouchPoint(touchx - 1f, touchy - 1f);

            if (m_TouchActionCurFrame == kActionMaxFrame)
            {
                m_TouchActionCurFrame = 0;
                m_TouchAction = TouchActionState.Idle;
                ImageLeft.SetActive(false);
                ImageRight.SetActive(false);
                ImageUp.SetActive(false);
                ImageDown.SetActive(false);
            }

            m_TouchActionCurFrame++;

        }

        private void UpdateControllerRotateByInput()
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                float mouse_x = Input.GetAxis("Mouse X") * HeadRotateSpeed;
                float mouse_y = Input.GetAxis("Mouse Y") * HeadRotateSpeed;

                Quaternion q = Quaternion.Euler(-mouse_y, mouse_x, 0);
                m_Target.transform.rotation = m_Target.transform.rotation * q;
            }
            EditorControllerProvider.SetControllerRotation(m_Target.transform.rotation);
        }

        /// <summary> Sets application button. </summary>
        /// <param name="touch"> True to touch.</param>
        public void SetAppButton(bool touch)
        {
            if (touch)
            {
                EditorControllerProvider.SetControllerTouchPoint(0f, 0f);
                EditorControllerProvider.SetControllerIsTouching(true);
                EditorControllerProvider.SetControllerButtonState(ControllerButton.APP, 1);
            }
            else
            {
                EditorControllerProvider.SetControllerButtonState(ControllerButton.APP, 0);
                EditorControllerProvider.SetControllerIsTouching(false);
            }
        }

        /// <summary> Sets home button. </summary>
        /// <param name="touch"> True to touch.</param>
        public void SetHomeButton(bool touch)
        {
            if (touch)
            {
                EditorControllerProvider.SetControllerTouchPoint(0f, 0f);
                EditorControllerProvider.SetControllerIsTouching(true);
                EditorControllerProvider.SetControllerButtonState(ControllerButton.HOME, 1);
            }
            else
            {
                EditorControllerProvider.SetControllerButtonState(ControllerButton.HOME, 0);
                EditorControllerProvider.SetControllerIsTouching(false);
            }
        }

        /// <summary> Sets confirm button. </summary>
        /// <param name="touch"> True to touch.</param>
        public void SetConfirmButton(bool touch)
        {
            if (touch)
            {
                EditorControllerProvider.SetControllerTouchPoint(0f, 0f);
                EditorControllerProvider.SetControllerIsTouching(true);
                EditorControllerProvider.SetControllerButtonState(ControllerButton.TRIGGER, 1);
            }
            else
            {
                EditorControllerProvider.SetControllerButtonState(ControllerButton.TRIGGER, 0);
                EditorControllerProvider.SetControllerIsTouching(false);
            }
        }
    }
#endif
}