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

    /// <summary>
    /// The interface contains methods for controller to update virtual controller visuals, and to
    /// show the feed back of user interactivation. </summary>
    public interface IControllerVisual
    {
        /// <summary> Sets an active. </summary>
        /// <param name="isActive"> True if is active, false if not.</param>
        void SetActive(bool isActive);
        /// <summary> Updates the visual described by state. </summary>
        /// <param name="state"> The state.</param>
        void UpdateVisual(ControllerState state);
        /// <summary> Destroys the self. </summary>
        void DestroySelf();
    }
}