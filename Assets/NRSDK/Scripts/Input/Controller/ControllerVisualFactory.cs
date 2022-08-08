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
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    
    /// <summary> A controller visual factory. </summary>
    internal static class ControllerVisualFactory
    {
        /// <summary> Creates controller visual object. </summary>
        /// <param name="visualType"> Type of the visual.</param>
        /// <returns> The new controller visual object. </returns>
        public static GameObject CreateControllerVisualObject(ControllerVisualType visualType)
        {
            GameObject visualObj = null;
            string prefabPath = "";
            string folderPath = "ControllerVisuals/";
            switch (visualType)
            {
                case ControllerVisualType.None:
                    return null;
                case ControllerVisualType.NrealLight:
                    prefabPath = folderPath + "nreal_light_controller_visual";
                    break;
                case ControllerVisualType.Phone:
                    prefabPath = folderPath + "phone_controller_visual";
                    break;
                default:
                    NRDebugger.Error("Can not find controller visual for: " + visualType + ", set to default visual");
                    prefabPath = folderPath + "nreal_light_controller_visual";
                    break;
            }
            if (!string.IsNullOrEmpty(prefabPath))
            {
                GameObject controllerPrefab = Resources.Load<GameObject>(prefabPath);
                if (controllerPrefab)
                    visualObj = GameObject.Instantiate(controllerPrefab);
            }
            if (visualObj == null)
                NRDebugger.Error("Create controller visual failed, prefab path:" + prefabPath);
            return visualObj;
        }

        /// <summary> Gets default visual type. </summary>
        /// <param name="controllerType"> Type of the controller.</param>
        /// <returns> The default visual type. </returns>
        public static ControllerVisualType GetDefaultVisualType(ControllerType controllerType)
        {
            switch (controllerType)
            {
                case ControllerType.CONTROLLER_TYPE_HAND:
                    return ControllerVisualType.None;
                case ControllerType.CONTROLLER_TYPE_PHONE:
                    return ControllerVisualType.Phone;
                default:
                    return ControllerVisualType.NrealLight;
            }
        }
    }
    
}