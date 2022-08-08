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

    /// <summary> A nr grabber. </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class NRGrabber : MonoBehaviour
    {
        /// <summary> The grab button. </summary>
        public ControllerButton grabButton = ControllerButton.GRIP;
        /// <summary> The hand enum. </summary>
        public ControllerHandEnum handEnum;
        /// <summary> True to enable, false to disable the grab multi. </summary>
        public bool grabMultiEnabled = false;
        /// <summary> True to update pose by rigidbody. </summary>
        public bool updatePoseByRigidbody = true;

        /// <summary> True to previous grab press. </summary>
        private bool m_PreviousGrabPress;
        /// <summary> Dictionary of grab readies. </summary>
        private Dictionary<NRGrabbableObject, int> m_GrabReadyDict = new Dictionary<NRGrabbableObject, int>();
        /// <summary> Dictionary of grab start offset. </summary>
        private Dictionary<NRGrabbableObject, Pose> m_GrabStartOffsetDict = new Dictionary<NRGrabbableObject, Pose>();
        /// <summary> List of grabbings. </summary>
        private List<NRGrabbableObject> m_GrabbingList = new List<NRGrabbableObject>();
        /// <summary> The children colliders. </summary>
        private Collider[] m_ChildrenColliders;
        /// <summary> Func to judge if is grabbing. </summary>
        private Func<bool> m_FunToJudgeIsGrabbing;

        public bool IsGrabbingObjects{ get { return m_GrabbingList.Count > 0; } }

        /// <summary> Awakes this object. </summary>
        private void Awake()
        {
            Rigidbody rigid = GetComponent<Rigidbody>();
            rigid.useGravity = false;
            rigid.isKinematic = true;
            m_ChildrenColliders = GetComponentsInChildren<Collider>();
        }

        /// <summary> Executes the 'enable' action. </summary>
        private void OnEnable()
        {
            NRInput.OnControllerStatesUpdated += OnControllerPoseUpdated;
        }

        /// <summary> Executes the 'disable' action. </summary>
        private void OnDisable()
        {
            NRInput.OnControllerStatesUpdated -= OnControllerPoseUpdated;
            ReleaseAllGrabbedObject();
        }

        /// <summary> Fixed update. </summary>
        private void FixedUpdate()
        {
            if (!updatePoseByRigidbody)
                return;
            UpdateGrabbles();
        }

        /// <summary> Executes the 'trigger enter' action. </summary>
        /// <param name="other"> The other.</param>
        private void OnTriggerEnter(Collider other)
        {
            NRGrabbableObject grabble = other.GetComponent<NRGrabbableObject>() ?? other.GetComponentInParent<NRGrabbableObject>();
            if (grabble == null)
                return;
            if (m_GrabReadyDict.ContainsKey(grabble))
            {
                m_GrabReadyDict[grabble] += 1;
            }
            else
            {
                m_GrabReadyDict.Add(grabble, 1);
            }
        }

        /// <summary> Executes the 'trigger exit' action. </summary>
        /// <param name="other"> The other.</param>
        private void OnTriggerExit(Collider other)
        {
            NRGrabbableObject grabble = other.GetComponent<NRGrabbableObject>() ?? other.GetComponentInParent<NRGrabbableObject>();
            if (grabble == null)
                return;
            int count = 0;
            if (m_GrabReadyDict.TryGetValue(grabble, out count))
            {
                if (count > 1)
                {
                    m_GrabReadyDict[grabble] = count - 1;
                }
                else
                {
                    m_GrabReadyDict.Remove(grabble);
                }
            }
        }

        /// <summary> Executes the 'controller pose updated' action. </summary>
        private void OnControllerPoseUpdated()
        {
            if (updatePoseByRigidbody)
                return;
            UpdateGrabbles();
        }

        /// <summary> Updates the grabbles. </summary>
        private void UpdateGrabbles()
        {
            bool pressGrab = false;
            if (m_FunToJudgeIsGrabbing != null)
            {
                pressGrab = m_FunToJudgeIsGrabbing.Invoke();
            }
            else
            {
                pressGrab = NRInput.GetButton(handEnum, grabButton);
            }
            bool triggeredGrab = !m_PreviousGrabPress && pressGrab;
            bool releaseAction = m_PreviousGrabPress && !pressGrab;
            m_PreviousGrabPress = pressGrab;
            if (triggeredGrab && m_GrabbingList.Count == 0 && m_GrabReadyDict.Keys.Count != 0)
            {
                if (!grabMultiEnabled)
                {
                    NRGrabbableObject nearestGrabble = GetNearestGrabbleObject();
                    if (nearestGrabble)
                    {
                        GrabTarget(nearestGrabble);
                    }
                }
                else
                {
                    var grabbleQueue = new Queue<NRGrabbableObject>(m_GrabReadyDict.Keys);
                    while(grabbleQueue.Count > 0)
                    {
                        GrabTarget(grabbleQueue.Dequeue());
                    }
                    m_GrabReadyDict.Clear();
                }
                SetChildrenCollidersEnabled(false);
            }

            if (releaseAction)
            {
                ReleaseAllGrabbedObject();
                return;
            }

            if (m_GrabbingList.Count > 0 && !triggeredGrab)
            {
                MoveGrabbingObjects();
            }
        }

        /// <summary> Gets nearest grabble object. </summary>
        /// <returns> The nearest grabble object. </returns>
        private NRGrabbableObject GetNearestGrabbleObject()
        {
            NRGrabbableObject nearestGrabble = null;
            float nearestSqrMagnitude = float.MaxValue;
            foreach (NRGrabbableObject grabbleObj in m_GrabReadyDict.Keys)
            {
                if (grabbleObj.AttachedColliders == null)
                    continue;
                for (int i = 0; i < grabbleObj.AttachedColliders.Length; i++)
                {
                    Vector3 closestPoint = grabbleObj.AttachedColliders[i].ClosestPointOnBounds(transform.position);
                    float grabbableSqrMagnitude = (transform.position - closestPoint).sqrMagnitude;
                    if (grabbableSqrMagnitude < nearestSqrMagnitude)
                    {
                        nearestSqrMagnitude = grabbableSqrMagnitude;
                        nearestGrabble = grabbleObj;
                    }
                }
            }
            return nearestGrabble;
        }

        /// <summary> Grab target. </summary>
        /// <param name="target"> Target for the.</param>
        private void GrabTarget(NRGrabbableObject target)
        {
            if (target == null || !target.CanGrab)
                return;
            if (!m_GrabbingList.Contains(target))
            {
                m_GrabbingList.Add(target);
            }
            if (!m_GrabStartOffsetDict.ContainsKey(target))
            {
                var offsetPosition = transform.InverseTransformPoint(target.transform.position);
                var offsetRotation = Quaternion.Inverse(transform.rotation) * target.transform.rotation;
                m_GrabStartOffsetDict.Add(target, new Pose(offsetPosition, offsetRotation));
            }
            m_GrabReadyDict.Remove(target);
            target.GrabBegin(this);
        }

        /// <summary> Move grabbing objects. </summary>
        private void MoveGrabbingObjects()
        {
            for (int i = 0; i < m_GrabbingList.Count; i++)
            {
                Vector3 targetPos;
                Quaternion targetRot;
                Pose offsetPose;
                if(m_GrabStartOffsetDict.TryGetValue(m_GrabbingList[i], out offsetPose))
                {
                    targetPos = transform.TransformPoint(offsetPose.position);
                    targetRot = transform.rotation * offsetPose.rotation;
                }
                else
                {
                    targetPos = transform.position;
                    targetRot = transform.rotation;
                }
                if (updatePoseByRigidbody)
                {
                    m_GrabbingList[i].MoveRigidbody(targetPos, targetRot);
                }
                else
                {
                    m_GrabbingList[i].MoveTransform(targetPos, targetRot);
                }
            }
        }

        /// <summary> Sets children colliders enabled. </summary>
        /// <param name="isEnabled"> True if is enabled, false if not.</param>
        private void SetChildrenCollidersEnabled(bool isEnabled)
        {
            if (m_ChildrenColliders == null)
                return;

            for (int i = 0; i < m_ChildrenColliders.Length; i++)
            {
                m_ChildrenColliders[i].enabled = isEnabled;
            }
        }

        /// <summary> Release all the grabbed objects. </summary>
        public void ReleaseAllGrabbedObject()
        {
            for (int i = 0; i < m_GrabbingList.Count; i++)
            {
                m_GrabbingList[i].GrabEnd();
            }
            m_GrabbingList.Clear();
            m_GrabStartOffsetDict.Clear();
            SetChildrenCollidersEnabled(true);
        }

        /// <summary> Set the condition to judge if is grabbing. </summary>
        /// <param name="judgeGrabbingFunc"></param>
        public void SetGrabJudgeCondition(Func<bool> judgeGrabbingFunc)
        {
            m_FunToJudgeIsGrabbing = judgeGrabbingFunc;
        }
    }
}
