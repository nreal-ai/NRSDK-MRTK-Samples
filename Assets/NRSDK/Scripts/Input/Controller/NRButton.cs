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
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    /// <summary> A nr button. </summary>
    public class NRButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary> The image normal. </summary>
        public Sprite ImageNormal;
        /// <summary> The image hover. </summary>
        public Sprite ImageHover;
        /// <summary> The trigger event. </summary>
        public Action<string, GameObject, RaycastResult> TriggerEvent;
        /// <summary> The enter. </summary>
        public const string Enter = "Enter";
        /// <summary> The hover. </summary>
        public const string Hover = "Hover";
        /// <summary> The exit. </summary>
        public const string Exit = "Exit";

        /// <summary> The button image. </summary>
        private Image m_ButtonImage;

        /// <summary> Starts this object. </summary>
        void Start()
        {
            m_ButtonImage = gameObject.GetComponent<Image>();
        }

        /// <summary> <para></para> </summary>
        /// <param name="eventData"> Current event data.</param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (TriggerEvent != null)
            {
                TriggerEvent(Enter, gameObject, eventData.pointerCurrentRaycast);
            }

            if (ImageHover != null && m_ButtonImage != null)
            {
                m_ButtonImage.sprite = ImageHover;
            }
        }

        /// <summary> <para></para> </summary>
        /// <param name="eventData"> Current event data.</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (TriggerEvent != null)
            {
                TriggerEvent(Exit, gameObject, eventData.pointerCurrentRaycast);
            }

            if (ImageNormal != null && m_ButtonImage != null)
            {
                m_ButtonImage.sprite = ImageNormal;
            }
        }

        /// <summary> Get onhover by NRMultScrPointerRaycaster. </summary>
        /// <param name="racastResult"> The racast result.</param>
        public void OnHover(RaycastResult racastResult)
        {
            if (TriggerEvent != null && m_ButtonImage != null)
            {
                TriggerEvent(Hover, gameObject, racastResult);
            }
        }
    }
}
