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

    public class NRTrackableImageSubsystemDescriptor : IntegratedSubsystemDescriptor<NRTrackableImageSubsystem>
    {
        public const string Name = "Subsystem.Trackable.Image";
        public override string id => Name;
    }
    public class NRTrackableImageSubsystem : IntegratedSubsystem<NRTrackableImageSubsystemDescriptor>
    {
        private ITrackableImageDataProvider m_Provider;

        public NRTrackableImageSubsystem(NRTrackableImageSubsystemDescriptor descriptor) : base(descriptor)
        {
#if UNITY_EDITOR
            m_Provider = new NREmulatorTrackImageProvider();
#else
            m_Provider = new NRTrackableImageProvider();
#endif
        }

        public UInt64 CreateDataBase()
        {
            return m_Provider.CreateDataBase();
        }

        public bool DestroyDataBase(UInt64 database_handle)
        {
            return m_Provider.DestroyDataBase(database_handle);
        }

        public bool LoadDataBase(UInt64 database_handle, string path)
        {
            return m_Provider.LoadDataBase(database_handle, path);
        }

        public Pose GetCenterPose(UInt64 trackable_handle)
        {
            return m_Provider.GetCenterPose(trackable_handle);
        }

        public Vector2 GetSize(UInt64 trackable_handle)
        {
            return m_Provider.GetSize(trackable_handle);
        }
    }
}
