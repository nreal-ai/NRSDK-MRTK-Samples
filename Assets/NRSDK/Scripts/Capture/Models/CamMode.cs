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
    /// <summary> The camera mode of capture. </summary>
    public enum CamMode
    {
        /// <summary>
        /// Resource is not in use.
        /// </summary>
        None = 0,

        /// <summary>
        /// Resource is in Photo Mode.
        /// </summary>
        PhotoMode = 1,

        /// <summary>
        /// Resource is in Video Mode.
        /// </summary>
        VideoMode = 2
    }
}
