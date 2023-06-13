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
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    
    /// <summary> A background job executor. </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
     Justification = "Internal")]
    public class BackgroundJobExecutor
    {
        /// <summary> The event. </summary>
        private AutoResetEvent m_Event = new AutoResetEvent(false);
        /// <summary> Queue of jobs. </summary>
        private Queue<Action> m_JobsQueue = new Queue<Action>();
        /// <summary> The thread. </summary>
        private Thread m_Thread;
        /// <summary> True to running. </summary>
        private bool m_Running = false;

        /// <summary> Default constructor. </summary>
        public BackgroundJobExecutor()
        {
            m_Thread = new Thread(Run);
            m_Thread.Start();
        }

        /// <summary> Gets the number of pending jobs. </summary>
        /// <value> The number of pending jobs. </value>
        public int PendingJobsCount
        {
            get
            {
                lock (m_JobsQueue)
                {
                    return m_JobsQueue.Count + (m_Running ? 1 : 0);
                }
            }
        }

        /// <summary> Pushes a job. </summary>
        /// <param name="job"> The job.</param>
        public void PushJob(Action job)
        {
            lock (m_JobsQueue)
            {
                m_JobsQueue.Enqueue(job);
            }

            m_Event.Set();
        }

        /// <summary> Removes all pending jobs. </summary>
        public void RemoveAllPendingJobs()
        {
            lock (m_JobsQueue)
            {
                m_JobsQueue.Clear();
            }
        }

        /// <summary> Runs this object. </summary>
        private void Run()
        {
            while (true)
            {
                if (PendingJobsCount == 0)
                {
                    m_Event.WaitOne();
                }

                Action job = null;
                lock (m_JobsQueue)
                {
                    if (m_JobsQueue.Count == 0)
                    {
                        continue;
                    }

                    job = m_JobsQueue.Dequeue();
                    m_Running = true;
                }

                job();
                lock (m_JobsQueue)
                {
                    m_Running = false;
                }
            }
        }
    }
}
