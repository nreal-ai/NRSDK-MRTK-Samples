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
    /// <summary> Device Tracking State. </summary>
    public enum TrackingState
    {
        /// <summary>
        /// TRACKING means the object is being tracked and its state is valid.
        /// </summary>
        Tracking = 0,

        /// <summary>
        /// PAUSED indicates that NRSDK has paused tracking, 
        /// and the related data is not accurate.  
        /// </summary>
        Paused = 1,

        /// <summary>
        /// STOPPED means that NRSDK has stopped tracking, and will never resume tracking. 
        /// </summary>
        Stopped = 2
    }

    /// <summary> Device Tracking Mode. </summary>
    public enum TrackingMode
    {
        /// <summary>
        /// 6Dof mode.
        /// </summary>
        MODE_6DOF = 0,

        /// <summary>
        /// 3Dof mode, only rotation.  
        /// </summary>
        MODE_3DOF = 1,

        /// <summary>
        /// 0Dof mode.  
        /// </summary>
        MODE_0DOF = 2,

        /// <summary>
        /// 0Dof stable mode. 
        /// </summary>
        MODE_0DOF_STAB = 3
    }
}
