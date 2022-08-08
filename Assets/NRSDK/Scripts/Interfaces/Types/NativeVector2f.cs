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

    /// <summary> A native vector 2f. </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeVector2f
    {
        /// <summary> The X coordinate. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float X;
        /// <summary> The Y coordinate. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float Y;

        /// <summary> Constructor. </summary>
        /// <param name="v"> A Vector2 to process.</param>
        public NativeVector2f(Vector2 v)
        {
            X = v.x;
            Y = v.y;
        }
        /// <summary> Converts this object to an unity vector 2. </summary>
        /// <returns> This object as a Vector2. </returns>
        public Vector2 ToUnityVector2()
        {
            return new Vector2(X, Y);
        }
        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            return string.Format("x:{0}, y:{1}", X, Y);
        }
    }
}
