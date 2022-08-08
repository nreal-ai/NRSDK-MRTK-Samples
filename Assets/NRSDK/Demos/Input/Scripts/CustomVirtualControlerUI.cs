/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    /// <summary> A custom virtual controler user interface. </summary>
    [HelpURL("https://developer.nreal.ai/develop/unity/customize-phone-controller")]
    public class CustomVirtualControlerUI : MonoBehaviour
    {
        /// <summary> The show control. </summary>
        public Button showBtn;
        /// <summary> The hide control. </summary>
        public Button hideBtn;
        /// <summary> The base controller panel. </summary>
        public GameObject baseControllerPanel;
        /// <summary> The color btns. </summary>
        public Button[] colorBtns;
        /// <summary> The reset control. </summary>
        public Button resetBtn;
        /// <summary> The scale slider. </summary>
        public Slider scaleSlider;

        /// <summary> The model control. </summary>
        private TargetModelDisplayCtrl m_ModelCtrl;

        /// <summary> Starts this object. </summary>
        private void Start()
        {
            Init();
        }

        /// <summary> Initializes this object. </summary>
        private void Init()
        {
            m_ModelCtrl = FindObjectOfType<TargetModelDisplayCtrl>();
            for (int i = 0; i < colorBtns.Length; i++)
            {
                int k = i;
                colorBtns[i].onClick.AddListener(() => { OnColorButtonClick(k); });
            }
            showBtn.onClick.AddListener(() => { SetBaseControllerEnabled(true); });
            hideBtn.onClick.AddListener(() => { SetBaseControllerEnabled(false); });
            resetBtn.onClick.AddListener(OnResetButtonClick);
            scaleSlider.onValueChanged.AddListener(OnScaleSliderValueChanged);
        }

        /// <summary> Executes the 'color button click' action. </summary>
        /// <param name="index"> Zero-based index of the.</param>
        private void OnColorButtonClick(int index)
        {
            m_ModelCtrl.ChangeModelColor(colorBtns[index].image.color);
        }

        /// <summary> Executes the 'scale slider value changed' action. </summary>
        /// <param name="val"> The value.</param>
        private void OnScaleSliderValueChanged(float val)
        {
            m_ModelCtrl.ChangeModelScale(val);
        }

        /// <summary> Executes the 'reset button click' action. </summary>
        private void OnResetButtonClick()
        {
            m_ModelCtrl.ResetModel();
            scaleSlider.value = 0f;
        }

        /// <summary> Sets base controller enabled. </summary>
        /// <param name="isEnabled"> True if is enabled, false if not.</param>
        private void SetBaseControllerEnabled(bool isEnabled)
        {
            baseControllerPanel.SetActive(isEnabled);
        }
    }
}
