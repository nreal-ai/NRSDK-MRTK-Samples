/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.NREditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    /// <summary> A nr image target accessor. </summary>
    internal class NRImageTargetAccessor : NRTrackableAccessor
    {

        /// <summary> The serialized object. </summary>
        private readonly NRSerializedImageTarget m_SerializedObject;

        /// <summary> Constructor. </summary>
        /// <param name="target"> Target for the.</param>
        public NRImageTargetAccessor(NRTrackableImageBehaviour target)
        {
            m_Target = target;
            m_SerializedObject = new NRSerializedImageTarget(new SerializedObject(m_Target));
        }

        /// <summary> Applies the data appearance. </summary>
        public override void ApplyDataAppearance()
        {
            NRTrackableImageEditor.UpdateAspectRatio(m_SerializedObject);
            NRTrackableImageEditor.UpdateMaterial(m_SerializedObject);
        }

        /// <summary> Applies the data properties. </summary>
        public override void ApplyDataProperties()
        {
            NRTrackableImageEditor.UpdateScale(m_SerializedObject);
        }
    }

    /// <summary> An image target data. </summary>
    public struct ImageTargetData
    {
        /// <summary> The size. </summary>
        public Vector2 Size;
        /// <summary> The preview image. </summary>
        public string PreviewImage;
    }
}