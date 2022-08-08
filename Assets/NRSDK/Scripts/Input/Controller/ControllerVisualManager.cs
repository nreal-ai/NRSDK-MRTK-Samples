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

    
    /// <summary> Manager for controller visuals. </summary>
    [DisallowMultipleComponent]
    public class ControllerVisualManager : MonoBehaviour
    {
        /// <summary> The states. </summary>
        private ControllerState[] m_States;
        /// <summary> The controller visuals. </summary>
        private IControllerVisual[] m_ControllerVisuals;

        /// <summary> Executes the 'enable' action. </summary>
        private void OnEnable()
        {
            NRInput.OnControllerStatesUpdated += OnControllerStatesUpdated;
        }

        /// <summary> Executes the 'disable' action. </summary>
        private void OnDisable()
        {
            NRInput.OnControllerStatesUpdated -= OnControllerStatesUpdated;
        }

        /// <summary> Initializes this object. </summary>
        /// <param name="states"> The states.</param>
        public void Init(ControllerState[] states)
        {
            this.m_States = states;
            m_ControllerVisuals = new IControllerVisual[states.Length];
        }

        /// <summary> Change controller visual. </summary>
        /// <param name="index">      Zero-based index of the.</param>
        /// <param name="visualType"> Type of the visual.</param>
        public void ChangeControllerVisual(int index, ControllerVisualType visualType)
        {
            if (m_ControllerVisuals[index] != null)
            {
                DestroyVisual(index);
            }
            CreateControllerVisual(index, visualType);
        }

        /// <summary> Executes the 'controller states updated' action. </summary>
        private void OnControllerStatesUpdated()
        {
            UpdateAllVisuals();
        }

        /// <summary> Updates all visuals. </summary>
        private void UpdateAllVisuals()
        {
            int availableCount = NRInput.GetAvailableControllersCount();
            if (availableCount > 1)
            {
                UpdateVisual(0, m_States[0]);
                UpdateVisual(1, m_States[1]);
            }
            else
            {
                int activeVisual = NRInput.DomainHand == ControllerHandEnum.Right ? 0 : 1;
                int deactiveVisual = NRInput.DomainHand == ControllerHandEnum.Right ? 1 : 0;
                UpdateVisual(activeVisual, m_States[0]);
                DestroyVisual(deactiveVisual);
            }
        }

        /// <summary> Updates the visual. </summary>
        /// <param name="index"> Zero-based index of the.</param>
        /// <param name="state"> The state.</param>
        private void UpdateVisual(int index, ControllerState state)
        {
            ControllerVisualType visualType = ControllerVisualFactory.GetDefaultVisualType(state.controllerType);
            if (m_ControllerVisuals[index] != null && visualType == ControllerVisualType.None)
            {
                DestroyVisual(index);
            }
            else if (m_ControllerVisuals[index] == null && visualType != ControllerVisualType.None)
            {
                CreateControllerVisual(index, visualType);
            }

            if (m_ControllerVisuals[index] != null)
            {
                m_ControllerVisuals[index].SetActive(NRInput.ControllerVisualActive);
                m_ControllerVisuals[index].UpdateVisual(state);
            }
        }

        /// <summary> Creates controller visual. </summary>
        /// <param name="index">      Zero-based index of the.</param>
        /// <param name="visualType"> Type of the visual.</param>
        private void CreateControllerVisual(int index, ControllerVisualType visualType)
        {
            GameObject visualGo = ControllerVisualFactory.CreateControllerVisualObject(visualType);
            if (visualGo == null)
                return;
            m_ControllerVisuals[index] = visualGo.GetComponent<IControllerVisual>();
            if (m_ControllerVisuals[index] != null)
            {
                ControllerAnchorEnum ancherEnum = (index == 0 ? ControllerAnchorEnum.RightModelAnchor : ControllerAnchorEnum.LeftModelAnchor);
                visualGo.transform.parent = NRInput.AnchorsHelper.GetAnchor(ancherEnum);
                visualGo.transform.localPosition = Vector3.zero;
                visualGo.transform.localRotation = Quaternion.identity;
                visualGo.transform.localScale = Vector3.one;
            }
            else
            {
                NRDebugger.Error("The ControllerVisual prefab:" + visualGo.name + " does not contain IControllerVisual interface");
                Destroy(visualGo);
            }
        }

        /// <summary> Destroys the visual described by index. </summary>
        /// <param name="index"> Zero-based index of the.</param>
        private void DestroyVisual(int index)
        {
            if (m_ControllerVisuals[index] != null)
            {
                m_ControllerVisuals[index].DestroySelf();
                m_ControllerVisuals[index] = null;
            }
        }
    }
    
}
