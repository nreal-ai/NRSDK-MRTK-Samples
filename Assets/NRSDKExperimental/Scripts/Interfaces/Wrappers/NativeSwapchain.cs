/****************************************************************************
* Copyright(C) 2019 Nreal
*                                                                                                                                                          
* This file is part of NrealSDK.                                                                                                          
*                                                                                                                                                           
* NrealSDK is distributed in the hope that it will be usefull                                                              
*                                                                                                                                                           
* https://www.nreal.ai/         
* 
*****************************************************************************/

namespace NRKernal.Experimental
{
    using System;
    using UnityEngine;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine.Assertions;

    /// <summary> Swapchain Native API. </summary>
    internal partial class NativeSwapchain
    {
        private UInt64 RenderHandler
        {
            get
            {
                return m_NativeRenderring.RenderingHandle;
            }
        }
        private NativeRenderring m_NativeRenderring;

        public NativeSwapchain(NativeRenderring nativeRenderring)
        {
            m_NativeRenderring = nativeRenderring;
        }

        #region swap chain
        internal UInt64 CreateSwapChain()
        {
            UInt64 swapchan_handle = 0;
            var result = NativeApi.NRSwapchainCreate(RenderHandler, ref swapchan_handle);
            Assert.IsTrue(result == NativeResult.Success);
            //NRDebugger.Info("[NativeSwapchain] CreateSwapChain, RenderHandler:{0} swapchan_handle:{1}", RenderHandler, swapchan_handle);
            return swapchan_handle;
        }

        internal int GetRecommandBufferCount(UInt64 swapchainhandler, int flag)
        {
            int buffer_count = 0;
            NativeApi.NRSwapchainGetRecommandBufferCount(RenderHandler, swapchainhandler, flag, ref buffer_count);
            return buffer_count;
        }

        internal int[] GetLayerIds(UInt64 swapchainhandler)
        {
            //NRDebugger.Info("[NativeSwapchain] NRSwapchainGetLayerIds.");
            int count = 0;
            NativeApi.NRSwapchainGetLayerCount(RenderHandler, swapchainhandler, ref count);
            int[] layerids = new int[count];
            NativeApi.NRSwapchainGetLayerIds(RenderHandler, swapchainhandler, count, layerids);
            return layerids;
        }

        internal int CreateLayer(UInt64 swapchainhandler, UInt64 bufferspechandler, int buffercount)
        {
            int layerid = -1;
            var result = NativeApi.NRSwapchainCreateLayer(RenderHandler, swapchainhandler, bufferspechandler, buffercount, ref layerid);
            Assert.IsTrue(result == NativeResult.Success);
            //NRDebugger.Info("[NativeSwapchain] CreateLayer,RenderHandler:{0} swapchainhandler:{1} bufferspechandler:{2} buffercount:{3} layerid:{4}",
            //    RenderHandler, swapchainhandler, bufferspechandler, buffercount, layerid);
            return layerid;
        }

        internal void DestroyLayer(UInt64 swapchainhandler, int layerid)
        {
            var result = NativeApi.NRSwapchainDestroyLayer(RenderHandler, swapchainhandler, layerid);
            //NRDebugger.Info("[NativeSwapchain] DestroyLayer:" + layerid);
            Assert.IsTrue(result == NativeResult.Success);
        }

        internal void DestroySwapChain(UInt64 swapchainhandler)
        {
            //NRDebugger.Info("[NativeSwapchain] DestroySwapChain.");
            var result = NativeApi.NRSwapchainDestroy(RenderHandler, swapchainhandler);
            Assert.IsTrue(result == NativeResult.Success);
        }
        #endregion

        #region buffer spec
        internal UInt64 CreateBufferSpec(BufferSpec spec)
        {
            UInt64 bufferSpecsHandler = 0;
            //NRDebugger.Info("[NativeSwapchain] CreateBufferSpec.");
            NativeApi.NRBufferSpecCreate(RenderHandler, ref bufferSpecsHandler);
            NativeApi.NRBufferSpecSetSize(RenderHandler, bufferSpecsHandler, spec.size);
            NativeApi.NRBufferSpecSetColorFormat(RenderHandler, bufferSpecsHandler, spec.colorFormat);
            NativeApi.NRBufferSpecSetDepthStencilFormat(RenderHandler, bufferSpecsHandler, spec.depthFormat);
            NativeApi.NRBufferSpecSetSamples(RenderHandler, bufferSpecsHandler, spec.samples);
            NativeApi.NRBufferSpecSetExternalSurface(RenderHandler, bufferSpecsHandler, spec.useExternalSurface);
            NativeApi.NRBufferSpecSetExternalSurfaceFlag(RenderHandler, bufferSpecsHandler, spec.surfaceFlag);
            return bufferSpecsHandler;
        }

        internal void DestroyBufferSpec(UInt64 bufferspechandler)
        {
            //NRDebugger.Info("[NativeSwapchain] DestroyBufferSpec.");
            NativeApi.NRBufferSpecDestroy(RenderHandler, bufferspechandler);
        }
        #endregion

        #region view port
        public UInt64 CreateBufferViewport(int layerid, NRReprojectionType reprojection, ref Rect sourceUV, ref Matrix4x4 transform, DisplayType targetEye)
        {
            //NRDebugger.Info("[NativeSwapchain] CreateBufferViewport.");
            UInt64 viewportHandle = 0;
            NativeResult result = NativeApi.NRBufferViewportCreate(RenderHandler, ref viewportHandle);
            if (result == NativeResult.Success)
            {
                UpdateBufferViewportData(viewportHandle, layerid, reprojection, ref sourceUV, ref transform, targetEye);
            }
            Assert.IsTrue(result == NativeResult.Success);
            return viewportHandle;
        }

        internal void UpdateBufferViewportData(UInt64 viewportHandle, int layerid, NRReprojectionType reprojection,
            ref Rect sourceUV, ref Matrix4x4 transform, DisplayType targetDisplay)
        {
            if (viewportHandle != 0)
            {
                //NRDebugger.Info("[NativeSwapchain] UpdateBufferViewportData.");
                NativeApi.NRBufferViewportSetSourceLayer(RenderHandler, viewportHandle, layerid);
                NativeApi.NRBufferViewportSetReprojection(RenderHandler, viewportHandle, reprojection);
                NativeRectf uv = new NativeRectf(sourceUV);
                NativeApi.NRBufferViewportSetSourceUV(RenderHandler, viewportHandle, ref uv);
                NativeMat4f mat = new NativeMat4f(transform);
                NativeApi.NRBufferViewportSetTransform(RenderHandler, viewportHandle, ref mat);
                NativeApi.NRBufferViewportSetTargetEye(RenderHandler, viewportHandle, targetDisplay);
            }
        }

        internal UInt64 CreateSourceFovBufferViewport(int layerid, NRReprojectionType reprojection, Rect sourceUV, DisplayType targetDisplay, NativeFov4f fov)
        {
            UInt64 viewportHandle = 0;
            NativeResult result = NativeApi.NRBufferViewportCreate(RenderHandler, ref viewportHandle);
            if (result == NativeResult.Success)
            {
                UpdateSourceFovBufferViewport(viewportHandle, layerid, reprojection, ref sourceUV, targetDisplay, fov);
            }
            return viewportHandle;
        }

        internal void UpdateSourceFovBufferViewport(UInt64 viewportHandle, int layerid, NRReprojectionType reprojection, ref Rect sourceUV, DisplayType targetDisplay, NativeFov4f fov)
        {
            if (viewportHandle != 0)
            {
                NativeApi.NRBufferViewportSetSourceLayer(RenderHandler, viewportHandle, layerid);
                NativeApi.NRBufferViewportSetReprojection(RenderHandler, viewportHandle, reprojection);
                NativeRectf uv = new NativeRectf(sourceUV);
                NativeApi.NRBufferViewportSetSourceUV(RenderHandler, viewportHandle, ref uv);
                NativeApi.NRBufferViewportSetTargetEye(RenderHandler, viewportHandle, targetDisplay);
                NativeApi.NRBufferViewportSetSourceFov(RenderHandler, viewportHandle, ref fov);
            }
        }

        internal void SetBufferViewPort(UInt64 viewportListHandle, int viewportIndex, UInt64 viewportHandle)
        {
            if (viewportHandle == 0)
            {
                return;
            }

            //NRDebugger.Info("[NativeSwapchain] SetBufferViewPort viewportIndex:{0} viewportHandle:{1}", viewportIndex, viewportHandle);
            NativeApi.NRBufferViewportListSetBufferviewport(RenderHandler, viewportListHandle, viewportIndex, viewportHandle);
        }

        internal void DestroyBufferViewport(UInt64 viewportHandle)
        {
            if (viewportHandle == 0)
            {
                return;
            }

            NativeApi.NRBufferViewportDestroy(RenderHandler, viewportHandle);
        }

        // internal void SetToRecommendedBufferViewports(UInt64 viewportListHandle)
        // {
        //     //NRDebugger.Info("[NativeSwapchain] SetToRecommendedBufferViewports.");
        //     if (viewportListHandle == 0)
        //     {
        //         return;
        //     }

        //     NativeApi.NRBufferViewportListSetToRecommendedBufferViewports(RenderHandler, viewportListHandle);
        // }

        internal void DestroyBufferViewPort(UInt64 viewportHandle)
        {
            if (viewportHandle == 0)
            {
                return;
            }

            NativeApi.NRBufferViewportDestroy(RenderHandler, viewportHandle);
        }

        internal UInt64 CreateBufferViewportList()
        {
            //NRDebugger.Info("[NativeSwapchain] NRBufferViewportListCreate");
            UInt64 viewport_list_handle = 0;
            var result = NativeApi.NRBufferViewportListCreate(RenderHandler, ref viewport_list_handle);
            Assert.IsTrue(result == NativeResult.Success);
            return viewport_list_handle;
        }

        internal int GetBufferViewportListSize(ulong viewportListHandle)
        {
            int length = 0;
            NativeApi.NRBufferViewportListGetCount(RenderHandler, viewportListHandle, ref length);
            return length;
        }

        internal void DestroyBufferViewportList(UInt64 viewportListHandle)
        {
            //NRDebugger.Info("[NativeSwapchain] NRBufferViewportListDestroy");
            NativeApi.NRBufferViewportListDestroy(RenderHandler, viewportListHandle);
        }
        #endregion

        #region frame
        public IntPtr GetSurface(UInt64 swapChainHandler, int layerid)
        {
            IntPtr surfaceHandler = IntPtr.Zero;
            NativeApi.NRSwapchainGetExternalSurface(RenderHandler, swapChainHandler, layerid, ref surfaceHandler);
            return surfaceHandler;
        }

        internal void SetLayerBuffer(UInt64 swapchainHandle, int layerid, IntPtr[] bufferHandler)
        {
            StringBuilder st = new StringBuilder();
            for (int i = 0; i < bufferHandler.Length; i++)
            {
                st.Append(bufferHandler[i] + " ");
            }
            //NRDebugger.Info("[NativeSwapchain] SetLayerBuffer layerid:{0} ids:[{1}]", layerid, st.ToString());
            NativeApi.NRSwapchainSetColorBuffers(RenderHandler, swapchainHandle, layerid, bufferHandler, bufferHandler.Length);
        }

        /// <returns> Camera frames. </returns>
        internal void AcquireCameraFrame(UInt64 swapchainHandle, ref UInt64 frameHandler, int[] layerids, ref IntPtr[] layerbuffers)
        {
            Assert.IsTrue((layerids != null && layerids.Length != 0), "[NativeSwapchain] AcquireCameraFrame layerids is empty.");
            NativeApi.NRSwapchainAcquireFrame(RenderHandler, swapchainHandle, ref frameHandler);
            // NRDebugger.Info("[NativeSwapchain] Acquire layer buffer, index1:{0} index2:{1} RenderHandler:{2} frameHandler:{3}", layerids[0], layerids[1], RenderHandler, frameHandler);

            if (layerbuffers == null || layerbuffers.Length != layerids.Length)
            {
                // NRDebugger.Info("[NativeSwapchain] Acquire layer buffer, (layerbuffers == null):{0}, len:{1}", layerbuffers == null,  layerbuffers != null ? layerbuffers.Length : 0);
                layerbuffers = new IntPtr[layerids.Length];
            }
            NativeApi.NRFrameAcquireColorBuffers(RenderHandler, frameHandler, layerids, layerids.Length, layerbuffers);
            // NRDebugger.Info("[NativeSwapchain] Acquire layer buffer, cameraTextures1:{0} cameraTextures2:{1}", layerbuffers[0], layerbuffers[1]);
        }

        internal void Submit(UInt64 frameHandle, UInt64 viewPortList)
        {
            if (frameHandle == 0)
            {
                return;
            }

            NativeApi.NRFrameSetBufferViewportList(RenderHandler, frameHandle, viewPortList);
            NativeApi.NRFrameSubmit(RenderHandler, frameHandle);
        }

        internal void UpdateExternalSurface(UInt64 swapchainHandle, int layerid, NativeMat4f transform, Int64 timestamp, int index)
        {
            //NRDebugger.Info("[NativeSwapchain] UpdateExternalSurface");
            NativeApi.NRSwapchainUpdateExternalSurface(RenderHandler, swapchainHandle, layerid, ref transform, timestamp, index);
        }
        #endregion

        private struct NativeApi
        {
            #region buffer spec
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferSpecCreate(UInt64 rendering_handle, ref UInt64 out_spec_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferSpecDestroy(UInt64 rendering_handle, UInt64 spec_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferSpecGetSize(UInt64 rendering_handle, UInt64 spec_handle, ref NativeResolution out_buffer_spec_size);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferSpecSetSize(UInt64 rendering_handle, UInt64 spec_handle, NativeResolution buffer_spec_size);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferSpecGetSamples(UInt64 rendering_handle, UInt64 spec_handle, ref int out_num_samples);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferSpecSetSamples(UInt64 rendering_handle, UInt64 spec_handle, int num_samples);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferSpecSetColorFormat(UInt64 rendering_handle, UInt64 spec_handle, NRColorFormat color_format);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferSpecSetDepthStencilFormat(UInt64 rendering_handle, UInt64 spec_handle, NRDepthStencilFormat depth_stencil_format);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferSpecSetMultiviewLayers(UInt64 rendering_handle, UInt64 spec_handle, int num_layers);

            //only for android
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferSpecSetExternalSurface(UInt64 rendering_handle, UInt64 spec_handle, bool external_surface);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferSpecSetExternalSurfaceFlag(UInt64 rendering_handle, UInt64 spec_handle, int surface_flag);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSwapchainCreateLayer(UInt64 rendering_handle, UInt64 swapchain_handle, UInt64 layer_buffer_spec, int layer_buffer_count, ref int out_layer_id);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSwapchainDestroyLayer(UInt64 rendering_handle, UInt64 swapchain_handle, int layer_id);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSwapchainUpdateExternalSurface(UInt64 rendering_handle, UInt64 swapchain_handle, int layer_id, ref NativeMat4f transform, Int64 present_time, int transform_index);
            #endregion

            #region swap chain
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSwapchainCreate(UInt64 rendering_handle, ref UInt64 out_swapchain_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSwapchainGetRecommandBufferCount(UInt64 rendering_handle, UInt64 swapchain_handle, int flag, ref int buffer_count);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSwapchainGetLayerIds(UInt64 rendering_handle, UInt64 swapchain_handle, int layer_count, int[] out_layer_ids);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSwapchainGetLayerCount(UInt64 rendering_handle, UInt64 swapchain_handle, ref int out_layer_count);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSwapchainSetColorBuffers(UInt64 rendering_handle, UInt64 swapchain_handle, int layer_id, IntPtr[] color_buffers, int buffer_count);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSwapchainGetExternalSurface(UInt64 rendering_handle, UInt64 swapchain_handle, int layer_id, ref IntPtr surface);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSwapchainAcquireFrame(UInt64 rendering_handle, UInt64 swapchain_handle, ref UInt64 out_frame_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSwapchainDestroy(UInt64 rendering_handle, UInt64 swapchain_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRFrameAcquireColorBuffers(UInt64 rendering_handle, UInt64 frame_handle, int[] layer_ids, int layer_count, IntPtr[] out_color_buffers);
            #endregion

            #region view port
            /// <summary>
            /// Set viewport list to frame buffer.
            /// </summary>
            /// <param name="rendering_handle"></param>
            /// <param name="frame_handle"></param>
            /// <param name="viewport_list_handle">根据layer的depth进行排序，最前面的在最底层</param>
            /// <returns></returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRFrameSetBufferViewportList(UInt64 rendering_handle, UInt64 frame_handle, UInt64 viewport_list_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportCreate(UInt64 rendering_handle, ref UInt64 out_viewport_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportDestroy(UInt64 rendering_handle, UInt64 viewport_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportGetSourceUV(UInt64 rendering_handle, UInt64 viewport_handle, ref NativeRectf out_uv_rect);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportSetSourceUV(UInt64 rendering_handle, UInt64 viewport_handle, ref NativeRectf uv_rect);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportGetTransform(UInt64 rendering_handle, UInt64 viewport_handle, ref NativeMat4f out_transform);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportSetTransform(UInt64 rendering_handle, UInt64 viewport_handle, ref NativeMat4f transform);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportGetTargetEye(UInt64 rendering_handle, UInt64 viewport_handle, ref DisplayType out_eye);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportSetTargetEye(UInt64 rendering_handle, UInt64 viewport_handle, DisplayType eye);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportGetSourceLayer(UInt64 rendering_handle, UInt64 viewport_handle, ref int out_layer_id);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportSetSourceLayer(UInt64 rendering_handle, UInt64 viewport_handle, int layer_id);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportGetReprojection(UInt64 rendering_handle, UInt64 viewport_handle, ref NRReprojectionType out_reprojection);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportSetReprojection(UInt64 rendering_handle, UInt64 viewport_handle, NRReprojectionType reprojection);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportGetMultiviewLayer(UInt64 rendering_handle, UInt64 viewport_handle, ref int out_texture_layer_index);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportSetMultiviewLayer(UInt64 rendering_handle, UInt64 viewport_handle, int texture_layer_index);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportGetSourceFov(UInt64 rendering_handle, UInt64 viewport_handle, ref NativeFov4f out_fov);
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportSetSourceFov(UInt64 rendering_handle, UInt64 viewport_handle, ref NativeFov4f fov);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportListCreate(UInt64 rendering_handle, ref UInt64 out_viewport_list_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportListDestroy(UInt64 rendering_handle, UInt64 viewport_list_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportListGetCount(UInt64 rendering_handle, UInt64 viewport_list_handle, ref int out_list_count);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportListGetBufferviewport(UInt64 rendering_handle, UInt64 viewport_list_handle, int viewport_index, ref UInt64 out_viewport_handle);

            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBufferViewportListSetBufferviewport(UInt64 rendering_handle, UInt64 viewport_list_handle, int viewport_index, UInt64 viewport_handle);

            // [DllImport(NativeConstants.NRNativeLibrary)]
            // public static extern NativeResult NRBufferViewportListSetToRecommendedBufferViewports(UInt64 rendering_handle, UInt64 viewport_list_handle);
            #endregion

            //for thread sync and fence
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRFrameSubmit(UInt64 rendering_handle, UInt64 frame_handle);
        };
    }
}
