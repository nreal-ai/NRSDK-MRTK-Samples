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
    /// <summary> Manager for applications. </summary>
    [DisallowMultipleComponent]
    [HelpURL("https://developer.nreal.ai/develop/discover/introduction-nrsdk")]
    public class NRAppManager : MonoBehaviour
    {
        /// <summary>
        /// If enable this, quick click app button for three times, a profiler bar would show. </summary>
        public bool enableTriggerProfiler;

        /// <summary> The last click time. </summary>
        private float m_LastClickTime = 0f;
        /// <summary> The cumulative click number. </summary>
        private int m_CumulativeClickNum = 0;
        /// <summary> True if is profiler opened, false if not. </summary>
        private bool m_IsProfilerOpened = false;
        /// <summary> System gesture duration timer. </summary>
        private float m_SystemGestureTimer;

        /// <summary> Number of trigger profiler clicks. </summary>
        private const int TRIGGER_PROFILER_CLICK_COUNT = 3;
        /// <summary> Duration of system gesture to trigger function. </summary>
        private const float SYSTEM_GESTURE_KEEP_DURATION = 1.2f;

        private static AndroidJavaObject currentActivity;
        public static AndroidJavaObject CurrentActivity
        {
            get
            {
                if (currentActivity == null)
                {
                    currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                }
                return currentActivity;

            }

        }

        /// <summary> Executes the 'enable' action. </summary>
        private void OnEnable()
        {
            NRInput.AddClickListener(ControllerHandEnum.Right, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.AddClickListener(ControllerHandEnum.Left, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.AddClickListener(ControllerHandEnum.Right, ControllerButton.APP, OnAppButtonClick);
            NRInput.AddClickListener(ControllerHandEnum.Left, ControllerButton.APP, OnAppButtonClick);
        }

        /// <summary> Executes the 'disable' action. </summary>
        private void OnDisable()
        {
            NRInput.RemoveClickListener(ControllerHandEnum.Right, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.RemoveClickListener(ControllerHandEnum.Left, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.RemoveClickListener(ControllerHandEnum.Right, ControllerButton.APP, OnAppButtonClick);
            NRInput.RemoveClickListener(ControllerHandEnum.Left, ControllerButton.APP, OnAppButtonClick);
        }

        /// <summary> Updates this object. </summary>
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitApplication();
            }
#endif
            CheckSystemGesture();
        }

        /// <summary> Executes the 'home button click' action. </summary>
        private void OnHomeButtonClick()
        {
            NRHomeMenu.Toggle();
        }

        /// <summary> Executes the 'application button click' action. </summary>
        private void OnAppButtonClick()
        {
            if (enableTriggerProfiler)
            {
                CollectClickEvent();
            }
        }

        /// <summary> Collect click event. </summary>
        private void CollectClickEvent()
        {
            if (Time.unscaledTime - m_LastClickTime < 0.2f)
            {
                m_CumulativeClickNum++;
                if (m_CumulativeClickNum == (TRIGGER_PROFILER_CLICK_COUNT - 1))
                {
                    // Show the VisualProfiler
                    NRVisualProfiler.Instance.Switch(!m_IsProfilerOpened);
                    m_IsProfilerOpened = !m_IsProfilerOpened;
                    m_CumulativeClickNum = 0;
                }
            }
            else
            {
                m_CumulativeClickNum = 0;
            }
            m_LastClickTime = Time.unscaledTime;
        }

        private void CheckSystemGesture()
        {
            if (NRInput.Hands.IsPerformingSystemGesture())
            {
                m_SystemGestureTimer += Time.deltaTime;
                if(m_SystemGestureTimer > SYSTEM_GESTURE_KEEP_DURATION)
                {
                    m_SystemGestureTimer = float.MinValue;
                    NRHomeMenu.Show();
                }
            }
            else
            {
                m_SystemGestureTimer = 0f;
            }
        }

        /// <summary> Quit application. </summary>
        public static void QuitApplication(bool backToMRSpace = false)
        {
            if (backToMRSpace)
            {
                NRDebugger.Info("QuitApplication backToMRSpace");
                if (Application.platform == RuntimePlatform.Android)
                {
                    // 这个Action会启动Nebula Space
                    string action = "ai.nreal.nebula.start.MRAPP";
                    AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
                    intent.Call<AndroidJavaObject>("setAction", action);
                    intent.Call<AndroidJavaObject>("addFlags", 0x10000000);
                    // 优先级最高
                    intent.Call<AndroidJavaObject>("putExtra", "backToMRSpace", true);
                    CurrentActivity.Call("startActivity", intent);
                }
            }

            NRDebugger.Info("QuitApplication QuitApp");
            NRDevice.QuitApp();
        }
    }
}
