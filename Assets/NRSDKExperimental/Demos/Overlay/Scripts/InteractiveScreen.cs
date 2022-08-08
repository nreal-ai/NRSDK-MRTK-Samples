using NRKernal;
using System;
using UnityEngine;

public class InteractiveScreen : MonoBehaviour
{
    public event Action<TouchState> onUpdateTouchState;

    public struct TouchState
    {
        public Vector2 touchPoint;
        public Int64 timeStamp;
        public bool isTouching;

        public override string ToString()
        {
            return string.Format("touchPoint:{0} isTouching:{1}", touchPoint.ToString("F2"), isTouching);
        }
    }

    public TouchState touchState;

    void Update()
    {
        if (NRInput.GetButtonDown(ControllerButton.TRIGGER))
        {
            touchState.isTouching = true;
        }

        if (NRInput.GetButtonUp(ControllerButton.TRIGGER))
        {
            touchState.isTouching = false;
        }

        var handControllerAnchor = NRInput.DomainHand == ControllerHandEnum.Left ? ControllerAnchorEnum.LeftLaserAnchor : ControllerAnchorEnum.RightLaserAnchor;
        Transform laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : handControllerAnchor);

        RaycastHit hitResult;
        if (Physics.Raycast(new Ray(laserAnchor.transform.position, laserAnchor.transform.forward), out hitResult, 10))
        {
            if (hitResult.collider != null && hitResult.collider is BoxCollider)
            {
                BoxCollider collider = hitResult.collider as BoxCollider;
                float scale_x, scale_y;
                scale_x = collider.transform.lossyScale.x * collider.size.x;
                scale_y = collider.transform.lossyScale.y * collider.size.y;
                Vector2 touchPoint = new Vector2(hitResult.point.x / scale_x + 0.5f, 0.5f - hitResult.point.y / scale_y);

                touchState.touchPoint = touchPoint;
                touchState.timeStamp = (Int64)(NRTools.GetTimeStampNanos());
                if (NRInput.GetButton(ControllerButton.TRIGGER) || NRInput.GetButtonUp(ControllerButton.TRIGGER))
                {
                    onUpdateTouchState?.Invoke(touchState);
                }
            }
        }
    }
}
