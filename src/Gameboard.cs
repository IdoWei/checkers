using System;
using pieces;
using Vector;

namespace Game
{
    public class GameBoard
    {
        public enum eGameboardSizes
        {
            Small = 6, Medium = 8, Large = 10
        }

        public enum eSoldierTypes
        {
            Empty = 0, O = 'O', X = 'X', U = 'U', K = 'K'
        }

        private readonly int m_SizeBoard;
        private Piece[,] m_Board;
        private static int s_NumberOfCirclesOnBoard;
        private static int s_NumberOfXOnBoard;
        private static int s_NumberOfKOnBoard;
        private static int s_NumberOfUOnBoard;

        public event Action<Vector2> ReportEatenEventHandler;

        public event Action<Piece> ReportBecameKingEventHandler;

        public event Action<Piece> ReportMovedEventHandler;

        public GameBoard(int i_size)
        {
            m_SizeBoard = i_size;
            m_Board = new Piece[m_SizeBoard, m_SizeBoard];
            s_NumberOfCirclesOnBoard = (m_SizeBoard * (m_SizeBoard - 2) * m_SizeBoard) / 2;
            s_NumberOfXOnBoard = s_NumberOfCirclesOnBoard;
            s_NumberOfKOnBoard = s_NumberOfUOnBoard = 0;
            Initial();
        }

        public void Initial()
        {
            int width = m_SizeBoard;
            int numOfRowEachPlayer = (width - 2) / 2; ////Length=width
            bool isCircle = false;
            Vector2 location;
            
            ////in even number row the initial position is (i,1), when in odd number row the initial position is (i,0)
            for (int row = 0; row < m_SizeBoard; row++)
            {
                for (int column = 1 - (row % 2); column < m_SizeBoard; column += 2)
                {
                    if (row == numOfRowEachPlayer)
                    {
                        row += 2; ////skip to relevante row
                        isCircle = true;
                    }

                    location = new Vector2(column, row);
                    if (isCircle)
                    {
                        m_Board[row, column] = new Piece('X', location);
                    }
                    else
                    {
                        m_Board[row, column] = new Piece('O', location);
                    }
                }
            }
        }

        public void OnMove(Piece i_MovedPiece)
        {
            if(ReportMovedEventHandler != null)
            {
                ReportMovedEventHandler.Invoke(i_MovedPiece);
            }
        }

        public void OnNewKing(Piece i_NewKing)
        {
            if(ReportBecameKingEventHandler != null)
            {
                ReportBecameKingEventHandler.Invoke(i_NewKing);
            }
        }

        public void OnEatMove(Vector2 i_EatenPieceLocation)
        {
            if (ReportEatenEventHandler != null)
            {
                ReportEatenEventHandler.Invoke(i_EatenPieceLocation);
            }
        }

        public int GetSumOfKOnBoard
        {
            get
            {
                return s_NumberOfKOnBoard;
            }

            set
            {
                s_NumberOfKOnBoard = value;
            }
        }

        public int GetSumOfUOnBoard
        {
            get
            {
                return s_NumberOfUOnBoard;
            }

            set
            {
                s_NumberOfUOnBoard = value;
            }
        }

        public int GetSumOfCirclesOnBoard
        {
            get
            {
                return s_NumberOfCirclesOnBoard;
            }

            set
            {
                s_NumberOfCirclesOnBoard = value;
            }
        }

        public int GetSumOfXOnBoard
        {
            get
            {
                return s_NumberOfXOnBoard;
            }

            set
            {
                s_NumberOfXOnBoard = value;
            }
        }

        public Piece this[Vector2 location]
        {
            get
            {
                return m_Board[location.GetRow, location.GetColumn];
            }

            set
            {
                m_Board[location.GetRow, location.GetColumn] = value;
            }
        }

        public int BoardHeight
        {
            get
            {
                return m_SizeBoard;
            }
        }

        private static bool isValidTypeOfsoldier(char i_SoldierType)
        {
            bool result = false;

            foreach (GameBoard.eSoldierTypes i in Enum.GetValues(typeof(GameBoard.eSoldierTypes)))
            {
                if ((GameBoard.eSoldierTypes)i_SoldierType == i)
                {
                    result = true;
                }
            }

            return result;
        }

        public bool IsLocationEmpty(Vector2 i_Location)
        {
            return this[i_Location] == null;
        }

        public char GetTypeOfPieceByLocation(Vector2 i_Location)
        {
            char group = char.MinValue;

            if (this[i_Location] != null)
            {
                group = this[i_Location].GetGroup;
            }

            return group;
        }

        public void SubtractionSumOfPiecesGroupOnBoard(char i_Group)
        {
            if (i_Group == 'X')
            {
                s_NumberOfXOnBoard--;
            }
            else
            {
                s_NumberOfCirclesOnBoard--;
            }
        }

        public void CleanBoard()
        {
            int width = m_SizeBoard;

            ////in even number row the initial position is (i,1), when in odd number row the initial position is (i,0)
            for (int row = 0; row < m_SizeBoard; row++)
            {
                for (int column = 1 - (row % 2); column < m_SizeBoard; column += 2)
                {
                    m_Board[row, column] = null;
                }
            }
        }
    }
}