/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Experimental
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// When attached to a GameObject with an NROverlay component, NROverlayMeshGenerator will use a mesh renderer
    /// to preview the appearance of the NROverlay as it would appear as a TimeWarp overlay on a headset.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class NROverlayMeshGenerator : MonoBehaviour
    {
        private Mesh m_Mesh;
        private List<Vector3> m_Verts = new List<Vector3>();
        private List<Vector2> m_UV = new List<Vector2>();
        private List<int> m_Tris = new List<int>();
        private NROverlay m_Overlay;
        private MeshFilter m_MeshFilter;
        private MeshCollider m_MeshCollider;
        private MeshRenderer m_MeshRenderer;
        private Transform m_Transform;

        private NROverlay.OverlayShape m_LastShape;
        private Vector3 m_LastPosition;
        private Quaternion m_LastRotation;
        private Vector3 m_LastScale;

        private bool m_Awake = false;

        private Material previewMat;
        private Material editorPreviewMat;

        protected void Awake()
        {
            //gameObject.hideFlags = HideFlags.HideInHierarchy;
            m_MeshFilter = GetComponent<MeshFilter>();
            m_MeshCollider = GetComponent<MeshCollider>();
            m_MeshRenderer = GetComponent<MeshRenderer>();

            m_Transform = transform;
            m_Awake = true;
        }

        public void SetOverlay(NROverlay overlay)
        {
            m_Overlay = overlay;
        }

        private Rect GetBoundingRect(Rect a, Rect b)
        {
            float xMin = Mathf.Min(a.x, b.x);
            float xMax = Mathf.Max(a.x + a.width, b.x + b.width);
            float yMin = Mathf.Min(a.y, b.y);
            float yMax = Mathf.Max(a.y + a.height, b.y + b.height);

            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        private Rect GetBoundingRect()
        {
            Rect a, b;
            a = b = new Rect(0, 0, 1, 1);
            float xMin = Mathf.Min(a.x, b.x);
            float xMax = Mathf.Max(a.x + a.width, b.x + b.width);
            float yMin = Mathf.Min(a.y, b.y);
            float yMax = Mathf.Max(a.y + a.height, b.y + b.height);

            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        protected void OnEnable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += Update;
#endif
        }

        protected void OnDisable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= Update;
#endif
        }

        private void Update()
        {
            if (!m_Awake)
            {
                Awake();
            }

            if (m_Overlay)
            {
                NROverlay.OverlayShape shape = m_Overlay.overlayShape;
                Vector3 position = m_Transform.position;
                Quaternion rotation = m_Transform.rotation;
                Vector3 scale = m_Transform.lossyScale;
                //Rect destRectLeft = _Overlay.overrideTextureRectMatrix ? _Overlay.destRectLeft : new Rect(0, 0, 1, 1);
                //Rect destRectRight = _Overlay.overrideTextureRectMatrix ? _Overlay.destRectRight : new Rect(0, 0, 1, 1);
                //Rect srcRectLeft = _Overlay.overrideTextureRectMatrix ? _Overlay.srcRectLeft : new Rect(0, 0, 1, 1);
                Texture texture = m_Overlay.MainTexture;

                // Re-generate the mesh if necessary
                if (m_Mesh == null ||
                    m_LastShape != shape ||
                    m_LastPosition != position ||
                    m_LastRotation != rotation ||
                    m_LastScale != scale
                    )
                {
                    UpdateMesh(shape, position, rotation, scale, GetBoundingRect());
                    m_LastShape = shape;
                    m_LastPosition = position;
                    m_LastRotation = rotation;
                    m_LastScale = scale;
                }

                // Generate the material and update textures if necessary
#if UNITY_EDITOR
                if (m_Overlay.previewInEditor)
                {
                    if (editorPreviewMat == null)
                    {
                        editorPreviewMat = new Material(Resources.Load<Shader>("Overlay/Materials/OverlayPreview"));
                    }
                    m_MeshRenderer.sharedMaterial = editorPreviewMat;
                    m_MeshRenderer.sharedMaterial.mainTexture = m_Overlay.MainTexture;
                }
                else
#endif
                {
                    if (previewMat == null)
                    {
                        previewMat = new Material(Resources.Load<Shader>("Overlay/Materials/OverlayMask"));
                    }
                    m_MeshRenderer.sharedMaterial = previewMat;
                }
            }
        }

        private void UpdateMesh(NROverlay.OverlayShape shape, Vector3 position, Quaternion rotation, Vector3 scale, Rect rect)
        {
            if (m_MeshFilter)
            {
                if (m_Mesh == null)
                {
                    m_Mesh = new Mesh() { name = "Overlay" };
                    m_Mesh.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
                }
                m_Mesh.Clear();
                m_Verts.Clear();
                m_UV.Clear();
                m_Tris.Clear();

                GenerateMesh(m_Verts, m_UV, m_Tris, shape, position, rotation, scale, rect);

                m_Mesh.SetVertices(m_Verts);
                m_Mesh.SetUVs(0, m_UV);
                m_Mesh.SetTriangles(m_Tris, 0);
                m_Mesh.UploadMeshData(false);

                m_MeshFilter.sharedMesh = m_Mesh;

                if (m_MeshCollider)
                {
                    m_MeshCollider.sharedMesh = m_Mesh;
                }
            }
        }

        public static void GenerateMesh(List<Vector3> verts, List<Vector2> uvs, List<int> tris, NROverlay.OverlayShape shape, Vector3 position, Quaternion rotation, Vector3 scale, Rect rect)
        {
            switch (shape)
            {
                //case NROverlay.OverlayShape.Equirect:
                //    BuildSphere(verts, uvs, tris, position, rotation, scale, rect);
                //    break;
                //case NROverlay.OverlayShape.Cubemap:
                //case NROverlay.OverlayShape.OffcenterCubemap:
                //    BuildCube(verts, uvs, tris, position, rotation, scale);
                //    break;
                case NROverlay.OverlayShape.Quad:
                    BuildQuad(verts, uvs, tris, rect);
                    break;
                    //case NROverlay.OverlayShape.Cylinder:
                    //    BuildHemicylinder(verts, uvs, tris, scale, rect);
                    //    break;
            }
        }

        private static Vector2 GetSphereUV(float theta, float phi, float expand_coef)
        {
            float thetaU = ((theta / (2 * Mathf.PI) - 0.5f) / expand_coef) + 0.5f;
            float phiV = ((phi / Mathf.PI) / expand_coef) + 0.5f;
            return new Vector2(thetaU, phiV);
        }

        private static Vector3 GetSphereVert(float theta, float phi)
        {
            return new Vector3(-Mathf.Sin(theta) * Mathf.Cos(phi), Mathf.Sin(phi), -Mathf.Cos(theta) * Mathf.Cos(phi));
        }

        public static void BuildSphere(List<Vector3> verts, List<Vector2> uv, List<int> triangles, Vector3 position, Quaternion rotation, Vector3 scale, Rect rect, float worldScale = 800, int latitudes = 128, int longitudes = 128, float expand_coef = 1.0f)
        {
            position = Quaternion.Inverse(rotation) * position;

            latitudes = Mathf.CeilToInt(latitudes * rect.height);
            longitudes = Mathf.CeilToInt(longitudes * rect.width);

            float minTheta = Mathf.PI * 2 * (rect.x);
            float minPhi = Mathf.PI * (0.5f - rect.y - rect.height);

            float thetaScale = Mathf.PI * 2 * rect.width / longitudes;
            float phiScale = Mathf.PI * rect.height / latitudes;

            for (int j = 0; j < latitudes + 1; j += 1)
            {
                for (int k = 0; k < longitudes + 1; k++)
                {
                    float theta = minTheta + k * thetaScale;
                    float phi = minPhi + j * phiScale;

                    Vector2 suv = GetSphereUV(theta, phi, expand_coef);
                    uv.Add(new Vector2((suv.x - rect.x) / rect.width, (suv.y - rect.y) / rect.height));
                    Vector3 vert = GetSphereVert(theta, phi);
                    vert.x = (worldScale * vert.x - position.x) / scale.x;
                    vert.y = (worldScale * vert.y - position.y) / scale.y;
                    vert.z = (worldScale * vert.z - position.z) / scale.z;
                    verts.Add(vert);
                }
            }

            for (int j = 0; j < latitudes; j++)
            {
                for (int k = 0; k < longitudes; k++)
                {
                    triangles.Add((j * (longitudes + 1)) + k);
                    triangles.Add(((j + 1) * (longitudes + 1)) + k);
                    triangles.Add(((j + 1) * (longitudes + 1)) + k + 1);
                    triangles.Add(((j + 1) * (longitudes + 1)) + k + 1);
                    triangles.Add((j * (longitudes + 1)) + k + 1);
                    triangles.Add((j * (longitudes + 1)) + k);
                }
            }
        }

        private enum CubeFace
        {
            Right,
            Left,
            Top,
            Bottom,
            Front,
            Back,
            COUNT
        }

        private static readonly Vector3[] BottomLeft = new Vector3[]{
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f)
        };

        private static readonly Vector3[] RightVector = new Vector3[]{
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.left,
            Vector3.left,
            Vector3.right
        };

        private static readonly Vector3[] UpVector = new Vector3[]{
            Vector3.up,
            Vector3.up,
            Vector3.forward,
            Vector3.back,
            Vector3.up,
            Vector3.up
        };

        private static Vector2 GetCubeUV(CubeFace face, Vector2 sideUV, float expand_coef)
        {
            sideUV = (sideUV - 0.5f * Vector2.one) / expand_coef + 0.5f * Vector2.one;
            switch (face)
            {
                case CubeFace.Bottom:
                    return new Vector2(sideUV.x / 3, sideUV.y / 2);
                case CubeFace.Front:
                    return new Vector2((1 + sideUV.x) / 3, sideUV.y / 2);
                case CubeFace.Back:
                    return new Vector2((2 + sideUV.x) / 3, sideUV.y / 2);
                case CubeFace.Right:
                    return new Vector2(sideUV.x / 3, (1 + sideUV.y) / 2);
                case CubeFace.Left:
                    return new Vector2((1 + sideUV.x) / 3, (1 + sideUV.y) / 2);
                case CubeFace.Top:
                    return new Vector2((2 + sideUV.x) / 3, (1 + sideUV.y) / 2);
                default:
                    return Vector2.zero;
            }
        }

        private static Vector3 GetCubeVert(CubeFace face, Vector2 sideUV, float expand_coef)
        {
            return BottomLeft[(int)face] + sideUV.x * RightVector[(int)face] + sideUV.y * UpVector[(int)face];
        }

        public static void BuildCube(List<Vector3> verts, List<Vector2> uv, List<int> triangles, Vector3 position, Quaternion rotation, Vector3 scale, float worldScale = 800, int subQuads = 1, float expand_coef = 1.01f)
        {
            position = Quaternion.Inverse(rotation) * position;

            int vertsPerSide = (subQuads + 1) * (subQuads + 1);

            for (int i = 0; i < (int)CubeFace.COUNT; i++)
            {
                for (int j = 0; j < subQuads + 1; j++)
                {
                    for (int k = 0; k < subQuads + 1; k++)
                    {
                        float u = j / (float)subQuads;
                        float v = k / (float)subQuads;

                        uv.Add(GetCubeUV((CubeFace)i, new Vector2(u, v), expand_coef));
                        Vector3 vert = GetCubeVert((CubeFace)i, new Vector2(u, v), expand_coef);
                        vert.x = (worldScale * vert.x - position.x) / scale.x;
                        vert.y = (worldScale * vert.y - position.y) / scale.y;
                        vert.z = (worldScale * vert.z - position.z) / scale.z;
                        verts.Add(vert);
                    }
                }

                for (int j = 0; j < subQuads; j++)
                {
                    for (int k = 0; k < subQuads; k++)
                    {
                        triangles.Add(vertsPerSide * i + ((j + 1) * (subQuads + 1)) + k);
                        triangles.Add(vertsPerSide * i + (j * (subQuads + 1)) + k);
                        triangles.Add(vertsPerSide * i + ((j + 1) * (subQuads + 1)) + k + 1);
                        triangles.Add(vertsPerSide * i + ((j + 1) * (subQuads + 1)) + k + 1);
                        triangles.Add(vertsPerSide * i + (j * (subQuads + 1)) + k);
                        triangles.Add(vertsPerSide * i + (j * (subQuads + 1)) + k + 1);
                    }
                }
            }
        }

        public static void BuildQuad(List<Vector3> verts, List<Vector2> uv, List<int> triangles, Rect rect)
        {
            verts.Add(new Vector3(rect.x - 0.5f, (1 - rect.y - rect.height) - 0.5f, 0));
            verts.Add(new Vector3(rect.x - 0.5f, (1 - rect.y) - 0.5f, 0));
            verts.Add(new Vector3(rect.x + rect.width - 0.5f, (1 - rect.y) - 0.5f, 0));
            verts.Add(new Vector3(rect.x + rect.width - 0.5f, (1 - rect.y - rect.height) - 0.5f, 0));

            uv.Add(new Vector2(0, 0));
            uv.Add(new Vector2(0, 1));
            uv.Add(new Vector2(1, 1));
            uv.Add(new Vector2(1, 0));

            triangles.Add(0);
            triangles.Add(1);
            triangles.Add(2);
            triangles.Add(2);
            triangles.Add(3);
            triangles.Add(0);
        }

        public static void BuildHemicylinder(List<Vector3> verts, List<Vector2> uv, List<int> triangles, Vector3 scale, Rect rect, int longitudes = 128)
        {
            float height = Mathf.Abs(scale.y) * rect.height;
            float radius = scale.z;
            float arcLength = scale.x * rect.width;

            float arcAngle = arcLength / radius;
            float minAngle = scale.x * (-0.5f + rect.x) / radius;

            int columns = Mathf.CeilToInt(longitudes * arcAngle / (2 * Mathf.PI));

            // we don't want super tall skinny triangles because that can lead to artifacting.
            // make triangles no more than 2x taller than wide

            float triangleWidth = arcLength / columns;
            float ratio = height / triangleWidth;

            int rows = Mathf.CeilToInt(ratio / 2);

            for (int j = 0; j < rows + 1; j += 1)
            {
                for (int k = 0; k < columns + 1; k++)
                {
                    uv.Add(new Vector2((k / (float)columns), 1 - (j / (float)rows)));

                    Vector3 vert = Vector3.zero;
                    // because the scale is used to control the parameters, we need
                    // to reverse multiply by scale to appear correctly
                    vert.x = (Mathf.Sin(minAngle + (k * arcAngle / columns)) * radius) / scale.x;

                    vert.y = (0.5f - rect.y - rect.height + rect.height * (1 - j / (float)rows));
                    vert.z = (Mathf.Cos(minAngle + (k * arcAngle / columns)) * radius) / scale.z;
                    verts.Add(vert);
                }
            }

            for (int j = 0; j < rows; j++)
            {
                for (int k = 0; k < columns; k++)
                {
                    triangles.Add((j * (columns + 1)) + k);
                    triangles.Add(((j + 1) * (columns + 1)) + k + 1);
                    triangles.Add(((j + 1) * (columns + 1)) + k);
                    triangles.Add(((j + 1) * (columns + 1)) + k + 1);
                    triangles.Add((j * (columns + 1)) + k);
                    triangles.Add((j * (columns + 1)) + k + 1);
                }
            }
        }
    }
}