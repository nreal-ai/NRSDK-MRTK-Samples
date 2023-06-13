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
    public class SetFocusPlaneTool : MonoBehaviour
    {
        private Transform m_HeadTransfrom;
        private Vector3 m_FocusPosition;
        RaycastHit hitResult;
        private const float MaxDistance = 500;

        void Update()
        {
            if (m_HeadTransfrom == null)
            {
                m_HeadTransfrom = NRSessionManager.Instance.CenterCameraAnchor;
                if (m_HeadTransfrom == null)
                {
                    return;
                }
            }

            if (Physics.Raycast(new Ray(m_HeadTransfrom.position, m_HeadTransfrom.forward), out hitResult, MaxDistance))
            {
                m_FocusPosition = m_HeadTransfrom.InverseTransformPoint(hitResult.point);
                NRSessionManager.Instance.NRRenderer?.SetFocusDistance(m_FocusPosition.magnitude);
            }
        }
    }
}
