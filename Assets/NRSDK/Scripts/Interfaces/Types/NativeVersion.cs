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

    /// <summary> A nr version. </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NRVersion
    {
        /// <summary> The major. </summary>
        [MarshalAs(UnmanagedType.I4)]
        int major;
        /// <summary> The minor. </summary>
        [MarshalAs(UnmanagedType.I4)]
        int minor;
        /// <summary> The revision. </summary>
        [MarshalAs(UnmanagedType.I4)]
        int revision;

        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", major, minor, revision);
        }
    }
}
