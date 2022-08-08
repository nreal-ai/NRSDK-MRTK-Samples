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
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class NRBackGroundRender : MonoBehaviour
    {
        /// <summary> A material used to render the AR background image. </summary>
        [Tooltip("A material used to render the AR background image.")]
        [SerializeField] Material m_Material;

        private Camera m_Camera;
        private MeshRenderer m_Renderer;
        private MeshFilter m_MeshFilter;
        private Mesh m_PlaneMesh;
        private Vector3[] m_Corners;

        private int[] Triangles = new int[6] {
            0,1,2,0,2,3
        };

        private Vector2[] UV = new Vector2[4] {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
        };

        private void OnEnable()
        {
            m_Camera = GetComponent<Camera>();
            EnableARBackgroundRendering();
        }

        private void OnDisable()
        {
            DisableARBackgroundRendering();
        }

        private void UpdateBackGroundMesh()
        {
            if (m_Corners == null)
            {
                m_Corners = new Vector3[4];
            }

            m_Camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), m_Camera.farClipPlane - 100, Camera.MonoOrStereoscopicEye.Mono, m_Corners);
            for (int i = 0; i < m_Corners.Length; i++)
            {
                m_Corners[i] = m_Camera.transform.TransformPoint(m_Corners[i]);
            }

            Vector3 center = (m_Corners[0] + m_Corners[2]) * 0.5f;
            DrawBackGroundMesh(new Pose(center, m_Camera.transform.rotation), m_Corners);
        }

        public void EnableARBackgroundRendering(bool updatemesh = true)
        {
            if (updatemesh)
            {
                UpdateBackGroundMesh();
            }
            m_Renderer.gameObject.SetActive(true);
        }

        public void DisableARBackgroundRendering()
        {
            if (m_Renderer != null)
            {
                m_Renderer.gameObject.SetActive(false);
            }
        }

        public void SetMaterial(Material mat)
        {
            m_Material = mat;

            if (m_Renderer != null)
            {
                m_Renderer.material = m_Material;
            }
        }

        /// <summary> Draw from center. </summary>
        /// <param name="centerPose"> The center pose.</param>
        /// <param name="vectors">    The vectors.</param>
        private void DrawBackGroundMesh(Pose centerPose, Vector3[] vectors)
        {
            if (vectors == null || vectors.Length < 3)
            {
                return;
            }

            if (m_PlaneMesh == null)
            {
                m_PlaneMesh = new Mesh();
            }

            if (m_Renderer == null)
            {
                var go = new GameObject("background");
                go.transform.SetParent(transform);
                m_Renderer = go.AddComponent<MeshRenderer>();
                m_MeshFilter = go.AddComponent<MeshFilter>();
            }

            m_Renderer.transform.position = centerPose.position;
            m_Renderer.transform.rotation = centerPose.rotation;

            Vector3[] vertices3D = new Vector3[vectors.Length];
            for (int i = 0; i < vectors.Length; i++)
            {
                vertices3D[i] = m_Renderer.transform.InverseTransformPoint(vectors[i]);
            }

            m_PlaneMesh.vertices = vertices3D;
            m_PlaneMesh.triangles = Triangles;
            m_PlaneMesh.uv = UV;

            m_MeshFilter.mesh = m_PlaneMesh;
            m_Renderer.material = m_Material;
        }
    }

    //[RequireComponent(typeof(Camera))]
    //public class NRBackGroundRender : MonoBehaviour
    //{
    //    /// <summary> A material used to render the AR background image. </summary>
    //    [Tooltip("A material used to render the AR background image.")]
    //    public Material BackgroundMaterial;
    //    private Camera m_Camera;
    //    private CameraClearFlags m_CameraClearFlags = CameraClearFlags.Skybox;
    //    private CommandBuffer m_CommandBuffer = null;

    //    private void OnEnable()
    //    {
    //        if (BackgroundMaterial == null)
    //        {
    //            NRDebugger.Error("[NRBackGroundRender] Material is null...");
    //            return;
    //        }
    //        m_Camera = GetComponent<Camera>();
    //        EnableARBackgroundRendering();
    //    }

    //    private void OnDisable()
    //    {
    //        DisableARBackgroundRendering();
    //    }

    //    public void SetMaterial(Material mat)
    //    {
    //        BackgroundMaterial = mat;
    //    }

    //    private void EnableARBackgroundRendering()
    //    {
    //        if (BackgroundMaterial == null || m_Camera == null)
    //        {
    //            return;
    //        }

    //        m_CameraClearFlags = m_Camera.clearFlags;
    //        m_Camera.clearFlags = CameraClearFlags.Depth;

    //        m_CommandBuffer = new CommandBuffer();
    //        m_CommandBuffer.Blit(null, BuiltinRenderTextureType.CameraTarget, BackgroundMaterial);

    //        m_Camera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_CommandBuffer);
    //        m_Camera.AddCommandBuffer(CameraEvent.BeforeGBuffer, m_CommandBuffer);
    //    }

    //    private void DisableARBackgroundRendering()
    //    {
    //        if (m_CommandBuffer == null || m_Camera == null)
    //        {
    //            return;
    //        }

    //        m_Camera.clearFlags = m_CameraClearFlags;

    //        m_Camera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, m_CommandBuffer);
    //        m_Camera.RemoveCommandBuffer(CameraEvent.BeforeGBuffer, m_CommandBuffer);
    //    }
    //}
}
