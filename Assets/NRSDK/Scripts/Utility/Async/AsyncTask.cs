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
    using UnityEngine;

    /// <summary> A class used for monitoring the status of an asynchronous task. </summary>
    /// <typeparam name="T"> The resultant type of the task.</typeparam>
    public class AsyncTask<T>
    {
        /// <summary>
        /// A collection of actons to perform on the main Unity thread after the task is complete. </summary>
        private List<Action<T>> m_ActionsUponTaskCompletion;

        /// <summary> Constructor for AsyncTask. </summary>
        /// <param name="asyncOperationComplete"> [out] A callback that, when invoked, changes the status
        ///                                       of the task to complete and sets the result based on
        ///                                       the argument supplied.</param>
        public AsyncTask(out Action<T> asyncOperationComplete)
        {
            // Register for early update event.
            if (!AsyncTask.IsInitialized)
            {
                AsyncTask.Init();
            }

            IsComplete = false;
            asyncOperationComplete = delegate (T result)
            {
                Result = result;
                IsComplete = true;
                if (m_ActionsUponTaskCompletion != null)
                {
                    AsyncTask.PerformActionInUpdate(() =>
                    {
                        for (int i = 0; i < m_ActionsUponTaskCompletion.Count; i++)
                        {
                            m_ActionsUponTaskCompletion[i](result);
                        }
                    });
                }
            };
        }

        /// <summary> Constructor for AsyncTask that creates a completed task. </summary>
        /// <param name="result"> The result of the completed task.</param>
        internal AsyncTask(T result)
        {
            Result = result;
            IsComplete = true;
        }

        /// <summary> Gets or sets a value indicating whether the task is complete. </summary>
        /// <value> <c>true</c> if the task is complete, otherwise <c>false</c>. </value>
        public bool IsComplete { get; private set; }

        /// <summary> Gets or sets the result of a completed task. </summary>
        /// <value> The result of the completed task. </value>
        public T Result { get; private set; }

        /// <summary>
        /// Returns a yield instruction that monitors this task for completion within a coroutine. </summary>
        /// <returns> A yield instruction that monitors this task for completion. </returns>
        public CustomYieldInstruction WaitForCompletion()
        {
            return new WaitForTaskCompletionYieldInstruction<T>(this);
        }

        /// <summary>
        /// Performs an action (callback) in the first Unity Update() call after task completion. </summary>
        /// <param name="doAfterTaskComplete"> The action to invoke when task is complete.  The result
        ///                                    of the task will be passed as an argument to the action.</param>
        /// <returns> The invoking asynchronous task. </returns>
        public AsyncTask<T> ThenAction(Action<T> doAfterTaskComplete)
        {
            // Perform action now if task is already complete.
            if (IsComplete)
            {
                doAfterTaskComplete(Result);
                return this;
            }

            // Allocate list if needed (avoids allocation if then is not used).
            if (m_ActionsUponTaskCompletion == null)
            {
                m_ActionsUponTaskCompletion = new List<Action<T>>();
            }

            m_ActionsUponTaskCompletion.Add(doAfterTaskComplete);
            return this;
        }
    }

    /// <summary> Helper methods for dealing with asynchronous tasks. </summary>
    internal class AsyncTask
    {
        /// <summary> Queue of update actions. </summary>
        private static Queue<Action> m_UpdateActionQueue = new Queue<Action>();
        /// <summary> The lock object. </summary>
        private static object m_LockObject = new object();

        /// <summary> Gets or sets a value indicating whether this object is initialized. </summary>
        /// <value> True if this object is initialized, false if not. </value>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// Queues an action to be performed on Unity thread in Update().  This method can be called by
        /// any thread. </summary>
        /// <param name="action"> The action to perform.</param>
        public static void PerformActionInUpdate(Action action)
        {
            lock (m_LockObject)
            {
                m_UpdateActionQueue.Enqueue(action);
            }
        }

        /// <summary> An Update handler called each frame. </summary>
        public static void OnUpdate()
        {
            if (m_UpdateActionQueue.Count == 0)
            {
                return;
            }

            lock (m_LockObject)
            {
                while (m_UpdateActionQueue.Count > 0)
                {
                    Action action = m_UpdateActionQueue.Dequeue();
                    action();
                }
            }
        }

        /// <summary> Initializes this object. </summary>
        public static void Init()
        {
            if (IsInitialized)
            {
                return;
            }

            NRKernalUpdater.OnUpdate += OnUpdate;
            IsInitialized = true;
        }
    }
}
