using Game;

namespace Vector
{
    public class Vector2
    {
        private int m_Row;
        private int m_Column;

        public Vector2(int _column, int _row)
        {
            m_Column = _column;
            m_Row = _row;
        }

        public int GetColumn
        {
            get
            {
                return m_Column;
            }

            set
            {
                m_Column = value;
            }
        }

        public int GetRow
        {
            get
            {
                return m_Row;
            }

            set
            {
                m_Row = value;
            }
        }
    }
}