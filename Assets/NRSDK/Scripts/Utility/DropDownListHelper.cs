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
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class DropDownListHelper : Dropdown
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            RemoveUnityCanvas();
        }

        private void RemoveUnityCanvas()
        {
            var raycasttarget = gameObject.GetComponentsInChildren<GraphicRaycaster>();
            foreach (var item in raycasttarget)
            {
                GameObject.Destroy(item);
            }

            var canvas = gameObject.GetComponentsInChildren<Canvas>();
            foreach (var item in canvas)
            {
                GameObject.Destroy(item);
            }
        }
    }
}
