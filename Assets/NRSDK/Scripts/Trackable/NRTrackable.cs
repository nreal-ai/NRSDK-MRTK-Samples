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

    /// <summary>
    /// A Trackable in the real world detected by NRInternel. The base class of TrackablePlane and
    /// TrackableImage.Through this class, application can get the infomation of a trackable object. </summary>
    public abstract class NRTrackable
    {
        /// <summary> Handle of the trackable native. </summary>
        internal UInt64 TrackableNativeHandle = 0;

        private NRTrackableSubsystem m_TrackableSubsystem;
        internal NRTrackableSubsystem TrackableSubsystem
        {
            get
            {
                if (m_TrackableSubsystem == null)
                {
                    m_TrackableSubsystem = NRSessionManager.Instance.TrackableFactory.TrackableSubsystem;
                }
                return m_TrackableSubsystem;
            }
        }

        /// <summary> Constructor. </summary>
        /// <param name="trackableNativeHandle"> Handle of the trackable native.</param>
        /// <param name="nativeinterface">       The nativeinterface.</param>
        internal NRTrackable(UInt64 trackableNativeHandle)
        {
            TrackableNativeHandle = trackableNativeHandle;
        }

        /// <summary> Get the id of trackable. </summary>
        /// <returns> The data base index. </returns>
        public int GetDataBaseIndex()
        {
            UInt32 identify = TrackableSubsystem.GetIdentify(TrackableNativeHandle);
            identify &= 0X0000FFFF;
            return (int)identify;
        }

        /// <summary> Get the tracking state of current trackable. </summary>
        /// <returns> The tracking state. </returns>
        public TrackingState GetTrackingState()
        {
            if (NRFrame.SessionStatus != SessionState.Running)
            {
                return TrackingState.Stopped;
            }
            return TrackableSubsystem.GetTrackingState(TrackableNativeHandle);
        }

        /// <summary> Type of the trackable. </summary>
        TrackableType trackableType;
        /// <summary> Get the tracking type of current trackable. </summary>
        /// <returns> The trackable type. </returns>
        public TrackableType GetTrackableType()
        {
            if (NRFrame.SessionStatus != SessionState.Running)
            {
                return trackableType;
            }
            trackableType = TrackableSubsystem.GetTrackableType(TrackableNativeHandle);
            return trackableType;
        }

        /// <summary> Get the center pose of current trackable. </summary>
        /// <returns> The center pose. </returns>
        public virtual Pose GetCenterPose()
        {
            return Pose.identity;
        }

        /// <summary> Creates an anchor attached to current trackable at given pose. </summary>
        /// <returns> The new anchor. </returns>
        public NRAnchor CreateAnchor()
        {
            return NRAnchor.Factory(this);
        }
    }
}
