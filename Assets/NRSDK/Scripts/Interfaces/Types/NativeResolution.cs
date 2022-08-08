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

    /// <summary> A native resolution. </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeResolution
    {
        /// <summary> The width. </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int width;
        /// <summary> The height. </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int height;

        /// <summary> Constructor. </summary>
        /// <param name="w"> The width.</param>
        /// <param name="h"> The height.</param>
        public NativeResolution(int w, int h)
        {
            this.width = w;
            this.height = h;
        }

        public NativeResolution(UnityEngine.Vector2 size)
        {
            this.width = (int)size.x;
            this.height = (int)size.y;
        }

        public static NativeResolution operator *(NativeResolution val, float aspect)
        {
            return new NativeResolution((int)(aspect * val.width), (int)(aspect * val.height));
        }

        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            return string.Format("width:{0} height:{1}", width, height);
        }
    }
}
