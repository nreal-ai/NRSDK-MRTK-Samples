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
    using System.Diagnostics;

    /// <summary>
    /// HMD Eye offset Native API .
    /// </summary>
    public class NativeRenderring
    {
        private UInt64 m_RenderingHandle = 0;
        public UInt64 RenderingHandle
        {
            get
            {
                return m_RenderingHandle;
            }
        }
        public IntPtr FrameInfoPtr;
        private IntPtr m_FrameTexturesPtr;

        public struct TexturesArray
        {
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr leftTex;
            [MarshalAs(UnmanagedType.SysInt)]
            public IntPtr rightTex;
        }

        public NativeRenderring()
        {
            int sizeOfPerson = Marshal.SizeOf(typeof(FrameInfo));
            FrameInfoPtr = Marshal.AllocHGlobal(sizeOfPerson);

            int sizeOfTextures = Marshal.SizeOf(typeof(TexturesArray));
            m_FrameTexturesPtr = Marshal.AllocHGlobal(sizeOfTextures);
        }

        ~NativeRenderring()
        {
            Marshal.FreeHGlobal(FrameInfoPtr);
            Marshal.FreeHGlobal(m_FrameTexturesPtr);
        }

        public bool Create()
        {
            var result = NativeApi.NRRenderingCreate(ref m_RenderingHandle);
            //AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            //AndroidJavaObject unityPlayerObj = activity.Get<AndroidJavaObject>("mUnityPlayer");
            //AndroidJavaObject surfaceViewObj = unityPlayerObj.Call<AndroidJavaObject>("getChildAt", 0);
            //AndroidJavaObject surfaceHolderObj = surfaceViewObj.Call<AndroidJavaObject>("getHolder");
            //AndroidJavaObject surfaceObj = surfaceHolderObj.Call<AndroidJavaObject>("getSurface");
            //var result = NativeApi.NRRenderingInitSetAndroidSurface(m_RenderingHandle, surfaceObj.GetRawObject());
            NativeErrorListener.Check(result, this, "Create");

#if !UNITY_STANDALONE_WIN
            NativeColorSpace colorspace = QualitySettings.activeColorSpace == ColorSpace.Gamma ?
               NativeColorSpace.COLOR_SPACE_GAMMA : NativeColorSpace.COLOR_SPACE_LINEAR;
            NativeApi.NRRenderingInitSetTextureColorSpace(m_RenderingHandle, colorspace);
#endif
            //NativeApi.NRRenderingInitSetFlags();
            return result == NativeResult.Success;
        }

        public bool Start()
        {
            if (m_RenderingHandle == 0)
            {
                return false;
            }

            var result = NativeApi.NRRenderingStart(m_RenderingHandle);
            NativeErrorListener.Check(result, this, "Start");
            return result == NativeResult.Success;
        }

        public bool Pause()
        {
            if (m_RenderingHandle == 0)
            {
                return false;
            }

            var result = NativeApi.NRRenderingPause(m_RenderingHandle);
            NativeErrorListener.Check(result, this, "Pause");
            return result == NativeResult.Success;
        }

        public bool Resume()
        {
            if (m_RenderingHandle == 0)
            {
                return false;
            }

            var result = NativeApi.NRRenderingResume(m_RenderingHandle);
            NativeErrorListener.Check(result, this, "Resume");
            return result == NativeResult.Success;
        }

        public void GetFramePresentTime(ref UInt64 present_time)
        {
            if (m_RenderingHandle == 0)
            {
                return;
            }

            NativeApi.NRRenderingGetFramePresentTime(m_RenderingHandle, ref present_time);
        }

        public bool GetFramePresentTimeByCount(ref UInt64 present_time, int count)
        {
            if (m_RenderingHandle == 0)
            {
                return false;
            }

            var result = NativeApi.NRRenderingGetFramePresentTimeByCount(m_RenderingHandle, count, ref present_time);
            return result == NativeResult.Success;
        }

        public bool DoRender()
        {
            if (m_RenderingHandle == 0)
            {
                return false;
            }

#if !UNITY_EDITOR
            FrameInfo framinfo = (FrameInfo)Marshal.PtrToStructure(FrameInfoPtr, typeof(FrameInfo));
            var result = NativeApi.NRRenderingDoRender(m_RenderingHandle, framinfo.leftTex, framinfo.rightTex, ref framinfo.headPose);
            return result == NativeResult.Success;
#else
            return true;
#endif
        }

        public void PrepareForFrame()
        {
            if (m_RenderingHandle == 0)
                return;

            FrameInfo framinfo = (FrameInfo)Marshal.PtrToStructure(FrameInfoPtr, typeof(FrameInfo));
            var frame_handle = framinfo.frameHandle;

            // NRDebugger.Info("[NativeRenderer] PrepareForFrame: frameHandle={0}", frame_handle);
            NativeApi.NRFrameSetRenderingPose(m_RenderingHandle, frame_handle, ref framinfo.headPose);
            NativeApi.NRFrameSetFocusPlane(m_RenderingHandle, frame_handle, ref framinfo.focusPosition, ref framinfo.normalPosition);
            NativeApi.NRFrameSetPresentTime(m_RenderingHandle, frame_handle, framinfo.presentTime);
            NativeApi.NRFrameSetFlag(m_RenderingHandle, frame_handle, (int)(framinfo.changeFlag));
        }

        public UInt64 CreateFrameHandle()
        {
            UInt64 frame_handle = 0;
            NativeApi.NRFrameCreate(m_RenderingHandle, ref frame_handle);
            return frame_handle;
        }

        public void DoExtendedRenderring()
        {
            FrameInfo framinfo = (FrameInfo)Marshal.PtrToStructure(FrameInfoPtr, typeof(FrameInfo));

            var frame_handle = framinfo.frameHandle;
            // NRDebugger.Info("[NativeRenderer] DoExtendedRenderring: frameHandle={0}", frame_handle);
            NativeApi.NRFrameSetColorTextures(m_RenderingHandle, frame_handle, m_FrameTexturesPtr, 2);
            NativeApi.NRFrameSetRenderingPose(m_RenderingHandle, frame_handle, ref framinfo.headPose);
            NativeApi.NRFrameSetFocusPlanePoint(m_RenderingHandle, frame_handle, ref framinfo.focusPosition);
            NativeApi.NRFrameSetFocusPlaneNormal(m_RenderingHandle, frame_handle, ref framinfo.normalPosition);
            NativeApi.NRFrameSetPresentTime(m_RenderingHandle, frame_handle, framinfo.presentTime);
            NativeApi.NRFrameSetFlag(m_RenderingHandle, frame_handle, (int)(framinfo.changeFlag));
            NativeApi.NRFrameSetColorTextureType(m_RenderingHandle, frame_handle, framinfo.textureType);

            var result = NativeApi.NRRenderingDoRenderEx(m_RenderingHandle, frame_handle);
            NativeErrorListener.Check(result, this, "DoRenderEXP");

            NativeApi.NRFrameDestroy(m_RenderingHandle, frame_handle);
        }

        public void WriteFrameData(FrameInfo frame, bool directMode)
        {
            Marshal.StructureToPtr(frame, FrameInfoPtr, true);
            // FrameInfo framinfo = (FrameInfo)Marshal.PtrToStructure(FrameInfoPtr, typeof(FrameInfo));
            // NRDebugger.Info("[NativeRenderer] WriteFrameData: frameHandle={0} -> {1}", frame.frameHandle, framinfo.frameHandle);

            if (directMode)
            {
                TexturesArray textures = new TexturesArray()
                {
                    leftTex = frame.leftTex,
                    rightTex = frame.rightTex
                };
                Marshal.StructureToPtr(textures, m_FrameTexturesPtr, true);
            }
        }

        public bool Destroy()
        {
            if (m_RenderingHandle == 0)
            {
                return false;
            }

            NativeResult result = NativeApi.NRRenderingDestroy(m_RenderingHandle);
            NativeErrorListener.Check(result, this, "Destroy");
            return result == NativeResult.Success;
        }

        private partial struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingCreate(ref UInt64 out_rendering_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingInitSetTextureColorSpace(UInt64 rendering_handle, NativeColorSpace color_space);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingStart(UInt64 rendering_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingDestroy(UInt64 rendering_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingPause(UInt64 rendering_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingResume(UInt64 rendering_handle);

#if !UNITY_EDITOR
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingInitSetAndroidSurface(
                UInt64 rendering_handle, IntPtr android_surface);
#endif

            //overlay dont need this function
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingDoRenderEx(UInt64 rendering_handle, UInt64 frame_handle);

            //overlay dont need this function
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingInitSetFlags(UInt64 rendering_handle, NRRenderingFlags rendering_flags);

            //overlay dont need this function
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRFrameCreate(UInt64 rendering_handle, ref UInt64 out_frame_handle);

            //overlay dont need this function
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRFrameDestroy(UInt64 rendering_handle, UInt64 frame_handle);

            /// <summary>
            /// overlay dont need this function
            /// </summary>
            /// <param name="rendering_handle"></param>
            /// <param name="frame_handle"></param>
            /// <param name="color_textures"></param>
            /// <param name="color_texture_count"></param>
            /// <returns></returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRFrameSetColorTextures(UInt64 rendering_handle, UInt64 frame_handle,
                IntPtr color_textures, int color_texture_count);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRFrameSetRenderingPose(UInt64 rendering_handle, UInt64 frame_handle,
                ref NativeMat4f transform);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRFrameSetFocusPlanePoint(UInt64 rendering_handle, UInt64 frame_handle,
                ref NativeVector3f plane_point);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRFrameSetFocusPlaneNormal(UInt64 rendering_handle, UInt64 frame_handle,
                ref NativeVector3f plane_normal);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRFrameSetPresentTime(UInt64 rendering_handle, UInt64 frame_handle, UInt64 present_time);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRFrameSetFlag(UInt64 rendering_handle, UInt64 frame_handle,
                int change_flag);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRFrameSetFocusPlane(UInt64 rendering_handle, UInt64 frame_handle, ref NativeVector3f plane_point, ref NativeVector3f plane_normal);

            /// <summary>
            /// overlay dont need this function
            /// </summary>
            /// <param name="rendering_handle"></param>
            /// <param name="frame_handle"></param>
            /// <param name="texture_type"></param>
            /// <returns></returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRFrameSetColorTextureType(UInt64 rendering_handle, UInt64 frame_handle,
                NRTextureType texture_type);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingGetFramePresentTime(UInt64 rendering_handle, ref UInt64 frame_present_time);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingGetFramePresentTimeByCount(UInt64 rendering_handle,
                int extra_frame_count, ref UInt64 frame_present_time);

            /// <summary>
            /// overlay dont need this function
            /// </summary>
            /// <param name="rendering_handle"></param>
            /// <param name="left_eye_texture"></param>
            /// <param name="right_eye_texture"></param>
            /// <param name="head_pose"></param>
            /// <returns></returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRRenderingDoRender(UInt64 rendering_handle,
                IntPtr left_eye_texture, IntPtr right_eye_texture, ref NativeMat4f head_pose);
        };
    }
}