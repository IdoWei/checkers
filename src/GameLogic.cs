using System;
using pieces;
using Game;
using Vector;

namespace Logic
{
    public class GameLogic
    {
        public static event Action ReportScoreChengeEventHandler;

        private static char s_currentPlayerGroup = 'X';
        private static int s_XScore = 0;
        private static int s_OScore = 0;

        public static void ScoreChenge()
        {
            if (ReportScoreChengeEventHandler != null)
            {
                ReportScoreChengeEventHandler.Invoke();
            }
        }

        public static char CurrentPlayerGroup
        {
            get
            {
                return GameLogic.s_currentPlayerGroup;
            }

            set
            {
                s_currentPlayerGroup = value;
            }
        }

        public static int XScore
        {
            get
            {
                return s_XScore;
            }

            set
            {
                s_XScore = value;
            }
        }

        public static int OScore
        {
            get
            {
                return s_OScore;
            }

            set
            {
                s_OScore = value;
            }
        }

        ////ChengeCurrentTurn
        public static void UpdateCurrentTurn(ref bool io_IsPlayerOneTurn)
        {
            if (GameLogic.CurrentPlayerGroup == 'X')
            {
                GameLogic.CurrentPlayerGroup = 'O';
                io_IsPlayerOneTurn = false;
            }
            else
            {
                GameLogic.CurrentPlayerGroup = 'X';
                io_IsPlayerOneTurn = true;
            }
        }

        public static bool CheckMove(Piece i_CurrentPiece, Vector.Vector2 i_Destinition, GameBoard io_GameBoard, ref bool io_HasAnotherEatMove, ref bool o_PrintMustEatError, bool i_IsPcMove, ref Vector2 o_PcMoveLocation)
        {
            bool result = false, hasMoved = false, didEat = false, isEatMove, isGenericEatMoveAvailable = false;
            Vector2 pcStartMoveLocation = new Vector2(-1, -1);

            o_PrintMustEatError = false;
            if (!i_IsPcMove)
            {
                if (i_CurrentPiece != null)
                {
                    if (CheckBordersOfMove(i_CurrentPiece.GetLocation, i_Destinition, io_GameBoard.BoardHeight) && i_CurrentPiece.GetGroup == s_currentPlayerGroup)
                    {
                        isEatMove = didEat = i_CurrentPiece.TryEat(false, i_Destinition, io_GameBoard);
                        if (!isEatMove)
                        {
                            isGenericEatMoveAvailable = isEatMoveAvailable(io_GameBoard, (Piece.eSoldierTypes)i_CurrentPiece.GetPieceType);
                            if (isGenericEatMoveAvailable)
                            {
                                o_PrintMustEatError = true;
                                result = false;
                            }
                        }

                        if (!isGenericEatMoveAvailable)
                        {
                            if (didEat)
                            {
                                io_HasAnotherEatMove = isSpecificEatMoveAvailable(io_GameBoard, io_GameBoard[i_Destinition]);
                                if (!io_HasAnotherEatMove)
                                {
                                    result = true;
                                }
                            }

                            if ((!io_HasAnotherEatMove || isEatMove) && !didEat)
                            {
                                hasMoved = i_CurrentPiece.Move(i_Destinition, io_GameBoard);
                                result = hasMoved || didEat;
                            }
                            else if (io_HasAnotherEatMove && !isEatMove)
                            {
                                o_PrintMustEatError = true;
                            }

                            if (io_HasAnotherEatMove)
                            {
                                result = !true;
                            }
                        }
                    }
                }
            }
            else
            {
                o_PcMoveLocation = MakePcRandomMove(io_GameBoard, ref hasMoved, ref io_HasAnotherEatMove, out pcStartMoveLocation);
                if (!hasMoved || io_HasAnotherEatMove)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        public static bool CheckBordersOfMove(Vector2 i_CurrentLocation, Vector2 i_Destination, int i_BoardHeight)
        {
            return CheckBorders(i_CurrentLocation, i_BoardHeight) && CheckBorders(i_Destination, i_BoardHeight);
        }

        public static bool CheckBorders(Vector.Vector2 i_Location, int i_Length)
        {
            return i_Location.GetColumn >= 0 && i_Location.GetColumn < i_Length && i_Location.GetRow >= 0 && i_Location.GetRow < i_Length;
        }

        public static void CalculateScore(GameBoard i_GameBoard)
        {
            int playerOneCurrentGameScore = 0, playerTwoCurrentGameScore = 0;

            if (!IsATie(i_GameBoard))
            {
                playerOneCurrentGameScore = i_GameBoard.GetSumOfXOnBoard + (4 * i_GameBoard.GetSumOfKOnBoard);
                playerTwoCurrentGameScore = i_GameBoard.GetSumOfCirclesOnBoard + (4 * i_GameBoard.GetSumOfUOnBoard);
                if (playerOneCurrentGameScore >= playerTwoCurrentGameScore)
                {
                    s_XScore = s_XScore + (playerOneCurrentGameScore - playerTwoCurrentGameScore);
                }
                else
                {
                    s_OScore = s_OScore + (playerTwoCurrentGameScore - playerOneCurrentGameScore);
                }
            }
        }

        public static bool IsATie(GameBoard i_GameBoard)
        {
            return numberOfCircleAvailableMoves(i_GameBoard) == 0 && numberOfXAvailableMoves(i_GameBoard) == 0;
        }

        public static bool IsGameOver(GameBoard i_GameBoard)
        {
            return numberOfCircleAvailableMoves(i_GameBoard) == 0 || numberOfXAvailableMoves(i_GameBoard) == 0;
        }

        private static int numberOfXAvailableMoves(GameBoard i_GameBoard)
        {
            int numOfAvailableMoves = 0;
            int width = i_GameBoard.BoardHeight;

            for (int i = 0; i < i_GameBoard.BoardHeight; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Vector2 location = new Vector2(j, i);
                    if (i_GameBoard[location] != null)
                    {
                        if (i_GameBoard[location].GetGroup == (char)Piece.eSoldierTypes.X)
                        {
                            if (i - 1 >= 0 && j - 1 >= 0)
                            {
                                location = new Vector2(j - 1, i - 1);
                                if (i_GameBoard[location] == null)
                                {
                                    ++numOfAvailableMoves;
                                }
                            }

                            if (i - 1 >= 0 && j + 1 <= i_GameBoard.BoardHeight - 1)
                            {
                                location = new Vector2(j + 1, i - 1);
                                if (i_GameBoard[location] == null)
                                {
                                    ++numOfAvailableMoves;
                                }
                            }
                        }
                        else if (i_GameBoard[location].GetPieceType == (char)GameBoard.eSoldierTypes.K)
                        {
                            if (i + 1 <= i_GameBoard.BoardHeight - 1 && j - 1 >= 0)
                            {
                                location = new Vector2(j + 1, i - 1);
                                if (i_GameBoard[location] == null)
                                {
                                    ++numOfAvailableMoves;
                                }
                            }

                            if (i + 1 <= i_GameBoard.BoardHeight - 1 && j + 1 <= i_GameBoard.BoardHeight - 1)
                            {
                                location = new Vector2(j + 1, i + 1);
                                if (i_GameBoard[location] == null)
                                {
                                    ++numOfAvailableMoves;
                                }
                            }
                        }
                    }
                }
            }

            return numOfAvailableMoves;
        }

        private static int numberOfCircleAvailableMoves(GameBoard i_GameBoard)
        {
            int numOfAvailableMoves = 0;

            for (int i = 0; i < i_GameBoard.BoardHeight; i++)
            {
                for (int j = 0; j < i_GameBoard.BoardHeight; j++)
                {
                    Vector2 location = new Vector2(j, i);
                    if (i_GameBoard[location] != null)
                    {
                        if (i_GameBoard[location].GetPieceType == (char)GameBoard.eSoldierTypes.O)
                        {
                            if (i + 1 <= i_GameBoard.BoardHeight - 1 && j - 1 >= 0)
                            {
                                location = new Vector2(j - 1, i + 1);
                                if (i_GameBoard[location] == null)
                                {
                                    ++numOfAvailableMoves;
                                }
                            }

                            if (i + 1 <= i_GameBoard.BoardHeight - 1 && j + 1 <= i_GameBoard.BoardHeight - 1)
                            {
                                location = new Vector2(j + 1, i + 1);
                                if (i_GameBoard[location] == null)
                                {
                                    ++numOfAvailableMoves;
                                }
                            }
                        }
                        else if (i_GameBoard[location].GetPieceType == (char)GameBoard.eSoldierTypes.U)
                        {
                            if (i - 1 >= 0 && j - 1 >= 0)
                            {
                                location = new Vector2(j - 1, i - 1);
                                if (i_GameBoard[location] == null)
                                {
                                    ++numOfAvailableMoves;
                                }
                            }

                            if (i - 1 >= 0 && j + 1 <= i_GameBoard.BoardHeight - 1)
                            {
                                location = new Vector2(j + 1, i - 1);
                                if (i_GameBoard[location] == null)
                                {
                                    ++numOfAvailableMoves;
                                }
                            }
                        }
                    }
                }
            }

            return numOfAvailableMoves;
        }

        public static void AddScore(Piece i_RevalPiece)
        {
            int score = 0;

            if (i_RevalPiece.GetGroup == 'X')
            {
                score += 1;
                if (i_RevalPiece.GetPieceType == 'K')
                {
                    score *= 4;
                }

                s_OScore += score;
                ScoreChenge();
            }
            else
            {
                score += 1;
                if (i_RevalPiece.GetPieceType == 'U')
                {
                    score *= 4;
                }

                s_XScore += score;
                ScoreChenge();
            }
        }

        ////Checking availability of an eat move for all soldiers of a certain type.
        private static bool isEatMoveAvailable(GameBoard i_GameBoard, Piece.eSoldierTypes i_PieceType)
        {
            Vector2 currLocation = new Vector2(0, 0);
            bool result = false;

            for (int i = 0; i < i_GameBoard.BoardHeight; i++)
            {
                currLocation.GetRow = i;
                for (int j = 0; j < i_GameBoard.BoardHeight; j++)
                {
                    currLocation.GetColumn = j;
                    if (i_GameBoard[currLocation] != null)
                    {
                        if (isSpecificEatMoveAvailable(i_GameBoard, i_GameBoard[currLocation]) && (char)i_PieceType == i_GameBoard[currLocation].GetPieceType)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private static bool isSpecificEatMoveAvailable(GameBoard i_GameBoard, Piece i_CurrentPiece)
        {
            bool result = false;
            Vector2 currentLocation = i_CurrentPiece.GetLocation;
            Vector2 dest1, dest2, dest3, dest4;
            int row, column;

            row = currentLocation.GetRow;
            column = currentLocation.GetColumn;
            dest1 = new Vector2(currentLocation.GetColumn + 2, currentLocation.GetRow + 2);
            dest2 = new Vector2(currentLocation.GetColumn - 2, currentLocation.GetRow + 2);
            dest3 = new Vector2(currentLocation.GetColumn + 2, currentLocation.GetRow - 2);
            dest4 = new Vector2(currentLocation.GetColumn - 2, currentLocation.GetRow - 2);
            result = i_CurrentPiece.TryEat(true, dest1, i_GameBoard) || i_CurrentPiece.TryEat(true, dest2, i_GameBoard) ||
                             i_CurrentPiece.TryEat(true, dest3, i_GameBoard) || i_CurrentPiece.TryEat(true, dest4, i_GameBoard);
            return result;
        }

        public static bool ExeAllPcEatMovesAvailableToCurrentLocation(GameBoard i_GameBoard, Piece i_CurrentPiece)
        {
            bool result = true;
            Vector2 currentLocation = i_CurrentPiece.GetLocation;
            Vector2 dest1, dest2, dest3, dest4;
            int row, column;

                row = currentLocation.GetRow;
                column = currentLocation.GetColumn;
                dest1 = new Vector2(currentLocation.GetColumn + 2, currentLocation.GetRow + 2);
                dest2 = new Vector2(currentLocation.GetColumn - 2, currentLocation.GetRow + 2);
                dest3 = new Vector2(currentLocation.GetColumn + 2, currentLocation.GetRow - 2);
                dest4 = new Vector2(currentLocation.GetColumn - 2, currentLocation.GetRow - 2);

                if (i_CurrentPiece.TryEat(!true, dest1, i_GameBoard))
                {
                    i_CurrentPiece.GetLocation = dest1;
                }
                else if (i_CurrentPiece.TryEat(!true, dest2, i_GameBoard))
                {
                    i_CurrentPiece.GetLocation = dest2;
                }
                else if (i_CurrentPiece.TryEat(!true, dest3, i_GameBoard))
                {
                    i_CurrentPiece.GetLocation = dest3;
                }
                else if (i_CurrentPiece.TryEat(!true, dest4, i_GameBoard))
                {
                    i_CurrentPiece.GetLocation = dest4;
                }
                else
                {
                    result = !true;
                }

            return result;
        }

        public static Vector2 MakePcRandomMove(GameBoard io_GameBoard, ref bool io_IsPcHasAvailableMoves, ref bool o_HasAnotherEatMove, out Vector2 io_StartLocation)
        {
            string move = null;
            int numOfAvailableMoves = 0, moveNumber;
            Random nextMove = new Random(0);
            bool madeEatMove = !true;
            io_StartLocation = new Vector2(-1, -1);
            ////Vector2 currLocation = new Vector2(0, 0), nextLocation = new Vector2(0, 0);
            Vector2 resultLocation = new Vector2(-1, -1), startLocation = new Vector2(-1, -1);
            Piece eaterPiece = null;

            numOfAvailableMoves = countHowManyMovesAvailable(io_GameBoard, ref madeEatMove, ref eaterPiece, ref startLocation, ref move);
            if (numOfAvailableMoves == 0 && !madeEatMove)
            {
                io_IsPcHasAvailableMoves = !true;
            }
            else if (numOfAvailableMoves > 0 || madeEatMove)
            {
                io_IsPcHasAvailableMoves = true;
            }

            if (io_IsPcHasAvailableMoves && !madeEatMove)
            {
                o_HasAnotherEatMove = !true;
                moveNumber = nextMove.Next(1, numOfAvailableMoves);
                resultLocation = executeSpecificPcMove(io_GameBoard, moveNumber, ref startLocation);
                io_StartLocation = startLocation;
            }
            else if (madeEatMove)
            {
                io_StartLocation = startLocation;
                resultLocation = eaterPiece.GetLocation;
                o_HasAnotherEatMove = isSpecificEatMoveAvailable(io_GameBoard, eaterPiece);
            }

            return resultLocation;
        }

        private static int countHowManyMovesAvailable(GameBoard io_GameBoard, ref bool io_MadeEatMove, ref Piece io_EaterPiece, ref Vector2 io_StartLocation, ref string o_Move)
        {
            int numOfAvailableMoves = 0;
            Vector2 currLocation = new Vector2(0, 0), nextLocation = new Vector2(0, 0);
            Vector2 eatLocation = new Vector2(0, 0);
            Piece eaterPiece;

            for (int i = 0; i < io_GameBoard.BoardHeight; i++)
            {
                currLocation.GetRow = i;
                for (int j = 0; j < io_GameBoard.BoardHeight; j++)
                {
                    currLocation.GetColumn = j;
                    if (io_GameBoard[currLocation] != null)
                    {
                        if (io_GameBoard[currLocation].GetPieceType == 'O' || io_GameBoard[currLocation].GetPieceType == 'U')
                        {
                            if (i + 1 <= io_GameBoard.BoardHeight - 1 && j - 1 >= 0)
                            {
                                nextLocation.GetColumn = j - 1;
                                nextLocation.GetRow = i + 1;
                                eatLocation.GetColumn = j - 2;
                                eatLocation.GetRow = i + 2;
                                eaterPiece = new Piece(io_GameBoard[currLocation].GetPieceType, currLocation);
                                if (i + 2 <= io_GameBoard.BoardHeight - 1 && j - 2 >= 0)
                                {
                                    if (io_GameBoard[eatLocation] == null)
                                    {
                                        io_MadeEatMove = eaterPiece.TryEat(false, eatLocation, io_GameBoard);
                                    }
                                }

                                if (!io_MadeEatMove)
                                {
                                    if (io_GameBoard[nextLocation] == null)
                                    {
                                        ++numOfAvailableMoves;
                                    }
                                }
                                else
                                {
                                    io_EaterPiece = io_GameBoard[eatLocation];
                                    io_StartLocation = eaterPiece.GetLocation;
                                    break;
                                }
                            }

                            if (i + 1 <= io_GameBoard.BoardHeight - 1 && j + 1 <= io_GameBoard.BoardHeight - 1)
                            {
                                nextLocation.GetColumn = j + 1;
                                nextLocation.GetRow = i + 1;
                                eatLocation.GetColumn = j + 2;
                                eatLocation.GetRow = i + 2;
                                eaterPiece = new Piece(io_GameBoard[currLocation].GetPieceType, currLocation);
                                if (i + 2 <= io_GameBoard.BoardHeight - 1 && j + 2 <= io_GameBoard.BoardHeight - 1)
                                {
                                    if (io_GameBoard[eatLocation] == null)
                                    {
                                        io_MadeEatMove = eaterPiece.TryEat(false, eatLocation, io_GameBoard);
                                    }
                                }

                                if (!io_MadeEatMove)
                                {
                                    if (io_GameBoard[nextLocation] == null)
                                    {
                                        ++numOfAvailableMoves;
                                    }
                                }
                                else
                                {
                                    io_EaterPiece = io_GameBoard[eatLocation];
                                    io_StartLocation = eaterPiece.GetLocation;
                                    break;
                                }
                            }
                        }
                    }

                    if (io_GameBoard[currLocation] != null)
                    {
                        if (io_GameBoard[currLocation].GetPieceType == 'U')
                        {
                            if (i - 1 >= 0 && j - 1 >= 0)
                            {
                                nextLocation.GetColumn = j - 1;
                                nextLocation.GetRow = i - 1;
                                eatLocation.GetColumn = j - 2;
                                eatLocation.GetRow = i - 2;
                                eaterPiece = new Piece(io_GameBoard[currLocation].GetPieceType, currLocation);
                                if (i - 2 >= 0 && j - 2 >= 0)
                                {
                                    if (io_GameBoard[eatLocation] == null)
                                    {
                                        io_MadeEatMove = eaterPiece.TryEat(false, eatLocation, io_GameBoard);
                                    }
                                }

                                if (!io_MadeEatMove)
                                {
                                    if (io_GameBoard[nextLocation] == null)
                                    {
                                        ++numOfAvailableMoves;
                                    }
                                }
                                else
                                {
                                    io_EaterPiece = io_GameBoard[eatLocation];
                                    io_StartLocation = eaterPiece.GetLocation;
                                    break;
                                }
                            }

                            if (i - 1 >= 0 && j + 1 <= io_GameBoard.BoardHeight - 1)
                            {
                                nextLocation.GetColumn = j + 1;
                                nextLocation.GetRow = i - 1;
                                eatLocation.GetColumn = j + 2;
                                eatLocation.GetRow = i - 2;
                                eaterPiece = new Piece(io_GameBoard[currLocation].GetPieceType, currLocation);

                                if (i - 2 >= 0 && j + 2 <= io_GameBoard.BoardHeight - 1)
                                {
                                    if (io_GameBoard[eatLocation] == null)
                                    {
                                        io_MadeEatMove = eaterPiece.TryEat(false, eatLocation, io_GameBoard);
                                    } 
                                }

                                if (!io_MadeEatMove)
                                {
                                    if (io_GameBoard[nextLocation] == null)
                                    {
                                        ++numOfAvailableMoves;
                                    }
                                }
                                else
                                {
                                    io_EaterPiece = io_GameBoard[eatLocation];
                                    io_StartLocation = eaterPiece.GetLocation;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (io_MadeEatMove)
                {
                    break;
                }
            }

            return numOfAvailableMoves;
        }

        private static Vector2 executeSpecificPcMove(GameBoard io_GameBoard, int i_MoveNumber, ref Vector2 io_StartLocation)
        {
            Vector2 currLocation = new Vector2(0, 0), nextLocation = new Vector2(0, 0);
            Vector2 finalNextLocation = new Vector2(-1, -1);
            int moveCounter = 0;
            string move = string.Empty;

            for (int i = 0; i < io_GameBoard.BoardHeight; i++)
            {
                currLocation.GetRow = i;
                for (int j = 0; j < io_GameBoard.BoardHeight; j++)
                {
                    currLocation.GetColumn = j;
                    if (io_GameBoard[currLocation] != null && io_GameBoard[currLocation].GetGroup == GameLogic.s_currentPlayerGroup)
                    {
                        if (countEmptyLocationesAroundCurrentLocation(i_MoveNumber, out finalNextLocation, ref moveCounter, currLocation, io_GameBoard))
                        {
                            Piece currentPiece = io_GameBoard[currLocation];
                            if (currentPiece.Move(finalNextLocation, io_GameBoard))
                            {
                                io_StartLocation = currLocation;
                                i = io_GameBoard.BoardHeight;
                                break;
                            }
                        }
                    }
                }
            }

            return finalNextLocation;
        }

        private static bool countEmptyLocationesAroundCurrentLocation(int i_MoveNumber, out Vector2 io_FinalLocation, ref int io_MoveCounter, Vector2 i_CurrentLocation, GameBoard io_GameBoard)
        {
            bool result = false;
            char kingOfCurrentGroup = io_GameBoard[i_CurrentLocation].GetKingTypeByGroup();
            int row = i_CurrentLocation.GetRow;
            int column = i_CurrentLocation.GetColumn;
            int signRow = 1;
            int signColumn = 1;
            char typePiece = io_GameBoard[i_CurrentLocation].GetPieceType;

            io_FinalLocation = new Vector2(column, row);
            for (int i = 0; i < 4 && !result; i++)
            {
                io_FinalLocation.GetRow = row;
                io_FinalLocation.GetColumn = column;
                io_FinalLocation.GetRow += 1 * signRow;
                io_FinalLocation.GetColumn += 1 * signColumn;
                if (CheckBorders(io_FinalLocation, io_GameBoard.BoardHeight) && io_GameBoard.IsLocationEmpty(io_FinalLocation))
                {
                    result = ++io_MoveCounter == i_MoveNumber;
                }

                if (i == 1 && typePiece != kingOfCurrentGroup)
                {
                    i += 2;
                }
                else if (i == 1)
                {
                    signRow *= -1;
                }

                signColumn *= -1;
            }

            return result;
        }

        //////////if i_MoveNumber!= -1 so the function run 4 time 
        private static bool countEmptyLocationesAroundCurrentLocation(int i_MoveNumber, out Vector2 i_FinalLocation, ref int io_MoveCounter, char i_TypePiece, Vector2 i_CurrentLocation, GameBoard io_GameBoard)
        {
            bool result = false;
            char kingOfCurrentGroup = io_GameBoard[i_CurrentLocation].GetKingTypeByGroup();
            int row = i_CurrentLocation.GetRow;
            int column = i_CurrentLocation.GetColumn;
            int signRow = +1;
            int signColumn = +1;

            i_FinalLocation = new Vector2(row, column);
            for (int i = 0; i < 4 && !result; i++)
            {
                i_FinalLocation.GetRow = row;
                i_FinalLocation.GetColumn = column;

                i_FinalLocation.GetRow += 1 * signRow;
                i_FinalLocation.GetColumn += 1 * signColumn;
                if (CheckBorders(i_FinalLocation, io_GameBoard.BoardHeight) && io_GameBoard.IsLocationEmpty(i_FinalLocation))
                {
                    if (i_MoveNumber != -1 && ++io_MoveCounter == i_MoveNumber)
                    {
                        result = true;
                    }
                }

                if (i == 1 && i_TypePiece != kingOfCurrentGroup)
                {
                    i += 2;
                }
                else if (i == 1)
                {
                    signRow *= -1;
                }

                signColumn *= -1;
            }

            return result;
        }

        public static char WhoWin()
        {
            int GetOScore = GameLogic.OScore;
            int GetXScore = GameLogic.XScore;
            char result = char.MinValue;

            if (GetOScore != GetXScore)
            {
                result = GetOScore > GetXScore ? 'O' : 'X';
            }

            s_OScore = 0;
            s_XScore = 0;
            return result;
        }
    }
}