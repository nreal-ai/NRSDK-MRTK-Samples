/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System;

namespace NRKernal
{
    /// <summary> Values that represent native eyes. </summary>
    [Obsolete("Use 'NativeDevice' instead.")]
    public enum NativeEye
    {
        /// <summary> 
        /// Left display. 
        /// </summary>
        LEFT = 0,

        /// <summary> 
        /// Right display. 
        /// </summary>
        RIGHT = 1,

        /// <summary> 
        /// RGB camera. 
        /// </summary>
        RGB = 2,
    }
}
