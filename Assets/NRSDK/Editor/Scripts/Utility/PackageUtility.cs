/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Release
{
    using LitJson;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.PackageManager;
    using UnityEditor.PackageManager.Requests;
    using UnityEngine;

    public enum TaskType
    {
        Add,
        Remove,
        Get
    }

    public struct TaskInfo
    {
        public TaskType taskType;
        public string param;
        public OnResponse onResponse;
    }

    public struct Result
    {
        public bool isSuccess;
        public List<UnityEditor.PackageManager.PackageInfo> packages;
    }

    public delegate void OnResponse(Result result);

    public static class PackageUtility
    {
        public static Queue<TaskInfo> taskQueue = new Queue<TaskInfo>();
        public static Dictionary<Request, TaskInfo> taskDict = new Dictionary<Request, TaskInfo>();
        private static Request currentRequest = null;

        static PackageUtility()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        public static void Update()
        {
            if (currentRequest != null)
            {
                TaskInfo info;
                taskDict.TryGetValue(currentRequest, out info);
                if (currentRequest.IsCompleted)
                {
                    Result result = new Result();
                    result.isSuccess = currentRequest.Status == StatusCode.Success;

                    if (info.taskType == TaskType.Get)
                    {
                        if (currentRequest.Status == StatusCode.Success)
                        {
                            result.packages = new List<UnityEditor.PackageManager.PackageInfo>();
                            foreach (var package in ((ListRequest)currentRequest).Result)
                            {
                                result.packages.Add(package);
                            }
                        }
                    }
                    info.onResponse?.Invoke(result);
                    taskDict.Remove(currentRequest);
                    currentRequest = null;
                }
                else
                {
                    //Debug.LogFormat("[{0}]:{1}", info.taskType, info.param);
                    return;
                }
            }

            if (taskQueue.Count != 0)
            {
                TaskInfo task = taskQueue.Dequeue();
                switch (task.taskType)
                {
                    case TaskType.Add:
                        currentRequest = Client.Add(task.param);
                        break;
                    case TaskType.Remove:
                        currentRequest = Client.Remove(task.param);
                        break;
                    case TaskType.Get:
                        currentRequest = Client.List();
                        break;
                    default:
                        break;
                }
                taskDict.Add(currentRequest, task);
            }
        }

        public static void Add(string package, OnResponse callback)
        {
            lock (taskQueue)
            {
                taskQueue.Enqueue(new TaskInfo() { taskType = TaskType.Add, onResponse = callback, param = package });
            }
        }

        public static void Remove(string package, OnResponse callback)
        {
            lock (taskQueue)
            {
                taskQueue.Enqueue(new TaskInfo() { taskType = TaskType.Remove, onResponse = callback, param = package });
            }
        }

        public static void GetAllPackages(OnResponse callback)
        {
            lock (taskQueue)
            {
                taskQueue.Enqueue(new TaskInfo() { taskType = TaskType.Get, onResponse = callback });
            }
        }

        public static Dictionary<string, string> GetAllPackagesByManifest()
        {
            string path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Packages/manifest.json");
            var json = JsonMapper.ToObject(File.ReadAllText(path));
            var packages = json["dependencies"];
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var item in packages.Keys)
            {
                dict.Add(item, packages[item].ToString());
            }
            return dict;
        }
    }
}
