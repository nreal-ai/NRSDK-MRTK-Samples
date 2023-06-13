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

    /// <summary> A nr tools. </summary>
    public class NRTools
    {
        /// <summary> Full pathname of the persistent data file. </summary>
        private static string persistentDataPath;

        /// <summary> Initializes this object. </summary>
        public static void Init()
        {
            persistentDataPath = Application.persistentDataPath;
        }

        #region path utility
        /// <summary> Gets streaming assets path. </summary>
        /// <returns> The streaming assets path. </returns>
        public static string GetStreamingAssetsPath()
        {
            string path = Application.streamingAssetsPath;
#if UNITY_EDITOR || UNITY_STANDALONE
            path = "file://" + Application.streamingAssetsPath + "/";
#elif UNITY_IPHONE
            path = Application.dataPath +"/Raw/";
#elif UNITY_ANDROID
            path ="jar:file://" + Application.dataPath + "!/assets/";
#endif
            return path;
        }

        /// <summary> Gets sdcard path. </summary>
        /// <returns> The sdcard path. </returns>
        public static string GetSdcardPath()
        {
            string path = null;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            path = System.IO.Directory.GetParent(Application.dataPath).ToString() + "/";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            path = "file://" + System.IO.Directory.GetCurrentDirectory()+"/";
#elif UNITY_ANDROID
            path ="file:///storage/emulated/0/";
#endif
            return path;
        }

        /// <summary> Gets tracking image data generate path. </summary>
        /// <returns> The tracking image data generate path. </returns>
        public static string GetTrackingImageDataGenPath()
        {
#if UNITY_EDITOR
            string path = Application.persistentDataPath + "/TrackingImageData/";
#else
            string path = persistentDataPath + "/TrackingImageData/";
#endif
            return path;
        }
        #endregion

        #region time stamp
        /// <summary> Gets time by m seconds. </summary>
        /// <param name="ms"> The milliseconds.</param>
        /// <returns> The time by m seconds. </returns>
        public static string GetTimeByMSeconds(long ms)
        {
            int s = (int)ms / 1000;
            int h = (int)(s / 3600);
            int m = (s % 3600) / 60;
            s = (s % 3600) % 60;
            return string.Format("{0}:{1}:{2}", h > 10 ? h.ToString() : "0" + h, m > 10 ? m.ToString() : "0" + m, s > 10 ? s.ToString() : "0" + s);
        }

        /// <summary> Gets time stamp. </summary>
        /// <returns> The time stamp. </returns>
        public static ulong GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToUInt64(ts.TotalMilliseconds);
        }

        /// <summary> Gets time stamp. </summary>
        /// <returns> The time stamp. </returns>
        public static ulong GetTimeStampNanos()
        {
            return GetTimeStamp() * 1000000;
        }
        #endregion
    }
}
