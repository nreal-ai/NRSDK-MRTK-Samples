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
    using System.Runtime.InteropServices;

    /// <summary> Session Native API. </summary>
    internal partial class NativeCamera : ICameraDataProvider
    {
        /// <summary> Handle of the native camera. </summary>
        private UInt64 m_NativeCameraHandle;
        private bool _IsErrorState = false;

        /// <summary> Creates a new bool. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Create()
        {
            _IsErrorState = false;
            var result = NativeApi.NRRGBCameraCreate(ref m_NativeCameraHandle);
            NativeErrorListener.Check(result, this, "Create", true);
            return result == NativeResult.Success;
        }

        /// <summary> Gets raw data. </summary>
        /// <param name="imageHandle"> Handle of the image.</param>
        /// <param name="eye">         The eye.</param>
        /// <param name="ptr">         [in,out] The pointer.</param>
        /// <param name="size">        [in,out] The size.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetRawData(UInt64 imageHandle, int eye, ref IntPtr ptr, ref int size)
        {
            if (_IsErrorState)
            {
                return false;
            }
            uint data_size = 0;
            var result = NativeApi.NRRGBCameraImageGetRawData(m_NativeCameraHandle, imageHandle, ref ptr, ref data_size);
            size = (int)data_size;
            NativeErrorListener.Check(result, this, "GetRawData");
            return result == NativeResult.Success;
        }

        /// <summary> Gets a resolution. </summary>
        /// <param name="imageHandle"> Handle of the image.</param>
        /// <param name="eye">         The eye.</param>
        /// <returns> The resolution. </returns>
        public NativeResolution GetResolution(UInt64 imageHandle, int eye)
        {
            NativeResolution resolution = new NativeResolution(0, 0);
            var result = NativeApi.NRRGBCameraImageGetResolution(m_NativeCameraHandle, imageHandle, ref resolution);
            NativeErrorListener.Check(result, this, "GetResolution");
            return resolution;
        }

        /// <summary> Gets hmd time nanos. </summary>
        /// <param name="imageHandle"> Handle of the image.</param>
        /// <param name="eye">         The eye.</param>
        /// <returns> The hmd time nanos. </returns>
        public UInt64 GetHMDTimeNanos(UInt64 imageHandle, int eye)
        {
            UInt64 time = 0;
            NativeApi.NRRGBCameraImageGetHMDTimeNanos(m_NativeCameraHandle, imageHandle, ref time);
            return time;
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

        /// <summary> Callback, called when the set capture. </summary>
        /// <param name="callback"> The callback.</param>
        /// <param name="userdata"> (Optional) The userdata.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetCaptureCallback(CameraImageCallback callback, UInt64 userdata = 0)
        {
            if (_IsErrorState)
            {
                return false;
            }
            var result = NativeApi.NRRGBCameraSetCaptureCallback(m_NativeCameraHandle, callback, userdata);
            NativeErrorListener.Check(result, this, "SetCaptureCallback");
            return result == NativeResult.Success;
        }

        /// <summary> Sets image format. </summary>
        /// <param name="format"> Describes the format to use.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetImageFormat(CameraImageFormat format)
        {
            if (_IsErrorState)
            {
                return false;
            }
            var result = NativeApi.NRRGBCameraSetImageFormat(m_NativeCameraHandle, format);
            NativeErrorListener.Check(result, this, "SetImageFormat");
            return result == NativeResult.Success;
        }

        /// <summary> Starts a capture. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool StartCapture()
        {
            if (_IsErrorState)
            {
                NativeErrorListener.Check(NativeResult.RGBCameraDeviceNotFind, this, "StartCapture", true);
                return false;
            }
            var result = NativeApi.NRRGBCameraStartCapture(m_NativeCameraHandle);
            _IsErrorState = (result != NativeResult.Success);
            NativeErrorListener.Check(result, this, "StartCapture", true);
            return result == NativeResult.Success;
        }

        /// <summary> Stops a capture. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool StopCapture()
        {
            if (_IsErrorState)
            {
                return false;
            }
            var result = NativeApi.NRRGBCameraStopCapture(m_NativeCameraHandle);
            NativeErrorListener.Check(result, this, "StopCapture", true);
            return result == NativeResult.Success;
        }

        /// <summary> Destroys the image described by imageHandle. </summary>
        /// <param name="imageHandle"> Handle of the image.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool DestroyImage(UInt64 imageHandle)
        {
            if (_IsErrorState)
            {
                return false;
            }
            var result = NativeApi.NRRGBCameraImageDestroy(m_NativeCameraHandle, imageHandle);
            NativeErrorListener.Check(result, this, "DestroyImage");
            return result == NativeResult.Success;
        }

        /// <summary> Releases this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Release()
        {
            _IsErrorState = false;
            var result = NativeApi.NRRGBCameraDestroy(m_NativeCameraHandle);
            NativeErrorListener.Check(result, this, "Release");
            return result == NativeResult.Success;
        }

        /// <summary> A native api. </summary>
        private struct NativeApi
        {
            /// <summary> Nrrgb camera image get raw data. </summary>
            /// <param name="rgb_camera_handle">       Handle of the RGB camera.</param>
            /// <param name="rgb_camera_image_handle"> Handle of the RGB camera image.</param>
            /// <param name="out_image_raw_data">      [in,out] Information describing the out image raw.</param>
            /// <param name="out_image_raw_data_size"> [in,out] Size of the out image raw data.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraImageGetRawData(UInt64 rgb_camera_handle,
                UInt64 rgb_camera_image_handle, ref IntPtr out_image_raw_data, ref UInt32 out_image_raw_data_size);

            /// <summary> Nrrgb camera image get resolution. </summary>
            /// <param name="rgb_camera_handle">       Handle of the RGB camera.</param>
            /// <param name="rgb_camera_image_handle"> Handle of the RGB camera image.</param>
            /// <param name="out_image_resolution">    [in,out] The out image resolution.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraImageGetResolution(UInt64 rgb_camera_handle,
                UInt64 rgb_camera_image_handle, ref NativeResolution out_image_resolution);

            /// <summary> Nrrgb camera image get hmd time nanos. </summary>
            /// <param name="rgb_camera_handle">        Handle of the RGB camera.</param>
            /// <param name="rgb_camera_image_handle">  Handle of the RGB camera image.</param>
            /// <param name="out_image_hmd_time_nanos"> [in,out] The out image hmd time nanos.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraImageGetHMDTimeNanos(
                UInt64 rgb_camera_handle, UInt64 rgb_camera_image_handle,
                ref UInt64 out_image_hmd_time_nanos);

            /// <summary> Nrrgb camera create. </summary>
            /// <param name="out_rgb_camera_handle"> [in,out] Handle of the out RGB camera.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraCreate(ref UInt64 out_rgb_camera_handle);

            /// <summary> Nrrgb camera destroy. </summary>
            /// <param name="rgb_camera_handle"> Handle of the RGB camera.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraDestroy(UInt64 rgb_camera_handle);

            /// <summary> Callback, called when the nrrgb camera set capture. </summary>
            /// <param name="rgb_camera_handle"> Handle of the RGB camera.</param>
            /// <param name="image_callback">    The image callback.</param>
            /// <param name="userdata">          The userdata.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern NativeResult NRRGBCameraSetCaptureCallback(
                UInt64 rgb_camera_handle, CameraImageCallback image_callback, UInt64 userdata);

            /// <summary> Nrrgb camera set image format. </summary>
            /// <param name="rgb_camera_handle"> Handle of the RGB camera.</param>
            /// <param name="format">            Describes the format to use.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraSetImageFormat(
                UInt64 rgb_camera_handle, CameraImageFormat format);

            /// <summary> Nrrgb camera start capture. </summary>
            /// <param name="rgb_camera_handle"> Handle of the RGB camera.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraStartCapture(UInt64 rgb_camera_handle);

            /// <summary> Nrrgb camera stop capture. </summary>
            /// <param name="rgb_camera_handle"> Handle of the RGB camera.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraStopCapture(UInt64 rgb_camera_handle);

            /// <summary> Nrrgb camera image destroy. </summary>
            /// <param name="rgb_camera_handle">       Handle of the RGB camera.</param>
            /// <param name="rgb_camera_image_handle"> Handle of the RGB camera image.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRGBCameraImageDestroy(UInt64 rgb_camera_handle,
                UInt64 rgb_camera_image_handle);
        };
    }
}
