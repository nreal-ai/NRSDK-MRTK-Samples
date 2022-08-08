/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.Experimental.NRExamples
{

    /// <summary> A colored pen. </summary>
    public class ColoredPen : MonoBehaviour
    {
        /// <summary> The line renderer prefab. </summary>
        public GameObject lineRendererPrefab;
        /// <summary> The pen point. </summary>
        public Transform penPoint;
        /// <summary> Width of the line. </summary>
        public float lineWidth = 0.005f;

        /// <summary> The line renderer object. </summary>
        private GameObject m_LineRendererObj;
        /// <summary> The line renderer. </summary>
        private LineRenderer m_LineRenderer;
        /// <summary> List of world positions. </summary>
        private List<Vector3> m_WorldPosList = new List<Vector3>();
        /// <summary> True if is drawing, false if not. </summary>
        private bool m_IsDrawing = false;
        /// <summary> True if is picked up, false if not. </summary>
        private bool m_IsPickedUp = true;

        /// <summary> The minimum line segment. </summary>
        private const float MIN_LINE_SEGMENT = 0.01f;

        /// <summary> hide laser and reticle in this demo. </summary>
        private void Start()
        {
            NRInput.LaserVisualActive = false;
            NRInput.ReticleVisualActive = false;
        }

        /// <summary> Updates this object. </summary>
        private void Update()
        {
            m_IsDrawing = m_IsPickedUp && NRInput.GetButton(ControllerButton.TRIGGER);
            if (m_IsDrawing)
            {
                if (m_LineRendererObj == null)
                    CreateColoredLine();
                Vector3 pos = penPoint.position;
                if (m_WorldPosList.Count > 1 && Vector3.Distance(pos, m_WorldPosList[m_WorldPosList.Count - 1]) < MIN_LINE_SEGMENT)
                    return;
                m_WorldPosList.Add(pos);
                Draw();
            }
            else
            {
                if (m_LineRendererObj)
                    m_LineRendererObj = null;
                if (m_WorldPosList.Count != 0)
                    m_WorldPosList.Clear();
            }

        }

        /// <summary> Creates colored line. </summary>
        private void CreateColoredLine()
        {
            m_LineRendererObj = Instantiate(lineRendererPrefab, this.transform);
            m_LineRendererObj.SetActive(true);
            m_LineRenderer = m_LineRendererObj.GetComponent<LineRenderer>();
            m_LineRenderer.numCapVertices = 8;
            m_LineRenderer.numCornerVertices = 8;
            m_LineRenderer.startWidth = 0.01f;
            m_LineRenderer.endWidth = 0.01f;
        }

        /// <summary> Draws this object. </summary>
        private void Draw()
        {
            m_LineRenderer.positionCount = m_WorldPosList.Count;
            m_LineRenderer.SetPositions(m_WorldPosList.ToArray());
        }
    }
}
