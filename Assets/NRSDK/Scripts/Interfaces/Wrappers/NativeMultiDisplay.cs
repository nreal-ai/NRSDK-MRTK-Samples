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

    /// <summary> Callback, called when the nr display resolution. </summary>
    /// <param name="width">  The width.</param>
    /// <param name="height"> The height.</param>
    public delegate void NRDisplayResolutionCallback(int width, int height);

    /// <summary> HMD Eye offset Native API . </summary>
    internal partial class NativeMultiDisplay
    {
        /// <summary> Handle of the multi display. </summary>
        private UInt64 m_MultiDisplayHandle;

        /// <summary> Creates a new bool. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Create()
        {
            NativeResult result = NativeApi.NRDisplayCreate(ref m_MultiDisplayHandle);
            NativeErrorListener.Check(result, this, "Create", true);
            return result == NativeResult.Success;
        }

        /// <summary> Initializes the color space. </summary>
        public void InitColorSpace()
        {
            NativeColorSpace colorspace = QualitySettings.activeColorSpace == ColorSpace.Gamma ?
                NativeColorSpace.COLOR_SPACE_GAMMA : NativeColorSpace.COLOR_SPACE_LINEAR;
            NativeResult result = NativeApi.NRDisplayInitSetTextureColorSpace(m_MultiDisplayHandle, colorspace);
            NativeErrorListener.Check(result, this, "InitColorSpace");
        }

        /// <summary> Starts this object. </summary>
        public void Start()
        {
            NativeResult result = NativeApi.NRDisplayStart(m_MultiDisplayHandle);
            NativeErrorListener.Check(result, this, "Start", true);
        }

        /// <summary> Listen main screen resolution changed. </summary>
        /// <param name="callback"> The callback.</param>
        public void ListenMainScrResolutionChanged(NRDisplayResolutionCallback callback)
        {
            NativeResult result = NativeApi.NRDisplaySetMainDisplayResolutionCallback(m_MultiDisplayHandle, callback);
            NativeErrorListener.Check(result, this, "ListenMainScrResolutionChanged");
        }

        /// <summary> Stops this object. </summary>
        public void Stop()
        {
            NativeResult result = NativeApi.NRDisplayStop(m_MultiDisplayHandle);
            NativeErrorListener.Check(result, this, "Stop");
        }

        /// <summary> Updates the home screen texture described by rendertexture. </summary>
        /// <param name="rendertexture"> The rendertexture.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool UpdateHomeScreenTexture(IntPtr rendertexture)
        {
            NativeResult result = NativeApi.NRDisplaySetMainDisplayTexture(m_MultiDisplayHandle, rendertexture);
            NativeErrorListener.Check(result, this, "UpdateHomeScreenTexture");
            return result == NativeResult.Success;
        }

        /// <summary> Pauses this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Pause()
        {
            NativeResult result = NativeApi.NRDisplayPause(m_MultiDisplayHandle);
            NativeErrorListener.Check(result, this, "Pause");
            return result == NativeResult.Success;
        }

        /// <summary> Resumes this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Resume()
        {
            NativeResult result = NativeApi.NRDisplayResume(m_MultiDisplayHandle);
            NativeErrorListener.Check(result, this, "Resume");
            return result == NativeResult.Success;
        }

        /// <summary> Destroys this object. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Destroy()
        {
            NativeResult result = NativeApi.NRDisplayDestroy(m_MultiDisplayHandle);
            NativeErrorListener.Check(result, this, "Destroy");
            return result == NativeResult.Success;
        }

        /// <summary> A native api. </summary>
        private struct NativeApi
        {
            /// <summary> Nr display create. </summary>
            /// <param name="out_display_handle"> [in,out] Handle of the out display.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplayCreate(ref UInt64 out_display_handle);

            /// <summary> Callback, called when the nr display set main display resolution. </summary>
            /// <param name="display_handle">      The display handle.</param>
            /// <param name="resolution_callback"> The resolution callback.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplaySetMainDisplayResolutionCallback(UInt64 display_handle,
                NRDisplayResolutionCallback resolution_callback);

            /// <summary> Nr display initialize set texture color space. </summary>
            /// <param name="display_handle"> The display handle.</param>
            /// <param name="color_space">    The color space.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplayInitSetTextureColorSpace(UInt64 display_handle,
                NativeColorSpace color_space);

            /// <summary> Nr display start. </summary>
            /// <param name="display_handle"> The display handle.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplayStart(UInt64 display_handle);

            /// <summary> Nr display stop. </summary>
            /// <param name="display_handle"> The display handle.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplayStop(UInt64 display_handle);

            /// <summary> Nr display pause. </summary>
            /// <param name="display_handle"> The display handle.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplayPause(UInt64 display_handle);

            /// <summary> Nr display resume. </summary>
            /// <param name="display_handle"> The display handle.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplayResume(UInt64 display_handle);

            /// <summary> Nr display set main display texture. </summary>
            /// <param name="display_handle">     The display handle.</param>
            /// <param name="controller_texture"> The controller texture.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplaySetMainDisplayTexture(UInt64 display_handle,
                IntPtr controller_texture);

            /// <summary> Nr display destroy. </summary>
            /// <param name="display_handle"> The display handle.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRDisplayDestroy(UInt64 display_handle);
        };
    }
}
