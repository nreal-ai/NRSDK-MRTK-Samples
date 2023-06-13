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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    
    /// <summary> A nr phone controller visual. </summary>
    public class NRPhoneControllerVisual : MonoBehaviour, IControllerVisual
    {
        /// <summary> Destroys the self. </summary>
        public void DestroySelf()
        {
            if (gameObject)
                Destroy(gameObject);
        }

        /// <summary> Sets an active. </summary>
        /// <param name="isActive"> True if is active, false if not.</param>
        public void SetActive(bool isActive)
        {
            if (!gameObject)
                return;
            gameObject.SetActive(isActive);
        }

        /// <summary> Updates the visual described by state. </summary>
        /// <param name="state"> The state.</param>
        public void UpdateVisual(ControllerState state)
        {
            if (!gameObject || !gameObject.activeSelf)
                return;

        }
    }
    
}
