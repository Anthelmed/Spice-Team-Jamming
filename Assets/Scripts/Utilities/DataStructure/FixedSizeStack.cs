namespace Utilities.DataStructure
{
    public class FixedSizeStack<T>
    {
        private T[] m_Data;

        private int m_Start = 0;
        private int m_End = 0;

        private bool m_ShouldPushStart;

        public FixedSizeStack(int _capacity)
        {
            m_Data = new T[_capacity];
        }

        public void Add(T _value)
        {
            m_Data[m_End] = _value;
            ++m_End;
            if (m_End >= m_Data.Length)
            {
                m_End = 0;
                m_ShouldPushStart = true;
            }
        }

        public void Clear()
        {
            m_Start = 0;
            m_End = 0;
        }
    }
}