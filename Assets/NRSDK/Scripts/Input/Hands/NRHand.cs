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
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> NRHand usually used as a root of left/right hand. </summary>
    public class NRHand : MonoBehaviour
    {
        [SerializeField]
        private HandEnum m_HandEnum = HandEnum.None;

        public HandEnum HandEnum { get { return m_HandEnum; } }

        private void Awake()
        {
            if(m_HandEnum == HandEnum.None)
            {
                Debug.LogError("HandEnum Should Not Be None !");
                return;
            }
            NRInput.Hands.RegistHand(this);
        }

        private void OnDestroy()
        {
            NRInput.Hands.UnRegistHand(this);
        }

        public HandState GetHandState()
        {
            return NRInput.Hands.GetHandState(m_HandEnum);
        }
    }
}
