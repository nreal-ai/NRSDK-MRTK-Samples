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

    /// <summary> Values that represent codec types. </summary>
    public enum CodecType
    {
        /// <summary> An enum constant representing the local option. </summary>
        Local = 0,
        /// <summary> An enum constant representing the rtmp option. </summary>
        Rtmp = 1,
        /// <summary> An enum constant representing the rtp option. </summary>
        Rtp = 2,
    }

    /// <summary> Values that represent blend modes. </summary>
    public enum BlendMode
    {
        /// <summary> Blend the virtual image and rgb camera image. </summary>
        Blend,
        /// <summary> Only rgb camera image. </summary>
        RGBOnly,
        /// <summary> Only virtual image. </summary>
        VirtualOnly,
        /// <summary> Arrange virtual image and rgb camera image from left to right. </summary>
        [Obsolete]
        WidescreenBlend
    }

    /// <summary> Values that represent record type index. </summary>
    public enum RecorderIndex
    {
        /// <summary> Recorder index of mic. </summary>
        REC_MIC = 0,
        /// <summary> Recorder index of application. </summary>
        REC_APP = 1,
    }

    /// <summary> Callback, called when the capture task. </summary>
    /// <param name="task"> The task.</param>
    /// <param name="data"> The data.</param>
    public delegate void CaptureTaskCallback(CaptureTask task, byte[] data);

    /// <summary> A capture task. </summary>
    public struct CaptureTask
    {
        /// <summary> The width of capture image task. </summary>
        public int Width;
        /// <summary> The height of capture image task. </summary>
        public int Height;
        /// <summary> The capture format. </summary>
        public PhotoCaptureFileOutputFormat CaptureFormat;
        /// <summary> The on receive callback. </summary>
        public CaptureTaskCallback OnReceive;

        /// <summary> Constructor. </summary>
        /// <param name="w">        The width.</param>
        /// <param name="h">        The height.</param>
        /// <param name="format">   Describes the format to use.</param>
        /// <param name="callback"> The callback.</param>
        public CaptureTask(int w, int h, PhotoCaptureFileOutputFormat format, CaptureTaskCallback callback)
        {
            this.Width = w;
            this.Height = h;
            this.CaptureFormat = format;
            this.OnReceive = callback;
        }
    }

    /// <summary> A native encode configuration. </summary>
    public class NativeEncodeConfig
    {
        /// <summary> Gets or sets the width. </summary>
        /// <value> The width. </value>
        public int width { get; private set; }
        /// <summary> Gets or sets the height. </summary>
        /// <value> The height. </value>
        public int height { get; private set; }
        /// <summary> Gets or sets the bit rate. </summary>
        /// <value> The bit rate. </value>
        public int bitRate { get; private set; }
        /// <summary> Gets or sets the FPS. </summary>
        /// <value> The FPS. </value>
        public int fps { get; private set; }
        /// <summary> Gets or sets the type of the codec. </summary>
        /// <value> The type of the codec. </value>
        public int codecType { get; private set; }    // 0 local; 1 rtmp ; 2 rtp
        /// <summary> Gets or sets the full pathname of the out put file. </summary>
        /// <value> The full pathname of the out put file. </value>
        public string outPutPath { get; private set; }
        /// <summary> Gets or sets the use step time. </summary>
        /// <value> The use step time. </value>
        public int useStepTime { get; private set; }
        /// <summary> Gets or sets a value indicating whether this object use alpha. </summary>
        /// <value> True if use alpha, false if not. </value>
        public bool useAlpha { get; private set; }
        /// <summary>
        /// Gets or sets a value indicating whether this object use linner texture. </summary>
        /// <value> True if use linner texture, false if not. </value>
        public bool useLinnerTexture { get; private set; }

        public bool addMicphoneAudio { get; private set; }

        public bool addInternalAudio { get; private set; }

        public int audioSampleRate { get; private set; }

        public int audioBitRate { get; private set; }

        /// <summary> Constructor. </summary>
        /// <param name="cameraparam"> The cameraparam.</param>
        /// <param name="path">        (Optional) Full pathname of the file.</param>
        public NativeEncodeConfig(CameraParameters cameraparam, string path = "")
        {
            this.width = cameraparam.blendMode == BlendMode.WidescreenBlend ? 2 * cameraparam.cameraResolutionWidth : cameraparam.cameraResolutionWidth;
            this.height = cameraparam.cameraResolutionHeight;
            this.bitRate = NativeConstants.RECORD_VIDEO_BITRATE_DEFAULT;
            this.fps = cameraparam.frameRate;
            this.codecType = GetCodecTypeByPath(path);
            this.outPutPath = path;
            this.useStepTime = 0;
            this.addMicphoneAudio = cameraparam.CaptureAudioMic;
            this.addInternalAudio = cameraparam.CaptureAudioApplication;
            this.useAlpha = cameraparam.hologramOpacity < float.Epsilon;
            this.useLinnerTexture = NRRenderer.isLinearColorSpace;
            this.audioBitRate = NativeConstants.RECORD_AUDIO_BITRATE_DEFAULT;
            this.audioSampleRate = NativeConstants.RECORD_AUDIO_SAMPLERATE_DEFAULT;
        }

        /// <summary> Sets out put path. </summary>
        /// <param name="path"> Full pathname of the file.</param>
        public void SetOutPutPath(string path)
        {
            this.codecType = GetCodecTypeByPath(path);
            this.outPutPath = path;
        }

        /// <summary> Gets codec type by path. </summary>
        /// <param name="path"> Full pathname of the file.</param>
        /// <returns> The codec type by path. </returns>
        private static int GetCodecTypeByPath(string path)
        {
            if (path.StartsWith("rtmp://"))
            {
                return 1;
            }
            else if (path.StartsWith("rtp://"))
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }

        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            return LitJson.JsonMapper.ToJson(this);
        }
    }
}
