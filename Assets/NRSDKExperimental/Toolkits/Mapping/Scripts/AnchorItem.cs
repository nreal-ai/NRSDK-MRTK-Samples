/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.Persistence
{
    using NRKernal.Experimental.Persistence;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary> An anchor item. </summary>
    public class AnchorItem : MonoBehaviour, IPointerClickHandler
    {
        /// <summary> The key. </summary>
        public string key;
        /// <summary> The on anchor item click. </summary>
        public Action<string, GameObject> OnAnchorItemClick;

        private NRWorldAnchorStore m_NRWorldAnchorStore;

        void Start()
        {
            NRWorldAnchorStore.GetAsync((anchorstore) =>
            {
                m_NRWorldAnchorStore = anchorstore;
            });
        }

        public void DeleteKey()
        {
            m_NRWorldAnchorStore?.Delete(key);
        }

        /// <summary> <para>Use this callback to detect clicks.</para> </summary>
        /// <param name="eventData"> Current event data.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            OnAnchorItemClick?.Invoke(key, gameObject);
        }
    }
}
