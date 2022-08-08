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


    /// <summary> A nr pointer event data. </summary>
    public class NRPointerEventData : PointerEventData
    {
        /// <summary> The raycaster. </summary>
        public readonly NRPointerRaycaster raycaster;

        /// <summary> The position 3D. </summary>
        public Vector3 position3D;
        /// <summary> The rotation. </summary>
        public Quaternion rotation;

        /// <summary> The position 3D delta. </summary>
        public Vector3 position3DDelta;
        /// <summary> The rotation delta. </summary>
        public Quaternion rotationDelta;

        /// <summary> The press position 3D. </summary>
        public Vector3 pressPosition3D;
        /// <summary> The press rotation. </summary>
        public Quaternion pressRotation;

        /// <summary> The press distance. </summary>
        public float pressDistance;
        /// <summary> The press enter. </summary>
        public GameObject pressEnter;
        /// <summary> True if press precessed. </summary>
        public bool pressPrecessed;

        /// <summary> Constructor. </summary>
        /// <param name="raycaster">   The raycaster.</param>
        /// <param name="eventSystem"> The event system.</param>
        public NRPointerEventData(NRPointerRaycaster raycaster, EventSystem eventSystem) : base(eventSystem)
        {
            this.raycaster = raycaster;
        }

        /// <summary> Gets the press. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public virtual bool GetPress()
        {
            if (raycaster is NRMultScrPointerRaycaster)
            {
                return NRVirtualDisplayer.SystemButtonState.pressing;
            }
            else
            {
                return NRInput.GetButton(raycaster.RelatedHand, ControllerButton.TRIGGER);
            }
        }

        /// <summary> Gets press down. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public virtual bool GetPressDown()
        {
            if (raycaster is NRMultScrPointerRaycaster)
            {
                return NRVirtualDisplayer.SystemButtonState.pressDown;
            }
            else
            {
                return NRInput.GetButtonDown(raycaster.RelatedHand, ControllerButton.TRIGGER);
            }
        }

        /// <summary> Gets press up. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public virtual bool GetPressUp()
        {
            if (raycaster is NRMultScrPointerRaycaster)
            {
                return NRVirtualDisplayer.SystemButtonState.pressUp;
            }
            else
            {
                return NRInput.GetButtonUp(raycaster.RelatedHand, ControllerButton.TRIGGER);
            }
        }

    }

}
