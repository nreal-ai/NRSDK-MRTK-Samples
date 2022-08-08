/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using UnityEngine;

namespace NRKernal.NRExamples
{
    public class GetSurportedFeatures : MonoBehaviour
    {
        void Start()
        {
            var deviceType = NRDevice.Subsystem.GetDeviceType();

            bool tracking_six_dof = NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_TRACKING_6DOF);
            bool tracking_three_dof = NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_TRACKING_3DOF);
            bool tracking_plane_horizontal = NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_TRACKING_FINDING_HORIZONTAL_PLANE);
            bool tracking_plane_vertical = NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_TRACKING_FINDING_VERTICAL_PLANE);
            bool tracking_imagetracking = NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_TRACKING_FINDING_MARKER);
            bool controller_three_dof = NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_CONTROLLER_3DOF);
            bool controller_six_dof = NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_CONTROLLER_6DOF);
            bool glasses_wearing_status = NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_WEARING_STATUS_OF_GLASSES);
            bool handtracking = NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_HANDTRACKING);
            bool rgbcamera = NRDevice.Subsystem.IsFeatureSupported(NRSupportedFeature.NR_FEATURE_RGB_CAMERA);

            NRDebugger.Info("deviceType:{10}, tracking_six_dof:{0} tracking_three_dof:{1} tracking_plane_horizontal:{2} tracking_plane_vertical:{3}" +
                "tracking_imagetracking:{4} controller_three_dof:{5} controller_six_dof:{6} glasses_wearing_status:{7} handtracking:{8} rgbcamera:{9}",
                tracking_six_dof, tracking_three_dof, tracking_plane_horizontal, tracking_plane_vertical, tracking_imagetracking, controller_three_dof,
                controller_six_dof, glasses_wearing_status, handtracking, rgbcamera, deviceType);
        }
    }
}
