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
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    /// <summary> A nr serialized image target. </summary>
    public class NRSerializedImageTarget : NRSerializedTrackable
    {
        /// <summary> The aspect ratio. </summary>
        private readonly SerializedProperty m_AspectRatio;
        /// <summary> The width. </summary>
        private readonly SerializedProperty m_Width;
        /// <summary> The height. </summary>
        private readonly SerializedProperty m_Height;
        /// <summary> The tracking image database. </summary>
        private readonly SerializedProperty m_TrackingImageDatabase;
        /// <summary> Zero-based index of the database. </summary>
        private readonly SerializedProperty m_DatabaseIndex;

        /// <summary> Gets the aspect ratio property. </summary>
        /// <value> The aspect ratio property. </value>
        public SerializedProperty AspectRatioProperty { get { return m_AspectRatio; } }
        /// <summary> Gets or sets the aspect ratio. </summary>
        /// <value> The aspect ratio. </value>
        public float AspectRatio { get { return m_AspectRatio.floatValue; } set { m_AspectRatio.floatValue = value; } }

        /// <summary> Gets the width property. </summary>
        /// <value> The width property. </value>
        public SerializedProperty WidthProperty { get { return m_Width; } }
        /// <summary> Gets or sets the width. </summary>
        /// <value> The width. </value>
        public float Width { get { return m_Width.floatValue; } set { m_Width.floatValue = value; } }

        /// <summary> Gets the height property. </summary>
        /// <value> The height property. </value>
        public SerializedProperty HeightProperty { get { return m_Height; } }
        /// <summary> Gets or sets the height. </summary>
        /// <value> The height. </value>
        public float Height { get { return m_Height.floatValue; } set { m_Height.floatValue = value; } }

        /// <summary> Gets the tracking image database property. </summary>
        /// <value> The tracking image database property. </value>
        public SerializedProperty TrackingImageDatabaseProperty { get { return m_TrackingImageDatabase; } }
        /// <summary> Gets or sets the tracking image database. </summary>
        /// <value> The tracking image database. </value>
        public string TrackingImageDatabase { get { return m_TrackingImageDatabase.stringValue; } set { m_TrackingImageDatabase.stringValue = value; } }


        /// <summary> Gets the database index property. </summary>
        /// <value> The database index property. </value>
        public SerializedProperty DatabaseIndexProperty { get { return m_DatabaseIndex; } }
        /// <summary> Gets or sets the zero-based index of the database. </summary>
        /// <value> The database index. </value>
        public int DatabaseIndex { get { return m_DatabaseIndex.intValue; } set { m_DatabaseIndex.intValue = value; } }


        /// <summary> Constructor. </summary>
        /// <param name="target"> Target for the.</param>
        public NRSerializedImageTarget(SerializedObject target) : base(target)
        {
            m_AspectRatio = target.FindProperty("m_AspectRatio");
            m_Width = target.FindProperty("m_Width");
            m_Height = target.FindProperty("m_Height");
            m_DatabaseIndex = target.FindProperty("m_DatabaseIndex");
        }

        /// <summary> Gets the behaviours. </summary>
        /// <returns> The behaviours. </returns>
        public List<NRTrackableImageBehaviour> GetBehaviours()
        {
            List<NRTrackableImageBehaviour> list = new List<NRTrackableImageBehaviour>();
            Object[] targetObjs = m_SerializedObject.targetObjects;
            for (int i = 0; i < targetObjs.Length; i++)
            {
                list.Add((NRTrackableImageBehaviour)targetObjs[i]);
            }
            return list;
        }
    }
}