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

    /// <summary> A conversion utility. </summary>
    public class ConversionUtility
    {
        #region transform utility
        /// <summary> Get a matrix from position and rotation. </summary>
        /// <param name="position"> The position.</param>
        /// <param name="rotation"> The rotation.</param>
        /// <returns> The matrix. </returns>
        public static Matrix4x4 GetTMatrix(Vector3 position, Quaternion rotation)
        {
            return Matrix4x4.TRS(position, rotation, Vector3.one);
        }

        /// <summary> Get a matrix from position , rotation and scale. </summary>
        /// <param name="position"> The position.</param>
        /// <param name="rotation"> The rotation.</param>
        /// <param name="scale">    The scale.</param>
        /// <returns> The matrix. </returns>
        public static Matrix4x4 GetTMatrix(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            return Matrix4x4.TRS(position, rotation, scale);
        }

        /// <summary> Get the position from a matrix4x4. </summary>
        /// <param name="matrix"> The matrix.</param>
        /// <returns> The position from t matrix. </returns>
        public static Vector3 GetPositionFromTMatrix(Matrix4x4 matrix)
        {
            Vector3 position;
            position.x = matrix.m03;
            position.y = matrix.m13;
            position.z = matrix.m23;

            return position;
        }

        /// <summary> Get the rotation from a matrix4x4. </summary>
        /// <param name="matrix"> The matrix.</param>
        /// <returns> The rotation from t matrix. </returns>
        public static Quaternion GetRotationFromTMatrix(Matrix4x4 matrix)
        {
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;

            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;

            return Quaternion.LookRotation(forward, upwards);
        }

        /// <summary> Convert position. </summary>
        /// <param name="vec"> The vector.</param>
        /// <returns> The position converted. </returns>
        public static Vector3 ConvertPosition(Vector3 vec)
        {
            // Convert to left-handed
            return new Vector3((float)vec.x, (float)vec.y, (float)-vec.z);
        }

        /// <summary> Convert orientation. </summary>
        /// <param name="quat"> The quaternion.</param>
        /// <returns> The orientation converted. </returns>
        public static Quaternion ConvertOrientation(Quaternion quat)
        {
            // Convert to left-handed
            return new Quaternion(-(float)quat.x, -(float)quat.y, (float)quat.z, (float)quat.w);
        }
        #endregion

        /// <summary> Unity pose to API pose. </summary>
        /// <param name="unityPose"> [out] The unity pose.</param>
        /// <param name="apiPose">   [out] The API pose.</param>
        public static void UnityPoseToApiPose(Pose unityPose, out NativeMat4f apiPose)
        {
            Matrix4x4 glWorld_T_glLocal = Matrix4x4.TRS(unityPose.position, unityPose.rotation, Vector3.one);
            Matrix4x4 unityWorld_T_glWorld = Matrix4x4.Scale(new Vector3(1, 1, -1));
            Matrix4x4 unityWorld_T_unityLocal = unityWorld_T_glWorld * glWorld_T_glLocal * unityWorld_T_glWorld.inverse;

            Vector3 position = unityWorld_T_unityLocal.GetColumn(3);
            Quaternion rotation = Quaternion.LookRotation(unityWorld_T_unityLocal.GetColumn(2),
                unityWorld_T_unityLocal.GetColumn(1));

            Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
            apiPose = new NativeMat4f(matrix);
        }

        /// <summary> API pose to unity pose. </summary>
        /// <param name="apiPose">   The API pose.</param>
        /// <param name="unityPose"> [out] The unity pose.</param>
        public static void ApiPoseToUnityPose(NativeMat4f apiPose, out Pose unityPose)
        {
            Matrix4x4 glWorld_T_glLocal = apiPose.ToUnityMat4f();
            Matrix4x4 unityWorld_T_glWorld = Matrix4x4.Scale(new Vector3(1, 1, -1));
            Matrix4x4 unityWorld_T_unityLocal = unityWorld_T_glWorld * glWorld_T_glLocal * unityWorld_T_glWorld.inverse;

            Vector3 position = unityWorld_T_unityLocal.GetColumn(3);
            Quaternion rotation = unityWorld_T_unityLocal.rotation;
            unityPose = new Pose(position, rotation);
        }

        /// <summary> API pose to unity matrix. </summary>
        /// <param name="apiPose">     The API pose.</param>
        /// <param name="unityMatrix"> [out] The unity matrix.</param>
        public static void ApiPoseToUnityMatrix(NativeMat4f apiPose, out Matrix4x4 unityMatrix)
        {
            Matrix4x4 glWorld_T_glLocal = apiPose.ToUnityMat4f();
            Matrix4x4 unityWorld_T_glWorld = Matrix4x4.Scale(new Vector3(1, 1, -1));
            Matrix4x4 unityWorld_T_unityLocal = unityWorld_T_glWorld * glWorld_T_glLocal * unityWorld_T_glWorld.inverse;

            unityMatrix = unityWorld_T_unityLocal;
        }

        /// <summary> Gets projection matrix from fov. </summary>
        /// <param name="fov">    The fov.</param>
        /// <param name="z_near"> The near.</param>
        /// <param name="z_far">  The far.</param>
        /// <returns> The projection matrix from fov. </returns>
        public static NativeMat4f GetProjectionMatrixFromFov(NativeFov4f fov, float z_near, float z_far)
        {
            NativeMat4f pm = NativeMat4f.identity;

            float l = fov.left_tan;
            float r = fov.right_tan;
            float t = fov.top_tan;
            float b = fov.bottom_tan;

            pm.column0.X = 2f / (r - l);
            pm.column1.Y = 2f / (t - b);

            pm.column2.X = (r + l) / (r - l);
            pm.column2.Y = (t + b) / (t - b);
            pm.column2.Z = (z_near + z_far) / (z_near - z_far);
            pm.column2.W = -1f;

            pm.column3.Z = (2 * z_near * z_far) / (z_near - z_far);
            pm.column3.W = 0f;

            return pm;
        }

        public static Pose ApiWorldToUnityWorld(Pose apiworld)
        {
            Matrix4x4 world_offse_matrix = NRFrame.GetWorldMatrixFromUnityToNative();
            Matrix4x4 native_pose_matrix = world_offse_matrix * Matrix4x4.TRS(apiworld.position, apiworld.rotation, Vector3.one);
            return new Pose(ConversionUtility.GetPositionFromTMatrix(native_pose_matrix), ConversionUtility.GetRotationFromTMatrix(native_pose_matrix));
        }

        public static Matrix4x4 UnityPoseToCVMatrix(Pose unityPose)
        {
            Matrix4x4 unityMat = GetTMatrix(unityPose.position, unityPose.rotation);
            Matrix4x4 cv_T_unity = Matrix4x4.Scale(new Vector3(1, -1, 1));
            Matrix4x4 cvWorld = cv_T_unity * unityMat * cv_T_unity.inverse;

            return cvWorld;
        }

        public static Matrix4x4 UnityMatrixToCVMatrix(Matrix4x4 unityMat)
        {
            Matrix4x4 cv_T_unity = Matrix4x4.Scale(new Vector3(1, -1, 1));
            Matrix4x4 cvWorld = cv_T_unity * unityMat * cv_T_unity.inverse;

            return cvWorld;
        }
    }
}
