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
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using UnityEngine;

    /// <summary> A main thread dispather. </summary>
    [ExecuteInEditMode]
    public class MainThreadDispather : MonoBehaviour
    {
        /// <summary> A delayed queue item. </summary>
        public class DelayedQueueItem
        {
            /// <summary> The time. </summary>
            public float time;

            /// <summary> The action. </summary>
            public Action action;
        }

        /// <summary> The current. </summary>
        private static MainThreadDispather m_Current;

        /// <summary> Number of. </summary>
        private int m_Count;

        /// <summary> True once initialization is complete. </summary>
        private static bool m_Initialized;

        /// <summary> Identifier for the thread. </summary>
        private static int m_ThreadId = -1;

        /// <summary> The actions. </summary>
        private List<Action> m_Actions = new List<Action>();

        /// <summary> The delayed. </summary>
        private List<MainThreadDispather.DelayedQueueItem> m_Delayed = new List<MainThreadDispather.DelayedQueueItem>();

        /// <summary> Gets the current. </summary>
        /// <value> The current. </value>
        public static MainThreadDispather Current
        {
            get
            {
                if (!MainThreadDispather.m_Initialized)
                {
                    MainThreadDispather.Initialize();
                }
                return MainThreadDispather.m_Current;
            }
        }

        /// <summary> Initializes this object. </summary>
        public static void Initialize()
        {
            bool flag = !MainThreadDispather.m_Initialized;
            if (flag && MainThreadDispather.m_ThreadId != -1 && MainThreadDispather.m_ThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                return;
            }
            if (flag)
            {
                GameObject gameObject = new GameObject("MainThreadDispather");
                gameObject.hideFlags = HideFlags.DontSave;
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                if (MainThreadDispather.m_Current)
                {
                    if (Application.isPlaying)
                    {
                        UnityEngine.Object.Destroy(MainThreadDispather.m_Current.gameObject);
                    }
                    else
                    {
                        UnityEngine.Object.DestroyImmediate(MainThreadDispather.m_Current.gameObject);
                    }
                }
                MainThreadDispather.m_Current = gameObject.AddComponent<MainThreadDispather>();
                UnityEngine.Object.DontDestroyOnLoad(MainThreadDispather.m_Current);
                MainThreadDispather.m_Initialized = true;
                MainThreadDispather.m_ThreadId = Thread.CurrentThread.ManagedThreadId;
            }
        }

        /// <summary> Executes the 'destroy' action. </summary>
        private void OnDestroy()
        {
            MainThreadDispather.m_Initialized = false;
        }

        /// <summary> Queue on main thread. </summary>
        /// <param name="action"> The action.</param>
        public static void QueueOnMainThread(Action action)
        {
            MainThreadDispather.QueueOnMainThread(action, 0f);
        }

        /// <summary> Queue on main thread. </summary>
        /// <param name="action"> The action.</param>
        /// <param name="time">   The time.</param>
        public static void QueueOnMainThread(Action action, float time)
        {
            if (time != 0f)
            {
                List<MainThreadDispather.DelayedQueueItem> delayed = MainThreadDispather.Current.m_Delayed;
                lock (delayed)
                {
                    MainThreadDispather.Current.m_Delayed.Add(new MainThreadDispather.DelayedQueueItem
                    {
                        time = Time.time + time,
                        action = action
                    });
                }
            }
            else
            {
                List<Action> actions = MainThreadDispather.Current.m_Actions;
                lock (actions)
                {
                    MainThreadDispather.Current.m_Actions.Add(action);
                }
            }
        }

        /// <summary> Executes the 'asynchronous' operation. </summary>
        /// <param name="action"> The action.</param>
        public static void RunAsync(Action action)
        {
            new Thread(new ParameterizedThreadStart(MainThreadDispather.RunAction))
            {
                Priority = System.Threading.ThreadPriority.Normal
            }.Start(action);
        }

        /// <summary> Executes the action. </summary>
        /// <param name="action"> The action.</param>
        private static void RunAction(object action)
        {
            ((Action)action)?.Invoke();
        }

        /// <summary> Updates this object. </summary>
        private void Update()
        {
            List<Action> actions = this.m_Actions;
            if (actions.Count > 0)
            {
                lock (actions)
                {
                    for (int i = 0; i < this.m_Actions.Count; i++)
                    {
                        this.m_Actions[i]();
                    }
                    this.m_Actions.Clear();
                }
            }

            List<MainThreadDispather.DelayedQueueItem> delayed = this.m_Delayed;
            if (delayed.Count > 0)
            {
                lock (delayed)
                {
                    for (int j = 0; j < this.m_Delayed.Count; j++)
                    {
                        MainThreadDispather.DelayedQueueItem delayedQueueItem = this.m_Delayed[j];
                        if (delayedQueueItem.time <= Time.time)
                        {
                            delayedQueueItem.action();
                            this.m_Delayed.RemoveAt(j);
                            j--;
                        }
                    }
                }
            }
        }
    }
}
