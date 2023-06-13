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
    using System.Runtime.InteropServices;

    /// <summary> 6-dof Trackable Image Tracking's Native API . </summary>
    internal partial class NativeVersion
    {
        /// <summary> Gets the version. </summary>
        /// <returns> The version. </returns>
        public static string GetVersion()
        {
            NRVersion version = new NRVersion();
            NativeApi.NRGetVersion(ref version);
            return version.ToString();
        }

        private partial struct NativeApi
        {
            /// <summary> Nr get version. </summary>
            /// <param name="out_version"> [in,out] The out version.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRGetVersion(ref NRVersion out_version);
        };
    }
}
