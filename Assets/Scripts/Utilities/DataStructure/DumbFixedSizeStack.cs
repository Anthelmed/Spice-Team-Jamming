using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Utilities.DataStructure
{
    /*
     * This stack is pretty dumb, when reaching capacity it just take the half after the middle and move it to the begining.
     * Thus we have enough i for other enqueuing.
     * It is there cause I am too tired to actually implement a right algorithm in #FixedSizeStack.cs.
     */
    public class DumbFixedSizeStack<T>
    {
        private T[] m_Data;

        private int m_End = 0;

        public bool IsEmpty => m_End == 0;


        public DumbFixedSizeStack(int _capacity)
        {
            if ((_capacity & 1) == 1)
            {
                _capacity += 1;
            }

            m_Data = new T[_capacity];
        }

        public void Add(T _value)
        {
            m_Data[m_End] = _value;
            ++m_End;
            if (m_End >= m_Data.Length)
            {
                m_End = m_End >> 1;
                for (int i = 0; i < m_End; ++i)
                {
                    m_Data[i] = m_Data[m_End + i];
                }
            }
        }

        public T Top
        {
            get {
                if (IsEmpty)
                {
                    throw new Exception("Can't get top as the stack is empty");
                }

                return m_Data[m_End - 1];
            }
        }

        public T GetTop(int _backIndex)
        {
            if (_backIndex >= m_End)
            {
                throw new Exception($"Can't get top({_backIndex}) because Count = {Count}");
            }

            return m_Data[m_End - 1 - _backIndex];
        }
        
        public void Clear()
        {
            m_End = 0;
        }

        public IEnumerable<T> Data
        {
            get
            {
                for (int i = 0; i < m_End; i++)
                {
                    yield return m_Data[i];
                }
            }
        }

        public int Count => m_End;
    }
}