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

    /// <summary> A follow mouse move. </summary>
    public class FollowMouseMove : MonoBehaviour
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        /// <summary> The camera tran. </summary>
        [SerializeField] Transform m_CameraTran;
        /// <summary> The move speed. </summary>
        [SerializeField] float m_MoveSpeed = 5.0f;
        /// <summary> Gets the mouse y coordinate. </summary>
        /// <value> The m mouse y coordinate. </value>
        private float m_MouseX, m_MouseY;
        /// <summary> True to need update rotation. </summary>
        private bool m_NeedUpdateRotation = false;
        /// <summary> True to need update position. </summary>
        private bool m_NeedUpdatePosition = false;

        /// <summary> Updates this object. </summary>
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                m_CameraTran.localPosition = Vector3.zero;
                m_CameraTran.localRotation = Quaternion.Euler(Vector3.zero);
            }
            if (Input.GetMouseButtonDown(1))
            {
                m_NeedUpdateRotation = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                m_NeedUpdateRotation = false;
            }

            if (m_NeedUpdateRotation)
            {
                m_MouseX = Input.GetAxis("Mouse X") * m_MoveSpeed;
                m_MouseY = Input.GetAxis("Mouse Y") * m_MoveSpeed;

                m_CameraTran.localEulerAngles = m_CameraTran.localEulerAngles + new Vector3(-m_MouseY, m_MouseX, 0);
            }

            if (Input.GetMouseButtonDown(0))
            {
                m_NeedUpdatePosition = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                m_NeedUpdatePosition = false;
            }

            if (m_NeedUpdatePosition)
            {
                m_MouseX = Input.GetAxis("Mouse X");
                m_MouseY = Input.GetAxis("Mouse Y");

                m_CameraTran.localPosition = m_CameraTran.localPosition + m_CameraTran.forward * m_MouseY + m_CameraTran.right * m_MouseX;
            }
        }
#endif
    }
}