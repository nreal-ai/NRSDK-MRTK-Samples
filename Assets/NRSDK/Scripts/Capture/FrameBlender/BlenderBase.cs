/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    using UnityEngine;

    public abstract class BlenderBase : IFrameConsumer
    {
        public virtual RenderTexture BlendTexture { get; protected set; }

        /// <summary> Gets or sets the width. </summary>
        /// <value> The width. </value>
        public int Width
        {
            get;
            protected set;
        }

        /// <summary> Gets or sets the height. </summary>
        /// <value> The height. </value>
        public int Height
        {
            get;
            protected set;
        }

        /// <summary> Gets the blend mode. </summary>
        /// <value> The blend mode. </value>
        public BlendMode BlendMode
        {
            get;
            protected set;
        }

        /// <summary> Gets or sets the number of frames. </summary>
        /// <value> The number of frames. </value>
        public int FrameCount
        {
            get;
            protected set;
        }

        public virtual void Init(Camera camera, IEncoder encoder, CameraParameters param) { }

        public virtual void OnFrame(UniversalTextureFrame frame) { }

        public virtual void Dispose() { }
    }
}
