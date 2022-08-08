/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.StreammingCast
{
    using NRKernal.Record;
    using UnityEngine;

    /// <summary> A nr observer view behaviour. </summary>
    public class NRObserverViewBehaviour : MonoBehaviour
    {
        /// <summary> The camera root. </summary>
        [SerializeField]
        private Transform m_CameraRoot;
        /// <summary> Information describing the debug. </summary>
        [SerializeField]
        private GameObject m_DebugInfo;
        /// <summary> The capture camera. </summary>
        [SerializeField]
        public Camera CaptureCamera;

        /// <summary> The defualt fcc. </summary>
        NativeMat3f defualtFCC = new NativeMat3f(
           new Vector3(1402.06530528f, 0, 0),
           new Vector3(0, 1401.16300406f, 0),
           new Vector3(939.51336953f, 545.53574753f, 1)
       );

        /// <summary> Awakes this object. </summary>
        void Awake()
        {
            m_DebugInfo.SetActive(false);
            //Use default fov.
            UpdateCameraParam(ProjectMatrixUtility.CalculateFOVByFCC(defualtFCC));
        }

        /// <summary> Switch debug panel. </summary>
        /// <param name="isopen"> True to isopen.</param>
        public void SwitchDebugPanel(bool isopen)
        {
            m_DebugInfo.SetActive(isopen);
        }

        /// <summary> Updates the pose described by pose. </summary>
        /// <param name="pose"> The pose.</param>
        public void UpdatePose(Pose pose)
        {
            m_CameraRoot.position = pose.position;
            m_CameraRoot.rotation = pose.rotation;
        }

        /// <summary> Sets culling mask. </summary>
        /// <param name="i"> Zero-based index of the.</param>
        public void SetCullingMask(int i)
        {
            this.CaptureCamera.cullingMask = i;
        }

        /// <summary> Updates the camera parameter described by fov. </summary>
        /// <param name="fov"> The fov.</param>
        public void UpdateCameraParam(Fov4f fov)
        {
            //Update fov.
            CaptureCamera.projectionMatrix = ProjectMatrixUtility.PerspectiveOffCenter(fov.left, fov.right, fov.bottom, fov.top,
                CaptureCamera.nearClipPlane, CaptureCamera.farClipPlane);
            NRDebugger.Info("[NRObserverViewBehaviour] UpdateCameraParam:" + CaptureCamera.projectionMatrix.ToString());
        }
    }
}
