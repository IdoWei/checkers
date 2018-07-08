using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Vector;

namespace DamkaForm
{
    public class ButtonWithLocation : Button
    {
        private Vector2 m_location;

        public ButtonWithLocation(int i_Row, int i_Col) : base()
        {
            m_location = new Vector2(i_Col, i_Row);
        }

        public Vector2 VectorLocation
        {
            get { return m_location; }
            set { m_location = value; }
        }
    }
}