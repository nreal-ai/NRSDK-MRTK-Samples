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
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary> A local map example. </summary>
    public class AnchorLoadTool : MonoBehaviour
    {
        /// <summary> The nr world anchor store. </summary>
        private NRWorldAnchorStore m_NRWorldAnchorStore;
        /// <summary> The anchor panel. </summary>
        public Transform m_AnchorsHolder;
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
            var anchorItems = FindObjectsOfType<AnchorItem>();
            foreach (var item in anchorItems)
            {
                item.OnAnchorItemClick += OnAnchorItemClick;
                m_AnchorPrefabDict.Add(item.key, item.gameObject);
            }
            m_AnchorsHolder.gameObject.SetActive(false);
            NRWorldAnchorStore.GetAsync(GetAnchorStoreCallBack);
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
            m_AnchorsHolder.gameObject.SetActive(!m_AnchorsHolder.gameObject.activeInHierarchy);
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

        /// <summary> Back, called when the get anchor store. </summary>
        /// <param name="store"> The store.</param>
        private void GetAnchorStoreCallBack(NRWorldAnchorStore store)
        {
            if (store == null)
            {
                NRDebugger.Warning("[AnchorLoadTool] Store is null");
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

        /// <summary> Clear all anchors. </summary>
        public void Clear()
        {
            if (m_NRWorldAnchorStore == null)
            {
                return;
            }
            m_NRWorldAnchorStore.Clear();
            m_LogStr.AppendLine("Clear all anchors");
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