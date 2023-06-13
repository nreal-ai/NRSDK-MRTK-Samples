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
    using UnityEngine;

    /// <summary> A controller for handling natives. </summary>
    internal partial class NativeController
    {
        /// <summary> Handle of the controller. </summary>
        private UInt64 m_ControllerHandle = 0;
        /// <summary> The state handles. </summary>
        private UInt64[] m_StateHandles = new UInt64[NRInput.MAX_CONTROLLER_STATE_COUNT] { 0, 0 };

        /// <summary> Initializes this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Init()
        {
            NRDebugger.Debug("[NativeController] Init");
            NativeResult result = NativeApi.NRControllerCreate(ref m_ControllerHandle);
            if (result == NativeResult.Success)
            {
                //manually start controller
                NativeApi.NRControllerStart(m_ControllerHandle);

                int count = GetControllerCount();
                NRDebugger.Debug("[NativeController] Get controller count:" + count);
                for (int i = 0; i < count; i++)
                {
                    result = NativeApi.NRControllerStateCreate(m_ControllerHandle, i, ref m_StateHandles[i]);
                    if (result != NativeResult.Success)
                    {
                        NRDebugger.Error("[NativeController] Create Failed!" + result.ToString());
                        return false;
                    }
                }
                NRDebugger.Debug("[NativeController] Created Successed");
                return true;
            }
            NRDebugger.Error("[NativeController] Create Failed!");
            m_ControllerHandle = 0;
            return false;
        }

        //public void TestApi(int index)
        //{
        //string msg = "GetControllerCount:" + GetControllerCount() + "\n"
        //+"GetAvailableFeatures:" + GetAvailableFeatures(index) + "\n"
        //+ "GetControllerType:" + GetControllerType(index) + "\n"
        //+ "GetConnectionState:" + GetConnectionState(index) + "\n"
        //+ "GetBatteryLevel:" + GetBatteryLevel(index) + "\n"
        //+ "IsCharging:" + IsCharging(index) + "\n"
        //+ "GetPose:" + GetPose(index).ToString("F3") + "\n"
        //+ "GetGyro:" + GetGyro(index).ToString("F3") + "\n"
        //+ "GetAccel:" + GetAccel(index).ToString("F3") + "\n"
        //+ "GetMag:" + GetMag(index).ToString("F3") + "\n"
        //+ "GetButtonState:" + GetButtonState(index) + "\n"
        //+ "GetButtonUp:" + GetButtonUp(index) + "\n"
        //+ "GetButtonDown:" + GetButtonDown(index) + "\n"
        //+ "IsTouching:" + IsTouching(index) + "\n"
        //+ "GetTouchUp:" + GetTouchUp(index) + "\n"
        //+ "GetTouchDown:" + GetTouchDown(index) + "\n"
        //+ "GetTouch:" + GetTouch(index).ToString("F3") + "\n";
        //NRDebugger.Info(msg);
        //}

        /// <summary> Gets controller count. </summary>
        /// <returns> The controller count. </returns>
        public int GetControllerCount()
        {
            if (m_ControllerHandle == 0)
            {
                return 0;
            }
            int count = 0;
            if (NativeApi.NRControllerGetCount(m_ControllerHandle, ref count) != NativeResult.Success)
                NRDebugger.Error("Get Controller Count Failed!");
            return Mathf.Min(count, m_StateHandles.Length);
        }

        /// <summary> Pauses this object. </summary>
        public void Pause()
        {
            if (m_ControllerHandle == 0)
            {
                return;
            }
            NativeApi.NRControllerPause(m_ControllerHandle);
        }

        /// <summary> Resumes this object. </summary>
        public void Resume()
        {
            if (m_ControllerHandle == 0)
            {
                return;
            }
            NativeApi.NRControllerResume(m_ControllerHandle);
        }

        /// <summary> Stops this object. </summary>
        public void Stop()
        {
            if (m_ControllerHandle == 0)
            {
                return;
            }
            NativeApi.NRControllerStop(m_ControllerHandle);
        }

        /// <summary> Destroys this object. </summary>
        public void Destroy()
        {
            if (m_ControllerHandle == 0)
            {
                return;
            }
            Stop();
            NativeApi.NRControllerDestroy(m_ControllerHandle);
        }

        /// <summary> Gets available features. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> The available features. </returns>
        public uint GetAvailableFeatures(int controllerIndex)
        {
            if (m_ControllerHandle == 0)
            {
                return 0;
            }
            uint availableFeature = 0;
            NativeApi.NRControllerGetAvailableFeatures(m_ControllerHandle, controllerIndex, ref availableFeature);
            return availableFeature;
        }

        /// <summary> Gets controller type. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> The controller type. </returns>
        public ControllerType GetControllerType(int controllerIndex)
        {
            if (m_ControllerHandle == 0)
            {
                return ControllerType.CONTROLLER_TYPE_UNKNOWN;
            }
            ControllerType controllerType = ControllerType.CONTROLLER_TYPE_UNKNOWN;
            NativeApi.NRControllerGetType(m_ControllerHandle, controllerIndex, ref controllerType);
            return controllerType;
        }

        /// <summary> Recenter controller. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        public void RecenterController(int controllerIndex)
        {
            if (m_ControllerHandle == 0)
            {
                return;
            }
            NativeApi.NRControllerRecenter(m_ControllerHandle, controllerIndex);
        }

        /// <summary> Trigger haptic vibrate. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <param name="duration">        The duration.</param>
        /// <param name="frequency">       The frequency.</param>
        /// <param name="amplitude">       The amplitude.</param>
        public void TriggerHapticVibrate(int controllerIndex, Int64 duration, float frequency, float amplitude)
        {
            if (m_ControllerHandle == 0)
            {
                return;
            }
            NativeApi.NRControllerHapticVibrate(m_ControllerHandle, controllerIndex, duration, frequency, amplitude);
        }

        /// <summary> Updates the state described by controllerIndex. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool UpdateState(int controllerIndex)
        {
            if (m_ControllerHandle == 0)
            {
                return false;
            }
            if (m_StateHandles[controllerIndex] == 0)
                NativeApi.NRControllerStateCreate(m_ControllerHandle, controllerIndex, ref m_StateHandles[controllerIndex]);
            if (m_StateHandles[controllerIndex] == 0)
                return false;
            NativeResult result = NativeApi.NRControllerStateUpdate(m_StateHandles[controllerIndex]);
            return result == NativeResult.Success;
        }

        /// <summary> Destroys the state described by controllerIndex. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        public void DestroyState(int controllerIndex)
        {
            NativeApi.NRControllerStateDestroy(m_StateHandles[controllerIndex]);
        }

        /// <summary> Gets connection state. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> The connection state. </returns>
        public ControllerConnectionState GetConnectionState(int controllerIndex)
        {
            ControllerConnectionState state = ControllerConnectionState.CONTROLLER_CONNECTION_STATE_NOT_INITIALIZED;
            NativeApi.NRControllerStateGetConnectionState(m_StateHandles[controllerIndex], ref state);
            return state;
        }

        /// <summary> Gets battery level. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> The battery level. </returns>
        public int GetBatteryLevel(int controllerIndex)
        {
            int batteryLevel = -1;
            NativeApi.NRControllerStateGetBatteryLevel(m_StateHandles[controllerIndex], ref batteryLevel);
            return batteryLevel;
        }

        /// <summary> Query if 'controllerIndex' is charging. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> True if charging, false if not. </returns>
        public bool IsCharging(int controllerIndex)
        {
            int isCharging = 0;
            NativeApi.NRControllerStateGetCharging(m_StateHandles[controllerIndex], ref isCharging);
            return isCharging == 1;
        }

        /// <summary> Gets a pose. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> The pose. </returns>
        public Pose GetPose(int controllerIndex)
        {
            Pose controllerPos = Pose.identity;
            NativeMat4f mat4f = new NativeMat4f(Matrix4x4.identity);
            NativeResult result = NativeApi.NRControllerStateGetPose(m_StateHandles[controllerIndex], ref mat4f);
            if (result == NativeResult.Success)
                ConversionUtility.ApiPoseToUnityPose(mat4f, out controllerPos);
            return controllerPos;
        }

        /// <summary> Gets a gyro. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> The gyro. </returns>
        public Vector3 GetGyro(int controllerIndex)
        {
            NativeVector3f vec3f = new NativeVector3f();
            NativeResult result = NativeApi.NRControllerStateGetGyro(m_StateHandles[controllerIndex], ref vec3f);
            if (result == NativeResult.Success)
                return vec3f.ToUnityVector3();
            return Vector3.zero;
        }

        /// <summary> Gets an accel. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> The accel. </returns>
        public Vector3 GetAccel(int controllerIndex)
        {
            NativeVector3f vec3f = new NativeVector3f();
            NativeResult result = NativeApi.NRControllerStateGetAccel(m_StateHandles[controllerIndex], ref vec3f);
            if (result == NativeResult.Success)
                return vec3f.ToUnityVector3();
            return Vector3.zero;
        }

        /// <summary> Gets a magnitude. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> The magnitude. </returns>
        public Vector3 GetMag(int controllerIndex)
        {
            NativeVector3f vec3f = new NativeVector3f();
            NativeResult result = NativeApi.NRControllerStateGetMag(m_StateHandles[controllerIndex], ref vec3f);
            if (result == NativeResult.Success)
                return vec3f.ToUnityVector3();
            return Vector3.zero;
        }

        /// <summary> Gets button state. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> The button state. </returns>
        public uint GetButtonState(int controllerIndex)
        {
            uint buttonPress = 0;
            NativeApi.NRControllerStateGetButtonState(m_StateHandles[controllerIndex], ref buttonPress);
            return buttonPress;
        }

        /// <summary> Gets button up. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> The button up. </returns>
        public uint GetButtonUp(int controllerIndex)
        {
            uint buttonUp = 0;
            NativeApi.NRControllerStateGetButtonUp(m_StateHandles[controllerIndex], ref buttonUp);
            return buttonUp;
        }

        /// <summary> Gets button down. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> The button down. </returns>
        public uint GetButtonDown(int controllerIndex)
        {
            uint buttonDown = 0;
            NativeApi.NRControllerStateGetButtonDown(m_StateHandles[controllerIndex], ref buttonDown);
            return buttonDown;
        }

        /// <summary> Query if 'controllerIndex' is touching. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> True if touching, false if not. </returns>
        public bool IsTouching(int controllerIndex)
        {
            uint touchState = 0;
            NativeApi.NRControllerStateTouchState(m_StateHandles[controllerIndex], ref touchState);
            return touchState == 1;
        }

        /// <summary> Gets touch up. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetTouchUp(int controllerIndex)
        {
            uint touchUp = 0;
            NativeApi.NRControllerStateGetTouchUp(m_StateHandles[controllerIndex], ref touchUp);
            return touchUp == 1;
        }

        /// <summary> Gets touch down. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetTouchDown(int controllerIndex)
        {
            uint touchDown = 0;
            NativeApi.NRControllerStateGetTouchDown(m_StateHandles[controllerIndex], ref touchDown);
            return touchDown == 1;
        }

        /// <summary> Gets a touch. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> The touch. </returns>
        public Vector2 GetTouch(int controllerIndex)
        {
            NativeVector2f touchPos = new NativeVector2f();
            NativeResult result = NativeApi.NRControllerStateGetTouchPose(m_StateHandles[controllerIndex], ref touchPos);
            if (result == NativeResult.Success)
                return touchPos.ToUnityVector2();
            return Vector3.zero;
        }

        /// <summary> Updates the head pose described by hmdPose. </summary>
        /// <param name="hmdPose"> The hmd pose.</param>
        public void UpdateHeadPose(Pose hmdPose)
        {
            NativeMat4f apiPose;
            ConversionUtility.UnityPoseToApiPose(hmdPose, out apiPose);
            NativeApi.NRControllerSetHeadPose(m_ControllerHandle, ref apiPose);
        }

        /// <summary> Gets a version. </summary>
        /// <param name="controllerIndex"> Zero-based index of the controller.</param>
        /// <returns> The version. </returns>
        public string GetVersion(int controllerIndex)
        {
            if (m_ControllerHandle == 0)
            {
                return "";
            }

            byte[] bytes = new byte[128];
            var result = NativeApi.NRControllerGetVersion(m_ControllerHandle, controllerIndex, bytes, bytes.Length);
            if (result == NativeResult.Success)
            {
                return System.Text.Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            }
            else
            {
                return "";
            }
        }

        public HandednessType GetHandednessType()
        {
            HandednessType handedness_type = HandednessType.RIGHT_HANDEDNESS;
            var result = NativeApi.NRControllerGetHandednessType(m_ControllerHandle, ref handedness_type);
            NativeErrorListener.Check(result, this, "GetHandednessType");
            return handedness_type;
        }

        private partial struct NativeApi
        {
            /// <summary> Nr controller create. </summary>
            /// <param name="out_controller_handle"> [in,out] Handle of the out controller.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerCreate(ref UInt64 out_controller_handle);

            /// <summary> Nr controller start. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStart(UInt64 controller_handle);

            /// <summary> Nr controller pause. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerPause(UInt64 controller_handle);

            /// <summary> Nr controller resume. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerResume(UInt64 controller_handle);

            /// <summary> Nr controller stop. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStop(UInt64 controller_handle);

            /// <summary> Nr controller destroy. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerDestroy(UInt64 controller_handle);

            /// <summary> Nr controller get count. </summary>
            /// <param name="controller_handle">    Handle of the controller.</param>
            /// <param name="out_controller_count"> [in,out] Number of out controllers.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerGetCount(UInt64 controller_handle, ref int out_controller_count);

            /// <summary> Nr controller get available features. </summary>
            /// <param name="controller_handle">                 Handle of the controller.</param>
            /// <param name="controller_index">                  Zero-based index of the controller.</param>
            /// <param name="out_controller_available_features"> [in,out] The out controller available
            ///                                                  features.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerGetAvailableFeatures(UInt64 controller_handle, int controller_index, ref uint out_controller_available_features);

            /// <summary> Nr controller get type. </summary>
            /// <param name="controller_handle">   Handle of the controller.</param>
            /// <param name="controller_index">    Zero-based index of the controller.</param>
            /// <param name="out_controller_type"> [in,out] Type of the out controller.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerGetType(UInt64 controller_handle, int controller_index, ref ControllerType out_controller_type);

            /// <summary> Nr controller recenter. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <param name="controller_index">  Zero-based index of the controller.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerRecenter(UInt64 controller_handle, int controller_index);

            /// <summary> Nr controller state create. </summary>
            /// <param name="controller_handle">           Handle of the controller.</param>
            /// <param name="controller_index">            Zero-based index of the controller.</param>
            /// <param name="out_controller_state_handle"> [in,out] Handle of the out controller state.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateCreate(UInt64 controller_handle, int controller_index, ref UInt64 out_controller_state_handle);

            /// <summary> Nr controller state update. </summary>
            /// <param name="controller_state_handle"> Handle of the controller state.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateUpdate(UInt64 controller_state_handle);

            /// <summary> Nr controller state destroy. </summary>
            /// <param name="controller_state_handle"> Handle of the controller state.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateDestroy(UInt64 controller_state_handle);

            /// <summary> duration(nanoseconds), frequency(Hz), amplitude(0.0f ~ 1.0f) </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <param name="controller_index">  Zero-based index of the controller.</param>
            /// <param name="duration">          The duration.</param>
            /// <param name="frequency">         The frequency.</param>
            /// <param name="amplitude">         The amplitude.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerHapticVibrate(UInt64 controller_handle, int controller_index, Int64 duration, float frequency, float amplitude);

            /// <summary> Nr controller state get connection state. </summary>
            /// <param name="controller_state_handle">         Handle of the controller state.</param>
            /// <param name="out_controller_connection_state"> [in,out] State of the out controller
            ///                                                connection.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetConnectionState(UInt64 controller_state_handle, ref ControllerConnectionState out_controller_connection_state);

            /// <summary> Nr controller state get battery level. </summary>
            /// <param name="controller_state_handle">      Handle of the controller state.</param>
            /// <param name="out_controller_battery_level"> [in,out] The out controller battery level.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetBatteryLevel(UInt64 controller_state_handle, ref int out_controller_battery_level);

            /// <summary> Nr controller state get charging. </summary>
            /// <param name="controller_state_handle"> Handle of the controller state.</param>
            /// <param name="out_controller_charging"> [in,out] The out controller charging.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetCharging(UInt64 controller_state_handle, ref int out_controller_charging);

            /// <summary> Nr controller state get pose. </summary>
            /// <param name="controller_state_handle"> Handle of the controller state.</param>
            /// <param name="out_controller_pose">     [in,out] The out controller pose.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetPose(UInt64 controller_state_handle, ref NativeMat4f out_controller_pose);

            /// <summary> Nr controller state get gyro. </summary>
            /// <param name="controller_state_handle"> Handle of the controller state.</param>
            /// <param name="out_controller_gyro">     [in,out] The out controller gyro.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetGyro(UInt64 controller_state_handle, ref NativeVector3f out_controller_gyro);

            /// <summary> Nr controller state get accel. </summary>
            /// <param name="controller_state_handle"> Handle of the controller state.</param>
            /// <param name="out_controller_accel">    [in,out] The out controller accel.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetAccel(UInt64 controller_state_handle, ref NativeVector3f out_controller_accel);

            /// <summary> Nr controller state get magnitude. </summary>
            /// <param name="controller_state_handle"> Handle of the controller state.</param>
            /// <param name="out_controller_mag">      [in,out] The out controller magnitude.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetMag(UInt64 controller_state_handle, ref NativeVector3f out_controller_mag);

            /// <summary> Nr controller state get button state. </summary>
            /// <param name="controller_state_handle">     Handle of the controller state.</param>
            /// <param name="out_controller_button_state"> [in,out] State of the out controller button.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetButtonState(UInt64 controller_state_handle, ref uint out_controller_button_state);

            /// <summary> Nr controller state get button up. </summary>
            /// <param name="controller_state_handle">  Handle of the controller state.</param>
            /// <param name="out_controller_button_up"> [in,out] The out controller button up.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetButtonUp(UInt64 controller_state_handle, ref uint out_controller_button_up);

            /// <summary> Nr controller state get button down. </summary>
            /// <param name="controller_state_handle">    Handle of the controller state.</param>
            /// <param name="out_controller_button_down"> [in,out] The out controller button down.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetButtonDown(UInt64 controller_state_handle, ref uint out_controller_button_down);

            /// <summary> Nr controller state touch state. </summary>
            /// <param name="controller_state_handle">    Handle of the controller state.</param>
            /// <param name="out_controller_touch_state"> [in,out] State of the out controller touch.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateTouchState(UInt64 controller_state_handle, ref uint out_controller_touch_state);

            /// <summary> Nr controller state get touch up. </summary>
            /// <param name="controller_state_handle"> Handle of the controller state.</param>
            /// <param name="out_controller_touch_up"> [in,out] The out controller touch up.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetTouchUp(UInt64 controller_state_handle, ref uint out_controller_touch_up);

            /// <summary> Nr controller state get touch down. </summary>
            /// <param name="controller_state_handle">   Handle of the controller state.</param>
            /// <param name="out_controller_touch_down"> [in,out] The out controller touch down.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetTouchDown(UInt64 controller_state_handle, ref uint out_controller_touch_down);

            /// <summary> Nr controller state get touch pose. </summary>
            /// <param name="controller_state_handle">   Handle of the controller state.</param>
            /// <param name="out_controller_touch_pose"> [in,out] The out controller touch pose.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerStateGetTouchPose(UInt64 controller_state_handle, ref NativeVector2f out_controller_touch_pose);

            /// <summary> Nr controller set head pose. </summary>
            /// <param name="controller_handle">   Handle of the controller.</param>
            /// <param name="out_controller_pose"> [in,out] The out controller pose.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerSetHeadPose(UInt64 controller_handle, ref NativeMat4f out_controller_pose);

            /// <summary> Nr controller get version. </summary>
            /// <param name="controller_handle"> Handle of the controller.</param>
            /// <param name="controller_index">  Zero-based index of the controller.</param>
            /// <param name="out_version">       The out version.</param>
            /// <param name="len">               The length.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerGetVersion(UInt64 controller_handle, int controller_index, byte[] out_version, int len);

            /// <summary> Get the handedness left or right. </summary>
            /// <param name="controller_handle">    The handle of controller state object. </param>
            /// <param name="handedness_type">      The handedness type returned. </param>
            /// <returns>The result of operation.</returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRControllerGetHandednessType(UInt64 controller_handle, ref HandednessType handedness_type);
        };
    }
}
