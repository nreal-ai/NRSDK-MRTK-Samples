using NRKernal.Record;
using System.Linq;
using UnityEngine;

namespace NRKernal.Experimental.NRExamples
{
    /// <summary> A video capture 2 rtp example. </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/video-capture")]
    public class VideoCapture2RTPExample : MonoBehaviour
    {
        /// <summary> The previewer. </summary>
        public NRPreviewer Previewer;

        /// <summary> Gets the full pathname of the rtp file. </summary>
        /// <value> The full pathname of the rtp file. </value>
        public string RTPPath
        {
            get
            {
                return @"rtp://192.168.31.6:5555";
            }
        }
        /// <summary> The video capture. </summary>
        NRVideoCapture m_VideoCapture = null;

        /// <summary> Starts this object. </summary>
        void Start()
        {
            CreateVideoCaptureTest();
        }

        /// <summary> Updates this object. </summary>
        void Update()
        {
            if (m_VideoCapture == null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.R) || NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                StartVideoCapture();

                Previewer.SetData(m_VideoCapture.PreviewTexture, true);
            }

            if (Input.GetKeyDown(KeyCode.T) || NRInput.GetButtonDown(ControllerButton.HOME))
            {
                StopVideoCapture();

                Previewer.SetData(m_VideoCapture.PreviewTexture, false);
            }
        }

        /// <summary> Tests create video capture. </summary>
        void CreateVideoCaptureTest()
        {
            NRVideoCapture.CreateAsync(false, delegate (NRVideoCapture videoCapture)
            {
                if (videoCapture != null)
                {
                    m_VideoCapture = videoCapture;
                }
                else
                {
                    NRDebugger.Error("Failed to create VideoCapture Instance!");
                }
            });
        }

        /// <summary> Starts video capture. </summary>
        void StartVideoCapture()
        {
            Resolution cameraResolution = NRVideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            NRDebugger.Info(cameraResolution);

            int cameraFramerate = NRVideoCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();
            NRDebugger.Info(cameraFramerate);

            if (m_VideoCapture != null)
            {
                NRDebugger.Info("Created VideoCapture Instance!");
                CameraParameters cameraParameters = new CameraParameters();
                cameraParameters.hologramOpacity = 1f;
                cameraParameters.frameRate = cameraFramerate;
                cameraParameters.cameraResolutionWidth = cameraResolution.width;
                cameraParameters.cameraResolutionHeight = cameraResolution.height;
                cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;
                cameraParameters.blendMode = BlendMode.Blend;

                m_VideoCapture.StartVideoModeAsync(cameraParameters, OnStartedVideoCaptureMode, true);
            }

        }

        /// <summary> Stops video capture. </summary>
        void StopVideoCapture()
        {
            NRDebugger.Info("Stop Video Capture!");
            m_VideoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
        }

        /// <summary> Executes the 'started video capture mode' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStartedVideoCaptureMode(NRVideoCapture.VideoCaptureResult result)
        {
            NRDebugger.Info("Started Video Capture Mode!");
            m_VideoCapture.StartRecordingAsync(RTPPath, OnStartedRecordingVideo);
        }

        /// <summary> Executes the 'stopped video capture mode' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedVideoCaptureMode(NRVideoCapture.VideoCaptureResult result)
        {
            NRDebugger.Info("Stopped Video Capture Mode!");
        }

        /// <summary> Executes the 'started recording video' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStartedRecordingVideo(NRVideoCapture.VideoCaptureResult result)
        {
            NRDebugger.Info("Started Recording Video!");
        }

        /// <summary> Executes the 'stopped recording video' action. </summary>
        /// <param name="result"> The result.</param>
        void OnStoppedRecordingVideo(NRVideoCapture.VideoCaptureResult result)
        {
            NRDebugger.Info("Stopped Recording Video!");
            m_VideoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
        }
    }
}
