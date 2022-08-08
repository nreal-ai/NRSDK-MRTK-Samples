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
    using System.Collections.Generic;
    using UnityEngine;

    public class NRPlaneSubsystemDescriptor : IntegratedSubsystemDescriptor<NRPlaneSubsystem>
    {
        public const string Name = "Subsystem.Trackable.Plane";
        public override string id => Name;
    }

    public class NRPlaneSubsystem : IntegratedSubsystem<NRPlaneSubsystemDescriptor>
    {
        private ITrackablePlaneDataProvider m_Provider;

        public NRPlaneSubsystem(NRPlaneSubsystemDescriptor descriptor) : base(descriptor)
        {
#if UNITY_EDITOR
            m_Provider = new NREmulatorTrackPlaneProvider();
#else
            m_Provider = new NRTrackablePlaneProvider();
#endif
        }

        public TrackablePlaneType GetPlaneType(UInt64 trackable_handle)
        {
            return m_Provider.GetPlaneType(trackable_handle);
        }

        public Pose GetCenterPose(UInt64 trackable_handle)
        {
            return m_Provider.GetCenterPose(trackable_handle);
        }

        public float GetExtentX(UInt64 trackable_handle)
        {
            return m_Provider.GetExtentX(trackable_handle);
        }

        public float GetExtentZ(UInt64 trackable_handle)
        {
            return m_Provider.GetExtentZ(trackable_handle);
        }

        public void GetBoundaryPolygon(UInt64 trackable_handle, List<Vector3> polygonList)
        {
            m_Provider.GetBoundaryPolygon(trackable_handle, polygonList);
        }
    }
}
