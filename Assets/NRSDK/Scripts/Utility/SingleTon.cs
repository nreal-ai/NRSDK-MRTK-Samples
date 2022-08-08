/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

using UnityEngine;

namespace NRKernal
{
    /// <summary> A single ton. </summary>
    /// <typeparam name="T"> Generic type parameter.</typeparam>
    public class SingleTon<T> where T : new()
    {
        private static T instane = default(T);
        public static void CreateInstance()
        {
            if(instane == null)
            {
                instane = new T();
            }
        }

        /// <summary> Gets the instance. </summary>
        /// <value> The instance. </value>
        public static T Instance
        {
            get
            {
                if(instane == null)
                {
                    CreateInstance();
                }
                return instane;
            }
        }
    }

    /// <summary> A singleton behaviour. </summary>
    /// <typeparam name="T"> Generic type parameter.</typeparam>
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        /// <summary> The instance. </summary>
        private static T instance;

        /// <summary>
        /// Returns the Singleton instance of the classes type. If no instance is found, then we search
        /// for an instance in the scene. If more than one instance is found, we throw an error and no
        /// instance is returned. </summary>
        /// <value> The instance. </value>
        protected static T Instance
        {
            get
            {
                if (instance == null && searchForInstance)
                {
                    searchForInstance = false;
                    T[] objects = FindObjectsOfType<T>();
                    if (objects.Length == 1)
                    {
                        instance = objects[0];
                    }
                    else if (objects.Length > 1)
                    {
                        Debug.LogErrorFormat("Expected exactly 1 {0} but found {1}.", typeof(T).ToString(), objects.Length);
                    }
                }
                return instance;
            }
        }

        /// <summary> True to search for instance. </summary>
        private static bool searchForInstance = true;

        /// <summary> Assert is initialized. </summary>
        public static void AssertIsInitialized()
        {
            Debug.Assert(IsInitialized, string.Format("The {0} singleton has not been initialized.", typeof(T).Name));
        }

        /// <summary> Returns whether the instance has been initialized or not. </summary>
        /// <value> True if this object is initialized, false if not. </value>
        public static bool IsInitialized
        {
            get
            {
                return instance != null;
            }
        }

        /// <summary> True if is dirty, false if not. </summary>
        protected bool isDirty = false;

        /// <summary>
        /// Base Awake method that sets the Singleton's unique instance. Called by Unity when
        /// initializing a MonoBehaviour. Scripts that extend Singleton should be sure to call
        /// base.Awake() to ensure the static Instance reference is properly created. </summary>
        protected virtual void Awake()
        {
            if (IsInitialized && instance != this)
            {
                isDirty = true;
                DestroyImmediate(gameObject);
                NRDebugger.Info("Trying to instantiate a second instance of singleton class {0}. Additional Instance was destroyed", GetType().Name);
            }
            else if (!IsInitialized)
            {
                instance = (T)this;
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// Base OnDestroy method that destroys the Singleton's unique instance. Called by Unity when
        /// destroying a MonoBehaviour. Scripts that extend Singleton should be sure to call
        /// base.OnDestroy() to ensure the underlying static Instance reference is properly cleaned up. </summary>
        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                searchForInstance = true;
            }
        }
    }
}
