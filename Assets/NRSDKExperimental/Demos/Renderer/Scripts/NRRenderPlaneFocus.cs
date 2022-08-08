/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental
{
    using NRKernal;
    using UnityEngine;

    public class NRRenderPlaneFocus : MonoBehaviour
    {
        private Transform m_HeadTransfrom;
        private Vector3 m_FocusPosition;
        RaycastHit hitResult;

        void Start()
        {
            m_HeadTransfrom = NRSessionManager.Instance.CenterCameraAnchor;
        }

        void Update()
        {
            if (Physics.Raycast(new Ray(m_HeadTransfrom.position, m_HeadTransfrom.forward), out hitResult, 100))
            {
                m_FocusPosition = m_HeadTransfrom.InverseTransformPoint(hitResult.point);
#if USING_XR_SDK && !UNITY_EDITOR
                NRSessionManager.Instance.XRDisplaySubsystem?.SetFocusPlane(m_FocusPosition,hitResult.normal,Vector3.zero);
#else
                NRSessionManager.Instance.NRRenderer?.SetFocusDistance(m_FocusPosition.magnitude);
#endif
            }
        }
    }
}
