using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class PenCtrl : MonoBehaviour
{
    public HandEnum handEnum;
    public BasePen pen;

	void Update ()
    {
        if (!NRInput.Hands.IsRunning)
            return;
        var handState = NRInput.Hands.GetHandState(handEnum);
        pen.IsDrawing = (handState.currentGesture == HandGesture.Point);
        if (pen.IsDrawing)
        {
            var pose = handState.GetJointPose(HandJointID.IndexTip);
            pen.PenTransform.position = pose.position;
        }
	}
}
