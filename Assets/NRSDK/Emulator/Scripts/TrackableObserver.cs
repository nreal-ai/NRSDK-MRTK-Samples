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

    /// <summary> A trackable observer. </summary>
    public class TrackableObserver : MonoBehaviour
    {
        /// <summary> Tracking delegate. </summary>
        /// <param name="pos"> The position.</param>
        /// <param name="qua"> The qua.</param>
        public delegate void TrackingDelegate(Vector3 pos, Quaternion qua);
        /// <summary> The found event. </summary>
        public event TrackingDelegate FoundEvent;
        /// <summary> The lost evnet. </summary>
        public event Action LostEvent;

        /// <summary> Type of the target. </summary>
        public TrackableType TargetType;

        /// <summary> The trackable behaviour. </summary>
        private NRTrackableBehaviour m_TrackableBehaviour;
        /// <summary> The temporary tracking images. </summary>
        private List<NRTrackableImage> m_TempTrackingImages = new List<NRTrackableImage>();
        /// <summary> The temporary tracking plane. </summary>
        private List<NRTrackablePlane> m_TempTrackingPlane = new List<NRTrackablePlane>();

        /// <summary> Values that represent trackable types. </summary>
        public enum TrackableType
        {
            /// <summary> An enum constant representing the trackable image option. </summary>
            TrackableImage,
            /// <summary> An enum constant representing the trackable plane option. </summary>
            TrackablePlane,
        }

        /// <summary> Use this for initialization. </summary>
        void Start()
        {
            m_TrackableBehaviour = GetComponent<NRTrackableBehaviour>();
        }

        /// <summary> Update is called once per frame. </summary>
        void Update()
        {
            if (TargetType == TrackableType.TrackableImage)
            {
                NRFrame.GetTrackables<NRTrackableImage>(m_TempTrackingImages, NRTrackableQueryFilter.All);
                foreach (var trackableImage in m_TempTrackingImages)
                {
                    if (trackableImage.GetDataBaseIndex() == m_TrackableBehaviour.DatabaseIndex)
                    {
                        if (trackableImage.GetTrackingState() == TrackingState.Tracking)
                        {
                            FoundEvent?.Invoke(trackableImage.GetCenterPose().position, trackableImage.GetCenterPose().rotation);
                        }
                        else
                        {
                            LostEvent?.Invoke();
                        }
                        break;
                    }
                }
            }
            else if (TargetType == TrackableType.TrackablePlane)
            {
                NRFrame.GetTrackables<NRTrackablePlane>(m_TempTrackingPlane, NRTrackableQueryFilter.All);
                foreach (var trackablePlane in m_TempTrackingPlane)
                {
                    if (m_TrackableBehaviour.DatabaseIndex == -1)
                        m_TrackableBehaviour.DatabaseIndex = trackablePlane.GetDataBaseIndex();
                    if (trackablePlane.GetDataBaseIndex() == m_TrackableBehaviour.DatabaseIndex)
                    {
                        if (trackablePlane.GetTrackingState() == TrackingState.Tracking)
                        {
                            FoundEvent?.Invoke(trackablePlane.GetCenterPose().position, trackablePlane.GetCenterPose().rotation);
                        }
                        else
                        {
                            LostEvent?.Invoke();
                        }
                        break;
                    }
                }
            }
        }
    }
}