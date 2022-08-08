/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.Persistence
{
    using UnityEngine;

    [RequireComponent(typeof(ImageTrackingAnchorTool))]
    public class AnchorSynchronization : MonoBehaviour
    {
        // This is the object which you want to synchronize.
        public Transform m_SynchronousTransform;
        // This is an anchor load/add tool using image tracking.
        private ImageTrackingAnchorTool m_ImageTrackingAnchorTool;
        private NRWorldAnchor m_Anchor;

        private void Start()
        {
            m_ImageTrackingAnchorTool = gameObject.GetComponent<ImageTrackingAnchorTool>();
            m_ImageTrackingAnchorTool.OnAnchorLoaded += OnImageTrackingAnchorLoaded;
        }

        void Update()
        {
            if (m_Anchor != null && m_Anchor.GetTrackingState() == TrackingState.Tracking)
            {
                m_SynchronousTransform.position = m_Anchor.transform.position;
                m_SynchronousTransform.rotation = m_Anchor.transform.rotation;
            }
        }

        private void OnImageTrackingAnchorLoaded(string key, NRWorldAnchor anchor)
        {
            NRDebugger.Info("[AnchorSynchronization] The anchor:{0} is Loaded", key);
            this.m_Anchor = anchor;
        }
    }
}
