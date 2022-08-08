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
    using System.Collections.Generic;

    public class NRTrackableSubsystemDescriptor : IntegratedSubsystemDescriptor<NRTrackableSubsystem>
    {
        public const string Name = "Subsystem.Trackable";
        public override string id => Name;
    }

    public class NRTrackableSubsystem : IntegratedSubsystem<NRTrackableSubsystemDescriptor>
    {
        private ITrackableDataProvider m_Provider;

        public NRTrackableSubsystem(NRTrackableSubsystemDescriptor descriptor) : base(descriptor)
        {
#if UNITY_EDITOR
            m_Provider = new NREmulatorTrackableProvider();
#else
            m_Provider = new NRTrackableProvider();
#endif
        }

        public uint GetIdentify(ulong trackable_handle)
        {
            return m_Provider.GetIdentify(trackable_handle);
        }

        public TrackableType GetTrackableType(ulong trackable_handle)
        {
            return m_Provider.GetTrackableType(trackable_handle);
        }

        public TrackingState GetTrackingState(ulong trackable_handle)
        {
            return m_Provider.GetTrackingState(trackable_handle);
        }

        public bool UpdateTrackables(TrackableType trackable_type, List<ulong> trackables)
        {
            return m_Provider.UpdateTrackables(trackable_type, trackables);
        }
    }
}
