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
    using UnityEngine;

    /// <summary> A phone vibrate tool. </summary>
    internal class PhoneVibrateTool
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        public AndroidJavaClass unityPlayer;
        public AndroidJavaObject currentActivity;
        public AndroidJavaObject vibrator;

        private static PhoneVibrateTool _instance;

        public PhoneVibrateTool()
        {
            unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            //to make unity auto add vibrate user permission
            if (Application.isEditor)
                Handheld.Vibrate();
        }

        private void DoVibrate()
        {
            vibrator.Call("vibrate");
        }

        private void DoVibrate(long milliseconds)
        {
            vibrator.Call("vibrate", milliseconds);
        }

        private void DoVibrate(long[] pattern, int repeat)
        {
            vibrator.Call("vibrate", pattern, repeat);
        }

        private void CancelVibrate()
        {
            vibrator.Call("cancel");
        }

        private bool HasVibrator()
        {
            return vibrator.Call<bool>("hasVibrator");
        }
#endif

        /// <summary> Trigger vibrate. </summary>
        /// <param name="durationSeconds"> (Optional) The duration in seconds.</param>
        public static void TriggerVibrate(float durationSeconds = 0.1f)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (_instance == null)
            _instance = new PhoneVibrateTool();
        if(_instance != null && _instance.HasVibrator())
            _instance.DoVibrate((long)(durationSeconds * 1000f));
#endif
        }
    }
}


