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
    using UnityEngine;
    using System.Runtime.InteropServices;


    public enum NRFrameFlags
    {
        NR_FRAME_CHANGED_FOCUS_PLANE = 1 << 0,
        NR_FRAME_CHANGED_NONE = 0
    };

    public enum NRTextureType
    {
        NR_TEXTURE_2D = 0,
        NR_TEXTURE_2D_ARRAY,
        NR_TEXTURE_TYPE_NUM
    };

    public enum FrameRateMode
    {
        NR_FPS_30 = 1,
        NR_UNLIMITED = 2
    };

    public struct SwitchModeFrameInfo
    {
        [MarshalAs(UnmanagedType.Bool)]
        public bool flag;
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr renderTexture;
    }

    public enum NRRenderingFlags
    {
        NR_RENDERING_FLAGS_MULTI_THREAD = 1 << 0,
        NR_RENDERING_FLAGS_NONE = 0,
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct FrameInfo
    {
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr leftTex;
        [MarshalAs(UnmanagedType.SysInt)]
        public IntPtr rightTex;
        [MarshalAs(UnmanagedType.Struct)]
        public NativeMat4f headPose;
        [MarshalAs(UnmanagedType.Struct)]
        public NativeVector3f focusPosition;
        [MarshalAs(UnmanagedType.Struct)]
        public NativeVector3f normalPosition;
        ///Time for the frame to present on screen
        [MarshalAs(UnmanagedType.U8)]
        public UInt64 presentTime;
        /// Bitfield representing NRFrameChanged fields changed last frame.  Combination of #NRFrameChanged.
        [MarshalAs(UnmanagedType.U4)]
        public NRFrameFlags changeFlag;
        [MarshalAs(UnmanagedType.U4)]
        public NRTextureType textureType;
        // local cache for frameHandle
        [MarshalAs(UnmanagedType.U8)]
        public UInt64 frameHandle;
        public FrameInfo(IntPtr left, IntPtr right, NativeMat4f p, Vector3 focuspos, Vector3 normal, UInt64 timestamp, NRFrameFlags flag, NRTextureType texturetype, UInt64 frameHandle)
        {
            this.leftTex = left;
            this.rightTex = right;
            this.headPose = p;
            this.focusPosition = new NativeVector3f(focuspos);
            this.normalPosition = new NativeVector3f(normal);
            this.presentTime = timestamp;
            this.changeFlag = flag;
            this.textureType = texturetype;
            this.frameHandle = frameHandle;
        }

        public override string ToString()
        {
            return string.Format("lefttex:{0} righttex:{1} headPose:{2} presentTime:{3} changeFlag:{4}, frameHandle:{5}", 
                leftTex, rightTex, headPose, presentTime, changeFlag, frameHandle);
        }
    }
}
