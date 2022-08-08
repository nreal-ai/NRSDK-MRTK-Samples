/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental
{
    using System.Runtime.InteropServices;
    using UnityEngine;

    public enum NRDepthStencilFormat
    {
        // No depth or stencil buffer.
        NR_DEPTH_STENCIL_FORMAT_NONE = 255,
        // Equivalent to GL_DEPTH_COMPONENT16.
        NR_DEPTH_STENCIL_FORMAT_DEPTH_16 = 0,
        // Equivalent to GL_DEPTH_COMPONENT24.
        NR_DEPTH_STENCIL_FORMAT_DEPTH_24 = 1,
        // Equivlaent to GL_DEPTH24_STENCIL8.
        NR_DEPTH_STENCIL_FORMAT_DEPTH_24_STENCIL_8 = 2,
        // Equivalent to GL_DEPTH_COMPONENT32F.
        NR_DEPTH_STENCIL_FORMAT_DEPTH_32_F = 3,
        // Equivalent to GL_DEPTH_32F_STENCIL8.
        NR_DEPTH_STENCIL_FORMAT_DEPTH_32_F_STENCIL_8 = 4,
        // Equivalent to GL_STENCIL8.
        NR_DEPTH_STENCIL_FORMAT_STENCIL_8 = 5,
    };

    /// Types of asynchronous reprojection.
    public enum NRReprojectionType
    {
        /// Reproject in all dimensions,6dof.
        NR_REPROJECTION_TYPE_FULL = 0,
        // For controller layer
        NR_REPROJECTION_TYPE_CONTROLLER = 1,
        /// Do not reproject, 0dof.
        NR_REPROJECTION_TYPE_NONE = 2,
        NR_REPROJECTION_TYPE_COUNT = 3,
    };

    public enum NRSwapchainFlags
    {
        NR_SWAPCHAIN_MULTI_THREAD_RENDERING = 1 << 0,
        NR_SWAPCHAIN_LAYER_DYNAMIC_TEXTURE = 1 << 1,
        NR_SWAPCHAIN_EXTERNAL_SURFACE = 1 << 2,
        NR_SWAPCHAIN_NONE = 0
    };

    public enum NRColorFormat
    {
        NR_COLOR_FORMAT_RGB = 0,
        // Equivalent to GL_RGBA8. Pixel values are expected to be premultiplied
        // with alpha.
        NR_COLOR_FORMAT_RGBA_8888 = 1,
        // Equivalent to GL_RGB565.
        NR_COLOR_FORMAT_RGB_565 = 2,

        NR_COLOR_FORMAT_A8 = 3,
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeRectf
    {
        /// <summary> The X coordinate. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float bottom;
        /// <summary> The Y coordinate. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float left;
        /// <summary> The Z coordinate. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float top;
        /// <summary> The width. </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float right;

        public NativeRectf(Rect rect)
        {
            bottom = rect.yMin;
            left = rect.xMin;
            top = rect.yMax;
            right = rect.xMax;
        }

        public override string ToString()
        {
            return string.Format("bottom:{0} left:{1} top:{2} right:{3}", bottom, left, top, right);
        }
    }

    public enum NRExternalSurfaceFlags
    {
        /// Create a protected surface, suitable for secure video playback.
        NR_EXTERNAL_SURFACE_FLAG_PROTECTED = 0x1,

        /// Create the underlying BufferQueue in synchronous mode, allowing multiple buffers to be
        /// queued instead of always replacing the last buffer.  Buffers are retired in order, and
        /// the producer may block until a new buffer is available.
        NR_EXTERNAL_SURFACE_FLAG_SYNCHRONOUS = 0x2,

        /// Indicates that the compositor should acquire the most recent buffer whose presentation
        /// timestamp is not greater than the expected display time of the final composited frame.
        /// Together with FLAG_SYNCHRONOUS, this flag is suitable for video surfaces where several
        /// frames can be queued ahead of time.
        NR_EXTERNAL_SURFACE_FLAG_USE_TIMESTAMPS = 0x4,
    }
}
