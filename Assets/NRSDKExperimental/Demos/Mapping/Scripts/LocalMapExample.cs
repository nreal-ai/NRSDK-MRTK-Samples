/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using NRKernal.Experimental.Persistence;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace NRKernal.Experimental.NRExamples
{
    /// <summary> A local map example. </summary>
    public class LocalMapExample : MonoBehaviour
    {
        /// <summary> The nr world anchor store. </summary>
        private NRWorldAnchorStore m_NRWorldAnchorStore;
        /// <summary> The image tracking anchor tool. </summary>
        private ImageTrackingAnchorTool m_ImageTrackingAnchorTool;
        /// <summary> The second image tracking anchor tool. </summary>
        public ImageTrackingAnchorTool m_ImageTrackingAnchorTool2;
        /// <summary> The anchor panel. </summary>
        public Transform m_AnchorPanel;
        /// <summary> The debug text. </summary>
        public Text debugText;
        /// <summary> Target for the. </summary>
        private Transform target;

        /// <summary> Dictionary of anchor prefabs. </summary>
        private Dictionary<string, GameObject> m_AnchorPrefabDict = new Dictionary<string, GameObject>();
        /// <summary> Dictionary of loaded anchors. </summary>
        private Dictionary<string, GameObject> m_LoadedAnchorDict = new Dictionary<string, GameObject>();
        /// <summary> The log string. </summary>
        private StringBuilder m_LogStr = new StringBuilder();

        /// <summary> Starts this object. </summary>
        private void Start()
        {
            m_ImageTrackingAnchorTool = gameObject.GetComponent<ImageTrackingAnchorTool>();
            var anchorItems = FindObjectsOfType<AnchorItem>();
            foreach (var item in anchorItems)
            {
                item.OnAnchorItemClick += OnAnchorItemClick;
                m_AnchorPrefabDict.Add(item.key, item.gameObject);
            }
            m_AnchorPanel.gameObject.SetActive(false);

            m_ImageTrackingAnchorTool.OnAnchorLoaded += OnImageTrackingAnchorLoaded;
            m_ImageTrackingAnchorTool2.OnAnchorLoaded += OnImageTrackingAnchorLoaded;
        }

        /// <summary> Executes the 'image tracking anchor loaded' action. </summary>
        /// <param name="key">    The key.</param>
        /// <param name="anchor"> The anchor.</param>
        private void OnImageTrackingAnchorLoaded(string key, NRWorldAnchor anchor)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(go.GetComponent<BoxCollider>());
            go.transform.parent = anchor.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one * 0.3f;
            go.name = key;
        }

        /// <summary> Updates this object. </summary>
        private void Update()
        {
            if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                AddAnchor();
            }
            debugText.text = m_LogStr.ToString();
        }

        /// <summary> Open or close anchor panel. </summary>
        public void SwitchAnchorPanel()
        {
            m_AnchorPanel.gameObject.SetActive(!m_AnchorPanel.gameObject.activeInHierarchy);
        }

        /// <summary> Executes the 'anchor item click' action. </summary>
        /// <param name="key">        The key.</param>
        /// <param name="anchorItem"> The anchor item.</param>
        private void OnAnchorItemClick(string key, GameObject anchorItem)
        {
            if (target != null)
            {
                DestroyImmediate(target.gameObject);
            }

            target = Instantiate(anchorItem).transform;
            target.parent = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.RightModelAnchor);
            target.position = target.parent.transform.position + target.parent.forward;
            target.forward = target.parent.forward;
            Destroy(target.gameObject.GetComponent<BoxCollider>());

            this.SwitchAnchorPanel();
        }

        /// <summary> Create NRWorldAnchorStore object. </summary>
        public void Load()
        {
            NRWorldAnchorStore.GetAsync(GetAnchorStoreCallBack);
        }

        /// <summary> Back, called when the get anchor store. </summary>
        /// <param name="store"> The store.</param>
        private void GetAnchorStoreCallBack(NRWorldAnchorStore store)
        {
            if (store == null)
            {
                NRDebugger.Info("store is null");
                return;
            }
            m_NRWorldAnchorStore = store;
            m_LogStr.AppendLine("Load map result: true");
            var keys = m_NRWorldAnchorStore.GetAllIds();
            if (keys != null)
            {
                foreach (var key in m_NRWorldAnchorStore.GetAllIds())
                {
                    m_LogStr.AppendLine("Get a anchor from NRWorldAnchorStore  key: " + key);
                    GameObject prefab;
                    if (m_AnchorPrefabDict.TryGetValue(key, out prefab))
                    {
                        var go = Instantiate(prefab);
                        m_NRWorldAnchorStore.Load(key, go);
                        m_LoadedAnchorDict[key] = go;
                    }
                }
            }
        }

        /// <summary> Save anchors your add. </summary>
        public void Save()
        {
            if (m_NRWorldAnchorStore == null)
            {
                return;
            }
            bool result = m_NRWorldAnchorStore.Save();
            m_LogStr.AppendLine("Save map result:" + result);
        }

        /// <summary> Clear all anchors. </summary>
        public void Clear()
        {
            if (m_NRWorldAnchorStore == null)
            {
                return;
            }
            m_NRWorldAnchorStore.Clear();
            m_LogStr.AppendLine("Clear map anchor");
        }

        public void Reset()
        {
            m_NRWorldAnchorStore?.Reset();
            m_LogStr.AppendLine("Reset map");
        }

        /// <summary> Add a new anchor. </summary>
        public void AddAnchor()
        {
            if (m_NRWorldAnchorStore == null || target == null)
            {
                return;
            }

            var anchorItem = target.GetComponent<AnchorItem>();
            if (anchorItem == null)
            {
                return;
            }
            var go = Instantiate(target.gameObject);
            go.transform.position = target.position;
            go.transform.rotation = target.rotation;
            go.SetActive(true);

            string key = go.GetComponent<AnchorItem>().key;

            m_NRWorldAnchorStore.Delete(key);
            bool result = m_NRWorldAnchorStore.AddAnchor(key, go);
            if (!result)
            {
                DestroyImmediate(go);
            }
            else
            {
                GameObject lastgo;
                m_LoadedAnchorDict.TryGetValue(key, out lastgo);
                if (lastgo != null)
                {
                    DestroyImmediate(lastgo);
                }
                m_LoadedAnchorDict[key] = go;
            }

            DestroyImmediate(target.gameObject);
            m_LogStr.AppendLine("Add anchor " + result);
        }
    }
}