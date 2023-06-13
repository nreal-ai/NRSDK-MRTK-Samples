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

    /// <summary>
    /// A yield instruction that blocks a coroutine until an AsyncTask has completed. </summary>
    /// <typeparam name="T"> The type of the AsyncTask result.</typeparam>
    public class WaitForTaskCompletionYieldInstruction<T> : CustomYieldInstruction
    {
        /// <summary> The AsyncTask the yield instruction waits on. </summary>
        private AsyncTask<T> m_Task;

        /// <summary> Constructor for WaitForTaskCompletionYieldInstruction. </summary>
        /// <param name="task"> The task to wait for completion.</param>
        public WaitForTaskCompletionYieldInstruction(AsyncTask<T> task)
        {
            m_Task = task;
        }

        /// <summary>
        /// Gets a value indicating whether the coroutine instruction should keep waiting. </summary>
        /// <value> <c>true</c> if the task is incomplete, otherwise <c>false</c>. </value>
        public override bool keepWaiting
        {
            get
            {
                return !m_Task.IsComplete;
            }
        }
    }
}
