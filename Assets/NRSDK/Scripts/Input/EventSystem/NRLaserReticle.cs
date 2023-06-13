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
    
    /// <summary> A nr laser reticle. </summary>
    public class NRLaserReticle : MonoBehaviour
    {
        /// <summary> Values that represent reticle states. </summary>
        public enum ReticleState
        {
            /// <summary> An enum constant representing the hide option. </summary>
            Hide,
            /// <summary> An enum constant representing the normal option. </summary>
            Normal,
            /// <summary> An enum constant representing the hover option. </summary>
            Hover,
        }

        /// <summary> The raycaster. </summary>
        [SerializeField]
        private NRPointerRaycaster m_Raycaster;
        /// <summary> The default visual. </summary>
        [SerializeField]
        private GameObject m_DefaultVisual;
        /// <summary> The hover visual. </summary>
        [SerializeField]
        private GameObject m_HoverVisual;

        /// <summary> The hit target. </summary>
        private GameObject m_HitTarget;
        /// <summary> True if is visible, false if not. </summary>
        private bool m_IsVisible = true;

        /// <summary> The default distance. </summary>
        public float defaultDistance = 2.5f;
        /// <summary> The reticle size ratio. </summary>
        public float reticleSizeRatio = 0.02f;

        /// <summary> Gets the camera root. </summary>
        /// <value> The m camera root. </value>
        private Transform m_CameraRoot
        {
            get
            {
                return NRInput.CameraCenter;
            }
        }

        /// <summary> Gets the hit target. </summary>
        /// <value> The hit target. </value>
        public GameObject HitTarget
        {
            get
            {
                return m_HitTarget;
            }
        }

        /// <summary> Awakes this object. </summary>
        private void Awake()
        {
            SwitchReticleState(ReticleState.Hide);
            defaultDistance = Mathf.Clamp(defaultDistance, m_Raycaster.NearDistance, m_Raycaster.FarDistance);
        }

        /// <summary> Late update. </summary>
        protected virtual void LateUpdate()
        {
            if (!m_IsVisible || !NRInput.ReticleVisualActive)
            {
                SwitchReticleState(ReticleState.Hide);
                return;
            }
            var result = m_Raycaster.FirstRaycastResult();
            var points = m_Raycaster.BreakPoints;
            var pointCount = points.Count;
            if (result.isValid)
            {
                SwitchReticleState(ReticleState.Hover);
                transform.position = result.worldPosition;
                transform.rotation = Quaternion.LookRotation(result.worldNormal, m_Raycaster.transform.forward);

                m_HitTarget = result.gameObject;
            }
            else
            {
                SwitchReticleState(ReticleState.Normal);
                if (pointCount != 0)
                {
                    transform.localPosition = Vector3.forward * defaultDistance;
                    transform.localRotation = Quaternion.identity;
                }

                m_HitTarget = null;
            }
            if (m_CameraRoot)
                transform.localScale = Vector3.one * reticleSizeRatio * (transform.position - m_CameraRoot.transform.position).magnitude;
        }

        /// <summary> Executes the 'disable' action. </summary>
        private void OnDisable()
        {
            SwitchReticleState(ReticleState.Hide);
        }

        /// <summary> Switch reticle state. </summary>
        /// <param name="state"> The state.</param>
        private void SwitchReticleState(ReticleState state)
        {
            switch (state)
            {
                case ReticleState.Hide:
                    m_DefaultVisual.SetActive(false);
                    m_HoverVisual.SetActive(false);
                    break;
                case ReticleState.Normal:
                    m_DefaultVisual.SetActive(true);
                    m_HoverVisual.SetActive(false);
                    break;
                case ReticleState.Hover:
                    m_DefaultVisual.SetActive(false);
                    m_HoverVisual.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        /// <summary> Sets a visible. </summary>
        /// <param name="isVisible"> True if is visible, false if not.</param>
        public void SetVisible(bool isVisible)
        {
            this.m_IsVisible = isVisible;
        }
    }
    
}


