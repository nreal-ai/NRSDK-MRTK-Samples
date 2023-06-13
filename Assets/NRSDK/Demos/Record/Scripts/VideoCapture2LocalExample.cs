/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using NRKernal.Record;
using System;
using System.IO;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
#if UNITY_ANDROID && !UNITY_EDITOR
    using GalleryDataProvider = NativeGalleryDataProvider;
#else
    using GalleryDataProvider = MockGalleryDataProvider;
#endif
    /// <summary> A video capture 2 local example. </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/video-capture")]
    public class VideoCapture2LocalExample : MonoBehaviour
    {
        [SerializeField] private Button m_PlayButton;
        [SerializeField] private NRPreviewer m_Previewer;
        [SerializeField] private Slider m_SliderMic;
        [SerializeField] private Text m_TextMic;
        [SerializeField] private Slider m_SliderApp;
        [SerializeField] private Text m_TextApp;

        public BlendMode blendMode = BlendMode.Blend;
        public ResolutionLevel resolutionLevel;
        public LayerMask cullingMask = -1;
        public NRVideoCapture.AudioState audioState = NRVideoCapture.AudioState.ApplicationAudio;
        public bool useGreenBackGround = false;

        public enum ResolutionLevel
        {
            High,
            Middle,
            Low,
        }

        /// <summary> Save the video to Application.persistentDataPath. </summary>
        /// <value> The full pathname of the video save file. </value>
        public string VideoSavePath
        {
            get
            {
                string timeStamp = Time.time.ToString().Replace(".", "").Replace(":", "");
                string filename = string.Format("Nreal_Record_{0}.mp4", timeStamp);
                return Path.Combine(Application.persistentDataPath, filename);
            }
        }

        GalleryDataProvider galleryDataTool;

        void Awake()
        {
            if (m_SliderMic != null)
            {
                m_SliderMic.maxValue = 10.0f;
                m_SliderMic.minValue = 0.1f;
                m_SliderMic.value = 1;
                m_SliderMic.onValueChanged.AddListener(OnSlideMicValueChange);
            }

            if (m_SliderApp != null)
            {
                m_SliderApp.maxValue = 10.0f;
                m_SliderApp.minValue = 0.1f;
                m_SliderApp.value = 1;
                m_SliderApp.onValueChanged.AddListener(OnSlideAppValueChange);
            }

            RefreshUIState();
        }

        void OnSlideMicValueChange(float val)
        {
            if (m_VideoCapture != null)
            {
                VideoEncoder encoder = m_VideoCapture.GetContext().GetEncoder() as VideoEncoder;
                if (encoder != null)
                    encoder.AdjustVolume(RecorderIndex.REC_MIC, val);
            }
            RefreshUIState();
        }

        void OnSlideAppValueChange(float val)
        {
            if (m_VideoCapture != null)
            {
                VideoEncoder encoder = m_VideoCapture.GetContext().GetEncoder() as VideoEncoder;
                if (encoder != null)
                    encoder.AdjustVolume(RecorderIndex.REC_APP, val);
            }
            RefreshUIState();
        }

        /// <summary> The video capture. </summary>
        NRVideoCapture m_VideoCapture = null;
        void CreateVideoCapture(Action callback)
        {
            NRVideoCapture.CreateAsync(false, delegate (NRVideoCapture videoCapture)
            {
                NRDebugger.Info("Created VideoCapture Instance!");
                if (videoCapture != null)
                {
                    m_VideoCapture = videoCapture;
                    callback?.Invoke();
                }
                else
                {
                    NRDebugger.Error("Failed to create VideoCapture Instance!");
                }
            });
        }

        public void OnClickPlayButton()
        {
            if (m_VideoCapture == null)
            {
                CreateVideoCapture(() =>
                {
                    StartVideoCapture();
                });
            }
            else if (m_VideoCapture.IsRecording)
            {
                this.StopVideoCapture();
            }
            else
            {
                this.StartVideoCapture();
            }
        }

        void RefreshUIState()
        {
            bool flag = m_VideoCapture == null || !m_VideoCapture.IsRecording;
            m_PlayButton.GetComponent<Image>().color = flag ? Color.red : Color.green;

            if (m_TextMic != null && m_SliderMic != null)
                m_TextMic.text = m_SliderMic.value.ToString();
            if (m_TextApp != null && m_SliderApp != null)
                m_TextApp.text = m_SliderApp.value.ToString();
        }

        /// <summary> Starts video capture. </summary>
        public void StartVideoCapture()
        {
            if (m_VideoCapture == null || m_VideoCapture.IsRecording)
            {
                NRDebugger.Warning("Can not start video capture!");
                return;
            }

            CameraParameters cameraParameters = new CameraParameters();
            Resolution cameraResolution = GetResolutionByLevel(resolutionLevel);
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.frameRate = cameraResolution.refreshRate;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
            // Set the blend mode.
            cameraParameters.blendMode = blendMode;
            // Set audio state, audio record needs the permission of "android.permission.RECORD_AUDIO",
            // Add it to your "AndroidManifest.xml" file in "Assets/Plugin".
            cameraParameters.audioState = audioState;

            m_VideoCapture.StartVideoModeAsync(cameraParameters, OnStartedVideoCaptureMode, true);
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

        /// <summary> Stops video capture. </summary>
        public void StopVideoCapture()
        {
            if (m_VideoCapture == null || !m_VideoCapture.IsRecording)
            {
                NRDebugger.Warning("Can not stop video capture!");
                return;
            }

            NRDebugger.Info("Stop Video Capture!");
            m_VideoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
            m_Previewer.SetData(null, false);
        }

        /// <summary> Executes the 'started video capture mode' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStartedVideoCaptureMode(NRVideoCapture.VideoCaptureResult result)
        {
            if (!result.success)
            {
                NRDebugger.Info("Started Video Capture Mode faild!");
                return;
            }

            NRDebugger.Info("Started Video Capture Mode!");
            float volumeMic = m_SliderMic != null ? m_SliderMic.value : NativeConstants.RECORD_VOLUME_MIC;
            float volumeApp = m_SliderApp != null ? m_SliderApp.value : NativeConstants.RECORD_VOLUME_APP;
            m_VideoCapture.StartRecordingAsync(VideoSavePath, OnStartedRecordingVideo, volumeMic, volumeApp);
            // Set preview texture.
            m_Previewer.SetData(m_VideoCapture.PreviewTexture, true);
        }

        /// <summary> Executes the 'started recording video' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStartedRecordingVideo(NRVideoCapture.VideoCaptureResult result)
        {
            if (!result.success)
            {
                NRDebugger.Info("Started Recording Video Faild!");
                return;
            }

            NRDebugger.Info("Started Recording Video!");
            if (useGreenBackGround)
            {
                // Set green background color.
                m_VideoCapture.GetContext().GetBehaviour().SetBackGroundColor(Color.green);
            }
            m_VideoCapture.GetContext().GetBehaviour().SetCameraMask(cullingMask.value);

            RefreshUIState();
        }

        /// <summary> Executes the 'stopped recording video' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedRecordingVideo(NRVideoCapture.VideoCaptureResult result)
        {
            if (!result.success)
            {
                NRDebugger.Info("Stopped Recording Video Faild!");
                return;
            }

            NRDebugger.Info("Stopped Recording Video!");
            m_VideoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
        }

        /// <summary> Executes the 'stopped video capture mode' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedVideoCaptureMode(NRVideoCapture.VideoCaptureResult result)
        {
            NRDebugger.Info("Stopped Video Capture Mode!");
            RefreshUIState();

            var encoder = m_VideoCapture.GetContext().GetEncoder() as VideoEncoder;
            string path = encoder.EncodeConfig.outPutPath;
            string filename = string.Format("Nreal_Shot_Video_{0}.mp4", NRTools.GetTimeStamp().ToString());
            
            StartCoroutine(DelayInsertVideoToGallery(path, filename, "Record"));

            // Release video capture resource.
            m_VideoCapture.Dispose();
            m_VideoCapture = null;
        }

        void OnDestroy()
        {
            // Release video capture resource.
            m_VideoCapture?.Dispose();
            m_VideoCapture = null;
        }

        IEnumerator DelayInsertVideoToGallery(string originFilePath, string displayName, string folderName)
        {
            yield return new WaitForSeconds(0.1f);
            InsertVideoToGallery(originFilePath, displayName, folderName);
        }

        public void InsertVideoToGallery(string originFilePath, string displayName, string folderName)
        {
            NRDebugger.Info("InsertVideoToGallery: {0}, {1} => {2}", displayName, originFilePath, folderName);
            if (galleryDataTool == null)
            {
                galleryDataTool = new GalleryDataProvider();
            }

            galleryDataTool.InsertVideo(originFilePath, displayName, folderName);
        }
    }
}
