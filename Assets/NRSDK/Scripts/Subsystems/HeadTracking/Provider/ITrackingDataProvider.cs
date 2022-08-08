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
    using System;
    using UnityEngine;

    public interface ITrackingDataProvider : ILifecycle
    {
        bool GetHeadPose(ref Pose pose, UInt64 timestamp);

        ulong GetHMDTimeNanos();

        bool GetFramePresentHeadPose(ref Pose pose, ref LostTrackingReason lostReason, ref UInt64 timestamp);

        bool GetFramePresentTimeByCount(int count, ref UInt64 timestamp);

        bool InitTrackingMode(TrackingMode mode);

        bool SwitchTrackingMode(TrackingMode mode);

        void Recenter();
    }
}
