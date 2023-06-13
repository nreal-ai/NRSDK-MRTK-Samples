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

    /// <summary> Update the transform of  a trackable. </summary>
    public partial class NRAnchor : MonoBehaviour
    {
        /// <summary> Dictionary of anchors. </summary>
        private static Dictionary<Int64, NRAnchor> m_AnchorDict = new Dictionary<Int64, NRAnchor>();

        /// <summary> The trackable. </summary>
        public NRTrackable Trackable;

        /// <summary> True if is session destroyed, false if not. </summary>
        private bool m_IsSessionDestroyed;

        /// <summary> Create a anchor for the trackable object. </summary>
        /// <param name="trackable"> Instantiate a NRAnchor object which Update trackable pose every frame.</param>
        /// <returns> NRAnchor. </returns>
        public static NRAnchor Factory(NRTrackable trackable)
        {
            if (trackable == null)
            {
                return null;
            }

            NRAnchor result;
            if (m_AnchorDict.TryGetValue(trackable.GetDataBaseIndex(), out result))
            {
                return result;
            }

            NRAnchor anchor = (new GameObject()).AddComponent<NRAnchor>();
            anchor.gameObject.name = "Anchor";
            anchor.Trackable = trackable;

            m_AnchorDict.Add(trackable.GetDataBaseIndex(), anchor);
            return anchor;

        }

        /// <summary> Updates this object. </summary>
        private void Update()
        {
            if (Trackable == null)
            {
                NRDebugger.Error("NRAnchor components instantiated outside of NRInternel are not supported. " +
                    "Please use a 'Create' method within NRInternel to instantiate anchors.");
                return;
            }

            if (IsSessionDestroyed())
            {
                return;
            }

            var pose = Trackable.GetCenterPose();
            transform.position = pose.position;
            transform.rotation = pose.rotation;

        }

        /// <summary> Executes the 'destroy' action. </summary>
        private void OnDestroy()
        {
            if (Trackable == null)
            {
                return;
            }

            m_AnchorDict.Remove(Trackable.GetDataBaseIndex());
        }

        /// <summary> Check whether the session is already destroyed. </summary>
        /// <returns> True if session destroyed, false if not. </returns>
        private bool IsSessionDestroyed()
        {
            if (!m_IsSessionDestroyed)
            {
                var subsystem = NRSessionManager.Instance.TrackableFactory.TrackableSubsystem;
                if (subsystem != Trackable.TrackableSubsystem)
                {
                    Debug.LogErrorFormat("The session which created this anchor has been destroyed. " +
                    "The anchor on GameObject {0} can no longer update.",
                        this.gameObject != null ? this.gameObject.name : "Unknown");
                    m_IsSessionDestroyed = true;
                }
            }

            return m_IsSessionDestroyed;
        }
    }
}
