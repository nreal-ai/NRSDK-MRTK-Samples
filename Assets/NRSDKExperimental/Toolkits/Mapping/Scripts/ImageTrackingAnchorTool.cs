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
    using System.Collections.Generic;

    /// <summary> An image tracking anchor tool. </summary>
    public class ImageTrackingAnchorTool : MonoBehaviour
    {
        /// <summary> Event queue for all listeners interested in OnAnchorLoaded events. </summary>
        public event Action<string, NRWorldAnchor> OnAnchorLoaded;
        /// <summary> Event queue for all listeners interested in OnAnchorPoseUpdate events. </summary>
        public event Action<Pose> OnAnchorPoseUpdate;
        /// <summary> The tracking image visualizer prefab. </summary>
        public TrackingImageVisualizerForAnchor TrackingImageVisualizerPrefab;
        /// <summary> Zero-based index of the used image. </summary>
        public int usedImageIndex = 0;
        /// <summary> The visualizer. </summary>
        TrackingImageVisualizerForAnchor visualizer;

        private bool m_IsStart = true;
        /// <summary> True to use map. </summary>
        public bool useMap = false;
        /// <summary> The anchor key. </summary>
        public string anchorKey;

        /// <summary> The anchor store. </summary>
        private NRWorldAnchorStore anchorStore;
        /// <summary> The current anchor. </summary>
        private NRWorldAnchor currentAnchor;

        void Start()
        {
            if (useMap)
            {
                this.LoadAnchor();
            }
        }

        private void LoadAnchor()
        {
            NRWorldAnchorStore.GetAsync((store) =>
            {
                NRDebugger.Info("[ImageTrackingAnchorTool] Get world anchor map success.");
                this.anchorStore = store;
                var keys = anchorStore.GetAllIds();
                if (keys == null)
                {
                    NRDebugger.Info("[ImageTrackingAnchorTool] Can not get any keys...");
                    return;
                }

                foreach (var item in keys)
                {
                    if (item.Equals(anchorKey))
                    {
                        GameObject go = new GameObject(anchorKey);
                        currentAnchor = anchorStore.Load(anchorKey, go);
                        NRDebugger.Info("[ImageTrackingAnchorTool] Find the anchor ：" + anchorKey);
                        OnAnchorLoaded?.Invoke(anchorKey, currentAnchor);
                        break;
                    }
                }
            });
        }

        private void Update()
        {
            if (!m_IsStart)
            {
                return;
            }

            List<NRTrackableImage> trackableImages = new List<NRTrackableImage>();
            NRFrame.GetTrackables<NRTrackableImage>(trackableImages, NRTrackableQueryFilter.All);
            Pose pose;
            foreach (var image in trackableImages)
            {
                if (image.GetDataBaseIndex() == usedImageIndex)
                {
                    pose = image.GetCenterPose();
                    if (visualizer == null)
                    {
                        visualizer = (TrackingImageVisualizerForAnchor)Instantiate(TrackingImageVisualizerPrefab, pose.position, pose.rotation);
                        visualizer.Image = image;
                        if (useMap)
                        {
                            visualizer.OnConfirm += SaveAnchor;
                        }
                    }
                    break;
                }
            }

            // Update pose.
            if (useMap)
            {
                if (currentAnchor != null && currentAnchor.GetTrackingState() == TrackingState.Tracking)
                {
                    pose = currentAnchor.GetCenterPose();
                    OnAnchorPoseUpdate?.Invoke(pose);
                }
            }
            else
            {
                if (visualizer != null && visualizer.Image.GetTrackingState() == TrackingState.Tracking)
                {
                    pose = visualizer.Image.GetCenterPose();
                    OnAnchorPoseUpdate?.Invoke(pose);
                }
            }
        }

        /// <summary> True to lock, false to unlock the save. </summary>
        private bool saveLock = false;
        /// <summary> Saves the anchor. </summary>
        private void SaveAnchor()
        {
            if (anchorStore == null || saveLock || visualizer == null || string.IsNullOrEmpty(anchorKey)
                || visualizer.Image.GetTrackingState() != TrackingState.Tracking)
            {
                NRDebugger.Warning("[ImageTrackingAnchorTool] Can not save the anchor!");
                return;
            }

            Invoke("Unlock", 1f);

            anchorStore.Delete(anchorKey);

            NRDebugger.Info("[ImageTrackingAnchorTool] Save the anchor.");
            // Avoid to frequent calls.
            saveLock = true;

            currentAnchor = anchorStore.AddAnchor(anchorKey,
                new Pose(visualizer.transform.position, visualizer.transform.rotation));
            OnAnchorLoaded?.Invoke(anchorKey, currentAnchor);
        }

        /// <summary> Unlocks this object. </summary>
        private void Unlock()
        {
            saveLock = false;
        }

        /// <summary> Switch trackable image. </summary>
        /// <param name="isopen"> True to isopen.</param>
        public void SwitchTrackableImage(bool isopen)
        {
            NRDebugger.Info("[ImageTrackingAnchorTool] SwitchTrackableImage:" + isopen);
            m_IsStart = isopen;
            visualizer?.gameObject.SetActive(isopen);
            //var config = NRSessionManager.Instance.NRSessionBehaviour.SessionConfig;
            //config.ImageTrackingMode = isopen ? TrackableImageFindingMode.ENABLE : TrackableImageFindingMode.DISABLE;
            //NRSessionManager.Instance.SetConfiguration(config);
        }
    }
}