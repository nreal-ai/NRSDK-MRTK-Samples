/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal.Record
{
    using System;
    using UnityEngine;
    using UnityEngine.Assertions;

    public class AudioRecordTool : MonoBehaviour
    {
        private byte[] m_CacheBuffer;
        private const int MaxBufferSize = 2048 * 20;
        private int m_ReadIndex, m_WriteIndex;

        public int RIndex { get { return m_ReadIndex; } }
        public int WIndex { get { return m_WriteIndex; } }

        private bool m_RecOutput = false;
        public bool IsRecording { get { return m_RecOutput; } }

        private int m_OutputSampleRate;
        public int SampleRate { get { return m_OutputSampleRate; } }

        void Awake()
        {
            m_ReadIndex = 0;
            m_WriteIndex = 0;

            m_OutputSampleRate = AudioSettings.GetConfiguration().sampleRate;
            //AudioSettings.GetDSPBufferSize(out bufferSize, out numBuffers);
        }

        protected void InitBuffer(int len)
        {
            m_CacheBuffer = new byte[len <= MaxBufferSize ? len : MaxBufferSize];
        }

        public void StartRecord()
        {
            m_RecOutput = true;
        }

        public void StopRecord()
        {
            m_RecOutput = false;
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (m_RecOutput)
            {
                if (m_CacheBuffer == null)
                {
                    InitBuffer(data.Length * 10);
                }

                ConvertToSinChaAndWrite(data, channels); //audio data is interlaced
            }
        }

        /// <summary>
        /// Only take the data of the first channel.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="channels"></param>
        private void ConvertToSinChaAndWrite(float[] dataSource, int channels)
        {
            var intData = new Int16[dataSource.Length / channels];
            //converting in 2 steps : float[] to Int16[], //then Int16[] to Byte[]

            var bytesData = new Byte[intData.Length * 2];
            //bytesData array is twice the size of
            //dataSource array because a float converted in Int16 is 2 bytes.

            var rescaleFactor = Int16.MaxValue; //to convert float to Int16

            var byteArr = new Byte[2];
            for (var i = 0; i < intData.Length; i++)
            {
                intData[i] = (Int16)(dataSource[i * 2] * rescaleFactor);
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            Write(bytesData);
        }

        /// <summary>
        /// Take all data of the channel.
        /// </summary>
        /// <param name="dataSource"></param>
        private void ConvertAndWrite(float[] dataSource)
        {
            var intData = new Int16[dataSource.Length];
            //converting in 2 steps : float[] to Int16[], //then Int16[] to Byte[]

            var bytesData = new Byte[dataSource.Length * 2];
            //bytesData array is twice the size of
            //dataSource array because a float converted in Int16 is 2 bytes.

            var rescaleFactor = 32767; //to convert float to Int16

            for (var i = 0; i < dataSource.Length; i++)
            {
                intData[i] = (Int16)(dataSource[i] * rescaleFactor);
                var byteArr = new Byte[2];
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            Write(bytesData);
        }

        protected void Write(byte[] bytesData)
        {
            lock (m_CacheBuffer)
            {
                if (m_WriteIndex + bytesData.Length <= m_CacheBuffer.Length)
                {
                    Array.Copy(bytesData, 0, m_CacheBuffer, m_WriteIndex, bytesData.Length);
                    m_WriteIndex += bytesData.Length;
                }
                else
                {
                    int left = m_CacheBuffer.Length - m_WriteIndex;
                    Assert.IsTrue(left >= 0);
                    if (left > 0)
                    {
                        Array.Copy(bytesData, 0, m_CacheBuffer, m_WriteIndex, left);
                        m_WriteIndex = 0;
                        Array.Copy(bytesData, left, m_CacheBuffer, m_WriteIndex, bytesData.Length - left);
                        m_WriteIndex += bytesData.Length - left;
                    }
                    else
                    {
                        m_WriteIndex = 0;
                        Array.Copy(bytesData, 0, m_CacheBuffer, m_WriteIndex, bytesData.Length);
                        m_WriteIndex += bytesData.Length;
                    }
                }
            }
        }

        public bool Flush(ref byte[] outBytesData)
        {
            if (m_CacheBuffer == null || m_ReadIndex == m_WriteIndex)
            {
                return false;
            }

            lock (m_CacheBuffer)
            {
                int count = 0;
                if (m_ReadIndex < m_WriteIndex)
                {
                    count = m_WriteIndex - m_ReadIndex;
                    if (outBytesData == null || outBytesData.Length != count)
                    {
                        outBytesData = new byte[count];
                    }

                    Array.Copy(m_CacheBuffer, m_ReadIndex, outBytesData, 0, count);
                }
                else
                {
                    int left = m_CacheBuffer.Length - m_ReadIndex;
                    count = left + m_WriteIndex;

                    Assert.IsTrue(left >= 0);
                    if (outBytesData == null || outBytesData.Length != count)
                    {
                        outBytesData = new byte[count];
                    }

                    if (left == 0)
                    {
                        m_ReadIndex = 0;
                        Assert.IsTrue(m_WriteIndex != 0);
                        Array.Copy(m_CacheBuffer, m_ReadIndex, outBytesData, 0, count);
                    }
                    else
                    {
                        Array.Copy(m_CacheBuffer, m_ReadIndex, outBytesData, 0, m_CacheBuffer.Length - m_ReadIndex);
                        Array.Copy(m_CacheBuffer, 0, outBytesData, m_CacheBuffer.Length - m_ReadIndex, m_WriteIndex);
                    }
                }

                m_ReadIndex = m_WriteIndex;
            }

            return true;
        }
    }
}
