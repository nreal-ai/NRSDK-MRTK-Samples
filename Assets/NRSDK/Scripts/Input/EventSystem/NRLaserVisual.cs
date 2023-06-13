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

    
    /// <summary> A nr laser visual. </summary>
    public class NRLaserVisual : MonoBehaviour
    {
        /// <summary> The raycaster. </summary>
        [SerializeField]
        private NRPointerRaycaster m_Raycaster;
        /// <summary> The line renderer. </summary>
        [SerializeField]
        private LineRenderer m_LineRenderer;

        /// <summary> True to show, false to hide the on hit only. </summary>
        public bool showOnHitOnly;
        /// <summary> The default distance. </summary>
        public float defaultDistance = 1.2f;

        /// <summary> Awakes this object. </summary>
        private void Awake()
        {
            defaultDistance = Mathf.Clamp(defaultDistance, m_Raycaster.NearDistance, m_Raycaster.FarDistance);
        }

        /// <summary> Late update. </summary>
        protected virtual void LateUpdate()
        {
            if (!NRInput.LaserVisualActive)
            {
                m_LineRenderer.enabled = false;
                return;
            }
            var result = m_Raycaster.FirstRaycastResult();
            if (showOnHitOnly && !result.isValid)
            {
                m_LineRenderer.enabled = false;
                return;
            }

            var points = m_Raycaster.BreakPoints;
            var pointCount = points.Count;

            if (pointCount < 2)
            {
                return;
            }

            m_LineRenderer.enabled = true;
            m_LineRenderer.useWorldSpace = false;

            var startPoint = transform.TransformPoint(0f, 0f, m_Raycaster.NearDistance);
            var endPoint = result.isValid ? points[pointCount - 1]
                : (m_Raycaster.transform.position + m_Raycaster.transform.forward * defaultDistance);

            if (pointCount == 2)
            {
#if UNITY_5_6_OR_NEWER
                m_LineRenderer.positionCount = 2;
#elif UNITY_5_5_OR_NEWER
            lineRenderer.numPositions = 2;
#else
            lineRenderer.SetVertexCount(2);
#endif
                m_LineRenderer.SetPosition(0, transform.InverseTransformPoint(startPoint));
                m_LineRenderer.SetPosition(1, transform.InverseTransformPoint(endPoint));
            }
        }

        /// <summary> Executes the 'disable' action. </summary>
        protected virtual void OnDisable()
        {
            m_LineRenderer.enabled = false;
        }
    }
    
}