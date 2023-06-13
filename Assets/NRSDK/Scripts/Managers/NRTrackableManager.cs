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

    /// <summary>
    /// Manages AR system state and handles the session lifecycle. this class, application can create
    /// a session, configure it, start/pause or stop it. </summary>
    public class NRTrackableManager : ILifecycle
    {
        /// <summary> Dictionary of trackables. </summary>
        private Dictionary<UInt64, NRTrackable> m_TrackableDict = new Dictionary<UInt64, NRTrackable>();

        /// <summary> Dictionary of trackable types. </summary>
        private Dictionary<TrackableType, Dictionary<UInt64, NRTrackable>> m_TrackableTypeDict = new Dictionary<TrackableType, Dictionary<ulong, NRTrackable>>();

        /// <summary> all trackables. </summary>
        private List<NRTrackable> m_AllTrackables = new List<NRTrackable>();
        /// <summary> The new trackables. </summary>
        private List<NRTrackable> m_NewTrackables = new List<NRTrackable>();

        /// <summary> The old trackables. </summary>
        private HashSet<NRTrackable> m_OldTrackables = new HashSet<NRTrackable>();

        /// <summary> Temp trackable handle list. </summary>
        private List<UInt64> m_TempTrackableHandles = new List<UInt64>();

        private NRTrackableSubsystem m_TrackableSubsystem;
        public NRTrackableSubsystem TrackableSubsystem
        {
            get
            {
                if (m_TrackableSubsystem == null)
                {
                    string trackable_match = NRTrackableSubsystemDescriptor.Name;
                    List<NRTrackableSubsystemDescriptor> trackableDes = new List<NRTrackableSubsystemDescriptor>();
                    NRSubsystemManager.GetSubsystemDescriptors(trackableDes);
                    foreach (var descripe in trackableDes)
                    {
                        if (descripe.id.Equals(trackable_match))
                        {
                            m_TrackableSubsystem = descripe.Create();
                        }
                    }
                }

                return m_TrackableSubsystem;
            }
        }

        private NRPlaneSubsystem m_PlaneSubsystem;
        public NRPlaneSubsystem PlaneSubsystem
        {
            get
            {
                if (m_PlaneSubsystem == null)
                {
                    string trackable_match = NRPlaneSubsystemDescriptor.Name;
                    List<NRPlaneSubsystemDescriptor> trackableDes = new List<NRPlaneSubsystemDescriptor>();
                    NRSubsystemManager.GetSubsystemDescriptors(trackableDes);
                    foreach (var descripe in trackableDes)
                    {
                        if (descripe.id.Equals(trackable_match))
                        {
                            m_PlaneSubsystem = descripe.Create();
                        }
                    }
                }

                return m_PlaneSubsystem;
            }
        }

        private NRTrackableImageSubsystem m_TrackableImagesSubsystem;
        public NRTrackableImageSubsystem TrackableImageSubsystem
        {
            get
            {
                if (m_TrackableImagesSubsystem == null)
                {
                    string trackable_match = NRTrackableImageSubsystemDescriptor.Name;
                    List<NRTrackableImageSubsystemDescriptor> trackableDes = new List<NRTrackableImageSubsystemDescriptor>();
                    NRSubsystemManager.GetSubsystemDescriptors(trackableDes);
                    foreach (var descripe in trackableDes)
                    {
                        if (descripe.id.Equals(trackable_match))
                        {
                            m_TrackableImagesSubsystem = descripe.Create();
                        }
                    }
                }

                return m_TrackableImagesSubsystem;
            }
        }

        /// <summary> Constructor. </summary>
        /// <param name="nativeInterface"> The native interface.</param>
        public NRTrackableManager() { }

        /// <summary> Creates a new NRTrackable. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <param name="nativeInterface">  The native interface.</param>
        /// <returns> A NRTrackable. </returns>
        private NRTrackable Create(UInt64 trackable_handle)
        {
            if (trackable_handle == 0 || !TrackableSubsystem.running)
            {
                return null;
            }

            NRTrackable result;
            if (m_TrackableDict.TryGetValue(trackable_handle, out result))
            {
                return result;
            }

            TrackableType trackableType = TrackableSubsystem.GetTrackableType(trackable_handle);
            if (trackableType == TrackableType.TRACKABLE_PLANE)
            {
                result = new NRTrackablePlane(trackable_handle);
            }
            else if (trackableType == TrackableType.TRACKABLE_IMAGE)
            {
                result = new NRTrackableImage(trackable_handle);
            }
            else
            {
                throw new NotImplementedException(
                    "TrackableFactory::No constructor for requested trackable type.");
            }
            if (result != null)
            {
                m_TrackableDict.Add(trackable_handle, result);
                if (!m_TrackableTypeDict.ContainsKey(trackableType))
                {
                    m_TrackableTypeDict.Add(trackableType, new Dictionary<ulong, NRTrackable>());
                }
                Dictionary<ulong, NRTrackable> trackbletype_dict = null;
                m_TrackableTypeDict.TryGetValue(trackableType, out trackbletype_dict);
                if (!trackbletype_dict.ContainsKey(trackable_handle))
                {
                    trackbletype_dict.Add(trackable_handle, result);
                    m_AllTrackables.Add(result);
                }
            }

            return result;
        }

        /// <summary> Updates the trackables described by trackable_type. </summary>
        /// <param name="trackable_type"> Type of the trackable.</param>
        private void UpdateTrackables(TrackableType trackable_type)
        {
            if (!TrackableSubsystem.running)
            {
                return;
            }

            if (TrackableSubsystem.UpdateTrackables(trackable_type, m_TempTrackableHandles))
            {
                for (int i = 0; i < m_TempTrackableHandles.Count; i++)
                {
                    Create(m_TempTrackableHandles[i]);
                }
            }
        }

        /// <summary> Get the list of trackables with specified filter. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <param name="trackables"> trackableList A list where the returned trackable stored. The
        ///                           previous values will be cleared.</param>
        /// <param name="filter">     Query filter.</param>
        public void GetTrackables<T>(List<T> trackables, NRTrackableQueryFilter filter) where T : NRTrackable
        {
            TrackableType t_type = GetTrackableType<T>();

            // Update trackable by type
            UpdateTrackables(t_type);

            // Find the new trackable in this frame
            m_NewTrackables.Clear();
            for (int i = 0; i < m_AllTrackables.Count; i++)
            {
                NRTrackable trackable = m_AllTrackables[i];
                if (!m_OldTrackables.Contains(trackable))
                {
                    m_NewTrackables.Add(trackable);
                    m_OldTrackables.Add(trackable);
                }
            }

            trackables.Clear();

            if (filter == NRTrackableQueryFilter.All)
            {
                for (int i = 0; i < m_AllTrackables.Count; i++)
                {
                    SafeAdd<T>(m_AllTrackables[i], trackables);
                }
            }
            else if (filter == NRTrackableQueryFilter.New)
            {
                for (int i = 0; i < m_NewTrackables.Count; i++)
                {
                    SafeAdd<T>(m_NewTrackables[i], trackables);
                }
            }
        }

        /// <summary> Safe add. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <param name="trackable">  The trackable.</param>
        /// <param name="trackables"> trackableList A list where the returned trackable stored. The
        ///                           previous values will be cleared.</param>
        private void SafeAdd<T>(NRTrackable trackable, List<T> trackables) where T : NRTrackable
        {
            if (trackable is T)
            {
                trackables.Add(trackable as T);
            }
        }

        /// <summary> Gets trackable type. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <returns> The trackable type. </returns>
        private TrackableType GetTrackableType<T>() where T : NRTrackable
        {
            if (typeof(NRTrackablePlane).Equals(typeof(T)))
            {
                return TrackableType.TRACKABLE_PLANE;
            }
            else if (typeof(NRTrackableImage).Equals(typeof(T)))
            {
                return TrackableType.TRACKABLE_IMAGE;
            }
            return TrackableType.TRACKABLE_BASE;
        }

        public void Start()
        {
            TrackableSubsystem.Start();
            PlaneSubsystem.Start();
            TrackableImageSubsystem.Start();
        }

        public void Pause()
        {
            TrackableSubsystem.Pause();
            PlaneSubsystem.Pause();
            TrackableImageSubsystem.Pause();
        }

        public void Resume()
        {
            TrackableSubsystem.Resume();
            PlaneSubsystem.Resume();
            TrackableImageSubsystem.Resume();
        }

        public void Stop()
        {
            TrackableSubsystem.Stop();
            PlaneSubsystem.Stop();
            TrackableImageSubsystem.Stop();
        }
    }
}
