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
    using UnityEngine;

    /// <summary> Eye pose data. </summary>
    public struct EyePoseData
    {
        /// <summary> Left eye pose. </summary>
        public Pose LEyePose;

        /// <summary> Right eye pose. </summary>
        public Pose REyePose;

        /// <summary> RGB eye pose. </summary>
        public Pose RGBEyePose;
    }

    /// <summary> Eye project matrix. </summary>
    public struct EyeProjectMatrixData
    {
        /// <summary> Left display project matrix. </summary>
        public Matrix4x4 LEyeMatrix;

        /// <summary> Right display project matrix. </summary>
        public Matrix4x4 REyeMatrix;

        /// <summary> RGB camera project matrix. </summary>
        public Matrix4x4 RGBEyeMatrix;
    }
}
