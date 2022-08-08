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
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Assertions;

    public class AudioFilterStream : IDisposable
    {
        private byte[] m_CacheBuffer;
        private const int MaxBufferSize = 2048 * 20;
        private int m_ReadIndex, m_WriteIndex;

        public AudioFilterStream()
        {
            m_ReadIndex = 0;
            m_WriteIndex = 0;
        }

        public void Dispose()
        {
            m_ReadIndex = 0;
            m_WriteIndex = 0;
        }

        protected void InitBuffer(UInt32 len)
        {
            m_CacheBuffer = new byte[len <= MaxBufferSize ? len : MaxBufferSize];
        }

        public void OnAudioDataRead(IntPtr data, uint size)
        {
            if (m_CacheBuffer == null)
            {
                InitBuffer(size * 10);
            }

            Write(data, (int)size);
        }

        protected void Write(IntPtr data, int size)
        {
            if (data == IntPtr.Zero || size <= 0)
                return;
            
            var bytesData = new Byte[size];
            Marshal.Copy(data, bytesData, 0, size);
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
            // NRDebugger.Warning("[AudioFilterStream] Write: data={0}, writeIdx={1}", bytesData.Length, m_WriteIndex);
        }

        public bool Flush(ref byte[] outBytesData)
        {
            if (m_CacheBuffer == null || m_ReadIndex == m_WriteIndex)
            {
                // NRDebugger.Warning("[AudioFilterStream] Flush fail: readIdx={0}, writeIdx={1}", m_ReadIndex, m_WriteIndex);
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
            // NRDebugger.Warning("[AudioFilterStream] Flush: data={0}, readIdx={1}, writeIdx={2}", outBytesData.Length, m_ReadIndex, m_WriteIndex);

            return true;
        }
    }
}
