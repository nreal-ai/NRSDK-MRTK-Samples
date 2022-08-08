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
#if UNITY_EDITOR
    using System;
    using UnityEngine;
    using System.Runtime.InteropServices;

    [Obsolete]
    internal partial class NativeEmulator
    {
        /// <summary> Default constructor. </summary>
        public NativeEmulator()
        {
        }

        /// <summary> Handle of the tracking. </summary>
        private UInt64 m_TrackingHandle;
        /// <summary> Handle of the controller. </summary>
        private UInt64 m_ControllerHandle;


        #region Tracking

        /// <summary> Creates simulation tracking. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool CreateSIMTracking()
        {
            NativeResult result = NativeApi.NRSIMTrackingCreate(ref m_TrackingHandle);
            return result == NativeResult.Success;
        }

        /// <summary> Sets head tracking pose. </summary>
        /// <param name="position"> The position.</param>
        /// <param name="rotation"> The rotation.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetHeadTrackingPose(Vector3 position, Quaternion rotation)
        {
            NativeVector3f nativeVector3F = new NativeVector3f(new Vector3(position.x, position.y, -position.z));
            NativeVector4f nativeVector4F = new NativeVector4f(new Vector4(rotation.x, rotation.y, -rotation.z, -rotation.w));
            NativeResult result = NativeApi.NRSIMTrackingSetHeadTrackingPose(m_TrackingHandle, ref nativeVector3F, ref nativeVector4F);
            return result == NativeResult.Success;
        }

        /// <summary> Updates the trackable image data. </summary>
        /// <param name="centerPos">  The center position.</param>
        /// <param name="centerQua">  The center qua.</param>
        /// <param name="extentX">    The extent x coordinate.</param>
        /// <param name="extentZ">    The extent z coordinate.</param>
        /// <param name="identifier"> The identifier.</param>
        /// <param name="state">      The state.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool UpdateTrackableImageData(Vector3 centerPos, Quaternion centerQua, float extentX, float extentZ, UInt32 identifier, TrackingState state)
        {
            NativeVector3f pos = new NativeVector3f(new Vector3(centerPos.x, centerPos.y, -centerPos.z));
            NativeVector4f qua = new NativeVector4f(new Vector4(-centerQua.x, -centerQua.y, centerQua.z, centerQua.w));
            NativeResult result = NativeApi.NRSIMTrackingUpdateTrackableImageData(m_TrackingHandle, ref pos, ref qua, extentX, extentZ, identifier, (int)state);
            return result == NativeResult.Success;
        }

        /// <summary> Updates the trackable plane data. </summary>
        /// <param name="centerPos">  The center position.</param>
        /// <param name="centerQua">  The center qua.</param>
        /// <param name="extentX">    The extent x coordinate.</param>
        /// <param name="extentZ">    The extent z coordinate.</param>
        /// <param name="identifier"> The identifier.</param>
        /// <param name="state">      The state.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool UpdateTrackablePlaneData(Vector3 centerPos, Quaternion centerQua, float extentX, float extentZ, UInt32 identifier, TrackingState state)
        {
            NativeVector3f pos = new NativeVector3f(new Vector3(centerPos.x, centerPos.y, -centerPos.z));
            NativeVector4f qua = new NativeVector4f(new Vector4(-centerQua.x, -centerQua.y, centerQua.z, centerQua.w));
            NativeResult result = NativeApi.NRSIMTrackingUpdateTrackablePlaneData(m_TrackingHandle, ref pos, ref qua, extentX, extentZ, identifier, (int)state);
            return result == NativeResult.Success;
        }

        /// <summary> Set the trackables data for Unity Editor develop. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <param name="centerPos">  The center position.</param>
        /// <param name="centerQua">  The center qua.</param>
        /// <param name="extentX">    The extent x coordinate.</param>
        /// <param name="extentZ">    The extent z coordinate.</param>
        /// <param name="identifier"> The identifier.</param>
        /// <param name="state">      The state.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool UpdateTrackableData<T>(Vector3 centerPos, Quaternion centerQua, float extentX, float extentZ, System.UInt32 identifier, TrackingState state) where T : NRTrackable
        {
            if (typeof(NRTrackablePlane).Equals(typeof(T)))
            {
                return NREmulatorTrackableProvider.UpdateTrackablePlaneData(centerPos, centerQua, extentX, extentZ, identifier, state);
            }
            else if (typeof(NRTrackableImage).Equals(typeof(T)))
            {
                return NREmulatorTrackableProvider.UpdateTrackableImageData(centerPos, centerQua, extentX, extentZ, identifier, state);
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Controller

        /// <summary> Creates simulation controller. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool CreateSIMController()
        {
            NativeResult result = NativeApi.NRSIMControllerCreate(ref m_ControllerHandle);
            return result == NativeResult.Success;
        }

        /// <summary> Determines if we can destory simulation controller. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool DestorySIMController()
        {
            NativeResult result = NativeApi.NRSIMControllerDestroyAll();
            return result == NativeResult.Success;
        }

        /// <summary> Sets controller timestamp. </summary>
        /// <param name="timestamp"> The timestamp.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetControllerTimestamp(UInt64 timestamp)
        {
            NativeResult result = NativeApi.NRSIMControllerSetTimestamp(m_ControllerHandle, timestamp);
            return result == NativeResult.Success;
        }

        /// <summary> Sets controller position. </summary>
        /// <param name="postion"> The postion.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetControllerPosition(Vector3 postion)
        {
            NativeVector3f nv3 = new NativeVector3f(postion);
            NativeResult result = NativeApi.NRSIMControllerSetPosition(m_ControllerHandle, ref nv3);
            return result == NativeResult.Success;
        }

        /// <summary> Sets controller rotation. </summary>
        /// <param name="quaternion"> The quaternion.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetControllerRotation(Quaternion quaternion)
        {
            NativeVector4f nv4 = new NativeVector4f(new Vector4(quaternion.x, quaternion.y, -quaternion.z, -quaternion.w));
            NativeResult result = NativeApi.NRSIMControllerSetRotation(m_ControllerHandle, ref nv4);
            return result == NativeResult.Success;
        }

        /// <summary> Sets controller accelerometer. </summary>
        /// <param name="accel"> The accel.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetControllerAccelerometer(Vector3 accel)
        {
            NativeVector3f nv3 = new NativeVector3f(accel);
            NativeResult result = NativeApi.NRSIMControllerSetAccelerometer(m_ControllerHandle, ref nv3);
            return result == NativeResult.Success;
        }

        /// <summary> Sets controller button state. </summary>
        /// <param name="buttonState"> State of the button.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetControllerButtonState(Int32 buttonState)
        {
            NativeResult result = NativeApi.NRSIMControllerSetButtonState(m_ControllerHandle, buttonState);
            return result == NativeResult.Success;
        }

        /// <summary> Sets controller is touching. </summary>
        /// <param name="isTouching"> True if is touching, false if not.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetControllerIsTouching(bool isTouching)
        {
            NativeResult result = NativeApi.NRSIMControllerSetIsTouching(m_ControllerHandle, isTouching);
            return result == NativeResult.Success;
        }

        /// <summary> Sets controller touch point. </summary>
        /// <param name="x"> The x coordinate.</param>
        /// <param name="y"> The y coordinate.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetControllerTouchPoint(float x, float y)
        {
            NativeVector3f nv3 = new NativeVector3f(new Vector3(x, y, 0f));
            NativeResult result = NativeApi.NRSIMControllerSetTouchPoint(m_ControllerHandle, ref nv3);
            return result == NativeResult.Success;
        }

        /// <summary> Sets controller submit. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetControllerSubmit()
        {
            NativeResult result = NativeApi.NRSIMControllerSubmit(m_ControllerHandle);
            return result == NativeResult.Success;
        }

        #endregion


        private partial struct NativeApi
        {
            /// <summary> Nrsim tracking create. </summary>
            /// <param name="out_tracking_handle"> [in,out] Handle of the out tracking.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMTrackingCreate(ref UInt64 out_tracking_handle);

            /// <summary> Nrsim tracking set head tracking pose. </summary>
            /// <param name="tracking_handle"> Handle of the tracking.</param>
            /// <param name="position">        [in,out] The position.</param>
            /// <param name="rotation">        [in,out] The rotation.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMTrackingSetHeadTrackingPose(UInt64 tracking_handle,
                ref NativeVector3f position, ref NativeVector4f rotation);

            /// <summary> Nrsim tracking update trackable image data. </summary>
            /// <param name="tracking_handle"> Handle of the tracking.</param>
            /// <param name="center_pos">      [in,out] The center position.</param>
            /// <param name="center_rotation"> [in,out] The center rotation.</param>
            /// <param name="extent_x">        The extent x coordinate.</param>
            /// <param name="extent_z">        The extent z coordinate.</param>
            /// <param name="identifier">      The identifier.</param>
            /// <param name="state">           The state.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMTrackingUpdateTrackableImageData
                (UInt64 tracking_handle, ref NativeVector3f center_pos, ref NativeVector4f center_rotation,
                float extent_x, float extent_z, UInt32 identifier, int state);

            /// <summary> Nrsim tracking update trackable plane data. </summary>
            /// <param name="tracking_handle"> Handle of the tracking.</param>
            /// <param name="center_pos">      [in,out] The center position.</param>
            /// <param name="center_rotation"> [in,out] The center rotation.</param>
            /// <param name="extent_x">        The extent x coordinate.</param>
            /// <param name="extent_z">        The extent z coordinate.</param>
            /// <param name="identifier">      The identifier.</param>
            /// <param name="state">           The state.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMTrackingUpdateTrackablePlaneData(UInt64 tracking_handle,
                ref NativeVector3f center_pos, ref NativeVector4f center_rotation,
                float extent_x, float extent_z, UInt32 identifier, int state);

            /// <summary> Controller. </summary>
            /// <param name="out_controller_handle"> [in,out] Handle of the out controller.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerCreate(ref UInt64 out_controller_handle);

            /// <summary> Nrsim controller destroy all. </summary>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerDestroyAll();

            /// <summary> Nrsim controller set timestamp. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <param name="timestamp">         The timestamp.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetTimestamp(UInt64 controller_handle, UInt64 timestamp);

            /// <summary> Nrsim controller set position. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <param name="position">          [in,out] The position.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetPosition(UInt64 controller_handle, ref NativeVector3f position);

            /// <summary> Nrsim controller set rotation. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <param name="rotation">          [in,out] The rotation.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetRotation(UInt64 controller_handle, ref NativeVector4f rotation);

            /// <summary> Nrsim controller set accelerometer. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <param name="accel">             [in,out] The accel.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetAccelerometer(UInt64 controller_handle, ref NativeVector3f accel);

            /// <summary> Nrsim controller set button state. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <param name="buttonState">       State of the button.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetButtonState(UInt64 controller_handle, Int32 buttonState);

            /// <summary> Nrsim controller set is touching. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <param name="isTouching">        True if is touching, false if not.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetIsTouching(UInt64 controller_handle, bool isTouching);

            /// <summary> Nrsim controller set touch point. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <param name="point">             [in,out] The point.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSetTouchPoint(UInt64 controller_handle, ref NativeVector3f point);

            /// <summary> Nrsim controller submit. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSIMControllerSubmit(UInt64 controller_handle);

            /// <summary> Free library. </summary>
            /// <param name="hModule"> The module.</param>
            /// <returns> True if it succeeds, false if it fails. </returns>
            [DllImport("kernel32", SetLastError = true)]
            public static extern bool FreeLibrary(IntPtr hModule);
        }

    }
#endif
}
