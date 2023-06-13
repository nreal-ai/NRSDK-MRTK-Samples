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
    using System.Runtime.InteropServices;
    using UnityEngine;

    public enum TextureType
    {
        RGB,
        YUV
    }

    /// <summary> A camera texture frame. </summary>
    public struct CameraTextureFrame
    {
        /// <summary> The time stamp. </summary>
        public UInt64 timeStamp;
        /// <summary> The gain </summary>
        public UInt32 gain;
        /// <summary> The exposureTime </summary>
        public UInt32 exposureTime;
        /// <summary> The texture. </summary>
        public Texture texture;
    }

    public struct UniversalTextureFrame
    {
        public TextureType textureType;
        /// <summary> The time stamp. </summary>
        public UInt64 timeStamp;
        /// <summary> The gain </summary>
        public UInt32 gain;
        /// <summary> The exposureTime </summary>
        public UInt32 exposureTime;
        /// <summary> The textures. </summary>
        public Texture[] textures;
    }

    /// <summary> A frame raw data. </summary>
    public partial struct FrameRawData
    {
        /// <summary> The time stamp. </summary>
        public UInt64 timeStamp;
        /// <summary> The gain </summary>
        public UInt32 gain;
        /// <summary> The exposureTime </summary>
        public UInt32 exposureTime;
        /// <summary> The data. </summary>
        public byte[] data;
        public IntPtr nativeTexturePtr;

        /// <summary> Makes a safe. </summary>
        /// <param name="textureptr"> The textureptr.</param>
        /// <param name="size">       The size.</param>
        /// <param name="timestamp">  The timestamp.</param>
        /// <param name="frame">      [in,out] The frame.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool MakeSafe(IntPtr textureptr, int size, UInt64 timestamp, ref FrameRawData frame)
        {
            if (textureptr == IntPtr.Zero || size <= 0)
            {
                return false;
            }
            if (frame.data == null || frame.data.Length != size)
            {
                frame.data = new byte[size];
            }
            frame.timeStamp = timestamp;
            frame.gain = 0;
            frame.exposureTime = 0;
            frame.nativeTexturePtr = textureptr;
            Marshal.Copy(textureptr, frame.data, 0, size);
            return true;
        }

        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            return string.Format("timestamp :{0} data size:{1}", timeStamp, data.Length);
        }
    }
}
