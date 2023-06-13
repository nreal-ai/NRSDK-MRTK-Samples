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

    /// <summary>
    /// Gives access to subsystems.
    /// Provides the ability to query for SubsystemDescriptors which enumerate features. 
    /// Given an SubsystemDescriptor, you can create an Subsystem to utilize the subsystem.
    /// </summary>
    public static class NRSubsystemManager
    {
        private static List<ISubsystemDescriptor> supportedDescriptors = new List<ISubsystemDescriptor>()
        {
            new NRTrackingSubsystemDescriptor(),
            new NRTrackableSubsystemDescriptor(),
            new NRTrackableImageSubsystemDescriptor(),
            new NRPlaneSubsystemDescriptor(),
            new NRDeviceSubsystemDescriptor(),
            new NRDisplaySubsystemDescriptor(),
        };

        /// <summary>
        /// Gets all of the currently known subsystem descriptors regardless of specific subsystem type.
        /// </summary>
        /// <param name="descriptors">subsystem descriptor type.</param>
        public static void GetAllSubsystemDescriptors(List<ISubsystemDescriptor> descriptors)
        {
            if (descriptors == null)
            {
                descriptors = new List<ISubsystemDescriptor>();
            }
            descriptors.Clear();
            descriptors.AddRange(supportedDescriptors);
        }

        /// <summary>
        /// Get Active Subsystems of a specific instance type.
        /// </summary>
        /// <typeparam name="TSubSystem">subsystem type. </typeparam>
        /// <param name="subsystems"></param>
        public static void GetSubsystems<TSubSystem>(List<TSubSystem> subsystems) where TSubSystem : ISubsystem
        {
            if (subsystems == null)
            {
                subsystems = new List<TSubSystem>();
            }
            subsystems.Clear();

            foreach (var des in supportedDescriptors)
            {
                var subsystem = ((IntegratedSubsystemDescriptor)des).subsystem;
                if (subsystem != null && subsystem.GetType().Equals(typeof(TSubSystem)))
                {
                    subsystems.Add((TSubSystem)subsystem);
                }
            }
        }

        /// <summary>
        /// Gets A list of ISubsystemDescriptor which describe additional functionality that can be enabled.
        /// </summary>
        /// <typeparam name="TDescriptor">descriptor type.</typeparam>
        /// <param name="descriptors"></param>
        public static void GetSubsystemDescriptors<TDescriptor>(List<TDescriptor> descriptors) where TDescriptor : ISubsystemDescriptor
        {
            if (descriptors == null)
            {
                descriptors = new List<TDescriptor>();
            }
            descriptors.Clear();

            foreach (var descriptor in supportedDescriptors)
            {
                if (typeof(TDescriptor).Equals(descriptor.GetType()))
                {
                    descriptors.Add((TDescriptor)descriptor);
                }
            }
        }
    }
}
