using System;
using System.Collections;
using UnityEngine;

namespace NRKernal.NRExamples
{
    public class NativeGalleryDataProvider
    {
        private static AndroidJavaClass m_NativeClass;
        public static AndroidJavaClass NativeClass
        {
            get
            {
                if (m_NativeClass == null)
                    m_NativeClass = new AndroidJavaClass("ai.nreal.android.gallery.GalleryDataProvider");
                return m_NativeClass;
            }
        }

        public const string MAIN_ACTIVITY_CLASS = "com.unity3d.player.UnityPlayer";

        private static AndroidJavaObject m_CurrentActivity;
        public static AndroidJavaObject CurrentActivity
        {
            get
            {
                if (m_CurrentActivity == null)
                {
                    using (AndroidJavaClass jc = new AndroidJavaClass(MAIN_ACTIVITY_CLASS))
                    {
                        m_CurrentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
                    }
                }
                return m_CurrentActivity;
            }
        }

        private AndroidJavaObject m_NativeObject;

        public NativeGalleryDataProvider()
        {
            m_NativeObject = new AndroidJavaObject("ai.nreal.android.gallery.GalleryDataProvider",
                                                   CurrentActivity);
        }

        public void InsertImage(byte[] data, string displayName, string folderName)
        {
            InsertImageData(data, displayName, folderName);
        }

        public AndroidJavaObject InsertImageData(byte[] data, string displayName, string folderName)
        {
            AndroidJavaObject inputStream = new AndroidJavaObject("java.io.ByteArrayInputStream", data);
            return m_NativeObject.Call<AndroidJavaObject>("insertImage", inputStream, displayName, folderName, "image/png");
        }

        public AndroidJavaObject InsertVideo(string originFilePath, string displayName, string folderName)
        {
            return m_NativeObject.Call<AndroidJavaObject>("insertVideo", originFilePath, displayName, folderName);
        }
    }
}