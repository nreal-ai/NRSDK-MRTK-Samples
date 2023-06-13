/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using UnityEngine;

namespace NRKernal.NRExamples
{
    /// <summary> A trackable image behaviour. </summary>
    public class TrackableImageBehaviour : MonoBehaviour
    {
        /// <summary> The detected marker. </summary>
        private NRTrackableImage m_DetectedMarker;

        /// <summary> Initializes this object. </summary>
        /// <param name="marker"> The marker.</param>
        public void Initialize(NRTrackableImage marker)
        {
            m_DetectedMarker = marker;
        }

        /// <summary> Updates this object. </summary>
        private void Update()
        {
            if (m_DetectedMarker != null && m_DetectedMarker.GetTrackingState() == TrackingState.Tracking)
            {
                Vector2 size = m_DetectedMarker.Size;
                transform.localScale = new Vector3(size.x, transform.localScale.y, size.y);
            }
        }
    }
}
