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
    using System;

    /// <summary> An editor camera data provider. </summary>
    public class EditorCameraDataProvider : ICameraDataProvider
    {
        /// <summary> Creates a new bool. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Create()
        {
            return true;
        }

        /// <summary> Destroys the image described by imageHandle. </summary>
        /// <param name="imageHandle"> Handle of the image.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool DestroyImage(ulong imageHandle)
        {
            return true;
        }

        /// <summary> Gets hmd time nanos. </summary>
        /// <param name="imageHandle"> Handle of the image.</param>
        /// <param name="eye">         The eye.</param>
        /// <returns> The hmd time nanos. </returns>
        public ulong GetHMDTimeNanos(ulong imageHandle, int eye)
        {
            return 0;
        }

        /// <summary> Get exposure time. </summary>
        /// <param name="imageHandle"> Handle of the image. </param>
        /// <param name="eye">         The eye. </param>
        /// <returns> Exposure time of the image. </returns>
        public UInt32 GetExposureTime(UInt64 imageHandle, int eye)
        {
            UInt32 exposureTime = 0;
            return exposureTime;
        }

        /// <summary> Get Gain. </summary>
        /// <param name="imageHandle"> Handle of the image. </param>
        /// <param name="eye">         The eye. </param>
        /// <returns> Gain of the image. </returns>
        public UInt32 GetGain(UInt64 imageHandle, int eye)
        {
            UInt32 gain = 0;
            return gain;
        }

        /// <summary> Gets raw data. </summary>
        /// <param name="imageHandle"> Handle of the image.</param>
        /// <param name="eye">         The eye.</param>
        /// <param name="ptr">         [in,out] The pointer.</param>
        /// <param name="size">        [in,out] The size.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetRawData(ulong imageHandle, int eye, ref IntPtr ptr, ref int size)
        {
            return true;
        }

        /// <summary> Gets a resolution. </summary>
        /// <param name="imageHandle"> Handle of the image.</param>
        /// <param name="eye">         The eye.</param>
        /// <returns> The resolution. </returns>
        public NativeResolution GetResolution(ulong imageHandle, int eye)
        {
            return new NativeResolution(1280, 720);
        }

        /// <summary> Releases this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Release()
        {
            return true;
        }

        /// <summary> Callback, called when the set capture. </summary>
        /// <param name="callback"> The callback.</param>
        /// <param name="userdata"> (Optional) The userdata.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetCaptureCallback(CameraImageCallback callback, ulong userdata = 0)
        {
            return true;
        }

        /// <summary> Sets image format. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetImageFormat(CameraImageFormat format)
        {
            return true;
        }

        /// <summary> Starts a capture. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool StartCapture()
        {
            return true;
        }

        /// <summary> Stops a capture. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool StopCapture()
        {
            return true;
        }
    }
}
