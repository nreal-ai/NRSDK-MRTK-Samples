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
    using UnityEngine;

    /// <summary> A native vector 4f. </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeVector4f
    {
        /// <summary> The X coordinate. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float X;
        /// <summary> The Y coordinate. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float Y;
        /// <summary> The Z coordinate. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float Z;
        /// <summary> The width. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float W;

        /// <summary> Constructor. </summary>
        /// <param name="v"> A Vector4 to process.</param>
        public NativeVector4f(Vector4 v)
        {
            X = v.x;
            Y = v.y;
            Z = v.z;
            W = v.w;
        }

        public NativeVector4f(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary> Converts this object to an unity vector 4. </summary>
        /// <returns> This object as a Vector4. </returns>
        public Vector4 ToUnityVector4()
        {
            return new Vector4(X, Y, Z, W);
        }

        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", X, Y, Z, W);
        }
    }
}
