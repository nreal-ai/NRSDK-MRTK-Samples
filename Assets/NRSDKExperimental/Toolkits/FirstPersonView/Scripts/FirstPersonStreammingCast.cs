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
    using NRKernal.Record;
    using System.Linq;
    using UnityEngine;
    using System;
    using NRKernal.Experimental.NetWork;
    using System.IO;
    using System.Collections;

    /// <summary> A first person streamming cast. </summary>
    public class FirstPersonStreammingCast : MonoBehaviour
    {
        public delegate void OnResponse(bool result);

        /// <summary> The FPS configuration view. </summary>
        [SerializeField]
        private FPSConfigView m_FPSConfigView;
        public BlendMode m_BlendMode = BlendMode.Blend;
        public ResolutionLevel m_ResolutionLevel = ResolutionLevel.Middle;
        public LayerMask m_CullingMask = -1;
        public NRVideoCapture.AudioState m_AudioState = NRVideoCapture.AudioState.None;
        public bool useGreenBackGround = false;

        /// <summary> The net worker. </summary>
        private NetWorkBehaviour m_NetWorker;
        /// <summary> The video capture. </summary>
        private NRVideoCapture m_VideoCapture = null;
        public NRVideoCapture VideoCapture
        {
            get
            {
                return m_VideoCapture;
            }
        }

        private string m_ServerIP;
        /// <summary> Gets the full pathname of the rtp file. </summary>
        /// <value> The full pathname of the rtp file. </value>
        public string RTPPath
        {
            get
            {
                return string.Format(@"rtp://{0}:5555", m_ServerIP);
            }
        }

        public string VideoSavePath
        {
            get
            {
                string timeStamp = Time.time.ToString().Replace(".", "").Replace(":", "");
                string filename = string.Format("Nreal_Record_{0}.mp4", timeStamp);
                string filepath = Path.Combine(Application.persistentDataPath, filename);
                return filepath;
            }
        }

        private bool m_IsInitialized = false;
        private bool m_IsStreamLock = false;
        private bool m_IsStreamStarted = false;
        private bool m_IsRecordStarted = false;

        public enum ResolutionLevel
        {
            High,
            Middle,
            Low,
        }

        /// <summary> Starts this object. </summary>
        void Start()
        {
            this.Init();
        }

        /// <summary> Initializes this object. </summary>
        private void Init()
        {
            if (m_IsInitialized)
            {
                return;
            }

            m_FPSConfigView.OnStreamBtnClicked += OnStream;
            m_FPSConfigView.OnRecordBtnClicked += OnRecord;

            m_IsInitialized = true;
        }

        private void OnRecord(OnResponse response)
        {
            if (!m_IsRecordStarted)
            {
                CreateAndStart();
            }
            else
            {
                StopVideoCapture();
            }
            response?.Invoke(true);
            m_IsRecordStarted = !m_IsRecordStarted;
        }

        private void OnStream(OnResponse response)
        {
            if (m_IsStreamLock)
            {
                return;
            }

            m_IsStreamLock = true;
            if (m_NetWorker == null)
            {
                m_NetWorker = new NetWorkBehaviour();
                m_NetWorker.Listen();
            }

            if (!m_IsStreamStarted)
            {
                LocalServerSearcher.Instance.Search((result) =>
                {
                    NRDebugger.Info("[FPStreammingCast] Get the server result:{0} ip:{1}", result.isSuccess, result.endPoint?.Address);
                    if (result.isSuccess)
                    {
                        string ip = result.endPoint.Address.ToString();
                        m_NetWorker.CheckServerAvailable(ip, (isavailable) =>
                        {
                            NRDebugger.Info("[FPStreammingCast] Is the server {0} ok? {1}", ip, result);
                            if (isavailable)
                            {
                                m_ServerIP = ip;
                                m_IsStreamStarted = true;
                                CreateAndStart();
                            }
                            response?.Invoke(isavailable);
                            m_IsStreamLock = false;
                        });
                    }
                    else
                    {
                        m_IsStreamLock = false;
                        response?.Invoke(false);
                        NRDebugger.Error("[FPStreammingCast] Can not find the server...");
                    }
                });
            }
            else
            {
                StopVideoCapture();
                m_IsStreamStarted = false;
                m_IsStreamLock = false;
                response?.Invoke(true);
            }
        }

        /// <summary> Converts this object to a server. </summary>
        private void CreateAndStart()
        {
            CreateVideoCapture(delegate ()
            {
                NRDebugger.Info("[FPStreammingCast] Start video capture.");
                StartCoroutine(StartVideoCapture());
            });
        }

        private Resolution GetResolutionByLevel(ResolutionLevel level)
        {
            var resolutions = NRVideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height);
            Resolution resolution = new Resolution();
            switch (level)
            {
                case ResolutionLevel.High:
                    resolution = resolutions.ElementAt(0);
                    break;
                case ResolutionLevel.Middle:
                    resolution = resolutions.ElementAt(1);
                    break;
                case ResolutionLevel.Low:
                    resolution = resolutions.ElementAt(2);
                    break;
                default:
                    break;
            }
            return resolution;
        }

        #region video capture
        /// <summary> Creates video capture. </summary>
        /// <param name="callback"> The callback.</param>
        private void CreateVideoCapture(Action callback)
        {
            NRDebugger.Info("[FPStreammingCast] Created VideoCapture Instance!");
            if (m_VideoCapture != null)
            {
                callback?.Invoke();
                return;
            }

            NRVideoCapture.CreateAsync(false, delegate (NRVideoCapture videoCapture)
            {
                if (videoCapture != null)
                {
                    m_VideoCapture = videoCapture;

                    callback?.Invoke();
                }
                else
                {
                    NRDebugger.Error("[FPStreammingCast] Failed to create VideoCapture Instance!");
                }
            });
        }

        /// <summary> Starts video capture. </summary>
        public IEnumerator StartVideoCapture()
        {
            Resolution cameraResolution = GetResolutionByLevel(m_ResolutionLevel);
            NRDebugger.Info("[FPStreammingCast] cameraResolution:" + cameraResolution);

            if (m_VideoCapture != null)
            {
                CameraParameters cameraParameters = new CameraParameters();
                cameraParameters.hologramOpacity = 1f;
                cameraParameters.frameRate = cameraResolution.refreshRate;
                cameraParameters.cameraResolutionWidth = cameraResolution.width;
                cameraParameters.cameraResolutionHeight = cameraResolution.height;
                cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
                cameraParameters.blendMode = m_BlendMode;
                cameraParameters.audioState = m_AudioState;

                // Send the audioState to server.
                if (m_NetWorker != null)
                {
                    LitJson.JsonData json = new LitJson.JsonData();
                    json["useAudio"] = (cameraParameters.audioState != NRVideoCapture.AudioState.None);
                    m_NetWorker.SendMsg(json, (response) =>
                    {
                        bool result;
                        if (bool.TryParse(response["success"].ToString(), out result) && result)
                        {
                            m_VideoCapture.StartVideoModeAsync(cameraParameters, OnStartedVideoCaptureMode, true);
                        }
                        else
                        {
                            NRDebugger.Error("[FPStreammingCast] Can not received response from server.");
                        }
                    });
                }
                else
                {
                    m_VideoCapture.StartVideoModeAsync(cameraParameters, OnStartedVideoCaptureMode, true);
                }
            }
            else
            {
                NRDebugger.Info("[FPStreammingCast]  VideoCapture object is null...");
            }
            yield return new WaitForEndOfFrame();
        }

        /// <summary> Stops video capture. </summary>
        public void StopVideoCapture()
        {
            NRDebugger.Info("[FPStreammingCast] Stop Video Capture!");
            m_VideoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
        }

        /// <summary> Executes the 'started video capture mode' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStartedVideoCaptureMode(NRVideoCapture.VideoCaptureResult result)
        {
            if (!result.success)
            {
                NRDebugger.Info("[FPStreammingCast] Started Video Capture Mode Faild!");
                return;
            }

            NRDebugger.Info("[FPStreammingCast] Started Video Capture Mode!");
            m_VideoCapture.StartRecordingAsync(m_IsStreamStarted ? RTPPath : VideoSavePath, OnStartedRecordingVideo);
            m_VideoCapture.GetContext().GetBehaviour().CaptureCamera.cullingMask = m_CullingMask.value;
            m_VideoCapture.GetContext().GetBehaviour().CaptureCamera.backgroundColor = useGreenBackGround ? Color.green : Color.black;
        }

        /// <summary> Executes the 'stopped video capture mode' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedVideoCaptureMode(NRVideoCapture.VideoCaptureResult result)
        {
            NRDebugger.Info("[FPStreammingCast] Stopped Video Capture Mode!");
            m_VideoCapture = null;
        }

        /// <summary> Executes the 'started recording video' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStartedRecordingVideo(NRVideoCapture.VideoCaptureResult result)
        {
            NRDebugger.Info("[FPStreammingCast] Started Recording Video!");
        }

        /// <summary> Executes the 'stopped recording video' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedRecordingVideo(NRVideoCapture.VideoCaptureResult result)
        {
            NRDebugger.Info("[FPStreammingCast] Stopped Recording Video!");
            m_VideoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
            if (m_NetWorker != null)
            {
                m_NetWorker?.Close();
                m_NetWorker = null;
            }
        }
        #endregion
    }
}
