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
    /// <summary> A camera parameters. </summary>
    public struct CameraParameters
    {
        /// <summary> Constructor. </summary>
        /// <param name="webCamMode"> The web camera mode.</param>
        /// <param name="mode">       The mode.</param>
        public CameraParameters(CamMode webCamMode, BlendMode mode)
        {
            this.camMode = webCamMode;
            this.hologramOpacity = 1f;
            this.frameRate = NativeConstants.RECORD_FPS_DEFAULT;

            this.cameraResolutionWidth = 1280;
            this.cameraResolutionHeight = 720;

            this.pixelFormat = CapturePixelFormat.BGRA32;
            this.blendMode = mode;
            this.audioState = NRVideoCapture.AudioState.ApplicationAndMicAudio;
            this.mediaProjection = null;
        }

        /// <summary> The opacity of captured holograms. </summary>
        /// <value> The hologram opacity. </value>
        public float hologramOpacity { get; set; }

        /// <summary>
        /// The framerate at which to capture video. This is only for use with VideoCapture. </summary>
        /// <value> The frame rate. </value>
        public int frameRate { get; set; }

        /// <summary> A valid width resolution for use with the web camera. </summary>
        public int cameraResolutionWidth { get; set; }

        /// <summary> A valid height resolution for use with the web camera. </summary>
        /// <value> The height of the camera resolution. </value>
        public int cameraResolutionHeight { get; set; }

        /// <summary> The pixel format used to capture and record your image data. </summary>
        public CapturePixelFormat pixelFormat { get; set; }

        /// <summary> The camera mode of capture. </summary>
        /// <value> The camera mode. </value>
        public CamMode camMode { get; set; }

        /// <summary> The audio state of capture. </summary>
        public NRVideoCapture.AudioState audioState { get; set; }
        public bool CaptureAudioMic { get { return audioState == NRVideoCapture.AudioState.MicAudio || audioState == NRVideoCapture.AudioState.ApplicationAndMicAudio; }}
        public bool CaptureAudioApplication { get { return audioState == NRVideoCapture.AudioState.ApplicationAudio || audioState == NRVideoCapture.AudioState.ApplicationAndMicAudio; }}

        /// <summary> The android MediaProjection object. </summary>
        public UnityEngine.AndroidJavaObject mediaProjection { get; set; }

        /// <summary> The blend mode of camera output. </summary>
        public BlendMode blendMode { get; set; }
    }
}
