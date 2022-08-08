/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using AOT;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using static NRKernal.NRDevice;

namespace NRKernal
{
    /// <summary> Glasses event. </summary>
    /// <param name="eventtype"> The eventtype.</param>
    public delegate void GlassesEvent(NRDevice.GlassesEventType eventtype);
    /// <summary> Glasses disconnect event. </summary>
    /// <param name="reason"> The reason.</param>
    public delegate void GlassesDisconnectEvent(GlassesDisconnectReason reason);
    /// <summary> Glassed temporary level changed. </summary>
    /// <param name="level"> The level.</param>
    public delegate void GlassedTempLevelChanged(GlassesTemperatureLevel level);

    public class NRDeviceSubsystemDescriptor : IntegratedSubsystemDescriptor<NRDeviceSubsystem>
    {
        public const string Name = "Subsystem.HMD";
        public override string id => Name;
    }

    #region brightness KeyEvent on NrealLight.
    /// <summary> Values that represent nr brightness key events. </summary>
    public enum NRBrightnessKEYEvent
    {
        /// <summary> An enum constant representing the nr brightness key down option. </summary>
        NR_BRIGHTNESS_KEY_DOWN = 0,
        /// <summary> An enum constant representing the nr brightness key up option. </summary>
        NR_BRIGHTNESS_KEY_UP = 1,
    }

    /// <summary> Brightness key event. </summary>
    /// <param name="key"> The key.</param>
    public delegate void BrightnessKeyEvent(NRBrightnessKEYEvent key);
    /// <summary> Brightness value changed event. </summary>
    /// <param name="value"> The value.</param>
    public delegate void BrightnessValueChangedEvent(int value);

    /// <summary> Callback, called when the nr glasses control brightness key. </summary>
    /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
    /// <param name="key_event">              The key event.</param>
    /// <param name="user_data">              Information describing the user.</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void NRGlassesControlBrightnessKeyCallback(UInt64 glasses_control_handle, int key_event, UInt64 user_data);

    /// <summary> Callback, called when the nr glasses control brightness value. </summary>
    /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
    /// <param name="value">                  The value.</param>
    /// <param name="user_data">              Information describing the user.</param>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void NRGlassesControlBrightnessValueCallback(UInt64 glasses_control_handle, int value, UInt64 user_data);
    #endregion

    public class NRDeviceSubsystem : IntegratedSubsystem<NRDeviceSubsystemDescriptor>
    {
        public static event GlassesEvent OnGlassesStateChanged;
        public static event GlassesDisconnectEvent OnGlassesDisconnect;
        private NativeHMD m_NativeHMD = null;
        private NativeGlassesController m_NativeGlassesController = null;
        private Exception m_InitException = null;
        private static bool m_IsGlassesPlugOut = false;

        public UInt64 NativeGlassesHandler => m_NativeGlassesController.GlassesControllerHandle;
        public UInt64 NativeHMDHandler => m_NativeHMD.HmdHandle;
        public NativeHMD NativeHMD => m_NativeHMD;
        public bool IsAvailable => !m_IsGlassesPlugOut && running && m_InitException == null;


        /// <summary> Glass controll key event delegate for native. </summary>
        private delegate void NRGlassesControlKeyEventCallback(UInt64 glasses_control_handle, UInt64 key_event_handle, UInt64 user_data);
        /// <summary> Event queue for all listeners interested in OnBrightnessKeyCallback events. </summary>
        private static event BrightnessKeyEvent OnBrightnessKeyCallback;
        /// <summary> Event queue for all listeners interested in OnBrightnessValueCallback events. </summary>
        private static event BrightnessValueChangedEvent OnBrightnessValueCallback;
        /// <summary> The brightness minimum. </summary>
        public const int BRIGHTNESS_MIN = 0;
        /// <summary> The brightness maximum. </summary>
        public const int BRIGHTNESS_MAX = 7;

        public NRDeviceSubsystem(NRDeviceSubsystemDescriptor descriptor) : base(descriptor)
        {
            m_NativeGlassesController = new NativeGlassesController();
            m_NativeHMD = new NativeHMD();
        }

        #region LifeCycle
        public override void Start()
        {
            if (running)
            {
                return;
            }

            base.Start();

#if !UNITY_EDITOR
            try
            {
                m_NativeGlassesController.Create();
                m_NativeGlassesController.RegisGlassesWearCallBack(OnGlassesWear, 1);
                m_NativeGlassesController.RegistGlassesEventCallBack(OnGlassesDisconnectEvent);
                m_NativeGlassesController.Start();
                m_NativeHMD.Create();
            }
            catch (Exception e)
            {
                m_InitException = e;
                throw e;
            }
#endif
        }

        /// <summary> Executes the 'glasses wear' action. </summary>
        /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
        /// <param name="wearing_status">         The wearing status.</param>
        /// <param name="user_data">              Information describing the user.</param>
        [MonoPInvokeCallback(typeof(NRGlassesControlWearCallback))]
        private static void OnGlassesWear(UInt64 glasses_control_handle, int wearing_status, UInt64 user_data)
        {
            MainThreadDispather.QueueOnMainThread(() =>
            {
                OnGlassesStateChanged?.Invoke(wearing_status == 1 ? GlassesEventType.PutOn : GlassesEventType.PutOff);
            });
        }

        /// <summary> Executes the 'glasses disconnect event' action. </summary>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs.</exception>
        /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
        /// <param name="user_data">              Information describing the user.</param>
        /// <param name="reason">                 The reason.</param>
        [MonoPInvokeCallback(typeof(NRGlassesControlNotifyQuitAppCallback))]
        private static void OnGlassesDisconnectEvent(UInt64 glasses_control_handle, IntPtr user_data, GlassesDisconnectReason reason)
        {
            if (m_IsGlassesPlugOut)
            {
                return;
            }
            m_IsGlassesPlugOut = true;
            OnGlassesDisconnect?.Invoke(reason);
        }

        public void RegestEvents(GlassesEvent onGlassesWear, GlassesDisconnectEvent onGlassesDisconnectEvent)
        {
            OnGlassesStateChanged += onGlassesWear;
            OnGlassesDisconnect += onGlassesDisconnectEvent;
        }

        public override void Pause()
        {
            if (!running)
            {
                return;
            }

            base.Pause();

#if !UNITY_EDITOR
           m_NativeGlassesController?.Pause();
           m_NativeHMD?.Pause();
#endif
        }

        public override void Resume()
        {
            if (running)
            {
                return;
            }

            base.Resume();
#if !UNITY_EDITOR
            m_NativeGlassesController?.Resume();
            m_NativeHMD?.Resume();
#endif
        }

        public override void Stop()
        {
            if (!running)
            {
                return;
            }

            base.Stop();

#if !UNITY_EDITOR
            m_NativeGlassesController?.Stop();
            m_NativeGlassesController?.Destroy();
            m_NativeHMD.Destroy();
#endif
        }
        #endregion

        #region Glasses
        /// <summary> Gets the temperature level. </summary>
        /// <value> The temperature level. </value>
        public GlassesTemperatureLevel TemperatureLevel
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new NRGlassesNotAvailbleError("Device is not available.");
                }
#if !UNITY_EDITOR
                return m_NativeGlassesController.GetTempratureLevel();
#else
                return GlassesTemperatureLevel.TEMPERATURE_LEVEL_NORMAL;
#endif
            }
        }
        #endregion

        #region HMD
        public NRDeviceType GetDeviceType()
        {
            if (!IsAvailable)
            {
                throw new NRGlassesNotAvailbleError("Device is not available.");
            }

#if !UNITY_EDITOR
            return m_NativeHMD.GetDeviceType();
#else
            return NRDeviceType.NrealLight;
#endif
        }

        public bool IsFeatureSupported(NRSupportedFeature feature)
        {
            if (!IsAvailable)
            {
                throw new NRGlassesNotAvailbleError("Device is not available.");
            }

#if !UNITY_EDITOR
            return m_NativeHMD.IsFeatureSupported(feature);
#else
            return true;
#endif
        }

        /// <summary> Gets the resolution of device. </summary>
        /// <param name="eye"> device index.</param>
        /// <returns> The device resolution. </returns>
        public NativeResolution GetDeviceResolution(NativeDevice device)
        {
            if (!IsAvailable)
            {
                throw new NRGlassesNotAvailbleError("Device is not available.");
            }
#if !UNITY_EDITOR
            return m_NativeHMD.GetEyeResolution((int)device);
#else
            return new NativeResolution(1920, 1080);
#endif
        }

        /// <summary> Gets device fov. </summary>
        /// <param name="eye">         The display index.</param>
        /// <param name="fov"> [in,out] The out device fov.</param>
        /// <returns> A NativeResult. </returns>
        public void GetEyeFov(NativeDevice device, ref NativeFov4f fov)
        {
            if (!IsAvailable || (device != NativeDevice.LEFT_DISPLAY && device != NativeDevice.RIGHT_DISPLAY))
            {
                throw new NRGlassesNotAvailbleError("Device is not available.");
            }
#if !UNITY_EDITOR
            fov = m_NativeHMD.GetEyeFovInCoord(device);
#else
            fov = new NativeFov4f(0, 0, 1, 1);
#endif
        }

        /// <summary> Get the intrinsic matrix of device. </summary>
        /// <returns> The device intrinsic matrix. </returns>
        public NRDistortionParams GetDeviceDistortion(NativeDevice device)
        {
            if (!IsAvailable)
            {
                throw new NRGlassesNotAvailbleError("Device is not available.");
            }
            NRDistortionParams result = new NRDistortionParams();
#if !UNITY_EDITOR
            m_NativeHMD.GetCameraDistortion((int)device, ref result);
#endif
            return result;
        }

        /// <summary> Get the intrinsic matrix of device. </summary>
        /// <returns> The device intrinsic matrix. </returns>
        public NativeMat3f GetDeviceIntrinsicMatrix(NativeDevice device)
        {
            if (!IsAvailable)
            {
                throw new NRGlassesNotAvailbleError("Device is not available.");
            }
            NativeMat3f result = new NativeMat3f();
#if !UNITY_EDITOR
             m_NativeHMD.GetCameraIntrinsicMatrix((int)device, ref result);
#endif
            return result;
        }

        /// <summary> Get the project matrix of camera in unity. </summary>
        /// <param name="result"> [out] True to result.</param>
        /// <param name="znear">  The znear.</param>
        /// <param name="zfar">   The zfar.</param>
        /// <returns> project matrix of camera. </returns>
        public EyeProjectMatrixData GetEyeProjectMatrix(out bool result, float znear, float zfar)
        {
            if (!IsAvailable)
            {
                throw new NRGlassesNotAvailbleError("Device is not available.");
            }
            result = false;
            EyeProjectMatrixData m_EyeProjectMatrix = new EyeProjectMatrixData();
#if !UNITY_EDITOR
            result = m_NativeHMD.GetProjectionMatrix(ref m_EyeProjectMatrix, znear, zfar);
#endif
            return m_EyeProjectMatrix;
        }

        /// <summary> Get the offset position between device and head. </summary>
        /// <value> The device pose from head. </value>
        public Pose GetDevicePoseFromHead(NativeDevice device)
        {
            if (!IsAvailable)
            {
                throw new NRGlassesNotAvailbleError("Device is not available.");
            }
#if !UNITY_EDITOR
            return m_NativeHMD.GetDevicePoseFromHead(device);
#else
            return Pose.identity;
#endif
        }
        #endregion



        #region brightness KeyEvent on NrealLight.
        /// <summary> Adds an event listener to 'callback'. </summary>
        /// <param name="callback"> The callback.</param>
        public void AddEventListener(BrightnessKeyEvent callback)
        {
            OnBrightnessKeyCallback += callback;
        }

        /// <summary>
        /// Adds an event listener to 'callback'. </summary>
        /// <param name="callback"> The callback.</param>
        public void AddEventListener(BrightnessValueChangedEvent callback)
        {
            OnBrightnessValueCallback += callback;
        }

        /// <summary> Removes the event listener. </summary>
        /// <param name="callback"> The callback.</param>
        public void RemoveEventListener(BrightnessKeyEvent callback)
        {
            OnBrightnessKeyCallback -= callback;
        }

        /// <summary> Removes the event listener. </summary>
        /// <param name="callback"> The callback.</param>
        public void RemoveEventListener(BrightnessValueChangedEvent callback)
        {
            OnBrightnessValueCallback -= callback;
        }


        /// <summary> Gets the brightness. </summary>
        /// <returns> The brightness. </returns>
        public int GetBrightness()
        {
            if (!IsAvailable)
            {
                return -1;
            }

#if !UNITY_EDITOR
            int brightness = -1;
            var result = NativeApi.NRGlassesControlGetBrightness(NativeGlassesHandler, ref brightness);
            return result == NativeResult.Success ? brightness : -1;
#else
            return 0;
#endif
        }

        /// <summary> Sets the brightness. </summary>
        /// <param name="brightness">        The brightness.</param>
        public void SetBrightness(int brightness)
        {
            if (!IsAvailable)
            {
                return;
            }

            AsyncTaskExecuter.Instance.RunAction(() =>
            {
#if !UNITY_EDITOR
                NativeApi.NRGlassesControlSetBrightness(NativeGlassesHandler, brightness);
#endif
            });
        }


        /// <summary> Executes the 'brightness key callback internal' action. </summary>
        /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
        /// <param name="key_event">              The key event.</param>
        /// <param name="user_data">              Information describing the user.</param>
        [MonoPInvokeCallback(typeof(NRGlassesControlBrightnessKeyCallback))]
        private static void OnBrightnessKeyCallbackInternal(UInt64 glasses_control_handle, int key_event, UInt64 user_data)
        {
            OnBrightnessKeyCallback?.Invoke((NRBrightnessKEYEvent)key_event);
        }

        /// <summary> Executes the 'brightness value callback internal' action. </summary>
        /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
        /// <param name="brightness">             The brightness.</param>
        /// <param name="user_data">              Information describing the user.</param>
        [MonoPInvokeCallback(typeof(NRGlassesControlBrightnessValueCallback))]
        private static void OnBrightnessValueCallbackInternal(UInt64 glasses_control_handle, int brightness, UInt64 user_data)
        {
            OnBrightnessValueCallback?.Invoke(brightness);
        }

        /// <summary> Event queue for all listeners interested in KeyEvent events. </summary>
        private static event NRGlassControlKeyEvent OnKeyEventCallback;

        /// <summary>
        /// Adds an key event listener. </summary>
        /// <param name="callback"> The callback.</param>
        public void AddKeyEventListener(NRGlassControlKeyEvent callback)
        {
            OnKeyEventCallback += callback;
        }

        /// <summary> Removes the key event listener. </summary>
        /// <param name="callback"> The callback.</param>
        public void RemoveKeyEventListener(NRGlassControlKeyEvent callback)
        {
            OnKeyEventCallback -= callback;
        }

        /// <summary> Executes the 'key event callback internal' action. </summary>
        /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
        /// <param name="key_event_handle">       Handle of the key event.</param>
        /// <param name="user_data">              Information describing the user.</param>
        [MonoPInvokeCallback(typeof(NRGlassesControlKeyEventCallback))]
        private static void OnKeyEventCallbackInternal(UInt64 glasses_control_handle, UInt64 key_event_handle, UInt64 user_data)
        {
            int keyType = 0;
            int keyFunc = 0;
            int keyParam = 0;

#if !UNITY_EDITOR
            NativeResult result = NativeApi.NRGlassesControlKeyEventGetType(glasses_control_handle, key_event_handle, ref keyType);
            if (result == NativeResult.Success)
                result = NativeApi.NRGlassesControlKeyEventGetFunction(glasses_control_handle, key_event_handle, ref keyFunc);
            if (result == NativeResult.Success)
                result = NativeApi.NRGlassesControlKeyEventGetParam(glasses_control_handle, key_event_handle, ref keyParam);
#endif
            NRKeyEventInfo keyEvtInfo = new NRKeyEventInfo();
            keyEvtInfo.keyType = (NRKeyType)keyType;
            keyEvtInfo.keyFunc = (NRKeyFunction)keyFunc;
            keyEvtInfo.keyParam = keyParam;

            OnKeyEventCallback?.Invoke(keyEvtInfo);
        }

        /// <summary>
        /// Regis glasses controller extra callbacks. </summary>
        public void RegisGlassesControllerExtraCallbacks()
        {
            if (!IsAvailable)
            {
                NRDebugger.Warning("[NRDevice] Can not regist event when glasses disconnect...");
                return;
            }

#if !UNITY_EDITOR
            NativeApi.NRGlassesControlSetBrightnessKeyCallback(NativeGlassesHandler, OnBrightnessKeyCallbackInternal, 0);
            NativeApi.NRGlassesControlSetBrightnessValueCallback(NativeGlassesHandler, OnBrightnessValueCallbackInternal, 0);
            NativeApi.NRGlassesControlSetKeyEventCallback(NativeGlassesHandler, OnKeyEventCallbackInternal, 0);
#endif
        }
        #endregion


        private struct NativeApi
        {
            /// <summary> Nr glasses control get brightness. </summary>
            /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
            /// <param name="out_brightness">         [in,out] The out brightness.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRGlassesControlGetBrightness(UInt64 glasses_control_handle, ref int out_brightness);

            /// <summary> Nr glasses control set brightness. </summary>
            /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
            /// <param name="brightness">             The brightness.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRGlassesControlSetBrightness(UInt64 glasses_control_handle, int brightness);


            /// <summary> Callback, called when the nr glasses control set brightness key. </summary>
            /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
            /// <param name="callback">               The callback.</param>
            /// <param name="user_data">              Information describing the user.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern NativeResult NRGlassesControlSetBrightnessKeyCallback(UInt64 glasses_control_handle, NRGlassesControlBrightnessKeyCallback callback, UInt64 user_data);

            /// <summary> Callback, called when the nr glasses control set brightness value. </summary>
            /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
            /// <param name="callback">               The callback.</param>
            /// <param name="user_data">              Information describing the user.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern NativeResult NRGlassesControlSetBrightnessValueCallback(UInt64 glasses_control_handle, NRGlassesControlBrightnessValueCallback callback, UInt64 user_data);

            /// <summary> Registe the callback when the nr glasses control issue key event. </summary>
            /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
            /// <param name="callback">               The called.</param>
            /// <param name="user_data">              Information describing the user.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern NativeResult NRGlassesControlSetKeyEventCallback(UInt64 glasses_control_handle, NRGlassesControlKeyEventCallback callback, UInt64 user_data);

            /// <summary> Get key type of key event. </summary>
            /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
            /// <param name="key_event_handle">       Handle of key event.</param>
            /// <param name="out_key_event_type">     Key type retrieved.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern NativeResult NRGlassesControlKeyEventGetType(UInt64 glasses_control_handle, UInt64 key_event_handle, ref int out_key_event_type);

            /// <summary> Get key function of key event. </summary>
            /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
            /// <param name="key_event_handle">       Handle of key event.</param>
            /// <param name="out_key_event_type">     Key funtion retrieved.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern NativeResult NRGlassesControlKeyEventGetFunction(UInt64 glasses_control_handle, UInt64 key_event_handle, ref int out_key_event_function);

            /// <summary> Get key parameter of key event. </summary>
            /// <param name="glasses_control_handle"> Handle of the glasses control.</param>
            /// <param name="key_event_handle">       Handle of key event.</param>
            /// <param name="out_key_event_type">     Key parameter retrieved.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern NativeResult NRGlassesControlKeyEventGetParam(UInt64 glasses_control_handle, UInt64 key_event_handle, ref int out_key_event_param);

            
        }
    }
}