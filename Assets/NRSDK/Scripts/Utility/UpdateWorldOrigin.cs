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

    /// <summary> Align the world coordinate to the pose. </summary>
    public class UpdateWorldOrigin
    {
        /// <summary> Align the world coordinate to positon and rotation. </summary>
        /// <param name="cameraRoot"> .</param>
        /// <param name="position">   .</param>
        /// <param name="rotation">   .</param>
        public static void AlignWorldCoordinate(Transform cameraRoot, Vector3 position, Quaternion rotation)
        {
            var marker_in_world = ConversionUtility.GetTMatrix(position, rotation);
            var world_in_marker = Matrix4x4.Inverse(marker_in_world);
            cameraRoot.position = ConversionUtility.GetPositionFromTMatrix(world_in_marker);
            cameraRoot.rotation = ConversionUtility.GetRotationFromTMatrix(world_in_marker);
        }
    }
}