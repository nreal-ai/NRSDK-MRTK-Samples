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
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Holds information about NR Device's pose in the world coordinate, trackables, etc.. Through
    /// this class, application can get the information of current frame. It contains session status,
    /// lost tracking reason, device pose, trackables, etc. </summary>
    public class NRFrame
    {
        /// <summary> Get the tracking state of HMD. </summary>
        /// <value> The session status. </value>
        public static SessionState SessionStatus
        {
            get
            {
                return NRSessionManager.Instance.SessionState;
            }
        }

        /// <summary> Get the lost tracking reason of HMD. </summary>
        /// <value> The lost tracking reason. </value>
        public static LostTrackingReason LostTrackingReason
        {
            get
            {
                return NRSessionManager.Instance.LostTrackingReason;
            }
        }
        
        /// <summary> Gets the nr renderer. </summary>
        /// <value> The nr renderer. </value>
        public static NRRenderer NRRenderer
        {
            get
            {
                return NRSessionManager.Instance.NRRenderer;
            }
        }

        /// <summary> The head pose. </summary>
        private static Pose m_HeadPose;

        /// <summary> Get the pose of device in unity world coordinate. </summary>
        /// <value> Pose of device. </value>
        public static Pose HeadPose
        {
            get
            {
                return m_HeadPose;
            }
        }

        public static bool isHeadPoseReady { get; private set; } = false;

        /// <summary> Gets head pose by recommond timestamp. </summary>
        /// <param name="pose">      [in,out] The pose.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        [Obsolete("Use 'GetFramePresentHeadPose' instead.")]
        public static bool GetHeadPoseRecommend(ref Pose pose)
        {
            if (SessionStatus == SessionState.Running)
            {
                return NRSessionManager.Instance.NativeAPI.NativeHeadTracking.GetHeadPoseRecommend(ref pose);
            }
            return false;
        }

        /// <summary> Gets head pose by timestamp. </summary>
        /// <param name="pose">      [in,out] The pose.</param>
        /// <param name="timestamp"> The timestamp.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool GetHeadPoseByTime(ref Pose pose, UInt64 timestamp)
        {
            if (SessionStatus == SessionState.Running)
            {
                return NRSessionManager.Instance.TrackingSubSystem.GetHeadPose(ref pose, timestamp);
            }
            return false;
        }

        /// <summary>
        /// Get the pose information when the current frame display on the screen.
        /// </summary>
        /// <param name="pose"></param>
        /// <param name="timestamp">current timestamp to the pose.</param>
        /// <returns></returns>
        public static bool GetFramePresentHeadPose(ref Pose pose, ref LostTrackingReason lostReason, ref UInt64 timestamp)
        {
            if (SessionStatus == SessionState.Running &&
                (NRRenderer == null || NRRenderer.CurrentState == NRRenderer.RendererState.Running))
            {
                isHeadPoseReady = NRSessionManager.Instance.TrackingSubSystem.GetFramePresentHeadPose(ref pose, ref lostReason, ref timestamp);
                return isHeadPoseReady;
            }
            return false;
        }

        /// <summary> Get the pose of center camera between left eye and right eye. </summary>
        /// <value> The center eye pose. </value>
        public static Pose CenterEyePose
        {
            get
            {
                Transform leftCamera = NRSessionManager.Instance.NRHMDPoseTracker.leftCamera.transform;
                Transform rightCamera = NRSessionManager.Instance.NRHMDPoseTracker.rightCamera.transform;

                Vector3 centerEye_pos = (leftCamera.position + rightCamera.position) * 0.5f;
                Quaternion centerEye_rot = Quaternion.Lerp(leftCamera.rotation, rightCamera.rotation, 0.5f);

                return new Pose(centerEye_pos, centerEye_rot);
            }
        }

        /// <summary> The eye position from head. </summary>
        private static EyePoseData m_EyePosFromHead;

        /// <summary> Get the offset position between eye and head. </summary>
        /// <value> The eye pose from head. </value>
        public static EyePoseData EyePoseFromHead
        {
            get
            {
                if (SessionStatus == SessionState.Running)
                {
                    m_EyePosFromHead.LEyePose = NRDevice.Subsystem.GetDevicePoseFromHead(NativeDevice.LEFT_DISPLAY);
                    m_EyePosFromHead.REyePose = NRDevice.Subsystem.GetDevicePoseFromHead(NativeDevice.RIGHT_DISPLAY);
                    m_EyePosFromHead.RGBEyePose = NRDevice.Subsystem.GetDevicePoseFromHead(NativeDevice.RGB_CAMERA);
                }
                return m_EyePosFromHead;
            }
        }

        /// <summary> Get the offset position between device and head. </summary>
        /// <value> The device pose from head. </value>
        public static Pose GetDevicePoseFromHead(NativeDevice device)
        {
            return NRDevice.Subsystem.GetDevicePoseFromHead(device);
        }

        /// <summary> Get the project matrix of camera in unity. </summary>
        /// <param name="result"> [out] True to result.</param>
        /// <param name="znear">  The znear.</param>
        /// <param name="zfar">   The zfar.</param>
        /// <returns> project matrix of camera. </returns>
        public static EyeProjectMatrixData GetEyeProjectMatrix(out bool result, float znear, float zfar)
        {
            return NRDevice.Subsystem.GetEyeProjectMatrix(out result, znear, zfar);
        }

        /// <summary> Get the intrinsic matrix of device. </summary>
        /// <returns> The device intrinsic matrix. </returns>
        public static NativeMat3f GetDeviceIntrinsicMatrix(NativeDevice device)
        {
            return NRDevice.Subsystem.GetDeviceIntrinsicMatrix(device);
        }

        /// <summary> Get the distortion param of device. </summary>
        /// <returns> The device intrinsic matrix. </returns>
        public static NRDistortionParams GetDeviceDistortion(NativeDevice device)
        {
            return NRDevice.Subsystem.GetDeviceDistortion(device);
        }

        /// <summary> Get the intrinsic matrix of rgb camera. </summary>
        /// <returns> The RGB camera intrinsic matrix. </returns>
        public static NativeMat3f GetRGBCameraIntrinsicMatrix()
        {
            return GetDeviceIntrinsicMatrix(NativeDevice.RGB_CAMERA);
        }

        /// <summary> Get the Distortion of rgbcamera. </summary>
        /// <returns> The RGB camera distortion. </returns>
        public static NRDistortionParams GetRGBCameraDistortion()
        {
            return GetDeviceDistortion(NativeDevice.RGB_CAMERA);
        }

        /// <summary> Gets the resolution of device. </summary>
        /// <param name="eye"> device index.</param>
        /// <returns> The device resolution. </returns>
        public static NativeResolution GetDeviceResolution(NativeDevice device)
        {
            return NRDevice.Subsystem.GetDeviceResolution(device);
        }

        /// <summary> Gets device fov. </summary>
        /// <param name="eye">         The display index.</param>
        /// <param name="fov"> [in,out] The out device fov.</param>
        /// <returns> A NativeResult. </returns>
        public static void GetEyeFov(NativeDevice eye, ref NativeFov4f fov)
        {
            NRDevice.Subsystem.GetEyeFov(eye, ref fov);
        }

        private static UInt64 m_CurrentPoseTimeStamp = 0;
        public static UInt64 CurrentPoseTimeStamp
        {
            get
            {
#if USING_XR_SDK
                return NRSessionManager.Instance.TrackingSubSystem.GetHMDTimeNanos();
#else
                return m_CurrentPoseTimeStamp;
#endif
            }
        }

        internal static void OnPreUpdate(ref LostTrackingReason lostTrackReason)
        {
            Pose pose = Pose.identity;
            LostTrackingReason lostReason = LostTrackingReason.NONE;
            ulong timeStamp = 0;
            bool result = GetFramePresentHeadPose(ref pose, ref lostReason, ref timeStamp);
            if (result)
            {
                lostTrackReason = lostReason;
                if (lostReason != LostTrackingReason.PRE_INITIALIZING && lostReason != LostTrackingReason.INITIALIZING)
                {
                    m_HeadPose = pose;
                    m_CurrentPoseTimeStamp = timeStamp;
                }
                else
                {
                    NRDebugger.Info("[NRFrame] OnPreUpdate: LostTrackReason={0}, pose={1}, timeStamp={2}, lastHeadPose={3}, lastTimeStamp={4}",
                        lostReason, pose.ToString("F4"), timeStamp, m_HeadPose.ToString("F4"), m_CurrentPoseTimeStamp);
                }
            }
            // NRDebugger.Info("[NRFrame] OnPreUpdate: result={2}, LostTrackReason={3}, pos={0}, headPos={1}, ", pose.ToString("F4"), m_HeadPose.ToString("F4"), result, LostTrackingReason);
        }

        /// <summary> Get the list of trackables with specified filter. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <param name="trackables"> A list where the returned trackable is stored. The previous values
        ///                           will be cleared.</param>
        /// <param name="filter">     Query filter.</param>
        public static void GetTrackables<T>(List<T> trackables, NRTrackableQueryFilter filter) where T : NRTrackable
        {
            if (SessionStatus != SessionState.Running)
            {
                return;
            }
            trackables.Clear();
            NRSessionManager.Instance.TrackableFactory.GetTrackables<T>(trackables, filter);
        }

        public static Matrix4x4 GetWorldMatrixFromUnityToNative()
        {
            var hmdPoseTracker = NRSessionManager.Instance.NRHMDPoseTracker;
            if (hmdPoseTracker == null)
            {
                return Matrix4x4.identity;
            }
            else
            {
                return hmdPoseTracker.GetWorldOffsetMatrixFromNative();
            }
        }
    }
}
