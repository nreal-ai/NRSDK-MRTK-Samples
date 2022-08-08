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
    /// <summary> Tracable object type. </summary>
    public enum TrackableType
    {
        /// <summary>
        /// TRACKABLE_BASE means the base object of trackable.
        /// </summary>
        TRACKABLE_BASE = 0,

        /// <summary>
        /// TRACKABLE_PLANE means the trackable object is a plane.
        /// </summary>
        TRACKABLE_PLANE = 1,

        /// <summary>
        /// TRACKABLE_IMAGE means the trackable object is a tracking image.
        /// </summary>
        TRACKABLE_IMAGE = 2,
    }

    /// <summary> Trackable image's finding mode. </summary>
    public enum TrackableImageFindingMode
    {
        /// <summary>
        /// Disable image tracking.
        /// </summary>
        DISABLE = 0,

        /// <summary>
        /// Enable image tracking.
        /// </summary>
        ENABLE = 1,
    }
}
