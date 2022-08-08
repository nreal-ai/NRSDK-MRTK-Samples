using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.Experimental.NRExamples
{
    public interface IDrawListener
    {
        void onDrawBegin(Int64 nanos);
        void onDrawEnd(Int64 nanos);
    }

    public interface IWebEventListener
    {
        void onImmersiveSessionStart();
        void onImmersiveSessionEnd();
    }

    public class WebXRController
    {
        public bool isTracked;
        public bool isLeft;
        public float[] position = new float[3];
        public float[] orientation = new float[4];
        public float[] axes = new float[2];
        public float[] buttons = new float[5];

        private byte[] rawdata = new byte[58];

        public void UpdateData(bool tracked, bool isleft, Vector3 pos, Quaternion rotation, Vector2 touch, SystemInputState buttons)
        {
            this.isTracked = tracked;
            this.isLeft = isleft;
            this.position[0] = pos.x;
            this.position[1] = pos.y;
            this.position[2] = pos.z;

            var rot = ConversionUtility.ConvertOrientation(rotation);
            this.orientation[0] = rot.x;
            this.orientation[1] = rot.y;
            this.orientation[2] = rot.z;
            this.orientation[3] = rot.w;

            this.axes[0] = touch.x;
            this.axes[1] = touch.y;

            this.buttons[0] = buttons.buttons[0] ? 1 : 0;
            this.buttons[1] = buttons.buttons[1] ? 1 : 0;
            this.buttons[2] = buttons.buttons[2] ? 1 : 0;
        }

        public byte[] Serialize()
        {
            Array.Copy(BitConverter.GetBytes(isTracked), 0, rawdata, 0, 1);
            Array.Copy(BitConverter.GetBytes(isLeft), 0, rawdata, 1, 1);
            int index = 2;
            for (int i = 0; i < position.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes(position[i]), 0, rawdata, index, sizeof(float));
                index += sizeof(float);
            }
            for (int i = 0; i < orientation.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes(orientation[i]), 0, rawdata, index, sizeof(float));
                index += sizeof(float);
            }
            for (int i = 0; i < axes.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes(axes[i]), 0, rawdata, index, sizeof(float));
                index += sizeof(float);
            }
            for (int i = 0; i < buttons.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes(buttons[i]), 0, rawdata, index, sizeof(float));
                index += sizeof(float);
            }

            return rawdata;
        }

        public void DeSerialize(byte[] data)
        {
            isTracked = Convert.ToBoolean(data[0]);
            isLeft = Convert.ToBoolean(data[1]);

            int index = 2;
            for (int i = 0; i < position.Length; i++)
            {
                position[i] = ConvertUtility.IntBitsToFloat(BitConverter.ToInt32(data, index));
                index += sizeof(float);
            }
            for (int i = 0; i < orientation.Length; i++)
            {
                orientation[i] = ConvertUtility.IntBitsToFloat(BitConverter.ToInt32(data, index));
                index += sizeof(float);
            }
            for (int i = 0; i < axes.Length; i++)
            {
                axes[i] = ConvertUtility.IntBitsToFloat(BitConverter.ToInt32(data, index));
                index += sizeof(float);
            }
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = ConvertUtility.IntBitsToFloat(BitConverter.ToInt32(data, index));
                index += sizeof(float);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is WebXRController)
            {
                WebXRController new_info = (WebXRController)obj;
                if (new_info.isLeft != this.isLeft || new_info.isTracked != this.isTracked)
                {
                    return false;
                }
                for (int i = 0; i < position.Length; i++)
                {
                    if (new_info.position[i] != position[i])
                    {
                        return false;
                    }
                }
                for (int i = 0; i < orientation.Length; i++)
                {
                    if (new_info.orientation[i] != orientation[i])
                    {
                        return false;
                    }
                }
                for (int i = 0; i < axes.Length; i++)
                {
                    if (new_info.axes[i] != axes[i])
                    {
                        return false;
                    }
                }
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (new_info.buttons[i] != buttons[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("isTracked:{0} isLeft:{1} position:[{2}] orientation:[{3}] axes:[{4}] buttons:[{5}]",
                isTracked, isLeft, position.ToString(), orientation.ToString(), axes.ToString(), buttons.ToString());
        }
    }

    public class AndroidWebViewPlayer
    {
        public delegate void OnWebEvent();
        public delegate void OnWebDrawEvent(Int64 timestamp, int index);

        public class AndroidPoseProviderProxy : AndroidJavaProxy, IPoseProvider
        {
            private IPoseProvider PoseProvider { get; set; }

            public AndroidPoseProviderProxy(IPoseProvider provider) : base("ai.nreal.webframe.IDataSource")
            {
                PoseProvider = provider;
            }

            public Int64 getRecommendedTimeNanos()
            {
                return PoseProvider.getRecommendedTimeNanos();
            }

            public float[] getHeadPose(Int64 nanos)
            {
                return PoseProvider.getHeadPose(nanos);
            }

            public float[] getEyePoseFromHead(int eye)
            {
                return PoseProvider.getEyePoseFromHead(eye);
            }

            public float[] getEyeFov(int eye)
            {
                return PoseProvider.getEyeFov(eye);
            }

            public byte[] getControllerData()
            {
                return PoseProvider.getControllerData();
            }
        }

        public class AndroidDrawListenerProxy : AndroidJavaProxy, IDrawListener
        {
            public event OnWebDrawEvent onDrawBeginEvent;
            public event OnWebDrawEvent onDrawEndEvent;

            public Queue<TimeStampInfo> m_CacheTimestamp;
            public struct TimeStampInfo
            {
                public Int64 timeStamp;
                public int frameIndex;
            }

            private const int MaxCacheCount = 10;

            public AndroidDrawListenerProxy(OnWebDrawEvent onDrawBegin, OnWebDrawEvent onDrawEnd) : base("ai.nreal.webframe.IDrawListener")
            {
                onDrawBeginEvent += onDrawBegin;
                onDrawEndEvent += onDrawEnd;
                m_CacheTimestamp = new Queue<TimeStampInfo>();
            }

            public void onDrawBegin(Int64 nanos)
            {
                onDrawBeginEvent?.Invoke(nanos, 0);
            }

            public void onDrawEnd(Int64 nanos)
            {
                int index = 0;
                if (GetCacheFrameIndexByTime(nanos, ref index))
                {
                    onDrawEndEvent?.Invoke(nanos, index);
                }
                else
                {
                    NRDebugger.Warning("Can not find the frameIndex by time:" + nanos);
                }
            }

            void onPreparePose(long nanos, int index)
            {
                if (m_CacheTimestamp.Count >= MaxCacheCount)
                {
                    m_CacheTimestamp.Dequeue();
                }

                m_CacheTimestamp.Enqueue(new TimeStampInfo()
                {
                    timeStamp = nanos,
                    frameIndex = index
                });
            }

            private bool GetCacheFrameIndexByTime(Int64 timestamp, ref int index)
            {
                while (m_CacheTimestamp.Count != 0)
                {
                    var item = m_CacheTimestamp.Dequeue();
                    if (item.timeStamp == timestamp)
                    {
                        index = item.frameIndex;
                        return true;
                    }
                }
                NRDebugger.Info("[NRDataSourceProvider] Missing cache pose:" + timestamp);
                return false;
            }
        }

        public class AndroidWebEventListenerProxy : AndroidJavaProxy, IWebEventListener
        {
            private event OnWebEvent onImmersiveSessionStartEvent;
            private event OnWebEvent onImmersiveSessionEndEvent;
            public AndroidWebEventListenerProxy(OnWebEvent onImmersiveStart, OnWebEvent onImmersiveEnd) : base("ai.nreal.webframe.IWebEventListener")
            {
                onImmersiveSessionStartEvent += onImmersiveStart;
                onImmersiveSessionEndEvent += onImmersiveEnd;
            }

            public void onImmersiveSessionStart()
            {
                onImmersiveSessionStartEvent?.Invoke();
            }

            public void onImmersiveSessionEnd()
            {
                onImmersiveSessionEndEvent?.Invoke();
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

        AndroidJavaObject m_NRWebView;
        AndroidJavaObject NRWebView
        {
            get
            {
                if (m_NRWebView == null)
                {
                    m_NRWebView = new AndroidJavaObject("ai.nreal.webframe.NRWebView", UnityActivity);
                }
                return m_NRWebView;
            }
        }

        public void Initialize(IntPtr surface, IPoseProvider poseProvider, OnWebDrawEvent onDrawBegin,
            OnWebDrawEvent onDrawEnd, OnWebEvent onImmersiveStart, OnWebEvent onImmersiveEnd)
        {
            try
            {
                AndroidJNI.AttachCurrentThread();
                var datasource = new AndroidPoseProviderProxy(poseProvider);
                var listener = new AndroidDrawListenerProxy(onDrawBegin, onDrawEnd);
                var webevent = new AndroidWebEventListenerProxy(onImmersiveStart, onImmersiveEnd);

                SetSurface(surface);
                NRWebView.Call("setDataSource", datasource);
                NRWebView.Call("setListeners", listener, webevent);
                NRWebView.Call<bool>("show");
            }
            catch (Exception e)
            {
                NRDebugger.Error("[AndroidWebViewPlayer] Init error:" + e.ToString());
                throw;
            }
        }

        public void SetSurface(IntPtr surface, bool useoverlay = false)
        {
            AndroidJNI.AttachCurrentThread();
            string funcName = useoverlay ? "setOverlaySurface" : "setDefaultSurface";
            var methodID = AndroidJNI.GetMethodID(NRWebView.GetRawClass(), funcName, "(Landroid/view/Surface;)V");
            jvalue[] jniArgs = new jvalue[1];
            jniArgs[0].l = surface;
            AndroidJNI.CallVoidMethod(NRWebView.GetRawObject(), methodID, jniArgs);
        }

        public void LoadURL(string url)
        {
            try
            {
                NRWebView.Call("loadUrl", url);
            }
            catch (Exception e)
            {
                NRDebugger.Error("[AndroidWebViewPlayer] LoadURL error:" + e.ToString());
                throw;
            }
        }

        public void UpdateTouchState(Vector2 point, Int64 timestamp, bool istouching)
        {
            // Must use Int64 but not UInt64.
            NRWebView.Call("TouchEvent", point.x, point.y, timestamp, istouching ? 1 : 0);
        }

        public void Close()
        {
            try
            {
                NRWebView.Call("close");
            }
            catch (Exception e)
            {
                NRDebugger.Error("[AndroidWebViewPlayer] Close error:" + e.ToString());
                throw;
            }
        }
    }
}