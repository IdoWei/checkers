using System;
using Game;
using Logic;
using Vector;

namespace pieces
{
    public class Piece
    {
        public enum eSoldierTypes
        {
            Empty = 0, O = 'O', X = 'X', U = 'U', K = 'K'
        }

        private readonly int r_Ofset;
        private char m_Type;
        private char m_Group;
        private Vector2 m_Location;

        public Piece(char _type, Vector2 _location)
        {
            m_Type = _type;
            if (_type == 'K' || _type == 'X')
            {
                m_Group = 'X';
                r_Ofset = 1;
            }
            else if (_type == 'U' || _type == 'O')
            {
                m_Group = 'O';
                r_Ofset = -1;
            }

            m_Location = _location;
        }

        public Vector2 GetLocation
        {
            get
            {
                return this.m_Location;
            }

            set
            {
                this.m_Location = value;
            }
        }

        public int GetOfset
        {
            get
            {
                return r_Ofset;
            }
        }

        public bool TryEat(bool i_JustScaning, Vector.Vector2 i_Destinition, GameBoard io_GameBoard)
        {
            int ofset = this.GetOfset;
            int kingOfset = -1;
            bool result = false;
            Vector2 rivalLocation;

            if (this.GetGroup == GameLogic.CurrentPlayerGroup && GameLogic.CheckBorders(i_Destinition, io_GameBoard.BoardHeight) && io_GameBoard.IsLocationEmpty(i_Destinition))
            {
                if (this.m_Location.GetColumn - i_Destinition.GetColumn == -2 * ofset && this.m_Location.GetRow - i_Destinition.GetRow == 2 * ofset)
                {
                    rivalLocation = new Vector2(this.m_Location.GetColumn + (1 * ofset), this.m_Location.GetRow - (1 * ofset));
                    result = GameLogic.CheckBordersOfMove(this.GetLocation, i_Destinition, io_GameBoard.BoardHeight)
                             && io_GameBoard.GetTypeOfPieceByLocation(rivalLocation) == this.GetOtherGroupPlayer();
                    if (result && !i_JustScaning)
                    {
                        io_GameBoard.OnEatMove(rivalLocation);
                        eat(rivalLocation, i_Destinition, io_GameBoard);
                    }
                }
                else if (this.m_Location.GetColumn - i_Destinition.GetColumn == 2 * ofset && this.m_Location.GetRow - i_Destinition.GetRow == 2 * ofset)
                {
                    rivalLocation = new Vector2(this.m_Location.GetColumn - (1 * ofset), this.m_Location.GetRow - (1 * ofset));
                    result = GameLogic.CheckBordersOfMove(this.GetLocation, i_Destinition, io_GameBoard.BoardHeight) && io_GameBoard.GetTypeOfPieceByLocation(rivalLocation) == this.GetOtherGroupPlayer();
                    if (result && !i_JustScaning)
                    {
                        io_GameBoard.OnEatMove(rivalLocation);
                        eat(rivalLocation, i_Destinition, io_GameBoard);
                    }
                }
                else if (this.m_Type == 'K' || this.m_Type == 'U')
                {
                    if (!i_JustScaning)
                    {
                        result = tryKingEat(i_JustScaning, i_Destinition, io_GameBoard, kingOfset, out rivalLocation);
                        if(result)
                        {
                            io_GameBoard.OnEatMove(rivalLocation);
                        }
                        else
                        {
                            result = tryKingEat(i_JustScaning, i_Destinition, io_GameBoard, -kingOfset, out rivalLocation);
                            if(result)
                            {
                                io_GameBoard.OnEatMove(rivalLocation);
                            }
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        public bool Move(Vector.Vector2 i_Destinition, GameBoard io_GameBoard)
        {
            bool result = false;

            if (io_GameBoard[i_Destinition] == null && this.GetGroup == GameLogic.CurrentPlayerGroup)
            {
                result = isNormalMove(i_Destinition, io_GameBoard) || isKingMove(i_Destinition, io_GameBoard);
                if (result)
                {
                    if (!isNewKing(i_Destinition, io_GameBoard))
                    {
                        io_GameBoard[i_Destinition] = new Piece(this.m_Type, i_Destinition);
                    }

                    io_GameBoard[this.m_Location] = null;
                    io_GameBoard.OnMove(io_GameBoard[i_Destinition]);
                }
            }

            return result;
        }

        public bool IsKing()
        {
            return this.GetPieceType == 'K' || this.GetPieceType == 'U';
        }

        public char GetPieceType
        {
            get
            {
                return m_Type;
            }

            set
            {
                if (isValidTypeOfsoldier(value))
                {
                    m_Type = value;
                }
            }
        }

        public char GetGroup
        {
            get
            {
                return m_Group;
            }
        }

        private bool isValidTypeOfsoldier(char i_SoldierType)
        {
            bool result = false;

            foreach (eSoldierTypes i in Enum.GetValues(typeof(eSoldierTypes)))
            {
                if ((eSoldierTypes)i_SoldierType == i)
                {
                    result = true;
                }
            }

            return result;
        }

        private void eat(Vector2 i_RivalLocation, Vector.Vector2 i_Destinition, GameBoard io_GameBoard)
        {
            GameLogic.AddScore(io_GameBoard[i_RivalLocation]);

            io_GameBoard[i_RivalLocation] = null;
            if (!isNewKing(i_Destinition, io_GameBoard))
            {
                io_GameBoard[i_Destinition] = new Piece(this.m_Type, i_Destinition);
            }
            else
            {
                io_GameBoard.OnNewKing(io_GameBoard[i_Destinition]);
            }

            io_GameBoard.OnMove(io_GameBoard[i_Destinition]);
            io_GameBoard[this.m_Location] = null;
        }

        private bool tryKingEat(bool i_JustScaning, Vector.Vector2 i_Destinition, GameBoard io_GameBoard, int i_KingOfset, out Vector2 io_RivalLocation)
        {
            bool result = false;
            Vector2 rivalLocation = null;

            if (this.m_Location.GetColumn - i_Destinition.GetColumn == -2 * i_KingOfset)
            {
                if (this.m_Location.GetRow - i_Destinition.GetRow == 2 * i_KingOfset)
                {
                    rivalLocation = new Vector2(this.m_Location.GetColumn + (1 * i_KingOfset), this.m_Location.GetRow - (1 * i_KingOfset));
                    result = GameLogic.CheckBordersOfMove(this.GetLocation, i_Destinition, io_GameBoard.BoardHeight) && io_GameBoard.GetTypeOfPieceByLocation(rivalLocation) == this.GetOtherGroupPlayer();

                    if (result && !i_JustScaning)
                    {
                        eat(rivalLocation, i_Destinition, io_GameBoard);
                    }
                }
                else if (this.m_Location.GetRow - i_Destinition.GetRow == -2 * i_KingOfset)
                {
                    rivalLocation = new Vector2(this.m_Location.GetColumn + (1 * i_KingOfset), this.m_Location.GetRow + (1 * i_KingOfset));
                    result = GameLogic.CheckBordersOfMove(this.GetLocation, i_Destinition, io_GameBoard.BoardHeight) && io_GameBoard.GetTypeOfPieceByLocation(rivalLocation) == this.GetOtherGroupPlayer();

                    if (result && !i_JustScaning)
                    {
                        eat(rivalLocation, i_Destinition, io_GameBoard);
                    }
                }

                if (result == true)
                {
                    io_GameBoard.SubtractionSumOfPiecesGroupOnBoard(GetOtherGroupPlayer());
                }
            }

            io_RivalLocation = rivalLocation;
            return result;
        }

        private bool isKingMove(Vector.Vector2 i_Destinition, GameBoard io_GameBoard)
        {
            bool result = false;
            int kingOfset = -1;
            Vector2 rivalLocation;

            if (IsKing())
            {
                result = tryKingEat(false, i_Destinition, io_GameBoard, kingOfset, out rivalLocation);
                if (result)
                {
                    io_GameBoard.OnEatMove(rivalLocation);
                }
                else
                {
                    result = tryKingEat(false, i_Destinition, io_GameBoard, -kingOfset, out rivalLocation);
                    if (result)
                    {
                        io_GameBoard.OnEatMove(rivalLocation);
                    }
                }
            }

                return result;
        }

        private bool isNormalMove(Vector.Vector2 i_Destinition, GameBoard io_GameBoard)
        {
            bool result;

            int rowDistance = this.m_Location.GetRow - i_Destinition.GetRow;
            int columnrowDistance = this.m_Location.GetColumn - i_Destinition.GetColumn;
            if (this.GetPieceType == 'X' && rowDistance == -1)
            {
                result = false;
            }
            else if (this.GetPieceType == 'O' && rowDistance == 1)
            {
                result = false;
            }
            else
            {
                result = io_GameBoard.IsLocationEmpty(i_Destinition) && Math.Abs(rowDistance) == 1 && Math.Abs(columnrowDistance) == 1;
            }

            return result;
        }

        public bool IsPlayerOneTurn()
        {
            bool isPlayerOneTurn = false;

            if (this.m_Group == 'X')
            {
                isPlayerOneTurn = true;
            }

            return isPlayerOneTurn;
        }

        public char GetOtherGroupPlayer()
        {
            char rivalGroup;

            if (this.m_Group == 'X')
            {
                rivalGroup = 'O';
            }
            else
            {
                rivalGroup = 'X';
            }

            return rivalGroup;
        }

        public char GetKingTypeByGroup()
        {
            char result;

            if (GetGroup == 'X')
            {
                result = 'K';
            }
            else
            {
                result = 'U';
            }

            return result;
        }

        private bool isNewKing(Vector.Vector2 i_Destinition, GameBoard io_GameBoard)
        {
            bool result = false;

            if (GetGroup == 'X' && i_Destinition.GetRow == 0)
            {
                result = true;
                io_GameBoard[i_Destinition] = new Piece(io_GameBoard[this.m_Location].GetKingTypeByGroup(), i_Destinition);
            }
            else if (GetGroup == 'O' && i_Destinition.GetRow == io_GameBoard.BoardHeight - 1)
            {
                result = true;
                io_GameBoard[i_Destinition] = new Piece(io_GameBoard[this.m_Location].GetKingTypeByGroup(), i_Destinition);
            }

            return result;
        }
    }
}