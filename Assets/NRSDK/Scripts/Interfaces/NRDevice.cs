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
    using AOT;
    using System;
    using System.Collections.Generic;
#if UNITY_ANDROID && !UNITY_EDITOR
    using System.Runtime.InteropServices;
    using System.Threading;
    using UnityEngine;
#endif

    /// <summary> Manage the HMD device. </summary>
    public class NRDevice : SingleTon<NRDevice>
    {
        /// <summary> Event queue for all listeners interested in OnGlassesStateChanged events. </summary>
        public static event GlassesEvent OnGlassesStateChanged;
        /// <summary> Event queue for all listeners interested in OnGlassesDisconnect events. </summary>
        public static event GlassesDisconnectEvent OnGlassesDisconnect;

        /// <summary> Values that represent glasses event types. </summary>
        public enum GlassesEventType
        {
            /// <summary> An enum constant representing the put on option. </summary>
            PutOn,
            /// <summary> An enum constant representing the put off option. </summary>
            PutOff
        }

        private const float SDK_RELEASE_TIMEOUT = 2f;

        private bool m_IsInitialized = false;
        private Exception m_InitException = null;

        private static NRDeviceSubsystem m_Subsystem;
        public static NRDeviceSubsystem Subsystem
        {
            get
            {
                if (m_Subsystem == null)
                {
                    string str_match = NRDeviceSubsystemDescriptor.Name;
                    List<NRDeviceSubsystemDescriptor> descriptors = new List<NRDeviceSubsystemDescriptor>();
                    NRSubsystemManager.GetSubsystemDescriptors(descriptors);
                    foreach (var des in descriptors)
                    {
                        if (des.id.Equals(str_match))
                        {
                            m_Subsystem = des.Create();
                        }
                    }
                }

                return m_Subsystem;
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject m_UnityActivity;
#endif

        /// <summary> Init HMD device. </summary>
        public void Init()
        {
            // Keep the exception state.
            if (m_InitException != null)
            {
                throw m_InitException;
            }

            if (m_IsInitialized)
            {
                return;
            }

            NRTools.Init();
            MainThreadDispather.Initialize();
#if UNITY_ANDROID && !UNITY_EDITOR
            // Init before all actions.
            AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            m_UnityActivity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            NativeApi.NRSDKInitSetAndroidActivity(m_UnityActivity.GetRawObject()); 
#endif
            try
            {
                Subsystem.Start();
                Subsystem.RegestEvents(OnGlassesWear, OnGlassesDisconnectEvent);
            }
            catch (Exception e)
            {
                m_InitException = e;
                throw e;
            }

            m_IsInitialized = true;
        }

        public void Pause()
        {
            Subsystem.Pause();
        }

        public void Resume()
        {
            Subsystem.Resume();
        }

        public void Destroy()
        {
            Subsystem.Stop();
        }

        private void OnGlassesWear(NRDevice.GlassesEventType eventtype)
        {
            NRDebugger.Info("[NRDevice] " + (eventtype == GlassesEventType.PutOn ? "Glasses put on" : "Glasses put off"));
            OnGlassesStateChanged?.Invoke(eventtype);
            NRSessionManager.OnGlassesStateChanged?.Invoke(eventtype);
        }

        private void OnGlassesDisconnectEvent(GlassesDisconnectReason reason)
        {
            NRDebugger.Info("[NRDevice] OnGlassesDisconnectEvent:" + reason.ToString());
            // If NRSDK release time out in 2 seconds, FoceKill the process.
            AsyncTaskExecuter.Instance.RunAction(() =>
            {
                try
                {
                    OnGlassesDisconnect?.Invoke(reason);
                    NRSessionManager.OnGlassesDisconnect?.Invoke(reason);
                }
                catch (Exception e)
                {
                    NRDebugger.Info("[NRDevice] Operate OnGlassesDisconnect event error:" + e.ToString());
                    throw e;
                }
                finally
                {
                    ForceKill(true);
                }
            }, () =>
            {
                NRDebugger.Error("[NRDevice] Release sdk timeout, force kill the process!!!");
                ForceKill(false);
            }, SDK_RELEASE_TIMEOUT);
        }

        #region Quit
        /// <summary> Quit the app. </summary>
        public static void QuitApp()
        {
            NRDebugger.Info("[NRDevice] Start To Quit Application.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            ForceKill();
#endif
        }

        /// <summary> Force kill the app. Avoid timeout to pause UnityEngine. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        public static void ForceKill(bool needrelease = true)
        {
            try
            {
                ReleaseSDK();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                AndroidJNI.AttachCurrentThread();
                m_UnityActivity?.Call("finish");

                AndroidJavaClass processClass = new AndroidJavaClass("android.os.Process");
                int myPid = processClass.CallStatic<int>("myPid");
                processClass.CallStatic("killProcess", myPid);
#endif
            }
        }

        public static void ReleaseSDK()
        {
            NRDebugger.Info("[NRDevice] Start to release sdk");
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            NRSessionManager.Instance.DestroySession();
            NRDebugger.Info("[NRDevice] Release sdk end, cost:{0} ms", stopwatch.ElapsedMilliseconds);
        }
        #endregion

#if UNITY_ANDROID && !UNITY_EDITOR
        private struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSDKInitSetAndroidActivity(IntPtr android_activity);
        }
#endif
    }
}
