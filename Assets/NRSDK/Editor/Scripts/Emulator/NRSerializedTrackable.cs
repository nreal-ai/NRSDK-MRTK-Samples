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

    /// <summary> A nr serialized trackable. </summary>
    public class NRSerializedTrackable
    {
        /// <summary> The serialized object. </summary>
        protected readonly SerializedObject m_SerializedObject;
        /// <summary> Name of the trackable. </summary>
        private readonly SerializedProperty m_TrackableName;
        /// <summary> The initialized in editor. </summary>
        private readonly SerializedProperty m_InitializedInEditor;

        /// <summary> Gets the serialized object. </summary>
        /// <value> The serialized object. </value>
        public SerializedObject SerializedObject { get { return m_SerializedObject; } }
        /// <summary> Gets the trackable name property. </summary>
        /// <value> The trackable name property. </value>
        public SerializedProperty TrackableNameProperty { get { return m_TrackableName; } }
        /// <summary> Gets or sets the name of the trackable. </summary>
        /// <value> The name of the trackable. </value>
        public string TrackableName { get { return m_TrackableName.stringValue; } set { m_TrackableName.stringValue = value; } }
        /// <summary> Gets the initialize in editor preperty. </summary>
        /// <value> The initialize in editor preperty. </value>
        public SerializedProperty InitInEditorPreperty { get { return m_InitializedInEditor; } }
        /// <summary> Gets or sets a value indicating whether the initialized in editor. </summary>
        /// <value> True if initialized in editor, false if not. </value>
        public bool InitializedInEditor { get { return m_InitializedInEditor.boolValue; } set { m_InitializedInEditor.boolValue = value; } }


        /// <summary> Constructor. </summary>
        /// <param name="target"> Target for the.</param>
        public NRSerializedTrackable(SerializedObject target)
        {
            m_SerializedObject = target;
            m_TrackableName = m_SerializedObject.FindProperty("m_TrackableName");
            m_InitializedInEditor = m_SerializedObject.FindProperty("m_InitializedInEditor");
        }

        /// <summary> Gets the material. </summary>
        /// <returns> The material. </returns>
        public Material GetMaterial()
        {
            return ((MonoBehaviour)m_SerializedObject.targetObject).GetComponent<Renderer>().sharedMaterial;
        }

        /// <summary> Gets the materials. </summary>
        /// <returns> An array of material. </returns>
        public Material[] GetMaterials()
        {
            return ((MonoBehaviour)m_SerializedObject.targetObject).GetComponent<Renderer>().sharedMaterials;
        }


        /// <summary> Sets a material. </summary>
        /// <param name="material"> The material.</param>
        public void SetMaterial(Material material)
        {
            object[] targetObjs = m_SerializedObject.targetObjects;
            for (int i = 0; i < targetObjs.Length; i++)
            {
                ((MonoBehaviour)targetObjs[i]).GetComponent<Renderer>().sharedMaterial = material;
            }
            NREditorSceneManager.Instance.UnloadUnusedAssets();
        }

        /// <summary> Sets a material. </summary>
        /// <param name="materials"> The materials.</param>
        public void SetMaterial(Material[] materials)
        {
            object[] targetObjs = m_SerializedObject.targetObjects;
            for (int i = 0; i < targetObjs.Length; i++)
            {
                ((MonoBehaviour)targetObjs[i]).GetComponent<Renderer>().sharedMaterials = materials;
            }
            NREditorSceneManager.Instance.UnloadUnusedAssets();
        }

        /// <summary> Gets game objects. </summary>
        /// <returns> The game objects. </returns>
        public List<GameObject> GetGameObjects()
        {
            List<GameObject> list = new List<GameObject>();
            object[] targetObjs = m_SerializedObject.targetObjects;
            for (int i = 0; i < targetObjs.Length; i++)
            {
                list.Add(((MonoBehaviour)targetObjs[i]).gameObject);
            }
            return list;
        }
    }
}