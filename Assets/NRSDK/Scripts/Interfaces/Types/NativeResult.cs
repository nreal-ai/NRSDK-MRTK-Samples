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
    /// <summary> Values that represent native results. </summary>
    public enum NativeResult
    {
        /// <summary> Success. </summary>
        Success = 0,

        /// <summary> Failed. </summary>
        Failure = 1,

        /// <summary> Ivalid argument error. </summary>
        InvalidArgument = 2,

        /// <summary> Memory is not enough. </summary>
        NotEnoughMemory = 3,

        /// <summary> Unsupported error. </summary>
        UnSupported = 4,

        /// <summary> Glasses diconnect error. </summary>
        GlassesDisconnect = 5,

        /// <summary> SDK version is not matched with server. </summary>
        SdkVersionMismatch = 6,

        /// <summary> Sdcard read permission is denied. </summary>
        SdcardPermissionDeny = 7,

        /// <summary> RGB camera is not found. </summary>
        RGBCameraDeviceNotFind = 8,

        /// <summary> DP device is not found. </summary>
        DPDeviceNotFind = 9,

        /// <summary> Tracking system is not running. </summary>
        TrackingNotRunning = 10,

        /// <summary> Get glasses display failed. </summary>
        GetDisplayFailure = 11,

        /// <summary> Glasses display mode is not 3d. </summary>
        GetDisplayModeMismatch = 12,

        /// <summary> Not support hand tracking calculation. </summary>
        UnSupportedHandtrackingCalculation = 14
    }
}
