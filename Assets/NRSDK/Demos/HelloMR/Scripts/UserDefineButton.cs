/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NRKernal.NRExamples
{
    /// <summary> A user define button. </summary>
    public class UserDefineButton : MonoBehaviour, IPointerClickHandler
    {
        /// <summary> The on click. </summary>
        public Action<string> OnClick;

        /// <summary> <para>Use this callback to detect clicks.</para> </summary>
        /// <param name="eventData"> Current event data.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null)
            {
                OnClick(gameObject.name);
            }
        }
    }
}
