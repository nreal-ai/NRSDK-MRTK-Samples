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
    /// <summary> An automatic rotate. </summary>
    public class AutoRotate : MonoBehaviour
    {
        /// <summary> Updates this object. </summary>
        void Update()
        {
            transform.Rotate(Vector3.up, 1f);
        }
    }
}