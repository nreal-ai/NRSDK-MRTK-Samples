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

    /// <summary> Editor for nr trackable image. </summary>
    [CanEditMultipleObjects, CustomEditor(typeof(NRTrackableImageBehaviour))]
    public class NRTrackableImageEditor : Editor
    {
        /// <summary> The serialized object. </summary>
        private NRSerializedImageTarget m_SerializedObj;
        /// <summary> The database. </summary>
        private static NRTrackingImageDatabase m_Database;
        /// <summary> Name of the images. </summary>
        private static string[] m_ImagesName;
        /// <summary> imageIndex; </summary>
        private int m_PreSelectOption = -1;

        /// <summary> Executes the 'enable' action. </summary>
        private void OnEnable()
        {
            NRTrackableImageBehaviour itb = (NRTrackableImageBehaviour)target;
            m_SerializedObj = new NRSerializedImageTarget(serializedObject);
            m_Database = GameObject.FindObjectOfType<NRSessionBehaviour>().SessionConfig.TrackingImageDatabase;

            if (m_Database == null) return;
            m_ImagesName = new string[m_Database.Count];
            EditorDatabase(itb, m_SerializedObj);
        }

        /// <summary> <para>Implement this function to make a custom inspector.</para> </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary> Draw inspector graphical user interface. </summary>
        private void DrawInspectorGUI()
        {
            if (m_Database == null)
            {
                NRDebugger.Error("NRKernalSessionConfig.TrackingImageDatabase is null");
                return;
            }
            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.Popup("Database", 0, new string[] { m_Database.name });

            EditorGUI.EndDisabledGroup();

            for (int i = 0; i < m_ImagesName.Length; i++)
            {
                m_ImagesName[i] = m_Database[i].Name;
            }

            m_SerializedObj.DatabaseIndex = EditorGUILayout.Popup("Image Target", m_SerializedObj.DatabaseIndex, m_ImagesName);
            if (m_SerializedObj.DatabaseIndex != m_PreSelectOption)
            {
                m_PreSelectOption = m_SerializedObj.DatabaseIndex;

                // todo logic
                m_SerializedObj.Width = m_Database[m_SerializedObj.DatabaseIndex].Width;
                m_SerializedObj.Height = m_Database[m_SerializedObj.DatabaseIndex].Width;//Height;

                UpdateProperties(m_SerializedObj);
            }

            if (base.serializedObject.isEditingMultipleObjects)
            {
                m_SerializedObj.SerializedObject.ApplyModifiedPropertiesWithoutUndo();
                Object[] targetObjs = m_SerializedObj.SerializedObject.targetObjects;
                for (int i = 0; i < targetObjs.Length; i++)
                {
                    UpdateAppearance(m_SerializedObj);
                }
            }
            else
            {
                UpdateAppearance(m_SerializedObj);
            }

            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.PropertyField(m_SerializedObj.WidthProperty, new GUIContent("Width"), new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(m_SerializedObj.HeightProperty, new GUIContent("Height"), new GUILayoutOption[0]);

            EditorGUI.EndDisabledGroup();

        }

        /// <summary> Editor database. </summary>
        /// <param name="itb">           The itb.</param>
        /// <param name="serializedObj"> The serialized object.</param>
        public void EditorDatabase(NRTrackableImageBehaviour itb, NRSerializedImageTarget serializedObj)
        {
            if (!NREditorSceneManager.Instance.SceneInitialized)
            {
                NREditorSceneManager.Instance.InitScene();
            }

            if (!EditorApplication.isPlaying)
            {
                CheckMesh(serializedObj);
            }
        }

        /// <summary> Updates the properties described by sit. </summary>
        /// <param name="sit"> The sit.</param>
        private void UpdateProperties(NRSerializedImageTarget sit)
        {
            NRTrackableImageBehaviour itb = ((NRTrackableImageBehaviour)target);

            itb.TrackableName = m_ImagesName[m_SerializedObj.DatabaseIndex];
            itb.DatabaseIndex = m_SerializedObj.DatabaseIndex;
        }

        /// <summary> Updates the appearance described by serializedImageTarget. </summary>
        /// <param name="serializedImageTarget"> The serialized image target.</param>
        private static void UpdateAppearance(NRSerializedImageTarget serializedImageTarget)
        {
            UpdateAspectRatio(serializedImageTarget);
            UpdateScale(serializedImageTarget);
            UpdateMaterial(serializedImageTarget);
        }

        /// <summary> Updates the aspect ratio described by it. </summary>
        /// <param name="it"> The iterator.</param>
        internal static void UpdateAspectRatio(NRSerializedImageTarget it)
        {
            Vector2 size = new Vector2(it.Width, it.Height);
            it.AspectRatio = size.y / size.x;
            using (List<NRTrackableImageBehaviour>.Enumerator enumerator = it.GetBehaviours().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    UpdateMesh(enumerator.Current.gameObject, it.AspectRatio);
                }
            }
        }

        /// <summary> Updates the scale described by it. </summary>
        /// <param name="it"> The iterator.</param>
        internal static void UpdateScale(NRSerializedImageTarget it)
        {
            Vector2 size = new Vector2(it.Width, it.Height);
            foreach (NRTrackableImageBehaviour item in it.GetBehaviours())
            {
                if (it.AspectRatio <= 1f)   // AspectRatio =>  y/x
                {
                    item.transform.localScale = new Vector3(size.x, size.x, size.x) * 0.001f;
                }
                else
                {
                    item.transform.localScale = new Vector3(size.y, size.y, size.y) * 0.001f;
                }
            }
        }

        /// <summary> Updates the material described by sit. </summary>
        /// <param name="sit"> The sit.</param>
        internal static void UpdateMaterial(NRSerializedImageTarget sit)
        {
            Material mat = sit.GetMaterial();
            Material loadMat = LoadMat();

            if (mat == null || mat == loadMat)
            {
                mat = new Material(loadMat);
            }
            string name = m_ImagesName[sit.DatabaseIndex];
            Texture2D texture = m_Database[sit.DatabaseIndex].Texture;
            mat.mainTexture = texture;
            mat.mainTextureScale = new Vector2(1f, 1f);
            mat.name = name + "Material";

            sit.SetMaterial(mat);
        }

        /// <summary> Updates the mesh. </summary>
        /// <param name="itObj">       The iterator object.</param>
        /// <param name="aspectRatio"> The aspect ratio.</param>
        internal static void UpdateMesh(GameObject itObj, float aspectRatio)
        {
            MeshFilter meshFilter = itObj.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = itObj.AddComponent<MeshFilter>();
            }
            Vector3 v1, v2, v3, v4;
            if (aspectRatio <= 1)
            {
                v1 = new Vector3(-0.5f, 0f, -aspectRatio * 0.5f);
                v2 = new Vector3(-0.5f, 0f, aspectRatio * 0.5f);
                v3 = new Vector3(0.5f, 0f, -aspectRatio * 0.5f);
                v4 = new Vector3(0.5f, 0f, aspectRatio * 0.5f);
            }
            else
            {
                float f = 1 / aspectRatio;
                v1 = new Vector3(-f * 0.5f, 0f, -0.5f);
                v2 = new Vector3(-f * 0.5f, 0f, 0.5f);
                v3 = new Vector3(f * 0.5f, 0f, -0.5f);
                v4 = new Vector3(f * 0.5f, 0f, 0.5f);
            }
            Mesh mesh = meshFilter.sharedMesh ?? new Mesh();
            mesh.vertices = new Vector3[] { v1, v2, v3, v4 };
            mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
            mesh.normals = new Vector3[mesh.vertices.Length];
            mesh.uv = new Vector2[] { new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f) };
            mesh.RecalculateNormals();
            meshFilter.sharedMesh = mesh;
            if (!itObj.GetComponent<MeshRenderer>())
            {
                itObj.AddComponent<MeshRenderer>();
            }

            NREditorSceneManager.Instance.UnloadUnusedAssets();
        }

        /// <summary> Check mesh. </summary>
        /// <param name="serializedImageTarget"> The serialized image target.</param>
        private void CheckMesh(NRSerializedImageTarget serializedImageTarget)
        {
            using (List<NRTrackableImageBehaviour>.Enumerator enumerator = serializedImageTarget.GetBehaviours().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    GameObject go = enumerator.Current.gameObject;
                    MeshFilter mf = go.GetComponent<MeshFilter>();
                    if ((mf == null || mf.sharedMesh == null) && serializedImageTarget.AspectRatioProperty.hasMultipleDifferentValues)
                    {
                        UpdateMesh(go, serializedImageTarget.AspectRatio);
                    }
                }
            }
            if (!serializedImageTarget.TrackableNameProperty.hasMultipleDifferentValues)
            {
                UpdateMaterial(serializedImageTarget);
            }
        }

        /// <summary> Loads the matrix. </summary>
        /// <returns> The matrix. </returns>
        private static Material LoadMat()
        {
            string text = "Assets/NRSDK/Emulator/Materials/DefaultTarget.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(text);
            if (mat == null)
            {
                NRDebugger.Error("Could not find reference material at " + text + " please reimport Unity package.");
            }
            return mat;
        }
    }
}