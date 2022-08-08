using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.NRExamples
{
    public class ItemsCollector : MonoBehaviour
    {
        public NRGrabber rightGrabber;
        public NRGrabber leftGrabber;
        public Transform[] itemsArr;

        private Dictionary<Transform, Pose> m_ItemsPoseDict;
        private Dictionary<HandEnum, bool> m_GrabbingStateDict;
        private bool m_Inited;

        private void Update()
        {
            if (!m_Inited)
            {
                Init();
                return;
            }
            UpdateGrabber(HandEnum.RightHand, rightGrabber);
            UpdateGrabber(HandEnum.LeftHand, leftGrabber);
        }

        private void Init()
        {
            m_ItemsPoseDict = new Dictionary<Transform, Pose>();
            m_GrabbingStateDict = new Dictionary<HandEnum, bool>()
            {
                {HandEnum.RightHand, false},
                {HandEnum.LeftHand, false}
            };
            if (itemsArr != null)
            {
                for (int i = 0; i < itemsArr.Length; i++)
                {
                    var item = itemsArr[i];
                    if (item == null)
                        continue;
                    m_ItemsPoseDict.Add(item, new Pose(item.position, item.rotation));
                }
            }
            rightGrabber.SetGrabJudgeCondition(() => CheckIsGrabbing(HandEnum.RightHand));
            leftGrabber.SetGrabJudgeCondition(() => CheckIsGrabbing(HandEnum.LeftHand));
            rightGrabber.gameObject.SetActive(false);
            leftGrabber.gameObject.SetActive(false);
            m_Inited = true;
        }

        private void UpdateGrabber(HandEnum handEnum, NRGrabber grabber)
        {
            if (grabber == null)
                return;

            if (m_GrabbingStateDict[handEnum] != grabber.IsGrabbingObjects)
            {
                OnGrabStateChange(handEnum);
                m_GrabbingStateDict[handEnum] = grabber.IsGrabbingObjects;
            }

            var handState = NRInput.Hands.GetHandState(handEnum);
            if (handState == null || !handState.isTracked)
            {
                grabber.gameObject.SetActive(false);
                return;
            }

            grabber.gameObject.SetActive(true);
            var grabPose = handState.GetJointPose(HandJointID.IndexProximal);
            grabber.transform.position = grabPose.position;
            grabber.transform.rotation = grabPose.rotation;
        }

        private bool CheckIsGrabbing(HandEnum handEnum)
        {
            var handState = NRInput.Hands.GetHandState(handEnum);
            if (handState != null && handState.isTracked && handState.isPinching)
            {
                return true;
            }
            return false;
        }

        private void OnGrabStateChange(HandEnum handEnum)
        {

        }

        public void ResetItems()
        {
            if (m_ItemsPoseDict != null)
            {
                foreach (KeyValuePair<Transform, Pose> itemKeyPair in m_ItemsPoseDict)
                {
                    itemKeyPair.Key.position = itemKeyPair.Value.position;
                    itemKeyPair.Key.rotation = itemKeyPair.Value.rotation;
                    var rigid = itemKeyPair.Key.GetComponent<Rigidbody>();
                    if (rigid)
                    {
                        rigid.angularVelocity = Vector3.zero;
                        rigid.velocity = Vector3.zero;
                        rigid.constraints = RigidbodyConstraints.FreezeAll;
                        rigid.constraints = RigidbodyConstraints.None;
                    }
                }
            }
        }
    }
}
