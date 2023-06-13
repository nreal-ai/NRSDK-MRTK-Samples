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
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using System.Collections;

    /// <summary> Meshing Native API. </summary>
    public class NativeMeshing
    {
        /// <summary> Handle of meshing. </summary>
        private UInt64 m_MeshingHandle = 0;
        /// <summary> Handle of mesh info request. </summary>
        private UInt64 m_RequestMeshInfoHandle = 0;
        /// <summary> Handle of mesh info. </summary>
        private UInt64 m_MeshInfoHandle = 0;
        /// <summary> Handle of mesh detail request. </summary>
        private UInt64 m_RequestMeshDetailHandle = 0;
        /// <summary> Handle of mesh detail. </summary>
        private UInt64 m_MeshDetailHandle = 0;

        /// <summary> Struct contains information of a block. </summary>
        public struct BlockInfo
        {
            public ulong timestamp;
            public NRMeshingBlockState blockState;
            public NRMeshingFlags meshingFlag;
        }

        /// <summary> Dictionary contains the result of GetBlockInfoData. </summary>
        private Dictionary<ulong, BlockInfo> m_BlockInfos = new Dictionary<ulong, BlockInfo>();

        /// <summary>
        /// Create the Meshing system object.
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Create()
        {
            var result = NativeApi.NRMeshingCreate(ref m_MeshingHandle);
            NRDebugger.Info("[NativeMeshing] NRMeshingCreate result: {0} MeshingHandle: {1}.", result, m_MeshingHandle);
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Start the Meshing system object.
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Start()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingStart Zero MeshingHandle.");
                return false;
            }
            var result = NativeApi.NRMeshingStart(m_MeshingHandle);
            NRDebugger.Info("[NativeMeshing] NRMeshingStart result: {0}.", result);
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Pause the Meshing system object.
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Pause()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingPause Zero MeshingHandle.");
                return false;
            }
            var result = NativeApi.NRMeshingPause(m_MeshingHandle);
            NRDebugger.Info("[NativeMeshing] NRMeshingPause result: {0}.", result);
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Resume the Meshing system object.
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Resume()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingResume Zero MeshingHandle.");
                return false;
            }
            var result = NativeApi.NRMeshingResume(m_MeshingHandle);
            NRDebugger.Info("[NativeMeshing] NRMeshingResume result: {0}.", result);
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Stop the Meshing system object.
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Stop()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingStop Zero MeshingHandle.");
                return false;
            }
            NativeResult result = NativeApi.NRMeshingStop(m_MeshingHandle);
            NRDebugger.Info("[NativeMeshing] NRMeshingStop result: {0}.", result);
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Release memory used by the Meshing system object.
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Destroy()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingDestroy Zero MeshingHandle.");
                return true;
            }
            NativeResult result = NativeApi.NRMeshingDestroy(m_MeshingHandle);
            NRDebugger.Info("[NativeMeshing] NRMeshingDestroy result: {0}.", result);
            m_MeshingHandle = 0;
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Set flags which mesh runtime will use.
        /// </summary>
        /// <param name="flags"> Request flag that are a combination of NRMeshingFlags. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SetMeshingFlags(NRMeshingFlags flags)
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingSetFlags Zero MeshingHandle.");
                return false;
            }
            var result = NativeApi.NRMeshingSetFlags(m_MeshingHandle, flags);
            NRDebugger.Debug("[NativeMeshing] NRMeshingSetFlags result: {0} flag: {1}.", result, flags);
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Request mesh info which includes state and bounding extents of the block.
        /// </summary>
        /// <param name="boundingBoxSize"> The size of interest region for meshing. </param>
        /// <param name="pose"> The pose of interest region for meshing. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool RequestMeshInfo(Vector3 boundingBoxSize, Pose pose)
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingRequestMeshInfo Zero MeshingHandle.");
                return false;
            }
            if (m_MeshInfoHandle != 0)
            {
                DestroyMeshInfo();
            }
            if (m_RequestMeshInfoHandle != 0)
            {
                DestroyMeshInfoRequest();
            }
            NRExtents extents = new NRExtents
            {
                transform = new NRTransform
                {
                    position = new NativeVector3f(pose.position),
                    rotation = new NativeVector4f(pose.rotation.x, pose.rotation.y, pose.rotation.z, pose.rotation.w)
                },
                extents = new NativeVector3f(boundingBoxSize)
            };
            var result = NativeApi.NRMeshingRequestMeshInfo(m_MeshingHandle, ref extents, ref m_RequestMeshInfoHandle);
            NRDebugger.Debug("[NativeMeshing] NRMeshingRequestMeshInfo result: {0} Handle: {1}.", result, m_RequestMeshInfoHandle);
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Get the Result of a earlier request.
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetMeshInfoResult()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingGetMeshInfoResult Zero MeshingHandle.");
                return false;
            }
            if (m_RequestMeshInfoHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingGetMeshInfoResult Zero RequestMeshInfoHandle.");
                return false;
            }
            if (m_MeshInfoHandle != 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingGetMeshInfoResult Nonzero MeshInfoHandle.");
                return true;
            }
            NativeResult result = NativeApi.NRMeshingGetMeshInfoResult(m_MeshingHandle, m_RequestMeshInfoHandle, ref m_MeshInfoHandle);
            NRDebugger.Debug("[NativeMeshing] NRMeshingGetMeshInfoResult result: {0} Handle: {1}.", result, m_MeshInfoHandle);
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Get response timestamp (in nano seconds) to a earlier request.
        /// </summary>
        /// <returns> The timestamp in nano seconds. </returns>
        public ulong GetMeshInfoTimestamp()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshInfoGetTimestamp Zero MeshingHandle.");
                return 0;
            }
            if (m_MeshInfoHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshInfoGetTimestamp Zero MeshInfoHandle.");
                return 0;
            }
            ulong timeStamp = 0;
            NativeResult result = NativeApi.NRMeshInfoGetTimestamp(m_MeshingHandle, m_MeshInfoHandle, ref timeStamp);
            NRDebugger.Debug("[NativeMeshing] NRMeshInfoGetTimestamp result: {0} timestamp: {1}.", result, timeStamp);
            return timeStamp;
        }

        /// <summary>
        /// Get block information of interest region.
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetBlockInfoData()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshInfoGetBlockInfoCount Zero MeshingHandle.");
                return false;
            }
            if (m_MeshInfoHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshInfoGetBlockInfoCount Zero MeshInfoHandle.");
                return false;
            }
            uint blockCount = 0;
            m_BlockInfos.Clear();
            NativeResult retResult = NativeApi.NRMeshInfoGetBlockInfoCount(m_MeshingHandle, m_MeshInfoHandle, ref blockCount);
            NRDebugger.Debug("[NativeMeshing] NRMeshInfoGetBlockInfoCount result: {0} blockCount: {1}.", retResult, blockCount);
            for (uint i = 0; i < blockCount; i++)
            {
                UInt64 blockInfoHandle = 0;
                NativeResult result = NativeApi.NRMeshInfoGetBlockInfoData(m_MeshingHandle, m_MeshInfoHandle, i, ref blockInfoHandle);
                NRDebugger.Debug("[NativeMeshing] NRMeshInfoGetBlockInfoData result: {0} Handle: {1}.", result, blockInfoHandle);
                BlockInfo blockInfo = new BlockInfo();
                ulong identifier = 0;
                result = NativeApi.NRBlockInfoGetBlockIdentifier(m_MeshingHandle, blockInfoHandle, ref identifier);
                NRDebugger.Debug("[NativeMeshing] NRBlockInfoGetBlockIdentifier result: {0} identifier: {1}.", result, identifier);
                result = NativeApi.NRBlockInfoGetTimestamp(m_MeshingHandle, blockInfoHandle, ref blockInfo.timestamp);
                NRDebugger.Debug("[NativeMeshing] NRBlockInfoGetTimestamp result: {0} timestamp: {1}.", result, blockInfo.timestamp);
                result = NativeApi.NRBlockInfoGetBlockState(m_MeshingHandle, blockInfoHandle, ref blockInfo.blockState);
                NRDebugger.Debug("[NativeMeshing] NRBlockInfoGetBlockState result: {0} blockState: {1}.", result, blockInfo.blockState);
                result = NativeApi.NRBlockInfoDestroy(m_MeshingHandle, blockInfoHandle);
                NRDebugger.Debug("[NativeMeshing] NRBlockInfoDestroy: {0}.", result);
                m_BlockInfos.Add(identifier, blockInfo);
            }
            return retResult == NativeResult.Success;
        }

        /// <summary>
        /// Destroy the mesh info handle.
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool DestroyMeshInfo()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshInfoDestroy Zero MeshingHandle.");
                return false;
            }
            if (m_MeshInfoHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshInfoDestroy Zero MeshInfoHandle.");
                return true;
            }
            NativeResult result = NativeApi.NRMeshInfoDestroy(m_MeshingHandle, m_MeshInfoHandle);
            NRDebugger.Debug("[NativeMeshing] NRMeshInfoDestroy result: {0}.", result);
            m_MeshInfoHandle = 0;
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Destroy the request handle.
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool DestroyMeshInfoRequest()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingMeshInfoRequestDestroy Zero MeshingHandle.");
                return false;
            }
            if (m_RequestMeshInfoHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingMeshInfoRequestDestroy Zero RequestMeshInfoHandle.");
                return true;
            }
            NativeResult result = NativeApi.NRMeshingMeshInfoRequestDestroy(m_MeshingHandle, m_RequestMeshInfoHandle);
            NRDebugger.Debug("[NativeMeshing] NRMeshingMeshInfoRequestDestroy result: {0}.", result);
            m_RequestMeshInfoHandle = 0;
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Request mesh detail for all blocks in request.
        /// </summary>
        /// <param name="predicate"> The search function of block infos. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool RequestMeshDetail(Func<KeyValuePair<ulong, BlockInfo>, bool> predicate = null)
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingRequestMeshDetail Zero MeshingHandle.");
                return false;
            }
            if (m_MeshDetailHandle != 0)
            {
                DestroyMeshDetail();
            }
            if (m_RequestMeshDetailHandle != 0)
            {
                DestroyMeshDetailRequest();
            }
            ulong[] blockIdentifiers = m_BlockInfos.Where(predicate ?? (p => true)).Select(p => p.Key).ToArray();
            if (blockIdentifiers.Length == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingRequestMeshDetail Zero blockIdentifier.");
                return false;
            }
            var result = NativeApi.NRMeshingRequestMeshDetail(m_MeshingHandle, (uint)blockIdentifiers.Length, blockIdentifiers, ref m_RequestMeshDetailHandle);
            NRDebugger.Debug("[NativeMeshing] NRMeshingRequestMeshDetail result: {0} Handle: {1}.", result, m_RequestMeshDetailHandle);
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Get the Result of a earlier request.
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool GetMeshDetailResult()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingGetMeshDetailResult Zero MeshingHandle.");
                return false;
            }
            if (m_RequestMeshDetailHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingGetMeshDetailResult Zero RequestMeshDetailHandle.");
                return false;
            }
            if (m_MeshDetailHandle != 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingGetMeshDetailResult Nonzero MeshDetailHandle.");
                return true;
            }
            NativeResult result = NativeApi.NRMeshingGetMeshDetailResult(m_MeshingHandle, m_RequestMeshDetailHandle, ref m_MeshDetailHandle);
            NRDebugger.Debug("[NativeMeshing] NRMeshingGetMeshDetailResult result: {0} Handle: {1}.", result, m_MeshDetailHandle);
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Get response timestamp (in nano seconds) to a earlier request.
        /// </summary>
        /// <returns> The timestamp in nano seconds. </returns>
        public ulong GetMeshDetailTimestamp()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshDetailGetTimestamp Zero MeshingHandle.");
                return 0;
            }
            if (m_MeshDetailHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshDetailGetTimestamp Zero MeshDetailHandle.");
                return 0;
            }
            ulong timeStamp = 0;
            NativeResult result = NativeApi.NRMeshDetailGetTimestamp(m_MeshingHandle, m_MeshDetailHandle, ref timeStamp);
            NRDebugger.Debug("[NativeMeshing] NRMeshDetailGetTimestamp: {0} {1}.", result, timeStamp);
            return timeStamp;
        }

        /// <summary>
        ///  Get block detail data in request
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public IEnumerator GetMeshDetailData(Action<ulong, NRMeshingBlockState, Mesh> action)
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshDetailGetBlockDetailCount Zero MeshingHandle.");
                yield break;
            }
            if (m_MeshDetailHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshDetailGetBlockDetailCount Zero MeshDetailHandle.");
                yield break;
            }
            ulong blockDetailCount = 0;
            NativeResult retResult = NativeApi.NRMeshDetailGetBlockDetailCount(m_MeshingHandle, m_MeshDetailHandle, ref blockDetailCount);
            NRDebugger.Debug("[NativeMeshing] NRMeshDetailGetBlockDetailCount result: {0} blockDetailCount: {1}.", retResult, blockDetailCount);
            for (uint i = 0; i < blockDetailCount; i++)
            {
                UInt64 blockDetailHandle = 0;
                NativeResult result = NativeApi.NRMeshDetailGetBlockDetailData(m_MeshingHandle, m_MeshDetailHandle, i, ref blockDetailHandle);
                NRDebugger.Debug("[NativeMeshing] NRMeshDetailGetBlockDetailData result: {0} Handle: {1}.", result, blockDetailHandle);
                ulong identifier = 0;
                result = NativeApi.NRBlockDetailGetBlockIdentifier(m_MeshingHandle, blockDetailHandle, ref identifier);
                NRDebugger.Debug("[NativeMeshing] NRBlockDetailGetBlockIdentifier result: {0} identifier: {1}.", result, identifier);
                NRMeshingFlags meshingFlag = NRMeshingFlags.NR_MESHING_FLAGS_NULL;
                result = NativeApi.NRBlockDetailGetFlags(m_MeshingHandle, blockDetailHandle, ref meshingFlag);
                NRDebugger.Debug("[NativeMeshing] NRBlockDetailGetFlags result: {0} meshingFlag: {1}.", result, meshingFlag);
                uint vertexCount = 0;
                result = NativeApi.NRBlockDetailGetVertexCount(m_MeshingHandle, blockDetailHandle, ref vertexCount);
                NRDebugger.Debug("[NativeMeshing] NRBlockDetailGetVertexCount result: {0} vertexCount: {1}.", result, vertexCount);
                if (vertexCount != 0)
                {
                    NativeVector3f[] outVertices = new NativeVector3f[vertexCount];
                    result = NativeApi.NRBlockDetailGetVertices(m_MeshingHandle, blockDetailHandle, outVertices);
                    NRDebugger.Debug("[NativeMeshing] NRBlockDetailGetVertices result: {0}.", result);
                    NativeVector3f[] outNormals = new NativeVector3f[vertexCount];
                    result = NativeApi.NRBlockDetailGetNormals(m_MeshingHandle, blockDetailHandle, outNormals);
                    NRDebugger.Debug("[NativeMeshing] NRBlockDetailGetNormals result: {0}.", result);
                    uint indexCount = 0;
                    result = NativeApi.NRBlockDetailGetIndexCount(m_MeshingHandle, blockDetailHandle, ref indexCount);
                    NRDebugger.Debug("[NativeMeshing] NRBlockDetailGetIndexCount result: {0} indexCount: {1}.", result, indexCount);
                    ushort[] outIndex = new ushort[indexCount];
                    result = NativeApi.NRBlockDetailGetIndeices(m_MeshingHandle, blockDetailHandle, outIndex);
                    NRDebugger.Debug("[NativeMeshing] NRBlockDetailGetIndeices result: {0}.", result);

                    Vector3[] vertices = new Vector3[vertexCount];
                    Vector3[] normals = new Vector3[vertexCount];
                    for (int j = 0; j < vertexCount; j++)
                    {
                        vertices[j] = outVertices[j].ToUnityVector3();
                        normals[j] = outNormals[j].ToUnityVector3();
                    }
                    int[] triangles = new int[indexCount];
                    for (int j = 0; j < indexCount; j++)
                    {
                        triangles[j] = outIndex[j];
                    }
                    Mesh mesh = new Mesh
                    {
                        vertices = vertices,
                        normals = normals,
                        triangles = triangles
                    };
                    mesh.RecalculateBounds();

                    action?.Invoke(identifier, m_BlockInfos[identifier].blockState, mesh);
                    NRDebugger.Debug("[NativeMeshing] GetMeshDetailData Invoke: {0} {1} {2}.", identifier, m_BlockInfos[identifier].blockState, mesh.vertexCount);
                }
                result = NativeApi.NRBlockDetailDestroy(m_MeshingHandle, blockDetailHandle);
                NRDebugger.Debug("[NativeMeshing] NRBlockDetailDestroy result: {0}.", result);
                yield return null;
            }
        }

        /// <summary>
        /// Destroy the mesh detail handle.
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool DestroyMeshDetail()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshDetailDestroy Zero MeshingHandle.");
                return false;
            }
            if (m_MeshDetailHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshDetailDestroy Zero MeshDetailHandle.");
                return true;
            }
            NativeResult result = NativeApi.NRMeshDetailDestroy(m_MeshingHandle, m_MeshDetailHandle);
            NRDebugger.Debug("[NativeMeshing] NRMeshDetailDestroy.");
            m_MeshDetailHandle = 0;
            return result == NativeResult.Success;
        }

        /// <summary>
        /// Destroy the request handle.
        /// </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool DestroyMeshDetailRequest()
        {
            if (m_MeshingHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingMeshDetailRequestDestroy Zero MeshingHandle.");
                return false;
            }
            if (m_RequestMeshDetailHandle == 0)
            {
                NRDebugger.Warning("[NativeMeshing] NRMeshingMeshDetailRequestDestroy Zero RequestMeshDetailHandle.");
                return true;
            }
            NativeResult result = NativeApi.NRMeshingMeshDetailRequestDestroy(m_MeshingHandle, m_RequestMeshDetailHandle);
            NRDebugger.Debug("[NativeMeshing] NRMeshingMeshDetailRequestDestroy.");
            m_RequestMeshDetailHandle = 0;
            return result == NativeResult.Success;
        }

        private partial struct NativeApi
        {
            #region LifeCycle

            /// <summary> Create the Meshing system object. </summary>
            /// <param name="out_meshing_handle"> The handle of Meshing. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshingCreate(ref UInt64 out_meshing_handle);

            /// <summary> Start the Meshing system object. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshingStart(UInt64 meshing_handle);

            /// <summary> Pause the Meshing system object. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshingPause(UInt64 meshing_handle);

            /// <summary> Resume the Meshing system object. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshingResume(UInt64 meshing_handle);

            /// <summary> Stop the Meshing system object. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshingStop(UInt64 meshing_handle);

            /// <summary> Release memory used by the Meshing system object. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshingDestroy(UInt64 meshing_handle);

            /// <summary> Set flags which mesh runtime will use. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="flags"> Request flags that are a combination of NRMeshingFlags. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshingSetFlags(UInt64 meshing_handle, NRMeshingFlags flags);

            #endregion

            #region MeshInfo

            /// <summary> Request mesh info which includes state and bounding extents of the block. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="extents"> The region of interest for meshing. </param>
            /// <param name="out_request_mesh_info_handle"> The handle of request for mesh info, which identifies the request. </param>
            /// <returns> The result of operation. If the meshing is computing, the result will be NR_RESULT_BUSY, until the NRMeshingGetMeshInfoResult return success. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshingRequestMeshInfo(UInt64 meshing_handle, ref NRExtents extents, ref UInt64 out_request_mesh_info_handle);

            /// <summary> Get the Result of a earlier request. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="request_mesh_info_handle"> The handle of request for mesh info, which identifies the request. </param>
            /// <param name="out_mesh_info_handle"> The handle of mesh info. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshingGetMeshInfoResult(UInt64 meshing_handle, UInt64 request_mesh_info_handle, ref UInt64 out_mesh_info_handle);

            /// <summary> Get response timestamp (in nano seconds) to a earlier request. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="mesh_info_handle"> The handle of mesh info. </param>
            /// <param name="out_hmd_time_nanos"> The timestamp in nano seconds. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshInfoGetTimestamp(UInt64 meshing_handle, UInt64 mesh_info_handle, ref ulong out_hmd_time_nanos);

            /// <summary> Get the number of elements in block info buffer. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="mesh_info_handle"> The handle of mesh info. </param>
            /// <param name="out_block_info_count"> The count of block info. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshInfoGetBlockInfoCount(UInt64 meshing_handle, UInt64 mesh_info_handle, ref uint out_block_info_count);

            /// <summary> Get block info data reference to specific index. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="mesh_info_handle"> The handle of mesh info. </param>
            /// <param name="index"> The index of block in mesh, which should be less then block_info_count. </param>
            /// <param name="out_block_info_handle"> The handle of block info. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshInfoGetBlockInfoData(UInt64 meshing_handle, UInt64 mesh_info_handle, uint index, ref UInt64 out_block_info_handle);

            /// <summary> Get block identifier. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="block_info_handle"> The handle of block info. </param>
            /// <param name="out_block_identifier"> The identifier of block. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBlockInfoGetBlockIdentifier(UInt64 meshing_handle, UInt64 block_info_handle, ref ulong out_block_identifier);

            /// <summary> Get timestamp (in nano seconds) when block was updated. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="block_info_handle"> The handle of block info. </param>
            /// <param name="out_hmd_time_nanos"> The timestamp in nano seconds. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBlockInfoGetTimestamp(UInt64 meshing_handle, UInt64 block_info_handle, ref ulong out_hmd_time_nanos);

            /// <summary> Get the state of block. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="block_info_handle"> The handle of block info. </param>
            /// <param name="out_block_state"> The state of block. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBlockInfoGetBlockState(UInt64 meshing_handle, UInt64 block_info_handle, ref NRMeshingBlockState out_block_state);

            /// <summary> Destroy the block info handle. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="block_info_handle"> The handle of block info. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBlockInfoDestroy(UInt64 meshing_handle, UInt64 block_info_handle);

            /// <summary> Destroy the mesh info handle. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="mesh_info_handle"> The handle of mesh info. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshInfoDestroy(UInt64 meshing_handle, UInt64 mesh_info_handle);

            /// <summary> Destroy the request handle. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="request_mesh_info_handle"> The handle of request for mesh info, which identifies the request. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshingMeshInfoRequestDestroy(UInt64 meshing_handle, UInt64 request_mesh_info_handle);

            #endregion

            #region MeshDetail

            /// <summary> Request mesh detail for all blocks in request. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="block_identifier_count"> The numbers of block identifiers. </param>
            /// <param name="block_identifiers"> All blocks identifies to request. </param>
            /// <param name="out_request_mesh_detail_handle"> The handle of request for mesh detail, which identifies the request. </param>
            /// <returns> The result of operation. If the meshing is computing, the result will be NR_RESULT_BUSY, until the NRMeshingGetMeshDetailResult return success. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshingRequestMeshDetail(UInt64 meshing_handle, uint block_identifier_count, ulong[] block_identifiers, ref UInt64 out_request_mesh_detail_handle);

            /// <summary> Get the Result of a earlier request. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="request_mesh_detail_handle"> The handle of request for mesh detail, which identifies the request. </param>
            /// <param name="out_mesh_detail_handle"> The handle of mesh detail. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshingGetMeshDetailResult(UInt64 meshing_handle, UInt64 request_mesh_detail_handle, ref UInt64 out_mesh_detail_handle);

            /// <summary> Get the timestamp (in nano seconds) when data was generated. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="mesh_detail_handle"> The handle of mesh detail. </param>
            /// <param name="out_hmd_time_nanos"> The timestamp in nano seconds. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshDetailGetTimestamp(UInt64 meshing_handle, UInt64 mesh_detail_handle, ref ulong out_hmd_time_nanos);

            /// <summary> Get the number of element in block detail buffer. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="mesh_detail_handle"> The handle of mesh detail. </param>
            /// <param name="out_block_detail_count"> The count of block detail. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshDetailGetBlockDetailCount(UInt64 meshing_handle, UInt64 mesh_detail_handle, ref ulong out_block_detail_count);

            /// <summary> Get block detail data reference to specific index. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="mesh_detail_handle"> The handle of mesh detail. </param>
            /// <param name="index"> The index of block in mesh, which should be less then block_detail_count. </param>
            /// <param name="out_block_detail_handle"> The handle of block detail. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshDetailGetBlockDetailData(UInt64 meshing_handle, UInt64 mesh_detail_handle, uint index, ref UInt64 out_block_detail_handle);

            /// <summary> Get block identifier. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="block_detail_handle"> The handle of block detail. </param>
            /// <param name="out_block_identifier"> The identifier of block. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBlockDetailGetBlockIdentifier(UInt64 meshing_handle, UInt64 block_detail_handle, ref ulong out_block_identifier);

            /// <summary> Get block flags which mesh block took place. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="block_detail_handle"> The handle of block detail. </param>
            /// <param name="out_flags"> Flags that are a combination of NRMeshingFlags. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBlockDetailGetFlags(UInt64 meshing_handle, UInt64 block_detail_handle, ref NRMeshingFlags out_flags);

            /// <summary> Get the number of vertices in vertex/normal buffer. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="block_detail_handle"> The handle of block detail. </param>
            /// <param name="out_vertex_count">  Number of elements in buffer. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBlockDetailGetVertexCount(UInt64 meshing_handle, UInt64 block_detail_handle, ref uint out_vertex_count);

            /// <summary> Get the pointer to vertex buffer. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="block_detail_handle"> The handle of block detail. </param>
            /// <param name="out_vertices"> Pointer to vertex buffer. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBlockDetailGetVertices(UInt64 meshing_handle, UInt64 block_detail_handle, NativeVector3f[] out_vertices);

            /// <summary> Get the pointer to normal buffer. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="block_detail_handle"> The handle of block detail. </param>
            /// <param name="out_normals"> Pointer to normal buffer. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBlockDetailGetNormals(UInt64 meshing_handle, UInt64 block_detail_handle, NativeVector3f[] out_normals);

            /// <summary> Get the number of elements in face-vertex-index buffer. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="block_detail_handle"> The handle of block detail. </param>
            /// <param name="out_face_vertex_index_count"> Number of elements in buffer. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBlockDetailGetIndexCount(UInt64 meshing_handle, UInt64 block_detail_handle, ref uint out_face_vertex_index_count);

            /// <summary>
            /// Get the pointer to face-vertex-index buffer.
            /// In the buffer, each element is a index to vertex buffer.
            /// Three index elements will define one triangle.
            /// For example: the first triangle is: vertex[index[0]], vertex[index[1]], vertex[index[2]].
            /// The second triangle is: vertex[index[3]], vertex[index[4]], vertex[index[5]].
            /// All faces are listed back-to-back in counter-clockwise vertex order.
            /// </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="block_detail_handle"> The handle of block detail. </param>
            /// <param name="out_face_vertices_index"> Pointer of face-vertex-index buffer. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBlockDetailGetIndeices(UInt64 meshing_handle, UInt64 block_detail_handle, ushort[] out_face_vertices_index);

            /// <summary> Destroy the block detail handle. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="block_detail_handle"> The handle of block detail. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRBlockDetailDestroy(UInt64 meshing_handle, UInt64 block_detail_handle);

            /// <summary> Destroy the mesh detail handle. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="mesh_detail_handle"> The handle of mesh detail. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshDetailDestroy(UInt64 meshing_handle, UInt64 mesh_detail_handle);

            /// <summary> Destroy the request handle. </summary>
            /// <param name="meshing_handle"> The handle of Meshing. </param>
            /// <param name="request_mesh_detail_handle"> The handle of request for mesh detail, which identifies the request. </param>
            /// <returns> The result of operation. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMeshingMeshDetailRequestDestroy(UInt64 meshing_handle, UInt64 request_mesh_detail_handle);

            #endregion
        };
    }
}
