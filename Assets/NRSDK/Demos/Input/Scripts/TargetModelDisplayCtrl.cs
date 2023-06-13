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
    /// <summary> A target model display control. </summary>
    public class TargetModelDisplayCtrl : MonoBehaviour
    {
        /// <summary> The model target. </summary>
        public Transform modelTarget;
        /// <summary> The model renderer. </summary>
        public MeshRenderer modelRenderer;
        /// <summary> The default color. </summary>
        public Color defaultColor = Color.white;
        /// <summary> The minimum scale. </summary>
        public float minScale = 1f;
        /// <summary> The maximum scale. </summary>
        public float maxScale = 3f;

        /// <summary> The around local axis. </summary>
        private Vector3 m_AroundLocalAxis = Vector3.down;
        /// <summary> The touch scroll speed. </summary>
        private float m_TouchScrollSpeed = 10000f;
        /// <summary> The previous position. </summary>
        private Vector2 m_PreviousPos;

        /// <summary> Starts this object. </summary>
        void Start()
        {
            ResetModel();
        }

        /// <summary> Updates this object. </summary>
        private void Update()
        {
            if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                m_PreviousPos = NRInput.GetTouch();
            }
            else if (NRInput.GetButton(ControllerButton.TRIGGER))
            {
                UpdateScroll();
            }
            else if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
            {
                m_PreviousPos = Vector2.zero;
            }
        }

        /// <summary> Updates the scroll. </summary>
        private void UpdateScroll()
        {
            if (m_PreviousPos == Vector2.zero)
                return;
            Vector2 deltaMove = NRInput.GetTouch() - m_PreviousPos;
            m_PreviousPos = NRInput.GetTouch();
            modelTarget.Rotate(m_AroundLocalAxis, deltaMove.x * m_TouchScrollSpeed * Time.deltaTime, Space.Self);
        }

        /// <summary> Change model color. </summary>
        /// <param name="color"> The color.</param>
        public void ChangeModelColor(Color color)
        {
            modelRenderer.material.color = color;
        }

        /// <summary> Change model scale. </summary>
        /// <param name="val"> The value.</param>
        public void ChangeModelScale(float val) // 0 ~ 1 
        {
            modelTarget.localScale = Vector3.one * Mathf.SmoothStep(minScale, maxScale, val);
        }

        /// <summary> Resets the model. </summary>
        public void ResetModel()
        {
            modelTarget.localRotation = Quaternion.identity;
            ChangeModelScale(0f);
            ChangeModelColor(defaultColor);
        }
    }
}
