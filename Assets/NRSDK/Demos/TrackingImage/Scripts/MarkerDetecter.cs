/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.NRExamples
{
    /// <summary> A marker detecter. </summary>
    public class MarkerDetecter : MonoBehaviour
    {
        /// <summary> A prefab for tracking and visualizing detected markers. </summary>
        public GameObject DetectedMarkerPrefab;

        /// <summary>
        /// A list to hold new planes NRSDK began tracking in the current frame. This object is used
        /// across the application to avoid per-frame allocations. </summary>
        private List<NRTrackableImage> m_NewMarkers = new List<NRTrackableImage>();

        /// <summary> Updates this object. </summary>
        public void Update()
        {
            NRFrame.GetTrackables<NRTrackableImage>(m_NewMarkers, NRTrackableQueryFilter.New);
            for (int i = 0; i < m_NewMarkers.Count; i++)
            {
                NRDebugger.Info("[MarkerDetecter] Get New TrackableImages!! " + m_NewMarkers[i].TrackableNativeHandle);
                // Instantiate a visualization marker.
                NRAnchor anchor = m_NewMarkers[i].CreateAnchor();
                GameObject markerObject = Instantiate(DetectedMarkerPrefab, Vector3.zero, Quaternion.identity, anchor.transform);
                markerObject.GetComponent<TrackableImageBehaviour>().Initialize(m_NewMarkers[i]);
            }
        }

        /// <summary> Switch image tracking mode. </summary>
        /// <param name="on"> True to on.</param>
        public void SwitchImageTrackingMode(bool on)
        {
            var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
            config.ImageTrackingMode = on ? TrackableImageFindingMode.ENABLE : TrackableImageFindingMode.DISABLE;
            NRSessionManager.Instance.SetConfiguration(config);
        }
    }
}
