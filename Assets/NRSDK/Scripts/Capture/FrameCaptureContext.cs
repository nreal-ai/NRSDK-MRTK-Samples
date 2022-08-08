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
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> A frame capture context. </summary>
    public class FrameCaptureContext
    {
        /// <summary> The blender. </summary>
        private BlenderBase m_Blender;
        /// <summary> The encoder. </summary>
        private IEncoder m_Encoder;
        /// <summary> Options for controlling the camera. </summary>
        private CameraParameters m_CameraParameters;
        /// <summary> The frame provider. </summary>
        private AbstractFrameProvider m_FrameProvider;
        /// <summary> The capture behaviour. </summary>
        private CaptureBehaviourBase m_CaptureBehaviour;
        /// <summary> True if is initialize, false if not. </summary>
        private bool m_IsInitialized = false;

        private List<IFrameConsumer> m_FrameConsumerList;

        /// <summary> Gets the preview texture. </summary>
        /// <value> The preview texture. </value>
        public Texture PreviewTexture
        {
            get
            {
                return m_Blender?.BlendTexture;
            }
        }

        /// <summary> Gets the behaviour. </summary>
        /// <returns> The behaviour. </returns>
        public CaptureBehaviourBase GetBehaviour()
        {
            return m_CaptureBehaviour;
        }

        /// <summary> Gets frame provider. </summary>
        /// <returns> The frame provider. </returns>
        public AbstractFrameProvider GetFrameProvider()
        {
            return m_FrameProvider;
        }

        /// <summary> Gets the blender. </summary>
        /// <returns> The blender. </returns>
        public BlenderBase GetBlender()
        {
            return m_Blender;
        }

        /// <summary> Request camera parameter. </summary>
        /// <returns> The CameraParameters. </returns>
        public CameraParameters RequestCameraParam()
        {
            return m_CameraParameters;
        }

        /// <summary> Gets the encoder. </summary>
        /// <returns> The encoder. </returns>
        public IEncoder GetEncoder()
        {
            return m_Encoder;
        }

        /// <summary> Constructor. </summary>
        public FrameCaptureContext() { }

        /// <summary> Starts capture mode. </summary>
        /// <param name="param"> The parameter.</param>
        public void StartCaptureMode(CameraParameters param)
        {
            if (m_IsInitialized)
            {
                this.Release();
                NRDebugger.Warning("[CaptureContext] Capture context has been started already, release it and restart a new one.");
            }

            NRDebugger.Info("[CaptureContext] Create...");
            if (m_CaptureBehaviour == null)
            {
                this.m_CaptureBehaviour = this.GetCaptureBehaviourByMode(param.camMode);
            }

            this.m_CameraParameters = param;
            this.m_Encoder = GetEncoderByMode(param.camMode);
            this.m_Encoder.Config(param);
            this.m_Blender = new ExtraFrameBlender();
            this.m_Blender.Init(m_CaptureBehaviour.CaptureCamera, m_Encoder, param);
            this.m_CaptureBehaviour.Init(this);

            this.m_FrameProvider = CreateFrameProviderByMode(param.blendMode, param.frameRate);
            this.m_FrameProvider.OnUpdate += UpdateFrame;

            this.m_FrameConsumerList = new List<IFrameConsumer>();
            this.Sequence(m_CaptureBehaviour)
                .Sequence(m_Blender);

            this.m_IsInitialized = true;
        }

        /// <summary> Auto adaption for BlendMode based on supported feature on current device. </summary>
        /// <param name="blendMode"> source blendMode.</param>
        /// <returns> Fallback blendMode. </returns>
        public BlendMode AutoAdaptBlendMode(BlendMode blendMode)
        {
            if (!NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_RGB_CAMERA))
                return BlendMode.VirtualOnly;
                
            return blendMode;
        }

        private FrameCaptureContext Sequence(IFrameConsumer consummer)
        {
            this.m_FrameConsumerList.Add(consummer);
            return this;
        }

        private void UpdateFrame(UniversalTextureFrame frame)
        {
            for (int i = 0; i < m_FrameConsumerList.Count; i++)
            {
                m_FrameConsumerList[i].OnFrame(frame);
            }
        }

        /// <summary> Gets capture behaviour by mode. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        /// <param name="mode"> The mode.</param>
        /// <returns> The capture behaviour by mode. </returns>
        private CaptureBehaviourBase GetCaptureBehaviourByMode(CamMode mode)
        {
            if (mode == CamMode.PhotoMode)
            {
                NRCaptureBehaviour capture = GameObject.FindObjectOfType<NRCaptureBehaviour>();
                var headParent = NRSessionManager.Instance.NRSessionBehaviour.transform.parent;
                if (capture == null)
                {
                    capture = GameObject.Instantiate(Resources.Load<NRCaptureBehaviour>("Record/Prefabs/NRCaptureBehaviour"), headParent);
                }
                GameObject.DontDestroyOnLoad(capture.gameObject);
                return capture;
            }
            else if (mode == CamMode.VideoMode)
            {
                NRRecordBehaviour capture = GameObject.FindObjectOfType<NRRecordBehaviour>();
                var headParent = NRSessionManager.Instance.NRSessionBehaviour.transform.parent;
                if (capture == null)
                {
                    capture = GameObject.Instantiate(Resources.Load<NRRecordBehaviour>("Record/Prefabs/NRRecorderBehaviour"), headParent);
                }
                GameObject.DontDestroyOnLoad(capture.gameObject);
                return capture;
            }
            else
            {
                throw new Exception("CamMode need to be set correctly for capture behaviour!");
            }
        }

        private AbstractFrameProvider CreateFrameProviderByMode(BlendMode mode, int fps)
        {
            AbstractFrameProvider provider;
            switch (mode)
            {
                case BlendMode.Blend:
                case BlendMode.RGBOnly:
#if UNITY_EDITOR
                    provider = new EditorFrameProvider();
#else
                    provider = new RGBCameraFrameProvider();
#endif
                    break;
                case BlendMode.VirtualOnly:
                default:
                    provider = new NullDataFrameProvider(fps);
                    break;
            }

            return provider;
        }

        /// <summary> Gets encoder by mode. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        /// <param name="mode"> The mode.</param>
        /// <returns> The encoder by mode. </returns>
        private IEncoder GetEncoderByMode(CamMode mode)
        {
            if (mode == CamMode.PhotoMode)
            {
                return new ImageEncoder();
            }
            else if (mode == CamMode.VideoMode)
            {
                return new VideoEncoder();
            }
            else
            {
                throw new Exception("CamMode need to be set correctly for encoder!");
            }
        }

        /// <summary> Stops capture mode. </summary>
        public void StopCaptureMode()
        {
            this.Release();
        }

        /// <summary> Starts a capture. </summary>
        public void StartCapture()
        {
            if (!m_IsInitialized)
            {
                return;
            }
            NRDebugger.Info("[CaptureContext] Start...");

            m_Encoder?.Start();
            m_FrameProvider?.Play();
        }

        /// <summary> Stops a capture. </summary>
        public void StopCapture()
        {
            if (!m_IsInitialized)
            {
                return;
            }
            NRDebugger.Info("[CaptureContext] Stop...");

            // Need stop encoder firstly.
            m_Encoder?.Stop();
            m_FrameProvider?.Stop();
        }

        /// <summary> Releases this object. </summary>
        public void Release()
        {
            if (!m_IsInitialized)
            {
                return;
            }

            NRDebugger.Info("[CaptureContext] Release begin...");
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            if (m_FrameProvider != null)
            {
                m_FrameProvider.OnUpdate -= UpdateFrame;
                m_FrameProvider?.Release();
                m_FrameProvider = null;
            }

            m_Blender?.Dispose();
            m_Encoder?.Release();

            if (m_CaptureBehaviour != null)
            {
                GameObject.DestroyImmediate(m_CaptureBehaviour.gameObject);
                m_CaptureBehaviour = null;
            }
            NRDebugger.Info("[CaptureContext] Release end, cost:{0} ms", stopwatch.ElapsedMilliseconds);

            m_IsInitialized = false;
        }
    }
}
