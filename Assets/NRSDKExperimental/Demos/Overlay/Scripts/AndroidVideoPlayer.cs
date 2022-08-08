using System;
using UnityEngine;

namespace NRKernal.Experimental.NRExamples
{
    public class AndroidVideoPlayer
    {
        public class AndroidVideoPlayerEventProxy : AndroidJavaProxy
        {
            public const int ONPREPARED = 10001;
            public const int ONFRAMEREADY = 10002;
            public const int ONCOMPLETED = 10003;
            public const int ONERROR = 10004;

            public AndroidVideoPlayerEventProxy() : base("ai.nreal.videoplayer.IVideoPlayerEventProxy")
            {
            }

            public void OnEvent(int eventid)
            {
                Debug.Log("AndroidVideoPlayerEventProxy:" + eventid);
            }
        }

        AndroidJavaObject m_MediaPlayerObject;
        AndroidJavaObject MediaPlayerObject
        {
            get
            {
                if (m_MediaPlayerObject == null)
                {
                    //m_MediaPlayerObject = new AndroidJavaObject("ai.nreal.videoplayer.AndroidMediaPlayer");
                    m_MediaPlayerObject = new AndroidJavaObject("ai.nreal.videoplayer.ExoMediaPlayer");
                }
                return m_MediaPlayerObject;
            }
        }

        AndroidJavaObject m_UnityActivity;
        AndroidJavaObject UnityActivity
        {
            get
            {
                if (m_UnityActivity == null)
                {
                    AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    m_UnityActivity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }
                return m_UnityActivity;
            }
        }

        public void InitWithMediaPlayer(IntPtr surface, string url, bool usedrm)
        {
            MediaPlayerObject.Call("init", UnityActivity, new AndroidVideoPlayerEventProxy());
            var methodID = AndroidJNI.GetMethodID(MediaPlayerObject.GetRawClass(), "setSurface", "(Landroid/view/Surface;)V");
            jvalue[] jniArgs = new jvalue[1];
            jniArgs[0].l = surface;
            AndroidJNI.CallVoidMethod(MediaPlayerObject.GetRawObject(), methodID, jniArgs);
            MediaPlayerObject.Call("load", url, usedrm);
        }

        public void Play()
        {
            Debug.Log("AndroidVideoPlayerEventProxy Play");
            MediaPlayerObject.Call("play");
        }

        public void Pause()
        {
            Debug.Log("AndroidVideoPlayerEventProxy Pause");
            MediaPlayerObject.Call("pause");
        }

        public void Release()
        {
            Debug.Log("AndroidVideoPlayerEventProxy Release");
            MediaPlayerObject.Call("release");
        }
    }
}
