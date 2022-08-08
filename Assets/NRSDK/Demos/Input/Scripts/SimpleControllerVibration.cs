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
    /// <summary> A simple controller vibration. </summary>
    public class SimpleControllerVibration : MonoBehaviour
    {
        /// <summary> The vibration time. </summary>
        public float vibrationTime = 0.06f;
        /// <summary> The vibration buttons. </summary>
        public ControllerButton[] vibrationButtons = { ControllerButton.TRIGGER, ControllerButton.APP, ControllerButton.HOME };

        /// <summary> Updates this object. </summary>
        void Update()
        {
            if (vibrationButtons == null || vibrationButtons.Length == 0)
                return;
            for (int i = 0; i < vibrationButtons.Length; i++)
            {
                if (NRInput.GetButtonDown(ControllerHandEnum.Right, vibrationButtons[i]))
                    NRInput.TriggerHapticVibration(ControllerHandEnum.Right, vibrationTime);
                if (NRInput.GetButtonDown(ControllerHandEnum.Left, vibrationButtons[i]))
                    NRInput.TriggerHapticVibration(ControllerHandEnum.Left, vibrationTime);
            }
        }
    }
}
