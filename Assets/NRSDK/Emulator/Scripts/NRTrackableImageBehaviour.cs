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

    /// <summary> A nr trackable image behaviour. </summary>
    public class NRTrackableImageBehaviour : NRTrackableBehaviour
    {
        /// <summary> The aspect ratio. </summary>
        [HideInInspector, SerializeField]
        private float m_AspectRatio;

        /// <summary> The width. </summary>
        [HideInInspector, SerializeField]
        private float m_Width;

        /// <summary> The height. </summary>
        [HideInInspector, SerializeField]
        private float m_Height;

        /// <summary> The tracking image database. </summary>
        [HideInInspector, SerializeField]
        private string m_TrackingImageDatabase;

        /// <summary> Awakes this object. </summary>
        private void Awake()
        {
#if !UNITY_EDITOR
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null) Destroy(meshRenderer);
            MeshFilter mesh = GetComponent<MeshFilter>();
            if (mesh != null) Destroy(mesh);
#endif
        }

#if UNITY_EDITOR
        private void Update()
        {
            float extent = transform.lossyScale.x * 1000;
            if (NREmulatorManager.Instance.IsInGameView(transform.position))
            {
                NREmulatorTrackableProvider.UpdateTrackableData<NRTrackableImage>
                (transform.position, transform.rotation, extent, extent, (uint)DatabaseIndex, TrackingState.Tracking);
            }
            else
            {
                NREmulatorTrackableProvider.UpdateTrackableData<NRTrackableImage>
                (transform.position, transform.rotation, extent, extent, (uint)DatabaseIndex, TrackingState.Stopped);
            }
        }
#endif
    }
}