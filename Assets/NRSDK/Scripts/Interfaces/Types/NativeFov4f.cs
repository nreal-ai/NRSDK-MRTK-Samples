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

    /// <summary> A native fov 4f. </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeFov4f
    {
        /// <summary>
        /// The tangent of the angle between the viewing vector and the left edge of the field of view.
        /// The value is positive. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float left_tan;

        /// <summary>
        /// The tangent of the angle between the viewing vector and the right edge of the field of view.
        /// The value is positive. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float right_tan;

        /// <summary>
        /// The tangent of the angle between the viewing vector and the top edge of the field of view.
        /// The value is positive. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float top_tan;

        /// <summary>
        /// The tangent of the angle between the viewing vector and the bottom edge of the field of view.
        /// The value is positive. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float bottom_tan;

        public NativeFov4f(float left, float right, float top, float bottom)
        {
            left_tan = left;
            right_tan = right;
            top_tan = top;
            bottom_tan = bottom;
        }

        public float[] ToXRFloats()
        {
            return new float[] {
                left_tan,
                right_tan,
                bottom_tan,
                top_tan,
            };
        }

        public override string ToString()
        {
            return string.Format("left_tan:{0}, right_tan:{1}, top_tan:{2}, bottom_tan:{3}",
                left_tan, right_tan, top_tan, bottom_tan);
        }
    }
}