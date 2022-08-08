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
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary> 6-dof Plane Tracking's Native API . </summary>
    public partial class NativePlane
    {
        /// <summary> The native interface. </summary>
        private NativeInterface m_NativeInterface;
        /// <summary> The maximum size of the polygon. </summary>
        private const int m_MaxPolygonSize = 1024;
        /// <summary> The points. </summary>
        private float[] m_Points;
        /// <summary> Handle of the temporary points. </summary>
        private GCHandle m_TmpPointsHandle;

        /// <summary> Constructor. </summary>
        /// <param name="nativeInterface"> The native interface.</param>
        public NativePlane(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;

            m_Points = new float[m_MaxPolygonSize * 2];
            m_TmpPointsHandle = GCHandle.Alloc(m_Points, GCHandleType.Pinned);
        }

        /// <summary> Finalizer. </summary>
        ~NativePlane()
        {
            m_TmpPointsHandle.Free();
        }

        /// <summary> Gets plane type. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The plane type. </returns>
        public TrackablePlaneType GetPlaneType(UInt64 trackable_handle)
        {
            TrackablePlaneType plane_type = TrackablePlaneType.INVALID;
            NativeApi.NRTrackablePlaneGetType(m_NativeInterface.TrackingHandle, trackable_handle, ref plane_type);
            return plane_type;
        }

        /// <summary> Gets center pose. </summary>
        /// <param name="trackable_handle"> Handle of the trackble.</param>
        /// <returns> The center pose. </returns>
        public Pose GetCenterPose(UInt64 trackable_handle)
        {
            Pose pose = Pose.identity;
            NativeMat4f center_native_pos = NativeMat4f.identity;
            NativeApi.NRTrackablePlaneGetCenterPose(m_NativeInterface.TrackingHandle, trackable_handle, ref center_native_pos);
            ConversionUtility.ApiPoseToUnityPose(center_native_pos, out pose);
            return pose;
        }

        /// <summary> Gets extent x coordinate. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The extent x coordinate. </returns>
        public float GetExtentX(UInt64 trackable_handle)
        {
            float extent_x = 0;
            NativeApi.NRTrackablePlaneGetExtentX(m_NativeInterface.TrackingHandle, trackable_handle, ref extent_x);
            return extent_x;
        }

        /// <summary> Gets extent z coordinate. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The extent z coordinate. </returns>
        public float GetExtentZ(UInt64 trackable_handle)
        {
            float extent_z = 0;
            NativeApi.NRTrackablePlaneGetExtentZ(m_NativeInterface.TrackingHandle, trackable_handle, ref extent_z);
            return extent_z;
        }

        public void GetBoundaryPolygon(UInt64 trackable_handle, List<Vector3> polygonList)
        {
            polygonList.Clear();
            int polygon_size = 0;
            NativeApi.NRTrackablePlaneGetPolygonSize(m_NativeInterface.TrackingHandle, trackable_handle, ref polygon_size);
            int size = polygon_size / 2;
            NativeApi.NRTrackablePlaneGetPolygon(m_NativeInterface.TrackingHandle, trackable_handle, (m_TmpPointsHandle.AddrOfPinnedObject()));
            float[] point_data = new float[size * 2];
            Array.Copy(m_Points, point_data, size * 2);
            Pose centerPos = GetCenterPose(trackable_handle);
            var unityWorldTPlane = Matrix4x4.TRS(centerPos.position, centerPos.rotation, Vector3.one);
            var plane_type = GetPlaneType(trackable_handle);
            for (int i = 2 * size - 2; i >= 0; i -= 2)
            {
                Vector3 localpos = new Vector3(point_data[i], 0, -point_data[i + 1]);
                polygonList.Add(unityWorldTPlane.MultiplyPoint3x4(localpos));
            }
            if (plane_type == TrackablePlaneType.VERTICAL)
            {
                polygonList.Reverse();
            }
        }

        private partial struct NativeApi
        {
            /// <summary> Nr trackable plane get type. </summary>
            /// <param name="session_handle">   Handle of the session.</param>
            /// <param name="trackable_handle"> Handle of the trackable.</param>
            /// <param name="out_plane_type">   [in,out] Type of the out plane.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackablePlaneGetType(UInt64 session_handle,
                UInt64 trackable_handle, ref TrackablePlaneType out_plane_type);

            /// <summary> Nr trackable plane get center pose. </summary>
            /// <param name="session_handle">   Handle of the session.</param>
            /// <param name="trackable_handle"> Handle of the trackable.</param>
            /// <param name="out_center_pose">  [in,out] The out center pose.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackablePlaneGetCenterPose(UInt64 session_handle,
                UInt64 trackable_handle, ref NativeMat4f out_center_pose);

            /// <summary> Nr trackable plane get extent x coordinate. </summary>
            /// <param name="session_handle">   Handle of the session.</param>
            /// <param name="trackable_handle"> Handle of the trackable.</param>
            /// <param name="out_extent_x">     [in,out] The out extent x coordinate.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackablePlaneGetExtentX(UInt64 session_handle,
                UInt64 trackable_handle, ref float out_extent_x);

            /// <summary> Nr trackable plane get extent z coordinate. </summary>
            /// <param name="session_handle">   Handle of the session.</param>
            /// <param name="trackable_handle"> Handle of the trackable.</param>
            /// <param name="out_extent_z">     [in,out] The out extent z coordinate.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackablePlaneGetExtentZ(UInt64 session_handle,
                UInt64 trackable_handle, ref float out_extent_z);

            /// <summary> Nr trackable plane get polygon size. </summary>
            /// <param name="session_handle">   Handle of the session.</param>
            /// <param name="trackable_handle"> Handle of the trackable.</param>
            /// <param name="out_polygon_size"> [in,out] Size of the out polygon.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackablePlaneGetPolygonSize(UInt64 session_handle,
                UInt64 trackable_handle, ref int out_polygon_size);

            /// <summary> Nr trackable plane get polygon. </summary>
            /// <param name="session_handle">   Handle of the session.</param>
            /// <param name="trackable_handle"> Handle of the trackable.</param>
            /// <param name="out_polygon">      The out polygon.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackablePlaneGetPolygon(UInt64 session_handle,
                UInt64 trackable_handle, IntPtr out_polygon);
        };
    }
}
