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
    using System.Text;
    using UnityEngine;

    /// <summary> 6-dof Trackable Image Tracking's Native API . </summary>
    public partial class NativeTrackableImage
    {
        /// <summary> The native interface. </summary>
        private NativeInterface m_NativeInterface;

        /// <summary> Constructor. </summary>
        /// <param name="nativeInterface"> The native interface.</param>
        public NativeTrackableImage(NativeInterface nativeInterface)
        {
            m_NativeInterface = nativeInterface;
        }

        /// <summary> Creates data base. </summary>
        /// <returns> The new data base. </returns>
        public UInt64 CreateDataBase()
        {
            UInt64 database_handle = 0;
            var result = NativeApi.NRTrackableImageDatabaseCreate(m_NativeInterface.TrackingHandle, ref database_handle);
            NativeErrorListener.Check(result, this, "CreateDataBase");
            return database_handle;
        }

        /// <summary> Destroys the data base described by database_handle. </summary>
        /// <param name="database_handle"> Handle of the database.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool DestroyDataBase(UInt64 database_handle)
        {
            var result = NativeApi.NRTrackableImageDatabaseDestroy(m_NativeInterface.TrackingHandle, database_handle);
            NativeErrorListener.Check(result, this, "DestroyDataBase");
            return result == NativeResult.Success;
        }

        /// <summary> Loads data base. </summary>
        /// <param name="database_handle"> Handle of the database.</param>
        /// <param name="path">            Full pathname of the file.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool LoadDataBase(UInt64 database_handle, string path)
        {
            var result = NativeApi.NRTrackableImageDatabaseLoadDirectory(m_NativeInterface.TrackingHandle, database_handle, path);
            return result == NativeResult.Success;
        }

        /// <summary> Gets center pose. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The center pose. </returns>
        public Pose GetCenterPose(UInt64 trackable_handle)
        {
            Pose pose = Pose.identity;
            NativeMat4f center_pose_native = NativeMat4f.identity;
            NativeApi.NRTrackableImageGetCenterPose(m_NativeInterface.TrackingHandle, trackable_handle, ref center_pose_native);

            ConversionUtility.ApiPoseToUnityPose(center_pose_native, out pose);
            return pose;
        }

        /// <summary> Gets a size. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The size. </returns>
        public Vector2 GetSize(UInt64 trackable_handle)
        {
            float extent_x, extent_z;
            extent_x = extent_z = 0;
            NativeApi.NRTrackableImageGetExtentX(m_NativeInterface.TrackingHandle, trackable_handle, ref extent_x);
            NativeApi.NRTrackableImageGetExtentZ(m_NativeInterface.TrackingHandle, trackable_handle, ref extent_z);
            return new Vector2(extent_x * 0.001f, extent_z * 0.001f);
        }

        private partial struct NativeApi
        {
            /// <summary> Nr trackable image database create. </summary>
            /// <param name="session_handle">                      Handle of the session.</param>
            /// <param name="out_trackable_image_database_handle"> [in,out] Handle of the out trackable
            ///                                                    image database.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableImageDatabaseCreate(UInt64 session_handle,
                ref UInt64 out_trackable_image_database_handle);

            /// <summary> Nr trackable image database destroy. </summary>
            /// <param name="session_handle">                  Handle of the session.</param>
            /// <param name="trackable_image_database_handle"> Handle of the trackable image database.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableImageDatabaseDestroy(UInt64 session_handle,
                UInt64 trackable_image_database_handle);

            /// <summary> Nr trackable image database load directory. </summary>
            /// <param name="session_handle">                     Handle of the session.</param>
            /// <param name="trackable_image_database_handle">    Handle of the trackable image database.</param>
            /// <param name="trackable_image_database_directory"> Pathname of the trackable image database
            ///                                                   directory.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary, CallingConvention = CallingConvention.Cdecl)]
            public static extern NativeResult NRTrackableImageDatabaseLoadDirectory(UInt64 session_handle,
                UInt64 trackable_image_database_handle, string trackable_image_database_directory);

            /// <summary> Nr trackable image get center pose. </summary>
            /// <param name="session_handle">   Handle of the session.</param>
            /// <param name="trackable_handle"> Handle of the trackable.</param>
            /// <param name="out_center_pose">  [in,out] The out center pose.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableImageGetCenterPose(UInt64 session_handle,
                UInt64 trackable_handle, ref NativeMat4f out_center_pose);

            /// <summary> Nr trackable image get extent x coordinate. </summary>
            /// <param name="session_handle">   Handle of the session.</param>
            /// <param name="trackable_handle"> Handle of the trackable.</param>
            /// <param name="out_extent_x">     [in,out] The out extent x coordinate.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableImageGetExtentX(UInt64 session_handle,
                UInt64 trackable_handle, ref float out_extent_x);

            /// <summary> Nr trackable image get extent z coordinate. </summary>
            /// <param name="session_handle">   Handle of the session.</param>
            /// <param name="trackable_handle"> Handle of the trackable.</param>
            /// <param name="out_extent_z">     [in,out] The out extent z coordinate.</param>
            /// <returns> A NativeResult. </returns>
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRTrackableImageGetExtentZ(UInt64 session_handle,
                UInt64 trackable_handle, ref float out_extent_z);
        };
    }
}
