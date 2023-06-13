/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.NRExamples
{
    /// <summary> A mesh info processor to generate unity object. </summary>
    public class MeshObjGenerator : MonoBehaviour, IMeshInfoProcessor
    {
        /// <summary> Parent of every mesh object. </summary>
        [SerializeField]
        private Transform m_MeshObjRoot;
        /// <summary>
        /// GameObject for mesh generation, requires a MeshFilter component.
        /// MeshCollider and MeshRenderer components are optional.
        /// </summary>
        [SerializeField]
        private GameObject m_MeshObjPrefab;
        /// <summary> If set true, mesh objects are rendered using OverrideMaterial. </summary>
        [SerializeField]
        private bool m_UseOverrideMaterial;
        /// <summary> (Optional) Used as a override material. </summary>
        [SerializeField]
        private Material m_OverrideMaterial;

        private Dictionary<ulong, GameObject> m_MeshObjDict = new Dictionary<ulong, GameObject>();

        void Awake()
        {
            if (m_MeshObjPrefab == null)
            {
                NRDebugger.Warning("[MeshObjGenerator] MeshObjPrefab Null!");
            }
            else
            {
                Renderer renderer = m_MeshObjPrefab.GetComponent<Renderer>();
                if (renderer != null && m_UseOverrideMaterial)
                {
                    renderer.sharedMaterial = m_OverrideMaterial;
                }
            }
        }

        void IMeshInfoProcessor.UpdateMeshInfo(ulong identifier, NRMeshingBlockState meshingBlockState, Mesh mesh)
        {
            if (m_MeshObjPrefab != null)
            {
                NRDebugger.Debug("[MeshObjGenerator] UpdateMeshInfo identifier: {0} meshingBlockState: {1}", identifier, meshingBlockState);
                GameObject go = null;
                MeshFilter meshFilter = null;
                MeshCollider meshCollider = null;
                switch (meshingBlockState)
                {
                    case NRMeshingBlockState.NR_MESHING_BLOCK_STATE_NEW:
                    case NRMeshingBlockState.NR_MESHING_BLOCK_STATE_UPDATED:
                        if (m_MeshObjDict.ContainsKey(identifier))
                        {
                            go = m_MeshObjDict[identifier];
                            if (go == null)
                                m_MeshObjDict.Remove(identifier);
                        }
                        if (go == null)
                        {
                            go = Instantiate(m_MeshObjPrefab, m_MeshObjRoot);
                            go.name = identifier.ToString();
                        }
                        meshFilter = go.GetComponent<MeshFilter>();
                        meshFilter.sharedMesh = mesh;
                        meshCollider = go.GetComponent<MeshCollider>();
                        if (meshCollider != null)
                            meshCollider.sharedMesh = mesh;
                        if (!m_MeshObjDict.ContainsKey(identifier))
                            m_MeshObjDict.Add(identifier, go);
                        break;
                    case NRMeshingBlockState.NR_MESHING_BLOCK_STATE_DELETED:
                        if (m_MeshObjDict.ContainsKey(identifier))
                        {
                            go = m_MeshObjDict[identifier];
                            Destroy(go);
                            m_MeshObjDict.Remove(identifier);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        void IMeshInfoProcessor.ClearMeshInfo()
        {
            NRDebugger.Debug("[MeshObjGenerator] ClearMeshInfo.");
            foreach (var identifier in m_MeshObjDict.Keys)
            {
                Destroy(m_MeshObjDict[identifier]);
            }
            m_MeshObjDict.Clear();
        }
    }
}
