/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using AOT;
    using System.Runtime.InteropServices;

    /// <summary> A video encoder. </summary>
    public class AudioEncoder : IEncoderBase
    {
        private NativeEncoder mNativeEncoder;
        public NativeEncodeConfig EncodeConfig;
        private IntPtr androidMediaProjection { get; set; }
                
        private bool m_IsStarted = false;
        private AudioDataCallBack mDataCallBack = null;
        private static List<AudioEncoder> gInactiveAudioEncoders = new List<AudioEncoder>();

#if !UNITY_EDITOR
        private const int STARTENCODEEVENT = 0x2001;
        private const int STOPENCODEEVENT = 0x2002;
        private delegate void RenderEventDelegate(int eventID);
        private static RenderEventDelegate RenderThreadHandle = new RenderEventDelegate(RunOnRenderThread);
        private static IntPtr RenderThreadHandlePtr = Marshal.GetFunctionPointerForDelegate(RenderThreadHandle);
#endif

        /// <summary> Default constructor. </summary>
        public AudioEncoder()
        {
#if !UNITY_EDITOR
            mNativeEncoder = NativeEncoder.GetInstance();
            mNativeEncoder.Register(this);
#endif
        }

#if !UNITY_EDITOR
        [MonoPInvokeCallback(typeof(RenderEventDelegate))]
        private static void RunOnRenderThread(int eventID)
        {
            if (eventID == STARTENCODEEVENT)
            {
                lock (gInactiveAudioEncoders)
                {
                    for (int i = 0; i < gInactiveAudioEncoders.Count; i++)
                    {
                        NativeEncoder.GetInstance().StartAudioRecorder(gInactiveAudioEncoders[i].mDataCallBack);
                    }
                    gInactiveAudioEncoders.Clear();
                }
                
            }
        }
#endif

        /// <summary> Configurations the given parameter. </summary>
        /// <param name="param"> The parameter.</param>
        public void Config(CameraParameters param)
        {
            EncodeConfig = new NativeEncodeConfig(param);
            androidMediaProjection = (param.mediaProjection != null) ? param.mediaProjection.GetRawObject() : IntPtr.Zero;
        }

        /// <summary> Adjust the volume of encoder.</summary>
        /// <param name="recordIdx"> Recorder index.</param>
        /// <param name="factor"> The factor of volume.</param>
        public void AdjustVolume(RecorderIndex recordIdx, float factor)
        {
#if !UNITY_EDITOR
            mNativeEncoder.AdjustVolume(recordIdx, factor);
#endif
        }

        /// <summary> Starts this object. </summary>
        public void Start(AudioDataCallBack onAudioDataCallback = null)
        {
            if (m_IsStarted)
            {
                return;
            }
            NRDebugger.Info("[AudioEncoder] Start");
            NRDebugger.Info("[AudioEncoder] Config {0}", EncodeConfig.ToString());
            mDataCallBack = onAudioDataCallback;
            lock (gInactiveAudioEncoders)
            {
                gInactiveAudioEncoders.Add(this);
            }
#if !UNITY_EDITOR
            mNativeEncoder.SetConfigration(EncodeConfig, androidMediaProjection);
            //mNativeEncoder.StartAudioRecorder(mDataCallBack);
            GL.IssuePluginEvent(RenderThreadHandlePtr, STARTENCODEEVENT);
#endif
            m_IsStarted = true;
        }

        /// <summary> Stops this object. </summary>
        public void Stop()
        {
            if (!m_IsStarted)
            {
                return;
            }

            NRDebugger.Info("[AudioEncoder] Stop");
            lock (gInactiveAudioEncoders)
            {
                gInactiveAudioEncoders.Remove(this);
            }
#if !UNITY_EDITOR
            mNativeEncoder.StopAudioRecorder(mDataCallBack);
#endif
            m_IsStarted = false;
        }

        /// <summary> Releases this object. </summary>
        public void Release()
        {
            NRDebugger.Info("[AudioEncoder] Release...");
#if !UNITY_EDITOR
            mNativeEncoder.UnRegister(this);
            mNativeEncoder.Destroy();
#endif
        }
    }
}
