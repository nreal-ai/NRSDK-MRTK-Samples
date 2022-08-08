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
    using UnityEngine.EventSystems;

    /// <summary> A controller for handling multi screens. </summary>
    public class MultiScreenController : MonoBehaviour, ISystemButtonStateProvider
    {
        private SystemButtonState m_SystemButtonState = new SystemButtonState();
        private ISystemButtonStateReceiver m_Receiver;
        /// <summary> System defined button. </summary>
        [SerializeField]
        private NRButton Trigger;
        /// <summary> The application. </summary>
        [SerializeField]
        private NRButton App;
        /// <summary> The home. </summary>
        [SerializeField]
        private NRButton Home;

        public void BindReceiver(ISystemButtonStateReceiver receiver)
        {
            this.m_Receiver = receiver;
            InitSystemButtonEvent();
        }

        /// <summary> Initializes the system button event. </summary>
        private void InitSystemButtonEvent()
        {
            Trigger.TriggerEvent += OnBtnTrigger;
            App.TriggerEvent += OnBtnTrigger;
            Home.TriggerEvent += OnBtnTrigger;
        }

        /// <summary> Executes the 'button trigger' action. </summary>
        /// <param name="key">        The key.</param>
        /// <param name="go">         The go.</param>
        /// <param name="racastInfo"> Information describing the racast.</param>
        private void OnBtnTrigger(string key, GameObject go, RaycastResult racastInfo)
        {
            if (key.Equals(NRButton.Enter))
            {
                if (go == App.gameObject)
                {
                    m_SystemButtonState.buttons[0] = true;
                }
                if (go == Trigger.gameObject)
                {
                    m_SystemButtonState.buttons[1] = true;
                }
                if (go == Home.gameObject)
                {
                    m_SystemButtonState.buttons[2] = true;
                }
            }
            else if (key.Equals(NRButton.Exit))
            {
                if (go == App.gameObject)
                {
                    m_SystemButtonState.buttons[0] = false;
                }
                if (go == Trigger.gameObject)
                {
                    m_SystemButtonState.buttons[1] = false;
                }
                if (go == Home.gameObject)
                {
                    m_SystemButtonState.buttons[2] = false;
                }
            }

            if (go == Trigger.gameObject
                && (key.Equals(NRButton.Hover) || key.Equals(NRButton.Enter)))
            {
                CalculateTouchPos(go, racastInfo);
            }
            else
            {
                m_SystemButtonState.touch_x = 0f;
                m_SystemButtonState.touch_y = 0f;
            }

            this.m_Receiver?.OnDataReceived(m_SystemButtonState);
        }

        /// <summary> Calculates the touch position. </summary>
        /// <param name="go">         The go.</param>
        /// <param name="racastInfo"> Information describing the racast.</param>
        private void CalculateTouchPos(GameObject go, RaycastResult racastInfo)
        {
            RectTransform rect = go.GetComponent<RectTransform>();
            Vector3[] v = new Vector3[4];
            rect.GetWorldCorners(v);

            var touchToCenter = racastInfo.worldPosition - go.transform.position;
            var rightToCenter = (v[3] - v[0]) * 0.5f;
            var topToCenter = (v[1] - v[0]) * 0.5f;
            var halfWidth = (v[3] - v[0]).magnitude * 0.5f;
            var halfHeight = (v[1] - v[0]).magnitude * 0.5f;
            var alpha = Vector3.Angle(rightToCenter, touchToCenter);
            var touchToX = (touchToCenter * Mathf.Cos(alpha * Mathf.PI / 180)).magnitude;
            var touchToY = (touchToCenter * Mathf.Sin(alpha * Mathf.PI / 180)).magnitude;

            bool x_forward = Vector3.Dot(touchToCenter, rightToCenter) > 0;
            bool y_forward = Vector3.Dot(touchToCenter, topToCenter) > 0;

            var touchx = touchToX > halfWidth ? (x_forward ? 1f : -1f) : (x_forward ? touchToX / halfWidth : -touchToX / halfWidth);
            var touchy = touchToY > halfHeight ? (y_forward ? 1f : -1f) : (y_forward ? touchToY / halfHeight : -touchToY / halfHeight);
            m_SystemButtonState.touch_x = touchx;
            m_SystemButtonState.touch_y = touchy;
        }

#if UNITY_EDITOR
        /// <summary> Executes the 'disable' action. </summary>
        private void OnDisable()
        {
            if (!NRInput.EmulateVirtualDisplayInEditor)
                ClearSystemButtonState();
        }

        /// <summary> Clears the system button state. </summary>
        private void ClearSystemButtonState()
        {

        }
#endif
    }
}
