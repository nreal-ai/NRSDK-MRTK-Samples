/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.NRExamples
{
    using UnityEngine;

    /// <summary> A recyclable toys. </summary>
    public class RecyclableToys : NRGrabbableObject
    {
        /// <summary> The origin position. </summary>
        private Vector3 m_OriginPos;
        /// <summary> The origin rot. </summary>
        private Quaternion m_OriginRot;
        /// <summary> The minimum position y coordinate. </summary>
        private float m_MinPositionY = -1.8f;

        /// <summary> Starts this object. </summary>
        void Start()
        {
            m_OriginPos = transform.position;
            m_OriginRot = transform.rotation;
        }

        /// <summary> Updates this object. </summary>
        void Update()
        {
            if (transform.position.y < m_MinPositionY)
                RecycleSelf();
        }

        /// <summary> Recycle self. </summary>
        private void RecycleSelf()
        {
            transform.position = m_OriginPos;
            transform.rotation = m_OriginRot;
            m_AttachedRigidbody.velocity = Vector3.zero;
            m_AttachedRigidbody.angularVelocity = Vector3.zero;
        }
    }
}