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
    /// <summary> A trackable plane behaviour. </summary>
    public class TrackablePlaneBehaviour : NRTrackableBehaviour
    {
        /// <summary> Gets the trackable plane. </summary>
        /// <value> The trackable plane. </value>
        public NRTrackablePlane TrackablePlane
        {
            get
            {
                return (NRTrackablePlane)Trackable;
            }
        }

        /// <summary> Updates this object. </summary>
        private void Update()
        {
            if (TrackablePlane != null && TrackablePlane.GetTrackingState() == TrackingState.Tracking)
            {
                Pose pos = Trackable.GetCenterPose();
                transform.position = pos.position;
                transform.rotation = pos.rotation;

                transform.localScale = new Vector3(TrackablePlane.ExtentX, 0.001f, TrackablePlane.ExtentZ);
            }
        }
    }
}
