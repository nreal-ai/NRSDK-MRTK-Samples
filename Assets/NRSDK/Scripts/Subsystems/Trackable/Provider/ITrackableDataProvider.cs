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
    using UnityEngine;

    public interface ITrackableDataProvider
    {
        /// <summary>
        /// Updates trackable info.
        /// </summary>
        /// <param name="trackable_type"></param>
        /// <param name="trackables"></param>
        /// <returns></returns>
        bool UpdateTrackables(TrackableType trackable_type, List<UInt64> trackables);

        /// <summary> Gets an identify. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The identify. </returns>
        UInt32 GetIdentify(UInt64 trackable_handle);

        /// <summary> Gets trackable type. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The trackable type. </returns>
        TrackableType GetTrackableType(UInt64 trackable_handle);

        /// <summary> Gets tracking state. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The tracking state. </returns>
        TrackingState GetTrackingState(UInt64 trackable_handle);
    }

    public interface ITrackableImageDataProvider
    {
        /// <summary> Creates data base. </summary>
        /// <returns> The new data base. </returns>
        UInt64 CreateDataBase();

        /// <summary> Destroys the data base described by database_handle. </summary>
        /// <param name="database_handle"> Handle of the database.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        bool DestroyDataBase(UInt64 database_handle);

        /// <summary> Loads data base. </summary>
        /// <param name="database_handle"> Handle of the database.</param>
        /// <param name="path">            Full pathname of the file.</param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        bool LoadDataBase(UInt64 database_handle, string path);

        /// <summary> Gets center pose. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The center pose. </returns>
        Pose GetCenterPose(UInt64 trackable_handle);

        /// <summary> Gets a size. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The size. </returns>
        Vector2 GetSize(UInt64 trackable_handle);
    }

    public interface ITrackablePlaneDataProvider
    {
        /// <summary> Gets plane type. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The plane type. </returns>
        TrackablePlaneType GetPlaneType(UInt64 trackable_handle);

        /// <summary> Gets center pose. </summary>
        /// <param name="trackable_handle"> Handle of the trackble.</param>
        /// <returns> The center pose. </returns>
        Pose GetCenterPose(UInt64 trackable_handle);

        /// <summary> Gets extent x coordinate. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The extent x coordinate. </returns>
        float GetExtentX(UInt64 trackable_handle);

        /// <summary> Gets extent z coordinate. </summary>
        /// <param name="trackable_handle"> Handle of the trackable.</param>
        /// <returns> The extent z coordinate. </returns>
        float GetExtentZ(UInt64 trackable_handle);

        /// <summary>
        /// Gets points of boundary polygon.
        /// </summary>
        /// <param name="trackable_handle"></param>
        /// <param name="polygonList"></param>
        void GetBoundaryPolygon(UInt64 trackable_handle, List<Vector3> polygonList);
    }
}
