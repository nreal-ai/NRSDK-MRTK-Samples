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
    using UnityEngine.EventSystems;

    
    /// <summary> Interface for canvas raycast target. </summary>
    public interface ICanvasRaycastTarget
    {
        /// <summary> Gets the canvas. </summary>
        /// <value> The canvas. </value>
        Canvas canvas { get; }
        /// <summary> Gets a value indicating whether this object is enabled. </summary>
        /// <value> True if enabled, false if not. </value>
        bool enabled { get; }
        /// <summary> Gets a value indicating whether the ignore reversed graphics. </summary>
        /// <value> True if ignore reversed graphics, false if not. </value>
        bool ignoreReversedGraphics { get; }
    }
    

    /// <summary>
    /// The class enables an UGUI Canvas and its children to be interactive with NRInput raycasters. </summary>
    [RequireComponent(typeof(Canvas))]
    [DisallowMultipleComponent]
    public class CanvasRaycastTarget : UIBehaviour, ICanvasRaycastTarget
    {
        /// <summary> The canvas. </summary>
        private Canvas m_canvas;
        /// <summary> True to ignore reversed graphics. </summary>
        [SerializeField]
        private bool m_IgnoreReversedGraphics = true;

        /// <summary> Gets the canvas. </summary>
        /// <value> The canvas. </value>
        public virtual Canvas canvas { get { return m_canvas ?? (m_canvas = GetComponent<Canvas>()); } }
        /// <summary> Gets or sets a value indicating whether the ignore reversed graphics. </summary>
        /// <value> True if ignore reversed graphics, false if not. </value>
        public bool ignoreReversedGraphics { get { return m_IgnoreReversedGraphics; } set { m_IgnoreReversedGraphics = value; } }

        /// <summary> <para>See MonoBehaviour.OnEnable.</para> </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            CanvasTargetCollector.AddTarget(this);
        }

        /// <summary> <para>See MonoBehaviour.OnDisable.</para> </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            CanvasTargetCollector.RemoveTarget(this);
        }
    }
}