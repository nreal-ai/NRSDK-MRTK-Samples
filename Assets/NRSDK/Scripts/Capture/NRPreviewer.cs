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
    using UnityEngine.UI;

    /// <summary> Preview the camera's record or capture image. </summary>
    public class NRPreviewer : MonoBehaviour
    {
        /// <summary> The root. </summary>
        public GameObject Root;
        /// <summary> The preview screen. </summary>
        public RawImage PreviewScreen;
        /// <summary> The state icon. </summary>
        public Image StateIcon;

        /// <summary> True if is bind to controller, false if not. </summary>
        public bool isBindToController = true;

        /// <summary> Starts this object. </summary>
        private void Start()
        {
            Root.SetActive(false);
        }

        /// <summary> Sets a data. </summary>
        /// <param name="tex">       The tex.</param>
        /// <param name="isplaying"> True to isplaying.</param>
        public void SetData(Texture tex, bool isplaying)
        {
            PreviewScreen.texture = tex;
            StateIcon.color = isplaying ? Color.green : Color.red;
        }

        /// <summary> Updates this object. </summary>
        private void Update()
        {
            if (NRInput.GetButtonDown(ControllerButton.APP))
            {
                Root.SetActive(!Root.activeInHierarchy);

                NRInput.LaserVisualActive = !Root.activeInHierarchy;
                NRInput.ReticleVisualActive = !Root.activeInHierarchy;
            }
            if (isBindToController)
            {
                this.BindPreviewTOController();
            }
        }

        /// <summary> Bind preview to controller. </summary>
        private void BindPreviewTOController()
        {
            var inputAnchor = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.RightModelAnchor);
            transform.position = inputAnchor.TransformPoint(Vector3.forward * 0.3f);
            transform.forward = inputAnchor.forward;
        }

        /// <summary> Switch perview. </summary>
        /// <param name="flag"> True to flag.</param>
        public void SwitchPerview(bool flag)
        {
            Root.SetActive(flag);
        }
    }
}
