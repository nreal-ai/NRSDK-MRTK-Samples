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
    using UnityEngine.EventSystems;

    
    /// <summary> An event camera raycaster. </summary>
    public abstract class EventCameraRaycaster : BaseRaycaster
    {
        /// <summary> True if is destroying, false if not. </summary>
        private bool isDestroying = false;
        /// <summary> The default camera. </summary>
        private Camera defaultCam;

        /// <summary> The near distance. </summary>
        [SerializeField]
        private float nearDistance = 0f;
        /// <summary> The far distance. </summary>
        [SerializeField]
        private float farDistance = 20f;

        /// <summary> Gets or sets the near distance. </summary>
        /// <value> The near distance. </value>
        public float NearDistance
        {
            get { return nearDistance; }
            set
            {
                nearDistance = Mathf.Max(0f, value);
                if (eventCamera != null)
                {
                    eventCamera.nearClipPlane = nearDistance;
                }
            }
        }

        /// <summary> Gets or sets the far distance. </summary>
        /// <value> The far distance. </value>
        public float FarDistance
        {
            get { return farDistance; }
            set
            {
                farDistance = Mathf.Max(0f, nearDistance, value);
                if (eventCamera != null)
                {
                    eventCamera.farClipPlane = farDistance;
                }
            }
        }

        /// <summary> <para>The camera that will generate rays for this raycaster.</para> </summary>
        /// <value> The event camera. </value>
        public override Camera eventCamera
        {
            get
            {
                if (isDestroying)
                {
                    return null;
                }

                if (defaultCam == null)
                {
                    var go = new GameObject(name + " FallbackCamera");
                    go.SetActive(false);
                    go.transform.SetParent(EventSystem.current.transform, false);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;

                    defaultCam = go.AddComponent<Camera>();
                    defaultCam.clearFlags = CameraClearFlags.Nothing;
                    defaultCam.cullingMask = 0;
                    defaultCam.orthographic = true;
                    defaultCam.orthographicSize = 1;
                    defaultCam.useOcclusionCulling = false;
#if !(UNITY_5_3 || UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
                    defaultCam.stereoTargetEye = StereoTargetEyeMask.None;
#endif
                    defaultCam.nearClipPlane = nearDistance;
                    defaultCam.farClipPlane = farDistance;
                }

                return defaultCam;
            }
        }

        /// <summary> <para>See MonoBehaviour.OnDestroy.</para> </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            isDestroying = true;

            if (defaultCam != null)
            {
                Destroy(defaultCam);
                defaultCam = null;
            }
        }
    }
    
}
