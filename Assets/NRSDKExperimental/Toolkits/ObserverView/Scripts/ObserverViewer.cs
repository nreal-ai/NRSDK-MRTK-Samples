/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.StreammingCast
{
    using UnityEngine;
    using NRKernal.Record;
    using System.Linq;
    using NRKernal.Experimental.Persistence;
    using NRKernal.Experimental.NetWork;

    [RequireComponent(typeof(ImageTrackingAnchorTool))]
    public class ObserverViewer : MonoBehaviour
    {
        /// <summary> The pose alignment tool. </summary>
        public ImageTrackingAnchorTool PoseAlignmentTool;
        /// <summary> Type of the mask. </summary>
        public NRPointerRaycaster.MaskTypeEnum maskType = NRPointerRaycaster.MaskTypeEnum.Exclusive;
        /// <summary> The mask. </summary>
        public LayerMask mask;

        /// <summary> The observer capture. </summary>
        NRObserverViewCapture m_ObserverCapture = null;
        /// <summary> The configuration view. </summary>
        ObserverConfigView m_ConfigView;
        /// <summary> The current configuration. </summary>
        ObserverViewConfig m_CurrentConfig;

        public enum ViewState
        {
            UnInitialized,
            Initialized,
            Playing,
            Stopped,
            Error
        }
        private ViewState currentViewState = ViewState.UnInitialized;

        /// <summary> Starts this object. </summary>
        void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            CreateVideoCapture();

            m_ConfigView = FindObjectOfType<ObserverConfigView>();
            m_ConfigView.OnConfigrationChanged += OnConfigrationChanged;
            PoseAlignmentTool.OnAnchorPoseUpdate += UpdateObserverViewPose;
        }

        /// <summary> Executes the 'configration changed' action. </summary>
        /// <param name="config"> The configuration.</param>
        private void OnConfigrationChanged(ObserverViewConfig config)
        {
            NRDebugger.Info("[ObserverViewer] OnConfigrationChanged : " + config.ToString());
            this.m_CurrentConfig.serverIP = config.serverIP;
            this.m_CurrentConfig.useDebugUI = config.useDebugUI;
            this.m_CurrentConfig.offset = config.offset;
            if (currentViewState == ViewState.Initialized)
            {
                LocalServerSearcher.Instance.Search((serverinfo) =>
                {
                    if (serverinfo.isSuccess)
                    {
                        NRDebugger.Info("[ObserverViewer] Find observerview server success,ipaddress:" + serverinfo.endPoint.ToString());
                        m_CurrentConfig.serverIP = serverinfo.endPoint.Address.ToString();
                        StartVideoCapture();
                    }
                    else
                    {
                        NRDebugger.Warning("[ObserverViewer] Can not find observerview server.");
                    }
                });

            }
            else if (currentViewState == ViewState.Playing)
            {
                m_ObserverCapture.GetContext()?.GetBehaviour()?.SwitchDebugPanel(config.useDebugUI);
            }
        }

        /// <summary> Tests create video capture. </summary>
        void CreateVideoCapture()
        {
            NRObserverViewCapture.CreateAsync(false, delegate (NRObserverViewCapture videoCapture)
            {
                if (videoCapture != null)
                {
                    m_ObserverCapture = videoCapture;
                }
                else
                {
                    NRDebugger.Error("[ObserverViewer] Failed to create VideoCapture Instance!");
                }

                currentViewState = ViewState.Initialized;
            });
        }

        /// <summary> Updates the observer view pose described by pose. </summary>
        /// <param name="pose"> The pose.</param>
        void UpdateObserverViewPose(Pose pose)
        {
            if (m_ObserverCapture != null)
            {
                var newposition = pose.position + m_CurrentConfig.offset;
                var newpose = new Pose(newposition, pose.rotation);
                m_ObserverCapture.GetContext().GetBehaviour().UpdatePose(newpose);
            }
        }

        /// <summary> Starts video capture. </summary>
        public void StartVideoCapture()
        {
            if (currentViewState == ViewState.UnInitialized || currentViewState == ViewState.Playing)
            {
                NRDebugger.Warning("[ObserverViewer] StartVideoCapture faild : Current view state is in illegal status.");
                return;
            }
            Resolution cameraResolution = NRObserverViewCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).Last();
            NRDebugger.Info("[ObserverViewer] Get CameraResolution:" + cameraResolution);

            int cameraFramerate = NRObserverViewCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();
            NRDebugger.Info("[ObserverViewer] Get CameraFramerate:" + cameraFramerate);

            if (m_ObserverCapture != null)
            {
                NRDebugger.Info("[ObserverViewer] Created VideoCapture Instance!");
                CameraParameters cameraParameters = new CameraParameters();
                cameraParameters.hologramOpacity = 0.0f;
                cameraParameters.frameRate = cameraFramerate;
                cameraParameters.cameraResolutionWidth = cameraResolution.width;
                cameraParameters.cameraResolutionHeight = cameraResolution.height;
                cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

                m_ObserverCapture.StartObserverViewModeAsync(cameraParameters,
                    NRObserverViewCapture.AudioState.ApplicationAndMicAudio,
                    OnStartedVideoCaptureMode, true);
            }
        }

        /// <summary> Executes the 'started video capture mode' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStartedVideoCaptureMode(NRObserverViewCapture.ObserverViewCaptureResult result)
        {
            NRDebugger.Info("[ObserverViewer] Started Video Capture Mode!");
            if (result.success)
            {
                m_ObserverCapture.StartObserverViewAsync(this.m_CurrentConfig.serverIP, OnStartedRecordingVideo);
            }
            else
            {
                currentViewState = ViewState.Error;
                NRDebugger.Info("[ObserverViewer] Started Video Capture Mode faild！");
            }
        }

        /// <summary> Executes the 'started recording video' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStartedRecordingVideo(NRObserverViewCapture.ObserverViewCaptureResult result)
        {
            NRDebugger.Info("[ObserverViewer] Started Recording Video!");
            if (result.success)
            {
                m_ObserverCapture.GetContext().GetBehaviour().SwitchDebugPanel(m_CurrentConfig.useDebugUI);
                // Set the camera culling musk
                m_ObserverCapture.GetContext().GetBehaviour().SetCullingMask(maskType == NRPointerRaycaster.MaskTypeEnum.Inclusive ? (int)mask : ~mask);
                currentViewState = ViewState.Playing;
            }
            else
            {
                currentViewState = ViewState.Error;
                NRDebugger.Info("[ObserverViewer] Started Video Capture Mode faild！");
            }
        }

        /// <summary> Stops video capture. </summary>
        public void StopVideoCapture()
        {
            if (currentViewState != ViewState.Playing)
            {
                NRDebugger.Warning("[ObserverViewer] StopVideoCapture faild : Current view state is in illegal status.");
                return;
            }
            NRDebugger.Info("[ObserverViewer] Stop Video Capture!");
            m_ObserverCapture.StopObserverViewAsync(OnStoppedRecordingVideo);
        }

        /// <summary> Executes the 'stopped recording video' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedRecordingVideo(NRObserverViewCapture.ObserverViewCaptureResult result)
        {
            NRDebugger.Info("[ObserverViewer] Stopped Recording Video!");
            if (result.success)
            {
                m_ObserverCapture.StopObserverViewModeAsync(OnStoppedVideoCaptureMode);
            }
            else
            {
                currentViewState = ViewState.Error;
                NRDebugger.Info("[ObserverViewer] Stopped Video Capture faild！");
            }
        }

        /// <summary> Executes the 'stopped video capture mode' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedVideoCaptureMode(NRObserverViewCapture.ObserverViewCaptureResult result)
        {
            NRDebugger.Info("[ObserverViewer] Stopped Video Capture Mode!");
            if (result.success)
            {
                currentViewState = ViewState.Stopped;
            }
            else
            {
                currentViewState = ViewState.Error;
                NRDebugger.Info("[ObserverViewer] Stopped Video Capture Mode faild！");
            }
        }
    }
}
