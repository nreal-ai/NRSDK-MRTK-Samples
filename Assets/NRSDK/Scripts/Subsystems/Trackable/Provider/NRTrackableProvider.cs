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
    using System.Collections.Generic;
    using UnityEngine;

    public class NRTrackableProvider : ITrackableDataProvider
    {
        private NativeTrackable m_NativeTrackable;

        public NRTrackableProvider()
        {
            if (NRSessionManager.Instance.NativeAPI == null)
            {
                NRSessionManager.Instance.NativeAPI = new NativeInterface();
            }

            m_NativeTrackable = NRSessionManager.Instance.NativeAPI.NativeTrackable;
        }

        public uint GetIdentify(ulong trackable_handle)
        {
            return m_NativeTrackable.GetIdentify(trackable_handle);
        }

        public TrackableType GetTrackableType(ulong trackable_handle)
        {
            return m_NativeTrackable.GetTrackableType(trackable_handle);
        }

        public TrackingState GetTrackingState(ulong trackable_handle)
        {
            return m_NativeTrackable.GetTrackingState(trackable_handle);
        }

        public bool UpdateTrackables(TrackableType trackable_type, List<ulong> trackables)
        {
            return m_NativeTrackable.UpdateTrackables(trackable_type, trackables);
        }
    }

    public class NRTrackablePlaneProvider : NRTrackableProvider, ITrackablePlaneDataProvider
    {
        private NativePlane m_NativePlane;
        public NRTrackablePlaneProvider()
        {
            if (NRSessionManager.Instance.NativeAPI == null)
            {
                NRSessionManager.Instance.NativeAPI = new NativeInterface();
            }

            m_NativePlane = NRSessionManager.Instance.NativeAPI.NativePlane;
        }

        public void GetBoundaryPolygon(ulong trackable_handle, List<Vector3> polygonList)
        {
            m_NativePlane.GetBoundaryPolygon(trackable_handle, polygonList);
        }

        public Pose GetCenterPose(ulong trackable_handle)
        {
            return m_NativePlane.GetCenterPose(trackable_handle);
        }

        public float GetExtentX(ulong trackable_handle)
        {
            return m_NativePlane.GetExtentX(trackable_handle);
        }

        public float GetExtentZ(ulong trackable_handle)
        {
            return m_NativePlane.GetExtentZ(trackable_handle);
        }

        public TrackablePlaneType GetPlaneType(ulong trackable_handle)
        {
            return m_NativePlane.GetPlaneType(trackable_handle);
        }
    }

    public class NRTrackableImageProvider : NRTrackableProvider, ITrackableImageDataProvider
    {
        private NativeTrackableImage m_NativeTrackableImage;
        public NRTrackableImageProvider()
        {
            if (NRSessionManager.Instance.NativeAPI == null)
            {
                NRSessionManager.Instance.NativeAPI = new NativeInterface();
            }

            m_NativeTrackableImage = NRSessionManager.Instance.NativeAPI.NativeTrackableImage;
        }

        public ulong CreateDataBase()
        {
            return m_NativeTrackableImage.CreateDataBase();
        }

        public bool LoadDataBase(ulong database_handle, string path)
        {
            return m_NativeTrackableImage.LoadDataBase(database_handle, path);
        }

        public bool DestroyDataBase(ulong database_handle)
        {
            return m_NativeTrackableImage.DestroyDataBase(database_handle);
        }

        public Pose GetCenterPose(ulong trackable_handle)
        {
            return m_NativeTrackableImage.GetCenterPose(trackable_handle);
        }

        public Vector2 GetSize(ulong trackable_handle)
        {
            return m_NativeTrackableImage.GetSize(trackable_handle);
        }
    }
}
