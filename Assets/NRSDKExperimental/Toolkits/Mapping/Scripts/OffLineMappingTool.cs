/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.Persistence
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.IO;

    public class OffLineMappingTool : MonoBehaviour
    {
        public Text m_ConfidenceTips;
        public Slider m_PointSizeSlider;
        public Button m_StartMapBtn;
        public Button m_SaveMapBtn;
        public Slider m_ConfidenceSlider;
        public GameObject m_AnchorLoaders;
        private NRPointCloudCreator creator;
        private NRPointCloudVisualizer visualizer;
        public GameObject m_MapCreator;
        public GameObject m_AnchorsCreator;
        public GameObject m_Menu;

        void Start()
        {
            m_PointSizeSlider.onValueChanged.AddListener(OnScaleChange);
            visualizer = new NRPointCloudVisualizer();
            m_AnchorLoaders.SetActive(false);

            m_StartMapBtn.onClick.AddListener(Open);
            m_SaveMapBtn.onClick.AddListener(Save);

            m_StartMapBtn.gameObject.SetActive(true);
            m_SaveMapBtn.gameObject.SetActive(false);

            m_MapCreator.SetActive(false);
            m_AnchorsCreator.SetActive(false);
        }

        public void SwitchToMapMode()
        {
            m_MapCreator.SetActive(true);
            m_AnchorsCreator.SetActive(false);
            m_Menu.SetActive(false);
        }

        public void SwitchToAnchorMode()
        {
            m_MapCreator.SetActive(false);
            m_AnchorsCreator.SetActive(true);
            m_Menu.SetActive(false);
            m_AnchorLoaders.SetActive(true);
        }

        void Update()
        {
            int confidence = NRPointCloudCreator.confidence;
            m_ConfidenceTips.text = confidence.ToString();
            m_ConfidenceSlider.value = confidence;
        }

        public void Open()
        {
            if (creator != null)
            {
                return;
            }
            creator = NRPointCloudCreator.Create(visualizer);

            m_StartMapBtn.gameObject.SetActive(false);
            m_SaveMapBtn.gameObject.SetActive(true);
        }

        public void Save()
        {
            if (creator == null)
            {
                return;
            }

            string MapSavedPath = Path.Combine(Application.persistentDataPath, NRWorldAnchorStore.MapFolder);
            ClearMapFiles(MapSavedPath);
            if (!Directory.Exists(MapSavedPath))
            {
                Directory.CreateDirectory(MapSavedPath);
            }

            // Delete old files
            string[] files = Directory.GetFiles(MapSavedPath);
            foreach (var file in files)
            {
                NRDebugger.Info("[PointCloudTool] Delete file:" + file);
                File.Delete(file);
            }

            string mapfile = Path.Combine(MapSavedPath, NRWorldAnchorStore.MapFile);
            creator.Save(mapfile);

            Invoke("AfterBuildMap", 2f);

            m_StartMapBtn.gameObject.SetActive(true);
            m_SaveMapBtn.gameObject.SetActive(false);
        }

        public void Load()
        {
            SwitchToAnchorMode();
        }

        private void ClearMapFiles(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                NRDebugger.Info("[PointCloudTool] Copy file:" + file);
                string destiny = Path.Combine(path, Path.GetFileName(file));
                File.Delete(destiny);
            }
        }

        private void AfterBuildMap()
        {
            NRDebugger.Info("[PointCloudTool] Destroy creator.");
            creator.Destroy();
        }

        public void OnScaleChange(float value)
        {
            visualizer?.AdjustPointSize(value * 20);
        }
    }
}