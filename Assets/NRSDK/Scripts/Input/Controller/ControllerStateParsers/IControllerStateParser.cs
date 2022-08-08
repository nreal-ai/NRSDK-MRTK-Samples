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
    /// The interface contains methods for controller provider to parse raw sates to usable states. </summary>
    public interface IControllerStateParser
    {
        /// <summary> Parser controller state. </summary>
        /// <param name="state"> The state.</param>
        void ParserControllerState(ControllerState state);
    }
}