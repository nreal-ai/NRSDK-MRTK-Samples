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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary> A nr world anchor store. </summary>
    public class NRWorldAnchorStore : IDisposable
    {
        /// <summary> Gets the number of anchors. </summary>
        /// <value> The number of anchors. </value>
        public int anchorCount
        {
            get
            {
                return m_AnchorDict.Count;
            }
        }

        /// <summary> Gets or sets the native mapping. </summary>
        /// <value> The m native mapping. </value>
        private NativeMapping m_NativeMapping { get; set; }
        /// <summary> Dictionary of anchors. </summary>
        private Dictionary<int, NRWorldAnchor> m_AnchorDict = new Dictionary<int, NRWorldAnchor>();
        /// <summary> Dictionary of key and anchor id. </summary>
        private Dictionary<string, int> m_Anchor2ObjectDict = new Dictionary<string, int>();

        /// <summary> The nr world anchor store. </summary>
        private static NRWorldAnchorStore m_NRWorldAnchorStore;
        /// <summary> Pathname of the map folder. </summary>
        public const string MapFolder = "NrealMaps";
        /// <summary> The map file. </summary>
        public const string MapFile = "nreal_map.dat";
        /// <summary> The anchor 2 object file. </summary>
        public const string Anchor2ObjectFile = "anchor2object.json";
        public const string AnchorsFile = "nreal_map_anchors.json";
        /// <summary> The root anchor. </summary>
        public const string AnchorRootKey = "root";
        /// <summary> Anchors update interval. </summary>
        private const float Update_Interval = 0.2f;
        private float currentUpdateTime = 0f;

        /// <summary> Gets the WorldAnchorStore instance. </summary>
        /// <param name="onCompleted"> .</param>
        public static void GetAsync(GetAsyncDelegate onCompleted)
        {
            NRKernalUpdater.Instance.StartCoroutine(GetWorldAnchorStore(onCompleted));
        }

        /// <summary> Gets world anchor store. </summary>
        /// <param name="onCompleted"> .</param>
        /// <returns> The world anchor store. </returns>
        private static IEnumerator GetWorldAnchorStore(GetAsyncDelegate onCompleted)
        {
            // Wait for slam ready.
            while (NRFrame.LostTrackingReason != LostTrackingReason.NONE ||
                NRFrame.SessionStatus != SessionState.Running ||
                !NRFrame.isHeadPoseReady)
            {
                NRDebugger.Info("[NRWorldAnchorStore] Wait for slam ready...");
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(0.5f);

            if (m_NRWorldAnchorStore == null)
            {
                m_NRWorldAnchorStore = new NRWorldAnchorStore();
            }

            NRDebugger.Info("[NRWorldAnchorStore] : GetWorldAnchorStore true");
            onCompleted?.Invoke(m_NRWorldAnchorStore);
        }

        /// <summary> Default constructor. </summary>
        internal NRWorldAnchorStore()
        {
#if !UNITY_EDITOR
            m_NativeMapping = new NativeMapping(NRSessionManager.Instance.NativeAPI);
#endif
            EnsurePath(Path.Combine(Application.persistentDataPath, MapFolder));
            LoadWorldMap(Path.Combine(Application.persistentDataPath, MapFolder, MapFile));
            string path = Path.Combine(Application.persistentDataPath, MapFolder, Anchor2ObjectFile);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                NRDebugger.Info("[NRWorldAnchorStore] Anchor2Object json:" + json);
                m_Anchor2ObjectDict = LitJson.JsonMapper.ToObject<Dictionary<string, int>>(json);
            }

            // Add a root anchor for default.
            if (!m_Anchor2ObjectDict.ContainsKey(AnchorRootKey))
            {
                m_Anchor2ObjectDict.Add(AnchorRootKey, 0);
            }

            NRKernalUpdater.OnUpdate -= OnUpdate;
            NRKernalUpdater.OnUpdate += OnUpdate;

            this.UpdateAnchors();
        }

        /// <summary> Executes the 'update' action. </summary>
        private void OnUpdate()
        {
            currentUpdateTime += Time.deltaTime;
            if (currentUpdateTime < Update_Interval)
            {
                return;
            }
            currentUpdateTime = 0f;

            UpdateAnchors();
            foreach (var item in m_AnchorDict)
            {
                item.Value.OnUpdateState();
            }
        }

        /// <summary> Loads world map. </summary>
        /// <param name="path"> Full pathname of the file.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool LoadWorldMap(string path)
        {
#if !UNITY_EDITOR
            Assert.IsTrue(File.Exists(path), "[NRWorldAnchorStore] World map File is not exit!!!");
            m_NativeMapping.CreateDataBase();
            return m_NativeMapping.LoadMap(path);
#else
            return true;
#endif
        }

        /// <summary> Writes the world map. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool WriteWorldMap()
        {
            string basepath = Path.Combine(Application.persistentDataPath, MapFolder);
            EnsurePath(basepath);
#if !UNITY_EDITOR
            return m_NativeMapping.SaveMap(Path.Combine(basepath, MapFile));
#else
            return true;
#endif
        }

        /// <summary> Clears all persisted NRWorldAnchors. </summary>
        public void Clear()
        {
            foreach (var item in m_AnchorDict)
            {
                if (item.Value != null)
                {
#if !UNITY_EDITOR
                    m_NativeMapping.DestroyAnchor(item.Value.AnchorNativeHandle);
#endif
                    GameObject.Destroy(item.Value.gameObject);
                }
            }
            m_AnchorDict.Clear();
            m_Anchor2ObjectDict.Clear();

            this.Save();
        }

        /// <summary> Reset </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Reset()
        {
#if !UNITY_EDITOR
            return m_NativeMapping.Reset();
#else
            return true;
#endif
        }

        /// <summary> Deletes a persisted NRWorldAnchor from the store. </summary>
        /// <param name="key"> .</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Delete(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                NRDebugger.Warning("[NRWorldAnchorStore] Can not delete a null key");
                return false;
            }
            NRWorldAnchor anchor;
            if (TryGetValue(key, out anchor))
            {
                Assert.IsTrue(anchor != null);
#if !UNITY_EDITOR
                m_NativeMapping.DestroyAnchor(anchor.AnchorNativeHandle);
#endif
                m_AnchorDict.Remove(anchor.GetNativeSpatialAnchorPtr());
                m_Anchor2ObjectDict.Remove(key);
                GameObject.Destroy(anchor.gameObject);
                this.Save();
            }

            return false;
        }

        /// <summary> Cleans up the WorldAnchorStore and releases memory. </summary>
        public void Dispose()
        {
            if (m_NativeMapping == null)
            {
                return;
            }
#if !UNITY_EDITOR
            m_NativeMapping.DestroyDataBase();
            m_NativeMapping = null;
#endif
            m_NRWorldAnchorStore = null;
            NRKernalUpdater.OnUpdate -= OnUpdate;
        }

        /// <summary> Gets all of the identifiers of the currently persisted NRWorldAnchors. </summary>
        /// <returns> An array of string. </returns>
        public string[] GetAllIds()
        {
            if (m_Anchor2ObjectDict == null || m_Anchor2ObjectDict.Count == 0)
            {
                return null;
            }

            string[] ids = new string[m_Anchor2ObjectDict.Count];

            int index = 0;
            foreach (var item in m_Anchor2ObjectDict)
            {
                ids[index++] = item.Key;
            }

            return ids;
        }

        /// <summary> Update all NRWorldAnchors. </summary>
        ///
        /// ### <param name="anchorlist"> .</param>
        private void UpdateAnchors()
        {
#if !UNITY_EDITOR
            var listhandle = m_NativeMapping.CreateAnchorList();
            m_NativeMapping.UpdateAnchor(listhandle);
            var size = m_NativeMapping.GetAnchorListSize(listhandle);
            for (int i = 0; i < size; i++)
            {
                var anchorhandle = m_NativeMapping.AcquireItem(listhandle, i);
                if (!m_AnchorDict.ContainsKey(NativeMapping.GetAnchorNativeID(anchorhandle)))
                {
                    CreateAnchor(anchorhandle);
                }
            }
            m_NativeMapping.DestroyAnchorList(listhandle);
#endif
        }

        /// <summary>
        /// Loads a WorldAnchor from disk for given identifier and attaches it to the GameObject. If the
        /// GameObject has a WorldAnchor, that WorldAnchor will be updated. If the anchor is not found, a
        /// new anchor will be add in the position of go. If AddAnchor failed, null will be returned. The
        /// GameObject and any existing NRWorldAnchor attached to it will not be modified. </summary>
        /// <param name="key"> .</param>
        /// <param name="go"> .</param>
        /// <returns> A NRWorldAnchor. </returns>
        public NRWorldAnchor Load(string key, GameObject go)
        {
            if (string.IsNullOrEmpty(key) || go == null)
            {
                return null;
            }

            NRWorldAnchor anchor;
            if (TryGetValue(key, out anchor))
            {
                NRDebugger.Info("[NRWorldAnchorStore] load the cached anchor:" + key);
                go.transform.parent = anchor.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                return anchor;
            }

            NRDebugger.Warning("[NRWorldAnchorStore] can not find the anchor:" + key);
            return null;
        }

        /// <summary> Attempts to get value a NRWorldAnchor from the given string. </summary>
        /// <param name="key">        .</param>
        /// <param name="out_anchor"> [out] The out anchor.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool TryGetValue(string key, out NRWorldAnchor out_anchor)
        {
            NRWorldAnchor anchor;
            int anchorID;
            if (m_Anchor2ObjectDict.TryGetValue(key, out anchorID))
            {
                if (m_AnchorDict.TryGetValue(anchorID, out anchor))
                {
                    out_anchor = anchor;
                    return out_anchor != null;
                }
            }
            out_anchor = null;
            return false;
        }

        public NRWorldAnchor AddAnchor(string key, GameObject go)
        {
            if (go == null)
            {
                NRDebugger.Error("[NRWorldAnchorStore] Can not add a null gameobject as a anchor.");
                return null;
            }
            NRWorldAnchor anchor = this.AddAnchor(key, new Pose(go.transform.position, go.transform.rotation));
            go.transform.SetParent(anchor.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            return anchor;
        }

        /// <summary> Adds an anchor to 'worldPose'. </summary>
        /// <param name="key">       .</param>
        /// <param name="worldPose"> The world pose.</param>
        /// <returns> A NRWorldAnchor. </returns>
        public NRWorldAnchor AddAnchor(string key, Pose worldPose)
        {
            NRDebugger.Info("[NRWorldAnchorStore] Add a new worldanchor");
            if (string.IsNullOrEmpty(key))
            {
                NRDebugger.Error("[NRWorldAnchorStore] Can not add a null string as the key.");
                return null;
            }

            if (m_Anchor2ObjectDict.ContainsKey(key))
            {
                this.Delete(key);
            }

#if !UNITY_EDITOR
            var handle = m_NativeMapping.AddAnchor(worldPose);
#else
            var handle = (UInt64)UnityEngine.Random.Range(1, 100000);
#endif
            if (handle == 0)
            {
                NRDebugger.Error("[NRWorldAnchorStore] Add anchor failed for illegal anchor handle:" + handle);
                return null;
            }

            NRWorldAnchor anchor = CreateAnchor(handle, key);
            this.Save();

            return anchor;
        }

        /// <summary> Creates an anchor. </summary>
        /// <param name="handler">   The handler.</param>
        /// <param name="anchorkey"> (Optional) The anchorkey.</param>
        /// <returns> The new anchor. </returns>
        private NRWorldAnchor CreateAnchor(UInt64 handler, string anchorkey = null)
        {
            NRDebugger.Info("[NRWorldAnchorStore] Create a new worldanchor handle:" + handler);
            var anchor = new GameObject("NRWorldAnchor").AddComponent<NRWorldAnchor>();
            // Make sure anchor would not be destroied when change the scene.
            GameObject.DontDestroyOnLoad(anchor);
            anchor.SetNativeSpatialAnchorPtr(handler, m_NativeMapping);
            int anchorID = anchor.GetNativeSpatialAnchorPtr();
            m_AnchorDict[anchorID] = anchor;
            if (!string.IsNullOrEmpty(anchorkey))
            {
                m_Anchor2ObjectDict[anchorkey] = anchorID;
            }

            return anchor;
        }

        /// <summary>
        /// Saves the provided NRWorldAnchor with the provided identifier. If the identifier is already
        /// in use, the method will return false. </summary>
        /// <param name="key">     .</param>
        /// <param name="anchor"> .</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        private bool Save(string key, NRWorldAnchor anchor)
        {
            NRDebugger.Info("[NRWorldAnchorStore] Save a new worldanchor:" + key);
            if (m_Anchor2ObjectDict.ContainsKey(key))
            {
                NRDebugger.Warning("[NRWorldAnchorStore] Save a new anchor faild for repeated key:" + key);
                return false;
            }

            try
            {
                m_Anchor2ObjectDict.Add(key, anchor.GetNativeSpatialAnchorPtr());
                string json = LitJson.JsonMapper.ToJson(m_Anchor2ObjectDict);
                string path = Path.Combine(Application.persistentDataPath, MapFolder, Anchor2ObjectFile);
                NRDebugger.Info("[NRWorldAnchorStore] Save to the path:" + path + " json:" + json);
                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception e)
            {
                NRDebugger.Warning("Write anchor to object dict exception:" + e.ToString());
                return false;
            }
        }

        /// <summary> Saves all NRWorldAnchor. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Save()
        {
            if (m_Anchor2ObjectDict == null)
            {
                return false;
            }

            NRDebugger.Info("[NRWorldAnchorStore] Save all worldanchor");
            try
            {
                string path = Path.Combine(Application.persistentDataPath, MapFolder, Anchor2ObjectFile);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                if (m_Anchor2ObjectDict.Count != 0)
                {
                    string json = LitJson.JsonMapper.ToJson(m_Anchor2ObjectDict);
                    NRDebugger.Info("[NRWorldAnchorStore] Save to the path:" + path + " json:" + json);
                    File.WriteAllText(path, json);
                }

                return this.WriteWorldMap();
            }
            catch (Exception e)
            {
                NRDebugger.Warning("Write anchor to object dict exception:" + e.ToString());
                return false;
            }
        }

        /// <summary> Ensures that path. </summary>
        /// <param name="path"> Full pathname of the file.</param>
        private void EnsurePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary> The handler for when getting the WorldAnchorStore from GetAsync. </summary>
        /// <param name="store"> .</param>
        public delegate void GetAsyncDelegate(NRWorldAnchorStore store);
    }
}
