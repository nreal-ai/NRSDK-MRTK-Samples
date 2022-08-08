/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> A nr event center. </summary>
    public class NREventCenter
    {
        /// <summary> Gets current raycast target. </summary>
        /// <returns> The current raycast target. </returns>
        public static GameObject GetCurrentRaycastTarget()
        {
            NRPointerRaycaster raycaster = GetRaycaster(NRInput.DomainHand);
            if (raycaster == null)
                return null;
            var result = raycaster.FirstRaycastResult();
            if (!result.isValid)
                return null;
            return result.gameObject;
        }

        /// <summary> Dictionary of raycasters. </summary>
        private static Dictionary<ControllerAnchorEnum, NRPointerRaycaster> raycasterDict;

        /// <summary> Gets a raycaster. </summary>
        /// <param name="handEnum"> The hand enum.</param>
        /// <returns> The raycaster. </returns>
        public static NRPointerRaycaster GetRaycaster(ControllerHandEnum handEnum)
        {
            if (raycasterDict == null)
            {
                raycasterDict = new Dictionary<ControllerAnchorEnum, NRPointerRaycaster>();
                NRPointerRaycaster _raycaster = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.GazePoseTrackerAnchor).GetComponentInChildren<NRPointerRaycaster>(true);
                raycasterDict.Add(ControllerAnchorEnum.GazePoseTrackerAnchor, _raycaster);
                _raycaster = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.RightLaserAnchor).GetComponentInChildren<NRPointerRaycaster>(true);
                raycasterDict.Add(ControllerAnchorEnum.RightLaserAnchor, _raycaster);
                _raycaster = NRInput.AnchorsHelper.GetAnchor(ControllerAnchorEnum.LeftLaserAnchor).GetComponentInChildren<NRPointerRaycaster>(true);
                raycasterDict.Add(ControllerAnchorEnum.LeftLaserAnchor, _raycaster);
            }
            NRPointerRaycaster raycaster = null;
            if (NRInput.RaycastMode == RaycastModeEnum.Gaze)
                raycasterDict.TryGetValue(ControllerAnchorEnum.GazePoseTrackerAnchor, out raycaster);
            else if (NRInput.DomainHand == ControllerHandEnum.Right)
                raycasterDict.TryGetValue(ControllerAnchorEnum.RightLaserAnchor, out raycaster);
            else if (NRInput.DomainHand == ControllerHandEnum.Left)
                raycasterDict.TryGetValue(ControllerAnchorEnum.LeftLaserAnchor, out raycaster);
            return raycaster;
        }
    }
}
