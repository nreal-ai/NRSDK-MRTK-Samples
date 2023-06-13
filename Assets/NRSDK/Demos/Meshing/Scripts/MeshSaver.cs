/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace NRKernal.NRExamples
{
    /// <summary> A mesh info processor to save unity mesh to obj file. </summary>
    public class MeshSaver : MonoBehaviour, IMeshInfoProcessor
    {
        protected string SavePath
        {
            get
            {
                string folder;
#if UNITY_EDITOR
                folder = Directory.GetCurrentDirectory();
#else
                folder = Application.persistentDataPath;
#endif
                return Path.Combine(folder, "MeshSave");
            }
        }

        Dictionary<ulong, Mesh> m_MeshDict = new Dictionary<ulong, Mesh>();
        int m_SubFolderIndex = 0;
        Thread m_SaveThread;

        void Awake()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(SavePath);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            else
            {
                while (m_SubFolderIndex < int.MaxValue)
                {
                    string subFolder = Path.Combine(SavePath, m_SubFolderIndex.ToString());
                    directoryInfo = new DirectoryInfo(subFolder);
                    if (directoryInfo.Exists)
                        m_SubFolderIndex++;
                    else
                        break;
                }
            }
        }

        public void Save()
        {
            if (m_SaveThread == null)
            {
                m_SaveThread = new Thread(SaveMeshThread);
                m_SaveThread.Start();
            }
        }

        void IMeshInfoProcessor.UpdateMeshInfo(ulong identifier, NRMeshingBlockState meshingBlockState, Mesh mesh)
        {
            NRDebugger.Debug("[MeshSaver] meshingBlockState: {0} identifier: {1}", meshingBlockState, identifier);
            lock (m_MeshDict)
            {
                m_MeshDict[identifier] = mesh;
            }
        }

        void SaveMeshThread()
        {
            Dictionary<ulong, Mesh> meshDictCopy;
            lock (m_MeshDict)
            {
                meshDictCopy = new Dictionary<ulong, Mesh>(m_MeshDict);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(SavePath, m_SubFolderIndex.ToString()));
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            foreach (var item in meshDictCopy)
            {
                FileInfo fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, item.Key.ToString() + ".obj"));
                StreamWriter sw = new StreamWriter(fileInfo.FullName);
                string str = MeshToString(item.Value);
                sw.Write(str);
                sw.Flush();
                sw.Close();
            }
            m_SaveThread = null;
            m_SubFolderIndex++;
        }

        void IMeshInfoProcessor.ClearMeshInfo()
        {
            NRDebugger.Debug("[MeshSaver] ClearMeshInfo.");
            DirectoryInfo directoryInfo = new DirectoryInfo(SavePath);
            if (directoryInfo.Exists)
            {
                foreach (var file in directoryInfo.EnumerateFiles("*.obj"))
                {
                    file.Delete();
                }
            }
        }

        private static string MeshToString(Mesh mesh)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("# \n");

            Vector3[] vertices = mesh.vertices;
            foreach (Vector3 v in vertices)
            {
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");

            Vector3[] normals = mesh.normals;
            foreach (Vector3 vn in normals)
            {
                sb.Append(string.Format("vn {0} {1} {2}\n", vn.x, vn.y, vn.z));
            }
            sb.Append("\n");

            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
            }
            sb.Append("\n");
            return sb.ToString();
        }

        public static Mesh StringToMesh(string data)
        {
            var lines = data.Split('\n');
            List<Vector3> verticeList = new List<Vector3>();
            List<Vector3> normalList = new List<Vector3>();
            List<int> triangleList = new List<int>();

            foreach (var line in lines)
            {
                var nums = line.Split(' ', '/');
                switch (nums[0])
                {
                    case "v":
                        verticeList.Add(new Vector3(float.Parse(nums[1]), float.Parse(nums[2]), float.Parse(nums[3])));
                        break;
                    case "vn":
                        normalList.Add(new Vector3(float.Parse(nums[1]), float.Parse(nums[2]), float.Parse(nums[3])));
                        break;
                    case "f":
                        triangleList.Add(int.Parse(nums[1]) - 1);
                        triangleList.Add(int.Parse(nums[4]) - 1);
                        triangleList.Add(int.Parse(nums[7]) - 1);
                        break;
                    default:
                        break;
                }
            }

            return new Mesh
            {
                vertices = verticeList.ToArray(),
                normals = normalList.ToArray(),
                triangles = triangleList.ToArray()
            };
        }
    }
}
