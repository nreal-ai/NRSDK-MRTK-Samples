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
    using System.Collections.Generic;

    /// <summary> A camera proxy factory. </summary>
    public class CameraProxyFactory
    {
        /// <summary> Dictionary of camera controllers. </summary>
        private static Dictionary<string, NativeCameraProxy> m_CameraControllerDict = new Dictionary<string, NativeCameraProxy>();

        /// <summary> Creates RGB camera proxy. </summary>
        /// <returns> The new RGB camera proxy. </returns>
        public static NativeCameraProxy CreateRGBCameraProxy()
        {
            NativeCameraProxy controller;
            if (!m_CameraControllerDict.TryGetValue(NRRgbCamera.ID, out controller))
            {
                controller = new NRRgbCamera();
                m_CameraControllerDict.Add(NRRgbCamera.ID, controller);
            }
            return controller;
        }

        /// <summary> Gets an instance. </summary>
        /// <param name="id"> The identifier.</param>
        /// <returns> The instance. </returns>
        public static NativeCameraProxy GetInstance(string id)
        {
            if (!m_CameraControllerDict.ContainsKey(id))
            {
                return null;
            }
            return m_CameraControllerDict[id];
        }

        /// <summary> Regist camera proxy. </summary>
        /// <param name="id">    The identifier.</param>
        /// <param name="proxy"> The proxy.</param>
        public static void RegistCameraProxy(string id, NativeCameraProxy proxy)
        {
            m_CameraControllerDict[id] = proxy;
        }
    }
}
