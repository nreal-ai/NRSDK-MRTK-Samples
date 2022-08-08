/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    using NRKernal;
    using UnityEngine;
    using System.Collections;

    /// <summary> A nr device param initializer. </summary>
    [RequireComponent(typeof(Camera))]
    public class NRCameraInitializer : MonoBehaviour
    {
        /// <summary> Type of the device. </summary>
        [SerializeField] NativeDevice m_DeviceType = NativeDevice.RGB_CAMERA;

        private Camera m_TargetCamera;
        public bool IsInitialized { get; private set; } = false;

        void Start()
        {
            m_TargetCamera = gameObject.GetComponent<Camera>();
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
#if !UNITY_EDITOR
            bool result;
            EyeProjectMatrixData matrix_data = NRFrame.GetEyeProjectMatrix(out result, m_TargetCamera.nearClipPlane, m_TargetCamera.farClipPlane);
            while (!result)
            {
                NRDebugger.Info("[NRCameraInitializer] Waitting to initialize camera param.");
                yield return new WaitForEndOfFrame();
                matrix_data = NRFrame.GetEyeProjectMatrix(out result, m_TargetCamera.nearClipPlane, m_TargetCamera.farClipPlane);
            }

            if (m_DeviceType == NativeDevice.RGB_CAMERA && !NRKernal.NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_RGB_CAMERA))
            {
                NRDebugger.Warning("[NRCameraInitializer] Auto adaption for DevieType : {0} ==> {1}", m_DeviceType, NativeDevice.LEFT_DISPLAY);
                m_DeviceType = NativeDevice.LEFT_DISPLAY;
            }
            var eyeposFromHead = NRFrame.EyePoseFromHead;
            switch (m_DeviceType)
            {
                case NativeDevice.LEFT_DISPLAY:
                    m_TargetCamera.projectionMatrix = matrix_data.LEyeMatrix;
                    NRDebugger.Info("[NRCameraInitializer] Left Camera Project Matrix :" + m_TargetCamera.projectionMatrix.ToString());
                    transform.localPosition = eyeposFromHead.LEyePose.position;
                    transform.localRotation = eyeposFromHead.LEyePose.rotation;
                    NRDebugger.Info("[NRCameraInitializer] Left Camera pos:{0} rotation:{1}", transform.localPosition.ToString(), transform.localRotation.ToString());
                    break;
                case NativeDevice.RIGHT_DISPLAY:
                    m_TargetCamera.projectionMatrix = matrix_data.REyeMatrix;
                    NRDebugger.Info("[NRCameraInitializer] Right Camera Project Matrix :" + m_TargetCamera.projectionMatrix.ToString());
                    transform.localPosition = eyeposFromHead.REyePose.position;
                    transform.localRotation = eyeposFromHead.REyePose.rotation;
                    NRDebugger.Info("[NRCameraInitializer] Right Camera pos:{0} rotation:{1}", transform.localPosition.ToString(), transform.localRotation.ToString());
                    break;
                case NativeDevice.RGB_CAMERA:
                    m_TargetCamera.projectionMatrix = matrix_data.RGBEyeMatrix;
                    NRDebugger.Info("[NRCameraInitializer] RGB Camera Project Matrix :" + m_TargetCamera.projectionMatrix.ToString());
                    transform.localPosition = eyeposFromHead.RGBEyePose.position;
                    transform.localRotation = eyeposFromHead.RGBEyePose.rotation;
                    NRDebugger.Info("[NRCameraInitializer] RGB Camera pos:{0} rotation:{1}", transform.localPosition.ToString(), transform.localRotation.ToString());
                    break;
                default:
                    break;
            }
#else
            yield return new WaitForEndOfFrame();
#endif

            IsInitialized = true;
        }

        /// <summary> Switch to eye parameter. </summary>
        /// <param name="eye"> The eye.</param>
        public void SwitchToEyeParam(NativeDevice eye)
        {
            if (m_DeviceType == eye)
            {
                return;
            }

            m_DeviceType = eye;
#if !UNITY_EDITOR
            StartCoroutine(Initialize());
#endif
        }
    }
}
