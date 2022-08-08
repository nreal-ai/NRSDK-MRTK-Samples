namespace NRKernal
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary> An object pool. </summary>
    public class ObjectPool
    {
        /// <summary> Dictionary of cache pools. </summary>
        private Dictionary<Type, Queue<object>> m_CachePoolDict = new Dictionary<Type, Queue<object>>();
        /// <summary> Number of initializes. </summary>
        public int InitCount = 100;

        /// <summary> Expansions this object. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        public void Expansion<T>() where T : new()
        {
            var queue = GetQueue<T>();
            for (int i = 0; i < InitCount; i++)
            {
                T data = new T();
                queue.Enqueue(data);
            }
        }

        /// <summary> Gets the get. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <returns> A T. </returns>
        public T Get<T>() where T : new()
        {
            var queue = GetQueue<T>();
            if (queue.Count == 0)
            {
                Expansion<T>();
            }

            return (T)queue.Dequeue();
        }

        /// <summary> Puts the given data. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <param name="data"> The data.</param>
        public void Put<T>(T data) where T : new()
        {
            var queue = GetQueue<T>();
            queue.Enqueue(data);
        }

        /// <summary> Gets the queue. </summary>
        /// <typeparam name="T"> Generic type parameter.</typeparam>
        /// <returns> The queue. </returns>
        private Queue<object> GetQueue<T>() where T : new()
        {
            Queue<object> queue = null;
            m_CachePoolDict.TryGetValue(typeof(T), out queue);
            if (queue == null)
            {
                queue = new Queue<object>();
                m_CachePoolDict.Add(typeof(T), queue);
            }
            return queue;
        }

        public void Clear()
        {
            m_CachePoolDict.Clear();
        }
    }

    /// <summary> The bytes pool. </summary>
    public class BytesPool
    {
        /// <summary> Dictionary of bytes. </summary>
        public Dictionary<int, Queue<byte[]>> BytesDict = new Dictionary<int, Queue<byte[]>>();

        /// <summary> Gets a byte[] using the given length. </summary>
        /// <param name="len"> The Length to get.</param>
        /// <returns> A byte[]. </returns>
        public byte[] Get(int len)
        {
            if (len <= 0)
            {
                NRDebugger.Info("BytesPool get len is not valid :" + len);
                return null;
            }
            Queue<byte[]> que = null;
            BytesDict.TryGetValue(len, out que);
            if (que == null)
            {
                que = new Queue<byte[]>();
                byte[] temp = new byte[len];
                que.Enqueue(temp);

                BytesDict.Add(len, que);
            }
            else if (que.Count == 0)
            {
                byte[] temp = new byte[len];
                que.Enqueue(temp);
            }

            return que.Dequeue();
        }

        /// <summary> Puts the given data. </summary>
        /// <param name="data"> The data to put.</param>
        public void Put(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                NRDebugger.Info("BytesPool retrieve data is null.");
                return;
            }

            Queue<byte[]> que = null;
            BytesDict.TryGetValue(data.Length, out que);
            if (que == null)
            {
                que = new Queue<byte[]>();
                BytesDict.Add(data.Length, que);
            }
            que.Enqueue(data);
        }

        public void Clear()
        {
            BytesDict.Clear();
        }
    }
}
