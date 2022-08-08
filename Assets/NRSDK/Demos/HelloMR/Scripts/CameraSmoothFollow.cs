/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using UnityEngine;

namespace NRKernal.NRExamples
{
    /// <summary> A camera smooth follow. </summary>
    public class CameraSmoothFollow : MonoBehaviour
    {
        /// <summary> The anchor. </summary>
        [Header("Window Settings")]
        [SerializeField, Tooltip("What part of the view port to anchor the window to.")]
        private TextAnchor Anchor = TextAnchor.LowerCenter;
        /// <summary> The follow speed. </summary>
        [SerializeField, Range(0.0f, 100.0f), Tooltip("How quickly to interpolate the window towards its target position and rotation.")]
        private float FollowSpeed = 5.0f;
        /// <summary> The default distance. </summary>
        private float defaultDistance;

        /// <summary> default rotation at start. </summary>
        private Vector2 defaultRotation = new Vector2(0f, 0f);
        /// <summary> The horizontal rotation. </summary>
        private Quaternion HorizontalRotation;
        /// <summary> The horizontal rotation inverse. </summary>
        private Quaternion HorizontalRotationInverse;
        /// <summary> The vertical rotation. </summary>
        private Quaternion VerticalRotation;
        /// <summary> The vertical rotation inverse. </summary>
        private Quaternion VerticalRotationInverse;

        /// <summary> The offset. </summary>
        [SerializeField, Tooltip("The offset from the view port center applied based on the window anchor selection.")]
        private Vector2 Offset = new Vector2(0.1f, 0.1f);

        private Transform m_CenterCamera;
        private Transform CenterCamera
        {
            get
            {
                if (m_CenterCamera == null)
                {
                    if (NRSessionManager.Instance.CenterCameraAnchor != null)
                    {
                        m_CenterCamera = NRSessionManager.Instance.CenterCameraAnchor;
                    }
                    else if (Camera.main != null)
                    {
                        m_CenterCamera = Camera.main.transform;
                    }
                }
                return m_CenterCamera;
            }
        }

        /// <summary> Starts this object. </summary>
        void Start()
        {
            HorizontalRotation = Quaternion.AngleAxis(defaultRotation.y, Vector3.right);
            HorizontalRotationInverse = Quaternion.Inverse(HorizontalRotation);
            VerticalRotation = Quaternion.AngleAxis(defaultRotation.x, Vector3.up);
            VerticalRotationInverse = Quaternion.Inverse(VerticalRotation);

            defaultDistance = Vector3.Distance(transform.position, CenterCamera.position);
        }

        /// <summary> Late update. </summary>
        private void LateUpdate()
        {
            if (CenterCamera == null)
            {
                return;
            }
            float t = Time.deltaTime * FollowSpeed;
            transform.position = Vector3.Lerp(transform.position, CalculatePosition(CenterCamera), t);
            transform.rotation = Quaternion.Slerp(transform.rotation, CalculateRotation(CenterCamera), t);
        }

        /// <summary> Calculates the position. </summary>
        /// <param name="cameraTransform"> The camera transform.</param>
        /// <returns> The calculated position. </returns>
        private Vector3 CalculatePosition(Transform cameraTransform)
        {
            Vector3 position = cameraTransform.position + (cameraTransform.forward * defaultDistance);
            Vector3 horizontalOffset = cameraTransform.right * Offset.x;
            Vector3 verticalOffset = cameraTransform.up * Offset.y;

            switch (Anchor)
            {
                case TextAnchor.UpperLeft: position += verticalOffset - horizontalOffset; break;
                case TextAnchor.UpperCenter: position += verticalOffset; break;
                case TextAnchor.UpperRight: position += verticalOffset + horizontalOffset; break;
                case TextAnchor.MiddleLeft: position -= horizontalOffset; break;
                case TextAnchor.MiddleRight: position += horizontalOffset; break;
                case TextAnchor.LowerLeft: position -= verticalOffset + horizontalOffset; break;
                case TextAnchor.LowerCenter: position -= verticalOffset; break;
                case TextAnchor.LowerRight: position -= verticalOffset - horizontalOffset; break;
            }

            return position;
        }

        /// <summary> Calculates the rotation. </summary>
        /// <param name="cameraTransform"> The camera transform.</param>
        /// <returns> The calculated rotation. </returns>
        private Quaternion CalculateRotation(Transform cameraTransform)
        {
            Quaternion rotation = cameraTransform.rotation;

            switch (Anchor)
            {
                case TextAnchor.UpperLeft: rotation *= HorizontalRotationInverse * VerticalRotationInverse; break;
                case TextAnchor.UpperCenter: rotation *= HorizontalRotationInverse; break;
                case TextAnchor.UpperRight: rotation *= HorizontalRotationInverse * VerticalRotation; break;
                case TextAnchor.MiddleLeft: rotation *= VerticalRotationInverse; break;
                case TextAnchor.MiddleRight: rotation *= VerticalRotation; break;
                case TextAnchor.LowerLeft: rotation *= HorizontalRotation * VerticalRotationInverse; break;
                case TextAnchor.LowerCenter: rotation *= HorizontalRotation; break;
                case TextAnchor.LowerRight: rotation *= HorizontalRotation * VerticalRotation; break;
            }

            return rotation;
        }
    }
}