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
    /// <summary> A native error listener. </summary>
    public class NativeErrorListener
    {
        /// <summary> Checks. </summary>
        /// <exception cref="NRKernalError">                 Raised when a NR Kernal error condition
        ///                                                  occurs.</exception>
        /// <exception cref="NRInvalidArgumentError">        Raised when a NR Invalid Argument error
        ///                                                  condition occurs.</exception>
        /// <exception cref="NRNotEnoughMemoryError">        Raised when a NR Not Enough Memory error
        ///                                                  condition occurs.</exception>
        /// <exception cref="NRUnSupportedError">            Raised when a NR Un Supported error condition
        ///                                                  occurs.</exception>
        /// <exception cref="NRGlassesConnectError">         Raised when a NR Glasses Connect error
        ///                                                  condition occurs.</exception>
        /// <exception cref="NRSdkVersionMismatchError">     Raised when a NR Sdk Version Mismatch error
        ///                                                  condition occurs.</exception>
        /// <exception cref="NRSdcardPermissionDenyError">   Raised when a NR Sdcard Permission Deny error
        ///                                                  condition occurs.</exception>
        /// <exception cref="NRRGBCameraDeviceNotFindError"> Raised when a NRRGB Camera Device Not Find
        ///                                                  error condition occurs.</exception>
        /// <exception cref="NRDPDeviceNotFindError">        Raised when the Display device not find
        ///                                                  error condition occurs.</exception>
        /// <exception cref="NRGetDisplayFailureError">      Raised when the MRSpace Display device not find
        ///                                                  error condition occurs.</exception>
        /// <exception cref="NRDisplayModeMismatchError">    Raised when display mode mismatch, as MRSpace mode is needed
        ///                                                  condition occurs.</exception>
        /// <exception cref="Exception">                     Thrown when an exception error condition
        ///                                                  occurs.</exception>
        /// <param name="result">         The result.</param>
        /// <param name="module">         The module.</param>
        /// <param name="funcName">       (Optional) Name of the function.</param>
        /// <param name="needthrowerror"> (Optional) True to needthrowerror.</param>
        public static void Check(NativeResult result, object module, string funcName = "", bool needthrowerror = false)
        {
#if !UNITY_EDITOR
            if (result == NativeResult.Success)
            {
                return;
            }

            string module_tag = string.Format("[{0}] {1}: ", module.GetType().Name, funcName);
            if (needthrowerror)
            {
                try
                {
                    switch (result)
                    {
                        case NativeResult.Failure:
                            throw new NRNativeError(result, module_tag + "Failed!");
                        case NativeResult.InvalidArgument:
                            throw new NRInvalidArgumentError(result, module_tag + "InvalidArgument error!");
                        case NativeResult.NotEnoughMemory:
                            throw new NRNotEnoughMemoryError(result, module_tag + "NotEnoughMemory error!");
                        case NativeResult.UnSupported:
                            throw new NRUnSupportedError(result, module_tag + "UnSupported error!");
                        case NativeResult.GlassesDisconnect:
                            throw new NRGlassesConnectError(result, module_tag + "Glasses connect error!");
                        case NativeResult.SdkVersionMismatch:
                            throw new NRSdkVersionMismatchError(result, module_tag + "SDK version mismatch error!");
                        case NativeResult.SdcardPermissionDeny:
                            throw new NRSdcardPermissionDenyError(result, module_tag + "Sdcard permission deny error!");
                        case NativeResult.RGBCameraDeviceNotFind:
                            throw new NRRGBCameraDeviceNotFindError(result, module_tag + "Can not find the rgb camera device error!");
                        case NativeResult.DPDeviceNotFind:
                            throw new NRDPDeviceNotFindError(result, module_tag + "Display device Not Find!");
                        case NativeResult.GetDisplayFailure:
                            throw new NRGetDisplayFailureError(result, module_tag + "MRSpace display device Not Find!");
                        case NativeResult.GetDisplayModeMismatch:
                            throw new NRDisplayModeMismatchError(result, module_tag + "Display mode mismatch, as MRSpace mode is needed!");
                        case NativeResult.UnSupportedHandtrackingCalculation:
                            throw new NRUnSupportedHandtrackingCalculationError(result, module_tag + "Not support hand tracking calculation!");
                        default:
                            break;
                    }
                }
                catch (System.Exception e)
                {
                    MainThreadDispather.QueueOnMainThread(() =>
                    {
                        NRSessionManager.Instance.OprateInitException(e);
                    });

                    // Normal level Error don't need to throw.
                    if ((e is NRKernalError) && ((e as NRKernalError).level == Level.High))
                    {
                        throw e;
                    }
                }
            }
            else
            {
                NRDebugger.Error(module_tag + result.ToString());
            }
#endif
        }
    }
}
