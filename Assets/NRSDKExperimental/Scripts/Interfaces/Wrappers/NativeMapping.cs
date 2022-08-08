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
    using UnityEngine;

    /// <summary> Session Native API. </summary>
    public class NativeMapping
    {
        /// <summary> Handle of the database. </summary>
        private UInt64 m_DatabaseHandle;

        /// <summary> The native interface. </summary>
        private static NativeInterface m_NativeInterface;

        /// <summary> Constructor. </summary>
        /// <param name="nativeInterface"> The native interface.</param>
        public NativeMapping(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
        }

        /// <summary> Creates data base. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool CreateDataBase()
        {
            var result = NativeApi.NRWorldMapDatabaseCreate(m_NativeInterface.TrackingHandle, ref m_DatabaseHandle);
            NativeErrorListener.Check(result, this, "CreateDataBase");
            return result == NativeResult.Success;
        }

        /// <summary> Destroys the data base. </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool DestroyDataBase()
        {
            var result = NativeApi.NRWorldMapDatabaseDestroy(m_NativeInterface.TrackingHandle, m_DatabaseHandle);
            NativeErrorListener.Check(result, this, "DestroyDataBase");
            return result == NativeResult.Success;
        }

        /// <summary> Loads a map. </summary>
        /// <param name="path"> Full pathname of the file.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool LoadMap(string path)
        {
            var result = NativeApi.NRWorldMapDatabaseLoadFile(m_NativeInterface.TrackingHandle, m_DatabaseHandle, path);
            NativeErrorListener.Check(result, this, "LoadMap");
            return result == NativeResult.Success;
        }

        /// <summary> Saves a map. </summary>
        /// <param name="path"> Full pathname of the file.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool SaveMap(string path)
        {
            var result = NativeApi.NRWorldMapDatabaseSaveFile(m_NativeInterface.TrackingHandle, m_DatabaseHandle, path);
            NativeErrorListener.Check(result, this, "SaveMap");
            return result == NativeResult.Success;
        }

        /// <summary> Reset Map </summary>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Reset()
        {
            var result = NativeApi.NRMappingReset(m_NativeInterface.TrackingHandle);
            NativeErrorListener.Check(result, this, "Reset");
            return result == NativeResult.Success;
        }

        /// <summary> Adds an anchor. </summary>
        /// <param name="pose"> The pose.</param>
        /// <returns> An UInt64. </returns>
        public UInt64 AddAnchor(Pose pose)
        {
            UInt64 anchorHandle = 0;
            NativeMat4f nativePose;
            ConversionUtility.UnityPoseToApiPose(pose, out nativePose);
            var result = NativeApi.NRTrackingAcquireNewAnchor(m_NativeInterface.TrackingHandle, ref nativePose, ref anchorHandle);
            NativeErrorListener.Check(result, this, "AddAnchor");
            return anchorHandle;
        }

        /// <summary> Creates anchor list. </summary>
        /// <returns> The new anchor list. </returns>
        public UInt64 CreateAnchorList()
        {
            UInt64 anchorlisthandle = 0;
            var result = NativeApi.NRAnchorListCreate(m_NativeInterface.TrackingHandle, ref anchorlisthandle);
            NativeErrorListener.Check(result, this, "CreateAnchorList");
            return anchorlisthandle;
        }

        /// <summary> Updates the anchor described by anchorlisthandle. </summary>
        /// <param name="anchorlisthandle"> The anchorlisthandle.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool UpdateAnchor(UInt64 anchorlisthandle)
        {
            var result = NativeApi.NRTrackingUpdateAnchors(m_NativeInterface.TrackingHandle, anchorlisthandle);
            NativeErrorListener.Check(result, this, "UpdateAnchor");
            return result == NativeResult.Success;
        }

        /// <summary> Destroys the anchor list described by anchorlisthandle. </summary>
        /// <param name="anchorlisthandle"> The anchorlisthandle.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool DestroyAnchorList(UInt64 anchorlisthandle)
        {
            //NRDebugger.Info("Start to destroy anchor list...");
            var result = NativeApi.NRAnchorListDestroy(m_NativeInterface.TrackingHandle, anchorlisthandle);
            NativeErrorListener.Check(result, this, "DestroyAnchorList");
            return result == NativeResult.Success;
        }

        /// <summary> Gets anchor list size. </summary>
        /// <param name="anchor_list_handle"> Handle of the anchor list.</param>
        /// <returns> The anchor list size. </returns>
        public int GetAnchorListSize(UInt64 anchor_list_handle)
        {
            int size = 0;
            var result = NativeApi.NRAnchorListGetSize(m_NativeInterface.TrackingHandle, anchor_list_handle, ref size);
            NativeErrorListener.Check(result, this, "GetAnchorListSize");
            return size;
        }

        /// <summary> Acquires the item. </summary>
        /// <param name="anchor_list_handle"> Handle of the anchor list.</param>
        /// <param name="index">              Zero-based index of the.</param>
        /// <returns> An UInt64. </returns>
        public UInt64 AcquireItem(UInt64 anchor_list_handle, int index)
        {
            UInt64 anchorHandle = 0;
            var result = NativeApi.NRAnchorListAcquireItem(m_NativeInterface.TrackingHandle, anchor_list_handle, index, ref anchorHandle);
            NativeErrorListener.Check(result, this, "AcquireItem");
            return anchorHandle;
        }

        /// <summary> Gets tracking state. </summary>
        /// <param name="anchor_handle"> Handle of the anchor.</param>
        /// <returns> The tracking state. </returns>
        public TrackingState GetTrackingState(UInt64 anchor_handle)
        {
            TrackingState trackingState = TrackingState.Stopped;
            var result = NativeApi.NRAnchorGetTrackingState(m_NativeInterface.TrackingHandle, anchor_handle, ref trackingState);
            NativeErrorListener.Check(result, this, "GetTrackingState");
            return trackingState;
        }

        /// <summary> Gets anchor native identifier. </summary>
        /// <param name="anchor_handle"> Handle of the anchor.</param>
        /// <returns> The anchor native identifier. </returns>
        public static int GetAnchorNativeID(UInt64 anchor_handle)
        {
            int anchorID = -1;
            NativeApi.NRAnchorGetID(m_NativeInterface.TrackingHandle, anchor_handle, ref anchorID);
            return anchorID;
        }

        /// <summary> Gets anchor pose. </summary>
        /// <param name="anchor_handle"> Handle of the anchor.</param>
        /// <returns> The anchor pose. </returns>
        public Pose GetAnchorPose(UInt64 anchor_handle)
        {
            NativeMat4f nativePose = NativeMat4f.identity;
            NativeApi.NRAnchorGetPose(m_NativeInterface.TrackingHandle, anchor_handle, ref nativePose);

            Pose unitypose;
            ConversionUtility.ApiPoseToUnityPose(nativePose, out unitypose);
            return unitypose;
        }

        /// <summary> Destroys the anchor described by anchor_handle. </summary>
        /// <param name="anchor_handle"> Handle of the anchor.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool DestroyAnchor(UInt64 anchor_handle)
        {
            var result = NativeApi.NRAnchorDestroy(m_NativeInterface.TrackingHandle, anchor_handle);
            NativeErrorListener.Check(result, this, "DestroyAnchor");
            return result == NativeResult.Success;
        }

        /// <summary> A native api. </summary>
        private struct NativeApi
        {
            /// <summary> Nr world map database create. </summary>
            /// <param name="tracking_handle">               Handle of the tracking.</param>
            /// <param name="out_world_map_database_handle"> [in,out] Handle of the out world map database.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRWorldMapDatabaseCreate(UInt64 tracking_handle,
                ref UInt64 out_world_map_database_handle);

            /// <summary> Nr world map database destroy. </summary>
            /// <param name="tracking_handle">           Handle of the tracking.</param>
            /// <param name="world_map_database_handle"> Handle of the world map database.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRWorldMapDatabaseDestroy(UInt64 tracking_handle,
                UInt64 world_map_database_handle);

            /// <summary> Nr world map database load file. </summary>
            /// <param name="tracking_handle">              Handle of the tracking.</param>
            /// <param name="world_map_database_handle">    Handle of the world map database.</param>
            /// <param name="world_map_database_file_path"> Full pathname of the world map database file.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRWorldMapDatabaseLoadFile(UInt64 tracking_handle,
                UInt64 world_map_database_handle, string world_map_database_file_path);

            /// <summary> Nr world map database save file. </summary>
            /// <param name="tracking_handle">              Handle of the tracking.</param>
            /// <param name="world_map_database_handle">    Handle of the world map database.</param>
            /// <param name="world_map_database_file_path"> Full pathname of the world map database file.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRWorldMapDatabaseSaveFile(UInt64 tracking_handle,
                UInt64 world_map_database_handle, string world_map_database_file_path);

            /// <summary> Reset Map </summary>
            /// <param name="tracking_handle">  Handle of the tracking.</param>
            /// <returns>   A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRMappingReset(UInt64 tracking_handle);

            /// <summary> NRTracking. </summary>
            /// <param name="tracking_handle">   Handle of the tracking.</param>
            /// <param name="pose">              [in,out] The pose.</param>
            /// <param name="out_anchor_handle"> [in,out] Handle of the out anchor.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingAcquireNewAnchor(
                 UInt64 tracking_handle, ref NativeMat4f pose, ref UInt64 out_anchor_handle);

            /// <summary> Nr tracking update anchors. </summary>
            /// <param name="tracking_handle">        Handle of the tracking.</param>
            /// <param name="out_anchor_list_handle"> Handle of the out anchor list.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackingUpdateAnchors(
               UInt64 tracking_handle, UInt64 out_anchor_list_handle);

            /// <summary> NRAnchorList. </summary>
            /// <param name="tracking_handle">        Handle of the tracking.</param>
            /// <param name="out_anchor_list_handle"> [in,out] Handle of the out anchor list.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorListCreate(
                 UInt64 tracking_handle, ref UInt64 out_anchor_list_handle);

            /// <summary> Nr anchor list destroy. </summary>
            /// <param name="tracking_handle">    Handle of the tracking.</param>
            /// <param name="anchor_list_handle"> Handle of the anchor list.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorListDestroy(
                 UInt64 tracking_handle, UInt64 anchor_list_handle);

            /// <summary> Nr anchor list get size. </summary>
            /// <param name="tracking_handle">    Handle of the tracking.</param>
            /// <param name="anchor_list_handle"> Handle of the anchor list.</param>
            /// <param name="out_list_size">      [in,out] Size of the out list.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorListGetSize(UInt64 tracking_handle,
                UInt64 anchor_list_handle, ref int out_list_size);

            /// <summary> Nr anchor list acquire item. </summary>
            /// <param name="tracking_handle">    Handle of the tracking.</param>
            /// <param name="anchor_list_handle"> Handle of the anchor list.</param>
            /// <param name="index">              Zero-based index of the.</param>
            /// <param name="out_anchor">         [in,out] The out anchor.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorListAcquireItem(UInt64 tracking_handle,
                UInt64 anchor_list_handle, int index, ref UInt64 out_anchor);

            /// <summary> NRAnchor. </summary>
            /// <param name="tracking_handle">    Handle of the tracking.</param>
            /// <param name="anchor_handle">      Handle of the anchor.</param>
            /// <param name="out_tracking_state"> [in,out] State of the out tracking.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorGetTrackingState(UInt64 tracking_handle,
                UInt64 anchor_handle, ref TrackingState out_tracking_state);

            /// <summary> Nr anchor get identifier. </summary>
            /// <param name="tracking_handle"> Handle of the tracking.</param>
            /// <param name="anchor_handle">   Handle of the anchor.</param>
            /// <param name="out_anchor_id">   [in,out] Identifier for the out anchor.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorGetID(UInt64 tracking_handle,
                UInt64 anchor_handle, ref int out_anchor_id);

            /// <summary> Nr anchor get pose. </summary>
            /// <param name="tracking_handle"> Handle of the tracking.</param>
            /// <param name="anchor_handle">   Handle of the anchor.</param>
            /// <param name="out_pose">        [in,out] The out pose.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorGetPose(UInt64 tracking_handle,
                UInt64 anchor_handle, ref NativeMat4f out_pose);

            /// <summary> Nr anchor destroy. </summary>
            /// <param name="tracking_handle"> Handle of the tracking.</param>
            /// <param name="anchor_handle">   Handle of the anchor.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRAnchorDestroy(UInt64 tracking_handle,
                UInt64 anchor_handle);
        }
    }
}
