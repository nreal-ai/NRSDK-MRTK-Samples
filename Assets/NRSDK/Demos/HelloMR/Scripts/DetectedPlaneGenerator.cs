/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.NRExamples
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> Manages the visualization of detected planes in the scene. </summary>
    public class DetectedPlaneGenerator : MonoBehaviour
    {
        /// <summary> A prefab for tracking and visualizing detected planes. </summary>
        public GameObject DetectedPlanePrefab;

        /// <summary>
        /// A list to hold new planes NRSDK began tracking in the current frame. This object is used
        /// across the application to avoid per-frame allocations. </summary>
        private List<NRTrackablePlane> m_NewPlanes = new List<NRTrackablePlane>();

        /// <summary> The Unity Update method. </summary>
        public void Update()
        {
            // Check that motion tracking is tracking.
            if (NRFrame.SessionStatus != SessionState.Running)
            {
                return;
            }

            // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
            NRFrame.GetTrackables<NRTrackablePlane>(m_NewPlanes, NRTrackableQueryFilter.New);
            for (int i = 0; i < m_NewPlanes.Count; i++)
            {
                // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
                // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
                // coordinates.
                GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
                planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
            }
        }
    }
}
