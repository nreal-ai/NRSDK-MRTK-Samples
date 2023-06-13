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
    using UnityEngine;
    using System.Runtime.InteropServices;

    /// <summary> HMD Eye offset Native API . </summary>
    public partial class NativeHMD
    {
        /// <summary> Handle of the hmd. </summary>
        private UInt64 m_HmdHandle;
        /// <summary> Gets the handle of the hmd. </summary>
        /// <value> The hmd handle. </value>
        public UInt64 HmdHandle
        {
            get
            {
                return m_HmdHandle;
            }
        }

        /// <summary> Creates a new bool. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Create()
        {
            NativeResult result = NativeApi.NRHMDCreate(ref m_HmdHandle);
            NativeErrorListener.Check(result, this, "Create");
            return result == NativeResult.Success;
        }

        /// <summary> Pauses this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Pause()
        {
            NativeResult result = NativeApi.NRHMDPause(m_HmdHandle);
            NativeErrorListener.Check(result, this, "Pause");
            return result == NativeResult.Success;
        }

        /// <summary> Resumes this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Resume()
        {
            NativeResult result = NativeApi.NRHMDResume(m_HmdHandle);
            NativeErrorListener.Check(result, this, "Resume");
            return result == NativeResult.Success;
        }

        /// <summary> Gets device pose from head. </summary>
        /// <param name="device"> The device type.</param>
        /// <returns> The device pose from head. </returns>
        public Pose GetDevicePoseFromHead(NativeDevice device)
        {
            return GetDevicePoseFromHead((int)device);
        }

        /// <summary> Gets device pose from head. </summary>
        /// <param name="device"> The device type.</param>
        /// <returns> The device pose from head. </returns>
        public Pose GetDevicePoseFromHead(int device)
        {
            Pose outDevicePoseFromHead = Pose.identity;
            NativeMat4f mat4f = new NativeMat4f(Matrix4x4.identity);
            NativeResult result = NativeApi.NRHMDGetEyePoseFromHead(m_HmdHandle, device, ref mat4f);
            if (result == NativeResult.Success)
            {
                ConversionUtility.ApiPoseToUnityPose(mat4f, out outDevicePoseFromHead);
            }
            return outDevicePoseFromHead;
        }

        /// <summary> Gets projection matrix. </summary>
        /// <param name="outEyesProjectionMatrix"> [in,out] The out eyes projection matrix.</param>
        /// <param name="znear">                   The znear.</param>
        /// <param name="zfar">                    The zfar.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetProjectionMatrix(ref EyeProjectMatrixData outEyesProjectionMatrix, float znear, float zfar)
        {
            NativeFov4f fov = new NativeFov4f();
            NativeResult result_left = NativeApi.NRHMDGetEyeFovInCoord(m_HmdHandle, (int)NativeDevice.LEFT_DISPLAY, ref fov);
            NativeErrorListener.Check(result_left, this, "GetProjectionMatrix-L");
            NRDebugger.Info("[GetProjectionMatrix] LEFT_DISPLAY: {0}", fov.ToString());
            outEyesProjectionMatrix.LEyeMatrix = ConversionUtility.GetProjectionMatrixFromFov(fov, znear, zfar).ToUnityMat4f();
            
            NativeResult result_right = NativeApi.NRHMDGetEyeFovInCoord(m_HmdHandle, (int)NativeDevice.RIGHT_DISPLAY, ref fov);
            NativeErrorListener.Check(result_right, this, "GetProjectionMatrix-R");
            NRDebugger.Info("[GetProjectionMatrix] RIGHT_DISPLAY: {0}", fov.ToString());
            outEyesProjectionMatrix.REyeMatrix = ConversionUtility.GetProjectionMatrixFromFov(fov, znear, zfar).ToUnityMat4f();
            
            NativeResult result_RGB = NativeApi.NRHMDGetEyeFovInCoord(m_HmdHandle, (int)NativeDevice.RGB_CAMERA, ref fov);
            NativeErrorListener.Check(result_RGB, this, "GetProjectionMatrix-RGB");
            outEyesProjectionMatrix.RGBEyeMatrix = ConversionUtility.GetProjectionMatrixFromFov(fov, znear, zfar).ToUnityMat4f();

            return (result_left == NativeResult.Success && result_right == NativeResult.Success && result_RGB == NativeResult.Success);
        }

        [Obsolete("Use 'GetEyeFovInCoord' to replace.")]
        public NativeFov4f GetEyeFov(NativeEye eye)
        {
            NativeFov4f fov = new NativeFov4f();
            NativeApi.NRHMDGetEyeFov(m_HmdHandle, (int)eye, ref fov);
            return fov;
        }

        public NativeFov4f GetEyeFovInCoord(NativeDevice eye)
        {
            NativeFov4f fov = new NativeFov4f();
            NativeApi.NRHMDGetEyeFovInCoord(m_HmdHandle, (int)eye, ref fov);
            return fov;
        }

        /// <summary> Gets camera intrinsic matrix. </summary>
        /// <param name="eye">                  The eye.</param>
        /// <param name="CameraIntrinsicMatix"> [in,out] The camera intrinsic matix.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetCameraIntrinsicMatrix(int eye, ref NativeMat3f CameraIntrinsicMatix)
        {
            var result = NativeApi.NRHMDGetCameraIntrinsicMatrix(m_HmdHandle, (int)eye, ref CameraIntrinsicMatix);
            return result == NativeResult.Success;
        }

        /// <summary> Gets camera distortion. </summary>
        /// <param name="eye">        The eye.</param>
        /// <param name="distortion"> A variable-length parameters list containing distortion.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetCameraDistortion(int eye, ref NRDistortionParams distortion)
        {
            var result = NativeApi.NRHMDGetCameraDistortionParams(m_HmdHandle, eye, ref distortion);
            return result == NativeResult.Success;
        }

        /// <summary> Gets eye resolution. </summary>
        /// <param name="eye"> The eye.</param>
        /// <returns> The eye resolution. </returns>
        public NativeResolution GetEyeResolution(int eye)
        {
            NativeResolution resolution = new NativeResolution(1920, 1080);
#if UNITY_EDITOR
            return resolution;
#else
            var result = NativeApi.NRHMDGetEyeResolution(m_HmdHandle, eye, ref resolution);
            NativeErrorListener.Check(result, this, "GetEyeResolution");
            return resolution;
#endif
        }

        /// <summary> Gets device type of running device. </summary>
        /// <returns> The device type. </returns>
        public NRDeviceType GetDeviceType()
        {
            NRDeviceType deviceType = NRDeviceType.NrealLight;
            NativeApi.NRHMDGetDeviceType(m_HmdHandle, ref deviceType);
            return deviceType;
        }

        /// <summary> Gets device type of running device. </summary>
        /// <param name="feature"> The request feature.</param>
        /// <returns> Is the feature supported. </returns>
        public bool IsFeatureSupported(NRSupportedFeature feature)
        {
            bool result = false;
            NativeApi.NRHMDIsFeatureSupported(m_HmdHandle, feature, ref result);
            return result;
        }

        /// <summary> Destroys this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Destroy()
        {
            NativeResult result = NativeApi.NRHMDDestroy(m_HmdHandle);
            NativeErrorListener.Check(result, this, "Destroy");
            return result == NativeResult.Success;
        }

        /// <summary> A native api. </summary>
        private struct NativeApi
        {
            /// <summary> Nrhmd create. </summary>
            /// <param name="out_hmd_handle"> [in,out] Handle of the out hmd.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDCreate(ref UInt64 out_hmd_handle);

            /// <summary> Nrhmd get device type. </summary>
            /// <param name="hmd_handle">         Handle of the hmd.</param>
            /// <param name="out_device_type"> [in,out] The out device type.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDGetDeviceType(UInt64 hmd_handle, ref NRDeviceType out_device_type);

            /// <summary>
            /// Check whether the current feature is supported.
            /// </summary>
            /// <param name="hmd_handle"> Handle of the out hmd.</param>
            /// <param name="feature"> Current feature. </param>
            /// <param name="out_is_supported"> Result of  whether the current feature is supported. </param>
            /// <returns></returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDIsFeatureSupported(UInt64 hmd_handle, NRSupportedFeature feature, ref bool out_is_supported);

            /// <summary> Nrhmd pause. </summary>
            /// <param name="hmd_handle"> Handle of the hmd.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDPause(UInt64 hmd_handle);

            /// <summary> Nrhmd resume. </summary>
            /// <param name="hmd_handle"> Handle of the hmd.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDResume(UInt64 hmd_handle);

            /// <summary> Nrhmd get eye pose from head. </summary>
            /// <param name="hmd_handle">         Handle of the hmd.</param>
            /// <param name="eye">                The eye.</param>
            /// <param name="outEyePoseFromHead"> [in,out] The out eye pose from head.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDGetEyePoseFromHead(UInt64 hmd_handle, int eye, ref NativeMat4f outEyePoseFromHead);

            /// <summary> Nrhmd get eye fov. </summary>
            /// <param name="hmd_handle">  Handle of the hmd.</param>
            /// <param name="eye">         The eye.</param>
            /// <param name="out_eye_fov"> [in,out] The out eye fov.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            [Obsolete]
            public static extern NativeResult NRHMDGetEyeFov(UInt64 hmd_handle, int eye, ref NativeFov4f out_eye_fov);

            /// <summary> Nrhmd get eye fov. </summary>
            /// <param name="hmd_handle">  Handle of the hmd.</param>
            /// <param name="eye">         The eye.</param>
            /// <param name="out_eye_fov"> [in,out] The out eye fov.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDGetEyeFovInCoord(UInt64 hmd_handle, int eye, ref NativeFov4f out_eye_fov);

            /// <summary> Nrhmd get camera intrinsic matrix. </summary>
            /// <param name="hmd_handle">           Handle of the hmd.</param>
            /// <param name="eye">                  The eye.</param>
            /// <param name="out_intrinsic_matrix"> [in,out] The out intrinsic matrix.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDGetCameraIntrinsicMatrix(
                    UInt64 hmd_handle, int eye, ref NativeMat3f out_intrinsic_matrix);

            /// <summary> Nrhmd get camera distortion parameters. </summary>
            /// <param name="hmd_handle"> Handle of the hmd.</param>
            /// <param name="eye">        The eye.</param>
            /// <param name="out_params"> A variable-length parameters list containing out parameters.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDGetCameraDistortionParams(
                    UInt64 hmd_handle, int eye, ref NRDistortionParams out_params);

            /// <summary> Nrhmd get eye resolution. </summary>
            /// <param name="hmd_handle">         Handle of the hmd.</param>
            /// <param name="eye">                The eye.</param>
            /// <param name="out_eye_resolution"> [in,out] The out eye resolution.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDGetEyeResolution(UInt64 hmd_handle, int eye, ref NativeResolution out_eye_resolution);

            /// <summary> Nrhmd destroy. </summary>
            /// <param name="hmd_handle"> Handle of the hmd.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRHMDDestroy(UInt64 hmd_handle);
        };
    }
}
