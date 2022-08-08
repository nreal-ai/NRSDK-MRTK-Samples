/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental
{
    using NRKernal;
    using System;
    using UnityEngine;

    /// <summary> (Serializable) a fov 4f. </summary>
    [Serializable]
    public class Fov4f
    {
        /// <summary> The left. </summary>
        public double left;
        /// <summary> The right. </summary>
        public double right;
        /// <summary> The top. </summary>
        public double top;
        /// <summary> The bottom. </summary>
        public double bottom;

        /// <summary> Default constructor. </summary>
        public Fov4f() { }

        /// <summary> Constructor. </summary>
        /// <param name="l"> A double to process.</param>
        /// <param name="r"> A double to process.</param>
        /// <param name="t"> A double to process.</param>
        /// <param name="b"> A double to process.</param>
        public Fov4f(double l, double r, double t, double b)
        {
            this.left = l;
            this.right = r;
            this.top = t;
            this.bottom = b;
        }

        /// <summary> Convert this object into a string representation. </summary>
        /// <returns> A string that represents this object. </returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", left, right, top, bottom);
        }
    }


    /// <summary> A project matrix utility. </summary>
    public class ProjectMatrixUtility
    {
        /// <summary> Calculates the fov by fcc. </summary>
        /// <param name="k_mat">  The matrix.</param>
        /// <param name="width">  (Optional) The width.</param>
        /// <param name="height"> (Optional) The height.</param>
        /// <returns> The calculated fov by fcc. </returns>
        public static Fov4f CalculateFOVByFCC(NativeMat3f k_mat, int width = 1920, int height = 1080)
        {
            Fov4f fov = new Fov4f();
            float param = 1.0f / k_mat[2, 2];
            NativeMat3f k_mat_normal = NativeMat3f.identity;
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    k_mat_normal[i, j] = k_mat[i, j] * param;
                }
            }

            float fc0 = k_mat_normal[0, 0];
            float fc1 = k_mat_normal[1, 1];
            float cc0 = k_mat_normal[0, 2];
            float cc1 = k_mat_normal[1, 2];

            float l = -cc0;
            float r = width - cc0;
            float t = cc1;
            float b = cc1 - height;

            float x_margin = 0;
            float y_margin = 0;
            l -= x_margin;
            r += x_margin;
            t += y_margin;
            b -= y_margin;

            fov.left = Math.Abs(l / fc0);
            fov.right = Math.Abs(r / fc0);
            fov.top = Math.Abs(t / fc1);
            fov.bottom = Math.Abs(b / fc1);

            return fov;
        }

        /// <summary> Perspective off center. </summary>
        /// <param name="left">   The left.</param>
        /// <param name="right">  The right.</param>
        /// <param name="bottom"> The bottom.</param>
        /// <param name="top">    The top.</param>
        /// <param name="near">   The near.</param>
        /// <param name="far">    The far.</param>
        /// <returns> A Matrix4x4. </returns>
        public static Matrix4x4 PerspectiveOffCenter(double left, double right, double bottom, double top, float near, float far)
        {
            left = -left;
            bottom = -bottom;
            float x = (float)(2.0F / (right - left));
            float y = (float)(2.0F / (top - bottom));
            float a = (float)((right + left) / (right - left));
            float b = (float)((top + bottom) / (top - bottom));
            float c = -(far + near) / (far - near);
            float d = -(2.0F * far * near) / (far - near);
            float e = -1.0F;
            Matrix4x4 m = new Matrix4x4();
            m[0, 0] = x;
            m[0, 1] = 0;
            m[0, 2] = a;
            m[0, 3] = 0;
            m[1, 0] = 0;
            m[1, 1] = y;
            m[1, 2] = b;
            m[1, 3] = 0;
            m[2, 0] = 0;
            m[2, 1] = 0;
            m[2, 2] = c;
            m[2, 3] = d;
            m[3, 0] = 0;
            m[3, 1] = 0;
            m[3, 2] = e;
            m[3, 3] = 0;

            return m;
        }
    }
}