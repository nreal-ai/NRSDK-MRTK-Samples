/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.NRExamples
{
    /// <summary> The points visualizer. </summary>
    public class PointsVisualizer
    {
        /// <summary> The point entity. </summary>
        public List<GameObject> pointEntity = new List<GameObject>();

        /// <summary> Shows the given points. </summary>
        /// <param name="points"> The points.</param>
        public void Show(List<Vector3> points)
        {
            int objs_len = pointEntity.Count;
            int points_len = points.Count;
            if (objs_len < points_len)
            {
                for (int i = 0; i < points_len - objs_len; i++)
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    GameObject.Destroy(go.GetComponent<BoxCollider>());
                    go.transform.localScale = Vector3.one * 0.1f;
                    pointEntity.Add(go);
                }
            }
            else
            {
                for (int i = points_len; i < objs_len; i++)
                {
                    pointEntity[i].SetActive(false);
                }
            }

            for (int i = 0; i < points.Count; i++)
            {
                pointEntity[i].transform.position = points[i];
                pointEntity[i].SetActive(true);
            }
        }
    }
}
