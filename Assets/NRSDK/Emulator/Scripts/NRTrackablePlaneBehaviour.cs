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
    using UnityEngine;
    /// <summary> A nr trackable plane behaviour. </summary>
    public class NRTrackablePlaneBehaviour : NRTrackableBehaviour
    {
        /// <summary> Starts this object. </summary>
        private void Start()
        {
#if UNITY_EDITOR
            DatabaseIndex = NREmulatorManager.SIMPlaneID;
            NREmulatorManager.SIMPlaneID++;
#endif

#if !UNITY_EDITOR
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null) Destroy(meshRenderer);
            MeshFilter mesh = GetComponent<MeshFilter>();
            if (mesh != null) Destroy(mesh);
#endif
        }

#if UNITY_EDITOR
        /// <summary> Updates this object. </summary>
        private void Update()
        {
            float extent = transform.lossyScale.x * 1000;
            if (NREmulatorManager.Instance.IsInGameView(transform.position))
            {
                NREmulatorTrackableProvider.UpdateTrackableData<NRTrackablePlane>
                (transform.position, transform.rotation, extent, extent, (uint)DatabaseIndex, TrackingState.Tracking);
            }
            else
            {
                NREmulatorTrackableProvider.UpdateTrackableData<NRTrackablePlane>
                (transform.position, transform.rotation, extent, extent, (uint)DatabaseIndex, TrackingState.Stopped);
            }
        }
#endif
    }
}