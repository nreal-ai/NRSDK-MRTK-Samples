/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental
{
    using System;
    using System.Runtime.InteropServices;

    /**
    * @brief Native PointCloud API.
    */
    internal partial class NativePointCloud
    {
        private static UInt64 m_DatabaseHandle;

        public bool Create()
        {
            var result = NativeApi.NRPointCloudCreat(ref m_DatabaseHandle);
            NRDebugger.Debug("[NativePointCloud] End to create worldmap :" + result);
            NativeErrorListener.Check(result, this, "Create");
            return result == NativeResult.Success;
        }

        public bool IsUpdatedThisFrame()
        {
            bool result = NativeApi.NRPointCloudIsUpdated(m_DatabaseHandle);
            NRDebugger.Debug("[NativePointCloud] IsUpdatedThisFrame :" + result);
            return result;
        }

        public UInt64 CreatPointCloudList()
        {
            UInt64 pointlistHandle = 0;
            var result = NativeApi.NRPointCloudListCreat(m_DatabaseHandle, ref pointlistHandle);
            NativeErrorListener.Check(result, this, "CreatPointCloudList");
            NRDebugger.Debug("[NativePointCloud] create list :" + pointlistHandle);
            return pointlistHandle;
        }

        public void UpdatePointCloudList(UInt64 pointlistHandle)
        {
            NRDebugger.Debug("[NativePointCloud] UpdatePointCloudList");
            var result = NativeApi.NRPointCloudListUpdate(m_DatabaseHandle, pointlistHandle);
            NativeErrorListener.Check(result, this, "UpdatePointCloudList");
        }

        public void DestroyPointCloudList(UInt64 pointlistHandle)
        {
            var result = NativeApi.NRPointCloudListDestroy(m_DatabaseHandle, pointlistHandle);
            NRDebugger.Debug("[NativePointCloud] destroy list :" + pointlistHandle);
            NativeErrorListener.Check(result, this, "UpdatePointCloudList");
        }

        public int GetSize(UInt64 pointlistHandle)
        {
            int size = 0;
            var result = NativeApi.NRPointCloudListGetSize(m_DatabaseHandle, pointlistHandle, ref size);
            NRDebugger.Debug("[NativePointCloud] get size :" + size);
            NativeErrorListener.Check(result, this, "UpdatePointCloudList");
            return size;
        }

        public static int GetConfidence()
        {
            int confidence = -1;
            NativeApi.NRPointCloudConfidence(m_DatabaseHandle, ref confidence);
            return confidence;
        }

        public PointCloudPoint AquireItem(UInt64 pointlistHandle, int index)
        {
            PointCloudPoint point = new PointCloudPoint();
            NativeApi.NRPointCloudListAquireItem(m_DatabaseHandle, pointlistHandle, index, ref point);
            // Transform thr pos from oepngl to unity
            point.Position.Z = -point.Position.Z;
            return point;
        }

        public bool SaveMap(string path)
        {
            var result = NativeApi.NRPointCloudSave(m_DatabaseHandle, path);
            NRDebugger.Debug("[NativePointCloud] Save worldmap :" + result);
            NativeErrorListener.Check(result, this, "SaveMap");
            return result == NativeResult.Success;
        }

        public bool Destroy()
        {
            var result = NativeApi.NRPointCloudDestroy(m_DatabaseHandle);
            NRDebugger.Debug("[NativePointCloud] End to destroy worldmap :" + result);
            NativeErrorListener.Check(result, this, "Destroy");
            return result == NativeResult.Success;
        }

        private partial struct NativeApi
        {
            [DllImport(NativeConstants.NRNativeAnchorPointLib)]
            public static extern NativeResult NRPointCloudCreat(ref UInt64 world_map_database_handle);

            [DllImport(NativeConstants.NRNativeAnchorPointLib)]
            public static extern NativeResult NRPointCloudListCreat(UInt64 world_map_database_handle, ref UInt64 points_list_handle);

            [DllImport(NativeConstants.NRNativeAnchorPointLib)]
            public static extern NativeResult NRPointCloudListUpdate(UInt64 world_map_database_handle, UInt64 points_list_handle);

            [DllImport(NativeConstants.NRNativeAnchorPointLib)]
            public static extern NativeResult NRPointCloudListGetSize(UInt64 world_map_database_handle, UInt64 points_list_handle, ref int size);

            [DllImport(NativeConstants.NRNativeAnchorPointLib)]
            public static extern NativeResult NRPointCloudListDestroy(UInt64 world_map_database_handle, UInt64 points_list_handle);

            [DllImport(NativeConstants.NRNativeAnchorPointLib)]
            public static extern NativeResult NRPointCloudListAquireItem(UInt64 world_map_database_handle, UInt64 points_list_handle, int index, ref PointCloudPoint point);

            [DllImport(NativeConstants.NRNativeAnchorPointLib)]
            public static extern bool NRPointCloudIsUpdated(UInt64 world_map_database_handle);

            [DllImport(NativeConstants.NRNativeAnchorPointLib)]
            public static extern NativeResult NRPointCloudConfidence(UInt64 world_map_database_handle, ref int confidence);

            [DllImport(NativeConstants.NRNativeAnchorPointLib)]
            public static extern NativeResult NRPointCloudSave(UInt64 world_map_database_handle, string world_map_database_file_path);

            [DllImport(NativeConstants.NRNativeAnchorPointLib)]
            public static extern NativeResult NRPointCloudDestroy(UInt64 world_map_database_handle);
        }
    }
}
