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
    /// <summary> Values that represent glasses disconnect reasons. </summary>
    public enum GlassesDisconnectReason
    {
        /// <summary> 
        /// Device disconnected. 
        /// </summary>
        GLASSES_DEVICE_DISCONNECT = 1,

        /// <summary> 
        /// Device is notified to disconnect (mode switch).
        /// </summary>
        NOTIFY_TO_QUIT_APP = 2,

        /// <summary> 
        /// Device sleep.
        /// </summary>
        NOTIFY_GOTO_SLEEP = 3
    }
}
