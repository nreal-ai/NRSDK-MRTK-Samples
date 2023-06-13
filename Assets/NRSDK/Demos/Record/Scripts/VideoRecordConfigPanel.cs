/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using NRKernal.Record;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.NRExamples
{
    public class VideoRecordConfigPanel : MonoBehaviour
    {
        [SerializeField]
        private VideoCapture2LocalExample m_VideoCapture2LocalExample;
        [SerializeField]
        private Dropdown m_QualityDropDown;
        [SerializeField]
        private Dropdown m_RenderModeDropDown;
        [SerializeField]
        private Dropdown m_AudioStateDropDown;
        [SerializeField]
        private Toggle m_UseGreenBGToggle;

        List<string> _ResolutionOptions = new List<string>() {
            VideoCapture2LocalExample.ResolutionLevel.High.ToString(),
            VideoCapture2LocalExample.ResolutionLevel.Middle.ToString(),
            VideoCapture2LocalExample.ResolutionLevel.Low.ToString()
        };

        List<string> _RendermodeOptions = new List<string>() {
            BlendMode.Blend.ToString(),
            BlendMode.RGBOnly.ToString(),
            BlendMode.VirtualOnly.ToString()
        };

        List<string> _AudioStateOptions = new List<string>() {
            NRVideoCapture.AudioState.MicAudio.ToString(),
            NRVideoCapture.AudioState.ApplicationAudio.ToString(),
            NRVideoCapture.AudioState.ApplicationAndMicAudio.ToString(),
            NRVideoCapture.AudioState.None.ToString()
        };

        void Start()
        {
            InitConfigPanel();
        }

        private void InitConfigPanel()
        {
            m_QualityDropDown.options.Clear();
            m_QualityDropDown.AddOptions(_ResolutionOptions);
            int default_quality_index = 0;
            for (int i = 0; i < _ResolutionOptions.Count; i++)
            {
                if (_ResolutionOptions[i].Equals(m_VideoCapture2LocalExample.resolutionLevel.ToString()))
                {
                    default_quality_index = i;
                }
            }
            m_QualityDropDown.value = default_quality_index;
            m_QualityDropDown.onValueChanged.AddListener((index) =>
            {
                Enum.TryParse<VideoCapture2LocalExample.ResolutionLevel>(_ResolutionOptions[index],
                    out m_VideoCapture2LocalExample.resolutionLevel);
            });

            m_RenderModeDropDown.options.Clear();
            m_RenderModeDropDown.AddOptions(_RendermodeOptions);
            int default_blendmode_index = 0;
            for (int i = 0; i < _RendermodeOptions.Count; i++)
            {
                if (_RendermodeOptions[i].Equals(m_VideoCapture2LocalExample.blendMode.ToString()))
                {
                    default_blendmode_index = i;
                }
            }
            m_RenderModeDropDown.value = default_blendmode_index;
            m_RenderModeDropDown.onValueChanged.AddListener((index) =>
            {
                Enum.TryParse<BlendMode>(_RendermodeOptions[index],
                    out m_VideoCapture2LocalExample.blendMode);
            });


            m_AudioStateDropDown.options.Clear();
            m_AudioStateDropDown.AddOptions(_AudioStateOptions);
            int default_audiostate_index = 0;
            for (int i = 0; i < _AudioStateOptions.Count; i++)
            {
                if (_AudioStateOptions[i].Equals(m_VideoCapture2LocalExample.audioState.ToString()))
                {
                    default_audiostate_index = i;
                }
            }
            m_AudioStateDropDown.value = default_audiostate_index;
            m_AudioStateDropDown.onValueChanged.AddListener((index) =>
            {
                Enum.TryParse<NRVideoCapture.AudioState>(_AudioStateOptions[index],
                    out m_VideoCapture2LocalExample.audioState);
            });

            m_UseGreenBGToggle.isOn = m_VideoCapture2LocalExample.useGreenBackGround;
            m_UseGreenBGToggle.onValueChanged.AddListener((val) =>
            {
                m_VideoCapture2LocalExample.useGreenBackGround = val;
            });
        }
    }
}
