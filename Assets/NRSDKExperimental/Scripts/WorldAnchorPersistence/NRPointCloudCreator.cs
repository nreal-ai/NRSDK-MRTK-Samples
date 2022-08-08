/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.Persistence
{
    using System;
    using UnityEngine;
    using System.Collections;

    public class NRPointCloudCreator
    {
#if !UNITY_EDITOR
        private NativePointCloud NativePointCloud { get; set; }
#endif
        private IPointCloudDrawer PointCloudDrawer { get; set; }
        private static NRPointCloudCreator instance;

        public bool IsUpdatedThisFrame
        {
            get
            {
#if !UNITY_EDITOR
                return NativePointCloud.IsUpdatedThisFrame();
#else
                return true;
#endif
            }
        }

        private bool isInited = false;

        public static NRPointCloudCreator Create(IPointCloudDrawer drawer)
        {
            if (instance == null)
            {
                instance = new NRPointCloudCreator();
#if !UNITY_EDITOR
                instance.NativePointCloud = new NativePointCloud();
                instance.NativePointCloud.Create();
#endif
                instance.PointCloudDrawer = drawer;
                instance.Init();
            }

            return instance;
        }

        public IEnumerator UpdatePoints()
        {
#if UNITY_EDITOR
            PointCloudPoint[] points = new PointCloudPoint[1000];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new PointCloudPoint();
                points[i].Id = i;
                points[i].Position = new NativeVector3f(UnityEngine.Random.insideUnitSphere);
                points[i].Confidence = UnityEngine.Random.Range(0f, 1f);
            }
#endif
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                // Do the points update.
                if (IsUpdatedThisFrame)
                {
#if !UNITY_EDITOR
                    var listhandle = NativePointCloud.CreatPointCloudList();
                    NativePointCloud.UpdatePointCloudList(listhandle);
                    int len = NativePointCloud.GetSize(listhandle);
#else
                    int len = points.Length;
#endif

                    if (len > 0)
                    {
                        for (int i = 0; i < len; i++)
                        {
#if !UNITY_EDITOR
                            var point = NativePointCloud.AquireItem(listhandle, i);
#else
                            var point = points[i];
#endif
                            PointCloudDrawer.Update(point);
                        }
                        PointCloudDrawer.Draw();
                    }

#if !UNITY_EDITOR
                    NativePointCloud.DestroyPointCloudList(listhandle);
#endif
                }
            }
        }

        public void Init()
        {
            if (isInited)
            {
                return;
            }

            PointCloudCoroutine.Instance.StartCoroutine(UpdatePoints());

            isInited = true;
        }

        public void Save(string path)
        {
#if !UNITY_EDITOR
            NativePointCloud.SaveMap(path);
#endif
        }

        public static int confidence
        {
            get
            {
#if !UNITY_EDITOR
                return NativePointCloud.GetConfidence();
#else
                return 0;
#endif
            }
        }

        public void Destroy()
        {
#if !UNITY_EDITOR
            NativePointCloud.Destroy();
#endif
            instance = null;
            isInited = false;
        }
    }
}