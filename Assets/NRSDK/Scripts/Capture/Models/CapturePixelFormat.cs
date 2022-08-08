/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    /// <summary>
    /// The encoded image or video pixel format to use for PhotoCapture and VideoCapture. </summary>
    public enum CapturePixelFormat
    {
        /// <summary>
        /// 8 bits per channel (blue, green, red, and alpha).
        /// </summary>
        BGRA32 = 0,

        /// <summary>
        /// 8-bit Y plane followed by an interleaved U/V plane with 2x2 subsampling.
        /// </summary>
        NV12 = 1,

        /// <summary>
        /// Encode photo in JPEG format.
        /// </summary>
        JPEG = 2,

        /// <summary>
        /// Portable Network Graphics Format.
        /// </summary>
        PNG = 3
    }
}
