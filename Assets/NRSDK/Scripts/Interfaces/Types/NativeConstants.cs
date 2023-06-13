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

    /// <summary> A native constants. </summary>
    public class NativeConstants
    {
        /// <summary> The nrhandlenull. </summary>
        public const UInt64 NRHANDLENULL = 0;

        /// <summary> The illegal int. </summary>
        public const UInt32 IllegalInt = 0;

        public const string VersionCode = "1.5.12";

#if  UNITY_EDITOR
        /// <summary> The nr native library. </summary>
        public const string NRNativeLibrary = "libnr_api_editor";
#elif UNITY_STANDALONE_WIN
        public const string NRNativeLibrary = "libnr_api";
#else
        public const string NRNativeLibrary = "nr_api";
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
        public const string NRNativeAnchorPointLib = "nr_tracking";
#else
        public const string NRNativeAnchorPointLib = "libnr_tracking";
#endif

#if UNITY_EDITOR_OSX
        public static string TrackingImageCliBinary = "trackableImageTools_osx";
#elif UNITY_EDITOR_WIN
        /// <summary> The tracking image CLI binary. </summary>
        public static string TrackingImageCliBinary = "trackableImageTools_win";
#elif UNITY_EDITOR_LINUX
        public static string TrackingImageCliBinary = "trackableImageTools_linux";
#endif

        /// <summary> The read external storage permission. </summary>
        public const string READ_EXTERNAL_STORAGE_PERMISSION = "android.permission.READ_EXTERNAL_STORAGE";

        /// <summary> The zip key. </summary>
        public static string ZipKey = "89f55314-6d41-416c-b4d9-4bdbc155e576";

        #region error tips
        /// <summary> The glasses disconnect error tip. </summary>
        public static string GlassesDisconnectErrorTip = "Please connect your Glasses.";
        /// <summary> The glasses disconnect error tip. </summary>
        public static string GlassesNotAvailbleErrorTip = "Please make sure your Glasses is available";
        /// <summary> The sdk version mismatch error tip. </summary>
        public static string SdkVersionMismatchErrorTip = "Please update to the latest version of NRSDK.";
        /// <summary> The sdcard permission deny error tip. </summary>
        public static string SdcardPermissionDenyErrorTip = "There is no read permission for sdcard. Please go to the authorization management page of the device to authorize.";
        /// <summary> The unknow error tip. </summary>
        public static string UnknowErrorTip = "Unkown error! \nPlease contact customer service.";
        /// <summary> The error tip of running on unsupported device. </summary>
        public static string UnSupportDeviceErrorTip = "App doesn't suppport to run on current glass! \nPlease contact App developer's customer service.";
        /// <summary> The error tip of running on unsupported device. </summary>
        public static string DPDeviceNotFindErrorTip = "Glasses display device not find! \nPlease contact customer service.";
        /// <summary> The error tip of running on unsupported device. </summary>
        public static string GetDisplayFailureErrorTip = "MRSpace display device not find! \nPlease contact customer service.";
        /// <summary> The error tip of running on unsupported device. </summary>
        public static string DisplayModeMismatchErrorTip = "Display mode mismatch, as MRSpace mode is needed! \nPlease contact customer service.";
        public static string AudioPermissionDenyErrorTip = "Record audio needs the permission of \n" +
                            "'android.permission.RECORD_AUDIO', Add it to the 'AndroidManifest.xml'";
        public static string ScreenCaptureDenyErrorTip = "Screen capture needs to be approved.";
        public static string RGBCameraDeviceNotFindErrorTip = "Please check the FAQ on the official website to enable the RGB Camera.";
        public static string UnSupportedHandtrackingCalculationErrorTip = "The device in use does not support hand tracking calculation.";
        #endregion

        /// <summary> The notification level warning. </summary>
        public static string NotificationLevelWarning = "Warning";

        #region lost tracking tips
        public static string TRACKING_MODE_SWITCH_TIP = "Tracking systerm is switching mode...";
        #endregion

        #region script order
        public const int NRINPUT_ORDER = -50;
        public const int NRSESSIONBEHAVIOUR_ORDER = -100;
        public const int NRVIRTUALDISPLAY_ORDER = -200;
        public const int NRRENDER_ORDER = -300;
        public const int NRKERNALUPDATER_ORDER = -1100;
        #endregion

        #region settings
        public const int RECORD_FPS_DEFAULT = 30;
        public const int RECORD_VIDEO_BITRATE_DEFAULT = 10240000;
        public const int RECORD_AUDIO_BITRATE_DEFAULT = 128000;
        public const int RECORD_AUDIO_SAMPLERATE_DEFAULT = 16000;
        public const int RECORD_AUDIO_BYTES_PER_SAMPLE = 2;
        public const int RECORD_AUDIO_CHANNEL = 1;
        public const float RECORD_VOLUME_MIC = 5;
        public const float RECORD_VOLUME_APP = 1;
        #endregion

        #region XR
        public const string XRPLUGIN = "com.nreal.xr";
        public const string XRPLUGIN_MIN_VERSION = "0.1.0";
        public const string XRDEFINE = "USING_XR_SDK";
        public const string NRSDK_Assembly = "NRSDK.asmdef";
        #endregion
    }
}
