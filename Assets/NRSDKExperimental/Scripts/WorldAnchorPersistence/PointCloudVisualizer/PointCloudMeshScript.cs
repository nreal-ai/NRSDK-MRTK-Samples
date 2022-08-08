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
    using UnityEngine;

    /// <summary>
    /// Point cloud mesh script.
    /// Create random points and put it in a Mesh with Point Topology. The color depend of the vertical (Y) position of the point. 
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PointCloudMeshScript : MonoBehaviour
    {

        private Mesh mesh;
        int numPoints = 60000;

        // Use this for initialization
        void Start()
        {
            mesh = new Mesh();

            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshRenderer>().material = new Material(Shader.Find("Custom/VertexColor"));
            CreateMesh();
        }

        void CreateMesh()
        {
            Vector3[] points = new Vector3[numPoints];
            int[] indecies = new int[numPoints];
            Color[] colors = new Color[numPoints];

            int max = 10;
            int min = -10;

            for (int i = 0; i < points.Length; ++i)
            {
                int x = Random.Range(min, max);
                int y = Random.Range(min, max);
                int z = Random.Range(min, max);
                points[i] = new Vector3(x, y, z);
                indecies[i] = i;

                float value = (float)1.0 * (((float)y - (float)min) / ((float)max - (float)min));

                colors[i] = new Color(value, value, value, 1.0f);
            }

            mesh.vertices = points;
            mesh.colors = colors;
            mesh.SetIndices(indecies, MeshTopology.Points, 0);
        }
    }
}
