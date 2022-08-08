/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental.StreammingCast
{
    using NRKernal.Experimental.NetWork;

    /// <summary> An observer view net worker. </summary>
    public class ObserverViewNetWorker : NetWorkBehaviour
    {
        /// <summary> The context. </summary>
        ObserverViewFrameCaptureContext m_Context;

        public ObserverViewNetWorker(ObserverViewFrameCaptureContext contex)
        {
            this.m_Context = contex;
        }

        public override void Listen()
        {
            base.Listen();

            m_NetWorkClient.OnCameraParamUpdate += OnCameraParamUpdate;
        }

        /// <summary> Executes the 'camera parameter update' action. </summary>
        /// <param name="param"> The parameter.</param>
        private void OnCameraParamUpdate(CameraParam param)
        {
            if (this.m_Context == null)
            {
                return;
            }
            this.m_Context.GetBehaviour().UpdateCameraParam(param.fov);
        }
    }
}
