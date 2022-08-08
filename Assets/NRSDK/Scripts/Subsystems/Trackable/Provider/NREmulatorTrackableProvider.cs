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

#if UNITY_EDITOR
    public class NREmulatorTrackableProvider : ITrackableDataProvider
    {
        public class TrackableInfo
        {
            public Vector3 position;
            public Quaternion rotation;
            public float X;
            public float Z;
            public UInt32 identify;
            public ulong handler;
            public TrackingState trackingState;
            public TrackableType trackableType;
        }

        protected static Dictionary<UInt32, TrackableInfo> m_TrackablePlaneDict = new Dictionary<uint, TrackableInfo>();
        protected static Dictionary<UInt32, TrackableInfo> m_TrackableImageDict = new Dictionary<uint, TrackableInfo>();
        private static ulong trackableHandlerIndex = 1;

        public uint GetIdentify(ulong trackable_handle)
        {
            foreach (var item in m_TrackableImageDict)
            {
                if (item.Value.handler == trackable_handle)
                {
                    return item.Value.identify;
                }
            }

            foreach (var item in m_TrackablePlaneDict)
            {
                if (item.Value.handler == trackable_handle)
                {
                    return item.Value.identify;
                }
            }

            return 0;
        }

        public TrackableType GetTrackableType(ulong trackable_handle)
        {
            var info = GetTrackableInfo(trackable_handle);
            if (info != null)
            {
                return info.trackableType;
            }

            return TrackableType.TRACKABLE_BASE;
        }

        public TrackingState GetTrackingState(ulong trackable_handle)
        {
            var info = GetTrackableInfo(trackable_handle);
            if (info != null)
            {
                return info.trackingState;
            }

            return TrackingState.Stopped;
        }

        public TrackableInfo GetTrackableInfo(ulong trackable_handle)
        {
            foreach (var item in m_TrackableImageDict)
            {
                if (item.Value.handler == trackable_handle)
                {
                    return item.Value;
                }
            }

            foreach (var item in m_TrackablePlaneDict)
            {
                if (item.Value.handler == trackable_handle)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public bool UpdateTrackables(TrackableType trackable_type, List<ulong> trackables)
        {
            if (trackables == null)
            {
                trackables = new List<ulong>();
            }
            trackables.Clear();
            if (trackable_type == TrackableType.TRACKABLE_IMAGE)
            {
                foreach (var item in m_TrackableImageDict)
                {
                    trackables.Add(item.Value.handler);
                }
            }
            else if (trackable_type == TrackableType.TRACKABLE_PLANE)
            {
                foreach (var item in m_TrackablePlaneDict)
                {
                    trackables.Add(item.Value.handler);
                }
            }
            return true;
        }

        public static bool UpdateTrackableImageData(Vector3 centerPos, Quaternion centerQua, float extentX, float extentZ, UInt32 id, TrackingState state)
        {
            TrackableInfo info;
            if (m_TrackableImageDict.TryGetValue(id, out info))
            {
                info.position = centerPos;
                info.rotation = centerQua;
                info.X = extentX;
                info.Z = extentZ;
                info.identify = id;
                info.trackingState = state;
            }
            else
            {
                m_TrackableImageDict.Add(id, new TrackableInfo()
                {
                    position = centerPos,
                    rotation = centerQua,
                    X = extentX,
                    Z = extentZ,
                    identify = id,
                    handler = trackableHandlerIndex++,
                    trackableType = TrackableType.TRACKABLE_IMAGE,
                    trackingState = state
                });
            }
            return true;
        }

        public static bool UpdateTrackablePlaneData(Vector3 centerPos, Quaternion centerQua, float extentX, float extentZ, UInt32 identifier, TrackingState state)
        {
            TrackableInfo info;
            if (m_TrackablePlaneDict.TryGetValue(identifier, out info))
            {
                info.position = centerPos;
                info.rotation = centerQua;
                info.X = extentX;
                info.Z = extentZ;
                info.identify = identifier;
                info.trackingState = state;
            }
            else
            {
                m_TrackablePlaneDict.Add(identifier, new TrackableInfo()
                {
                    position = centerPos,
                    rotation = centerQua,
                    X = extentX,
                    Z = extentZ,
                    identify = identifier,
                    handler = trackableHandlerIndex++,
                    trackableType = TrackableType.TRACKABLE_PLANE,
                    trackingState = state
                });
            }
            return true;
        }

        public static bool UpdateTrackableData<T>(Vector3 centerPos, Quaternion centerQua, float extentX, float extentZ, System.UInt32 identifier, TrackingState state) where T : NRTrackable
        {
            if (typeof(T).Equals(typeof(NRTrackableImage)))
            {
                return UpdateTrackableImageData(centerPos, centerQua, extentX, extentZ, identifier, state);
            }
            else if (typeof(T).Equals(typeof(NRTrackablePlane)))
            {
                return UpdateTrackablePlaneData(centerPos, centerQua, extentX, extentZ, identifier, state);
            }
            return false;
        }
    }

    public class NREmulatorTrackPlaneProvider : NREmulatorTrackableProvider, ITrackablePlaneDataProvider
    {
        public void GetBoundaryPolygon(ulong trackable_handle, List<Vector3> polygonList)
        {
        }

        public Pose GetCenterPose(ulong trackable_handle)
        {
            var info = GetTrackableInfo(trackable_handle);

            if (info != null)
            {
                return new Pose(info.position, info.rotation);
            }

            return Pose.identity;
        }

        public float GetExtentX(ulong trackable_handle)
        {
            var info = GetTrackableInfo(trackable_handle);

            if (info != null)
            {
                return info.X;
            }

            return 0;
        }

        public float GetExtentZ(ulong trackable_handle)
        {
            var info = GetTrackableInfo(trackable_handle);

            if (info != null)
            {
                return info.Z;
            }

            return 0;
        }

        public TrackablePlaneType GetPlaneType(ulong trackable_handle)
        {
            return TrackablePlaneType.HORIZONTAL;
        }
    }

    public class NREmulatorTrackImageProvider : NREmulatorTrackableProvider, ITrackableImageDataProvider
    {
        public ulong CreateDataBase()
        {
            return 0;
        }

        public bool DestroyDataBase(ulong database_handle)
        {
            return true;
        }

        public Pose GetCenterPose(ulong trackable_handle)
        {
            var info = GetTrackableInfo(trackable_handle);
            if (info != null)
            {
                return new Pose(info.position, info.rotation);
            }

            return Pose.identity;
        }

        public Vector2 GetSize(ulong trackable_handle)
        {
            var info = GetTrackableInfo(trackable_handle);
            if (info != null)
            {
                return new Vector2(info.X, info.Z);
            }

            return Vector2.zero;
        }

        public bool LoadDataBase(ulong database_handle, string path)
        {
            return true;
        }
    }
#endif
}
