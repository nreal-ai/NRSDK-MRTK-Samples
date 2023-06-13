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
    /// <summary>
    /// Features that the sdk might support.
    /// </summary>
    public enum NRSupportedFeature
    {
        NR_FEATURE_TRACKING_6DOF = 1,
        NR_FEATURE_TRACKING_3DOF,
        NR_FEATURE_TRACKING_FINDING_HORIZONTAL_PLANE,
        NR_FEATURE_TRACKING_FINDING_VERTICAL_PLANE,
        NR_FEATURE_TRACKING_FINDING_MARKER,
        NR_FEATURE_CONTROLLER_3DOF,
        NR_FEATURE_CONTROLLER_6DOF,
        NR_FEATURE_WEARING_STATUS_OF_GLASSES,
        NR_FEATURE_HANDTRACKING,
        NR_FEATURE_RGB_CAMERA,
    }
}
