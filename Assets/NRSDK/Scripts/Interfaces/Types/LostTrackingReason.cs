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
    /// <summary> The reason of HMD untracked. </summary>
    public enum LostTrackingReason
    {
        /// <summary> Preinitializing.. </summary>
        PRE_INITIALIZING = -1,

        NONE = 0,

        /// <summary> Initializing.. </summary>
        INITIALIZING = 1,

        /// <summary> Move too fast.. </summary>
        EXCESSIVE_MOTION = 2,

        /// <summary> Feature point deficiency.. </summary>
        INSUFFICIENT_FEATURES = 3,

        /// <summary> Reposition. </summary>
        RELOCALIZING = 4,

        /// <summary> Reposition. </summary>
        ENTER_VRMODE = 5,
    }
}
