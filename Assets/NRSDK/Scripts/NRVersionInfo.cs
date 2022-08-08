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
    /// <summary> Holds information about Nreal SDK version info. </summary>
    public class NRVersionInfo
    {
        /// <summary> Gets the version. </summary>
        /// <returns> The version. </returns>
        public static string GetVersion()
        {
            return NativeVersion.GetVersion();
        }
    }
}
