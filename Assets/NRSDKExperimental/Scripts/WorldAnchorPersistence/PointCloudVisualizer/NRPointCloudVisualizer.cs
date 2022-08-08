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
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using System;

    public class NRPointCloudVisualizer : IPointCloudDrawer
    {
        public Dictionary<Int64, PointCloudPoint> Points = new Dictionary<Int64, PointCloudPoint>();
        private Mesh m_Mesh;
        private GameObject visualizer;
        public static Dictionary<ConfidenceLevel, Color> ConfidenceColorMap = new Dictionary<ConfidenceLevel, Color>() {
            { ConfidenceLevel.Nice,new Color(1,0,0)},
            { ConfidenceLevel.Good,new Color(1,0,0.4f)},
            { ConfidenceLevel.Normal,new Color(1,0,0.8f)},
            { ConfidenceLevel.Poor,new Color(0.8f,0,1)},
            { ConfidenceLevel.Bad,new Color(0.4f,0,1)},
            { ConfidenceLevel.VeryBad,new Color(0,0,1)},
        };

        public NRPointCloudVisualizer()
        {
            visualizer = new GameObject("PointCloudVisualizer");
            m_Mesh = new Mesh();
            visualizer.AddComponent<MeshFilter>().mesh = m_Mesh;
            visualizer.AddComponent<MeshRenderer>().material = new Material(Resources.Load<Shader>("VertexColor"));
        }

        public void Draw()
        {
            PointCloudPoint[] pointlist = Points.Values.ToArray<PointCloudPoint>();
            Vector3[] points = new Vector3[pointlist.Length];
            int[] indecies = new int[pointlist.Length];
            Color[] colors = new Color[pointlist.Length];

            for (int i = 0; i < points.Length; ++i)
            {
                var pos = pointlist[i].Position;
                points[i] = new Vector3(pos.X, pos.Y, pos.Z);
                indecies[i] = i;
                colors[i] = ConfidenceColorMap[pointlist[i].confidenceLevel];
            }

            m_Mesh.vertices = points;
            m_Mesh.colors = colors;
            m_Mesh.SetIndices(indecies, MeshTopology.Points, 0);
        }

        public void AdjustPointSize(float size)
        {
            visualizer.GetComponent<MeshRenderer>().material.SetFloat("_PointSize", size);
        }

        public void Update(PointCloudPoint point)
        {
            Points[point.Id] = point;
        }
    }
}
