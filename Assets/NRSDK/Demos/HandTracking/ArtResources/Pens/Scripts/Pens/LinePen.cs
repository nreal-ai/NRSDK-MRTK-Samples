using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePen : BasePen
{
    public GameObject lineRendererPrefab;
    public Transform penPoint;
    public float lineWidth = 0.005f;
    public float lineLifeTime = 8f;

    private GameObject m_LineRendererObj;
    private LineRenderer m_LineRenderer;
    private List<Vector3> m_WorldPosList = new List<Vector3>();

    private const float MIN_LINE_SEGMENT = 0.01f;

    private void Update()
    {
        if (IsDrawing)
        {
            if (m_LineRendererObj == null)
            {
                CreateColoredLine();
            }

            Vector3 pos = penPoint.position;
            if (m_WorldPosList.Count > 1 && Vector3.Distance(pos, m_WorldPosList[m_WorldPosList.Count - 1]) < MIN_LINE_SEGMENT)
                return;

            Draw(pos);
        }
        else
        {
            DelayClearLine();
        }
    }

    private void CreateColoredLine()
    {
        m_LineRendererObj = Instantiate(lineRendererPrefab, this.transform);
        m_LineRendererObj.SetActive(true);
        m_LineRenderer = m_LineRendererObj.GetComponent<LineRenderer>();
        m_LineRenderer.numCapVertices = 8;
        m_LineRenderer.numCornerVertices = 8;
        m_LineRenderer.startWidth = lineWidth;
        m_LineRenderer.endWidth = lineWidth;
    }

    private void Draw(Vector3 pos)
    {
        m_WorldPosList.Add(pos);
        m_LineRenderer.positionCount = m_WorldPosList.Count;
        m_LineRenderer.SetPositions(m_WorldPosList.ToArray());
    }

    private void DelayClearLine()
    {
        if (m_LineRendererObj)
        {
            m_LineRendererObj.transform.SetParent(null);
            m_LineRendererObj.AddComponent<DelayAutoDestroySelf>().DestroySelfWithDelay(lineLifeTime);
        }
        m_LineRendererObj = null;

        if (m_WorldPosList.Count != 0)
        {
            m_WorldPosList.Clear();
        }
    }
}
