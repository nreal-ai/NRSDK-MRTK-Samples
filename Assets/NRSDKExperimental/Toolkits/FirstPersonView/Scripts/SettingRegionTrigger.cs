using UnityEngine;
using UnityEngine.Events;

namespace NRKernal.Experimental.StreammingCast
{
    [RequireComponent(typeof(Collider))]
    public class SettingRegionTrigger : MonoBehaviour
    {
        public UnityEvent onPointerEnter;
        public UnityEvent onPointerOut;
        private Collider m_TriggerRegion;

        private Collider m_CurrentHover;
        private Collider currentHover
        {
            get
            {
                return m_CurrentHover;
            }
            set
            {
                if (m_CurrentHover != value)
                {
                    if (value == m_TriggerRegion)
                    {
                        OnPointerEnter();
                    }
                    else
                    {
                        OnPointerExit();
                    }
                }
                m_CurrentHover = value;
            }
        }

        private Transform laserAnchor
        {
            get
            {
                return NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ?
                    ControllerAnchorEnum.GazePoseTrackerAnchor : ControllerAnchorEnum.RightLaserAnchor);
            }
        }

        void Start()
        {
            m_TriggerRegion = gameObject.GetComponent<Collider>();
        }

        void Update()
        {
            RaycastHit hitResult;
            Physics.Raycast(new Ray(laserAnchor.position, laserAnchor.forward), out hitResult, 10);
            currentHover = hitResult.collider;
        }

        public void OnPointerEnter()
        {
            onPointerEnter?.Invoke();
        }

        public void OnPointerExit()
        {
            onPointerOut?.Invoke();
        }
    }
}
