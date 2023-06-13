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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using static NRKernal.NativeMeshing;

    public interface IMeshInfoProcessor
    {
        void UpdateMeshInfo(ulong identifier, NRMeshingBlockState meshingBlockState, Mesh mesh);
        void ClearMeshInfo();
    }

    public class NRMeshingManager : SingletonBehaviour<NRMeshingManager>
    {
        /// The size of the bounding box
        [SerializeField]
        private Vector3 m_BoundingBoxSize;
        [SerializeField]
        private float m_MeshRequestInterval;
        IMeshInfoProcessor[] m_MeshInfoProcessors;

        private NativeMeshing m_NativeMeshing;
        Coroutine m_MeshingCoroutine;
        float m_MeshUpdateTime = 0;
        Func<KeyValuePair<ulong, BlockInfo>, bool> m_Predicate = new Func<KeyValuePair<ulong, BlockInfo>, bool>(
            p => p.Value.blockState != NRMeshingBlockState.NR_MESHING_BLOCK_STATE_UNCHANGED);

        /// <summary> Starts this object. </summary>
        private void Start()
        {
            if (isDirty)
                return;
            NRDebugger.Info("[NRMeshingManager] Start");
            m_NativeMeshing = new NativeMeshing();
            m_NativeMeshing.Create();
            m_NativeMeshing.Start();
            m_NativeMeshing.SetMeshingFlags(NRMeshingFlags.NR_MESHING_FLAGS_COMPUTE_NORMAL);
            m_MeshInfoProcessors = GetComponents<IMeshInfoProcessor>();
        }

        void Update()
        {
            if (m_MeshingCoroutine == null)
            {
                m_MeshUpdateTime += Time.deltaTime;
                if (m_MeshUpdateTime >= m_MeshRequestInterval)
                {
                    RequestMeshing();
                    m_MeshUpdateTime = 0;
                }
            }
        }

        void RequestMeshing()
        {
            if (m_MeshingCoroutine != null)
                StopCoroutine(m_MeshingCoroutine);
            m_MeshingCoroutine = StartCoroutine(RequestMeshingCoroutine());
        }

        IEnumerator RequestMeshingCoroutine()
        {
            yield return RequestMeshInfoCoroutine();
            yield return RequestMeshDetailCoroutine();
            m_MeshingCoroutine = null;
        }

        IEnumerator RequestMeshInfoCoroutine()
        {
            NRDebugger.Info("[NRMeshingManager] Start RequestMeshInfoCoroutine");
            if (m_NativeMeshing.RequestMeshInfo(m_BoundingBoxSize, NRFrame.HeadPose))
            {
                while (!m_NativeMeshing.GetMeshInfoResult())
                {
                    NRDebugger.Debug("[NRMeshingManager] Wait GetMeshInfoResult");
                    yield return null;
                }
                var timestamp = m_NativeMeshing.GetMeshInfoTimestamp();
                NRDebugger.Info("[NRMeshingManager] GetMeshInfoTimestamp: {0}", timestamp);
                m_NativeMeshing.GetBlockInfoData();
                m_NativeMeshing.DestroyMeshInfo();
                m_NativeMeshing.DestroyMeshInfoRequest();
            }
        }

        IEnumerator RequestMeshDetailCoroutine()
        {
            NRDebugger.Info("[NRMeshingManager] Start RequestMeshDetailCoroutine");
            if (m_NativeMeshing.RequestMeshDetail(m_Predicate))
            {
                while (!m_NativeMeshing.GetMeshDetailResult())
                {
                    NRDebugger.Debug("[NRMeshingManager] Wait GetMeshDetailResult");
                    yield return null;
                }
                var timestamp = m_NativeMeshing.GetMeshDetailTimestamp();
                NRDebugger.Info("[NRMeshingManager] GetMeshDetailTimestamp: {0}", timestamp);
                yield return m_NativeMeshing.GetMeshDetailData(ProcessMeshDetail);
                m_NativeMeshing.DestroyMeshDetail();
                m_NativeMeshing.DestroyMeshDetailRequest();
            }
        }

        void ProcessMeshDetail(ulong identifier, NRMeshingBlockState meshingBlockState, Mesh mesh)
        {
            foreach (var processor in m_MeshInfoProcessors)
            {
                processor.UpdateMeshInfo(identifier, meshingBlockState, mesh);
            }
        }

        /// <summary> Executes the 'application pause' action. </summary>
        /// <param name="pause"> True to pause.</param>
        private void OnApplicationPause(bool pause)
        {
            NRDebugger.Info("[NRMeshingManager] OnApplicationPause: {0}", pause);
            if (m_NativeMeshing != null)
            {
                if (pause)
                {
                    m_NativeMeshing.Pause();
                }
                else
                {
                    m_NativeMeshing.Resume();
                }
            }
        }

        protected override void OnDestroy()
        {
            NRDebugger.Info("[NRMeshingManager] OnDestroy");
            base.OnDestroy();
            if (m_NativeMeshing != null)
            {
                m_NativeMeshing.Stop();
                m_NativeMeshing.Destroy();
            }
        }
    }
}
