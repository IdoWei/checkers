using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Game;
using Vector;
using pieces;
using Logic;

namespace DamkaForm
{
    public class FormGame : Form
    {
        private delegate void IsPcMoveDelegate(Point location);
        ////private event IsPcMoveDelegate pcMoves;
        private const int k_SmallGameBoardFormSize = 300;
        private const int k_MediumGameBoardFormSize = 400;
        private const int k_LargeGameBoardFormSize = 500;
        //// saves all pieces on the board
        public readonly List<Piece> m_Pieces = new List<Piece>();
        private readonly Label PlayerOneName = new Label();
        private readonly Label PlayerTwoName = new Label();
        private readonly GameBoard.eGameboardSizes r_GameBoardSize;
        //// Game board buttons array
        private ButtonWithLocation[,] GameBoardButtons;
        private Label PlayerOneScore = new Label();
        private Label PlayerTwoScore = new Label();
        private bool m_IsButtonClicked = false;
        private ButtonWithLocation ClickedButton;
        private bool m_IsPlayerOneTurn = true;
        private GameBoard m_GameBoard;
        private bool m_HasAnotherEatMove, m_IsPcMove = false, m_IsMoveLegal = false, m_IsPcHasAvailableMoves = false, m_Is2PlayersMode;
        private char m_LastTurnGroup;

        public FormGame(string io_PlayerOneName, string io_PlayerTwoName, GameBoard.eGameboardSizes io_GameBoardSize, GameBoard io_GameBoard, bool i_Is2PlayersMode)
        {
            m_Is2PlayersMode = i_Is2PlayersMode;
            PlayerOneName.Text = string.Format("{0} :", io_PlayerOneName);
            PlayerTwoName.Text = string.Format("{0} :", io_PlayerTwoName);
            r_GameBoardSize = io_GameBoardSize;
            this.Text = string.Format("{0}X{0} Checkers", (int)io_GameBoardSize);
            this.StartPosition = FormStartPosition.CenterScreen;
            initBoardSize(io_GameBoardSize);
            m_GameBoard = io_GameBoard;
            m_GameBoard.CleanBoard();
            m_GameBoard.Initial();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            m_GameBoard.ReportEatenEventHandler += reportEaten;
            m_GameBoard.ReportBecameKingEventHandler += reportBecomeKing;
            m_GameBoard.ReportMovedEventHandler += reportMoved;
            GameLogic.ReportScoreChengeEventHandler += scoreChanged;
        }

        private void GameBoardButtons_FirstClick(object sender, EventArgs e)
        {
            ButtonWithLocation button = sender as ButtonWithLocation;
            if (GameLogic.CurrentPlayerGroup.ToString() == button.Text || (GameLogic.CurrentPlayerGroup.ToString() == "X" && button.Text == "K") || (GameLogic.CurrentPlayerGroup.ToString() == "O" && button.Text == "U"))
            {
                //// if blue color Button clicked set white color
                if (m_IsButtonClicked)
                {
                    ClickedButton.Click += new EventHandler(GameBoardButtons_FirstClick);
                    ClickedButton.Click -= new EventHandler(GameBoardButtons_SecondClick);
                    ClickedButton.BackColor = Color.White;
                }

                button.BackColor = Color.CadetBlue;
                button.Click -= new EventHandler(GameBoardButtons_FirstClick);
                button.Click += new EventHandler(GameBoardButtons_SecondClick);
                m_IsButtonClicked = true;
                ClickedButton = button;
            }
        }

        private void GameBoardButtons_SecondClick(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (GameLogic.CurrentPlayerGroup.ToString() == button.Text)
            {
                button.BackColor = Color.White;
                button.Click += new EventHandler(GameBoardButtons_FirstClick);
                button.Click -= new EventHandler(GameBoardButtons_SecondClick);
                m_IsButtonClicked = false;
                ClickedButton = null;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            PlayerOneScore.Text = "0";
            PlayerTwoScore.Text = "0";
            this.Controls.Add(PlayerOneName);
            PlayerOneName.Location = new Point(70, 10);
            PlayerOneName.AutoSize = true;
            this.Controls.Add(PlayerOneScore);
            PlayerOneScore.Location = new Point(70 + PlayerOneName.Width, 10);
            PlayerOneScore.AutoSize = true;
            this.Controls.Add(PlayerTwoScore);
            PlayerTwoScore.Location = new Point(this.Width - 100, 10);
            PlayerTwoScore.AutoSize = true;
            this.Controls.Add(PlayerTwoName);
            PlayerTwoName.AutoSize = true;
            PlayerTwoName.Location = new Point(this.Width - PlayerTwoName.Width - 100, 10);
            initGameBoardButtons(r_GameBoardSize);
        }

        private void initGameBoardButtons(GameBoard.eGameboardSizes i_GameBoardSize)
        {
            int currentLocationTop, currentLocationLeft, leftLocationSaver;
            Vector2 location;

            GameBoardButtons = new ButtonWithLocation[(int)i_GameBoardSize, (int)i_GameBoardSize];
            getTopAndLeftInitButtonValue(out currentLocationTop, out currentLocationLeft, i_GameBoardSize);
            leftLocationSaver = currentLocationLeft;
            for (int i = 0; i < (int)i_GameBoardSize; i++)
            {
                for (int j = 0; j < (int)i_GameBoardSize; j++)
                {
                    location = new Vector2(j, i);
                    GameBoardButtons[i, j] = new ButtonWithLocation(i, j);
                    this.Controls.Add(GameBoardButtons[i, j]);
                    GameBoardButtons[i, j].AutoSize = true;
                    GameBoardButtons[i, j].Size = new Size(35, 35);
                    GameBoardButtons[i, j].Location = new Point(currentLocationLeft, currentLocationTop);
                    currentLocationLeft += 35;
                    if (((i % 2 == 0) && (j % 2 == 0)) || ((i % 2 == 1) && (j % 2 == 1)))
                    {
                        GameBoardButtons[i, j].Enabled = false;
                        GameBoardButtons[i, j].BackColor = Color.Gray;
                    }
                    else
                    {
                        GameBoardButtons[i, j].BackColor = Color.BurlyWood;
                        GameBoardButtons[i, j].ForeColor = Color.BurlyWood;
                    }

                    if (i < ((int)i_GameBoardSize / 2) - 1 && GameBoardButtons[i, j].Enabled == true)
                    {
                        GameBoardButtons[i, j].BackgroundImage = Image.FromFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\white_man.png");
                        GameBoardButtons[i, j].BackgroundImageLayout = ImageLayout.Stretch;
                        GameBoardButtons[i, j].Text = "O";
                        GameBoardButtons[i, j].TextAlign = ContentAlignment.MiddleCenter;
                        GameBoardButtons[i, j].Click += new EventHandler(GameBoardButtons_FirstClick);
                    }
                    else if (i > ((int)i_GameBoardSize / 2) && GameBoardButtons[i, j].Enabled == true)
                    {
                        GameBoardButtons[i, j].BackgroundImage = Image.FromFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\black_man.png");
                        GameBoardButtons[i, j].BackgroundImageLayout = ImageLayout.Stretch;
                        GameBoardButtons[i, j].Text = "X";
                        GameBoardButtons[i, j].TextAlign = ContentAlignment.MiddleCenter;
                        GameBoardButtons[i, j].Click += new EventHandler(GameBoardButtons_FirstClick);
                    }
                    else
                    {
                        GameBoardButtons[i, j].Click += new EventHandler(GameBoardButtons_ClickOnEmptyCell);
                    }
                }

                currentLocationLeft = leftLocationSaver;
                currentLocationTop += 35;
            }
        }

        private void reportMoved(Piece i_MovedPiece)
        {
            int row, col;

            row = i_MovedPiece.GetLocation.GetRow;
            col = i_MovedPiece.GetLocation.GetColumn;
            if (i_MovedPiece.GetPieceType == 'X')
            {
                GameBoardButtons[row, col].BackgroundImage = Image.FromFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\black_man.png");
                GameBoardButtons[row, col].BackgroundImageLayout = ImageLayout.Stretch;
            }
            else if (i_MovedPiece.GetPieceType == 'K')
            {
                GameBoardButtons[row, col].BackgroundImage = Image.FromFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\black_cro.png");
                GameBoardButtons[row, col].BackgroundImageLayout = ImageLayout.Stretch;
            }
            else if (i_MovedPiece.GetPieceType == 'O')
            {
                GameBoardButtons[row, col].BackgroundImage = Image.FromFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\white_man.png");
                GameBoardButtons[row, col].BackgroundImageLayout = ImageLayout.Stretch;
            }
            else if (i_MovedPiece.GetPieceType == 'U')
            {
                GameBoardButtons[row, col].BackgroundImage = Image.FromFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\white_cro.png");
                GameBoardButtons[row, col].BackgroundImageLayout = ImageLayout.Stretch;
            }

            GameBoardButtons[row, col].Text = i_MovedPiece.GetPieceType.ToString();
            GameBoardButtons[row, col].TextAlign = ContentAlignment.MiddleCenter;
            GameBoardButtons[row, col].Click += new EventHandler(GameBoardButtons_FirstClick);
            GameBoardButtons[row, col].Click -= new EventHandler(GameBoardButtons_ClickOnEmptyCell);
        }

        private void reportBecomeKing(Piece i_NewKing)
        {
            int row, col;

            row = i_NewKing.GetLocation.GetRow;
            col = i_NewKing.GetLocation.GetColumn;
            if (i_NewKing.GetPieceType == 'K')
            {
                GameBoardButtons[row, col].BackgroundImage = Image.FromFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\black_cro.png");
                GameBoardButtons[row, col].BackgroundImageLayout = ImageLayout.Stretch;
            }
            else if (i_NewKing.GetPieceType == 'U')
            {
                GameBoardButtons[row, col].BackgroundImage = Image.FromFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\white.cro.png");
                GameBoardButtons[row, col].BackgroundImageLayout = ImageLayout.Stretch;
            }

            GameBoardButtons[row, col].Text = i_NewKing.GetPieceType.ToString();
            GameBoardButtons[row, col].TextAlign = ContentAlignment.MiddleCenter;
            GameBoardButtons[row, col].Click += new EventHandler(GameBoardButtons_FirstClick);
            GameBoardButtons[row, col].Click -= new EventHandler(GameBoardButtons_ClickOnEmptyCell);
        }

        private void reportEaten(Vector2 i_EatenPieceLocation)
        {
            int row, col;

            row = i_EatenPieceLocation.GetRow;
            col = i_EatenPieceLocation.GetColumn;
            GameBoardButtons[row, col].Text = string.Empty;
            GameBoardButtons[row, col].BackgroundImage = null;
            GameBoardButtons[row, col].Click -= new EventHandler(GameBoardButtons_FirstClick);
            GameBoardButtons[row, col].Click += new EventHandler(GameBoardButtons_ClickOnEmptyCell);
        }

        private void getTopAndLeftInitButtonValue(out int o_Top, out int o_Left, GameBoard.eGameboardSizes i_GameBoardSize)
        {
            switch (i_GameBoardSize)
            {
                case GameBoard.eGameboardSizes.Small:
                    o_Left = 35;
                    o_Top = 40;
                    break;
                case GameBoard.eGameboardSizes.Medium:
                    o_Left = 45;
                    o_Top = 50;
                    break;
                case GameBoard.eGameboardSizes.Large:
                    o_Left = 60;
                    o_Top = 60;
                    break;
                default:
                    o_Left = 60;
                    o_Top = 60;
                    break;
            }
        }

        private void GameBoardButtons_ClickOnEmptyCell(object sender, EventArgs e)
        {
            bool printMustEatError = false;
            pieces.Piece currentPiece;
            ButtonWithLocation button = sender as ButtonWithLocation;
            string winner = string.Empty, winMessage = string.Empty;
            char whoWon;
            Vector2 pcMoveNewLocation = new Vector2(-1, -1), pcMoveStartLocation = new Vector2(-1, -1);

            if (ClickedButton != null)
            {
                currentPiece = new pieces.Piece(ClickedButton.Text[0], ClickedButton.VectorLocation);
                m_IsMoveLegal = Logic.GameLogic.CheckMove(currentPiece, button.VectorLocation, m_GameBoard, ref m_HasAnotherEatMove, ref printMustEatError, m_IsPcMove, ref pcMoveNewLocation);
                m_LastTurnGroup = Logic.GameLogic.CurrentPlayerGroup;

                if (m_HasAnotherEatMove || printMustEatError)
                {
                    printMustEatMessageBox();
                }
                else if (!m_IsMoveLegal)
                {
                    printIllegalMoveMessageBox();
                    ClickedButton.BackColor = Color.White;
                }

                if (m_IsMoveLegal)
                {
                    Logic.GameLogic.UpdateCurrentTurn(ref m_IsPlayerOneTurn);                  
                    m_Pieces.Add(m_GameBoard[button.VectorLocation]);
                    ClickedButton.Click -= new EventHandler(GameBoardButtons_SecondClick);
                    ClickedButton.Click += new EventHandler(GameBoardButtons_ClickOnEmptyCell);
                    ClickedButton.BackgroundImage = null;
                    ClickedButton.BackColor = Color.BurlyWood;
                    ClickedButton.Text = string.Empty;
                    ClickedButton = null;
                    m_IsButtonClicked = false;
                }

                if (!m_IsPlayerOneTurn)
                {
                    m_IsPlayerOneTurn = !m_IsPlayerOneTurn;
                    if (!m_Is2PlayersMode)
                    {
                        m_IsPcMove = true;
                        pcGameBoardMove();
                        Logic.GameLogic.UpdateCurrentTurn(ref m_IsPlayerOneTurn);
                    }
                }

                if (Logic.GameLogic.IsGameOver(m_GameBoard))
                {
                    if (!Logic.GameLogic.IsATie(m_GameBoard))
                    {
                        whoWon = GameLogic.WhoWin();
                        winner = whoWon == 'X' ? PlayerOneName.Text : PlayerTwoName.Text;
                        winMessage = string.Format("{0} Won!{1}Another Round?", winner, Environment.NewLine);
                    }
                    else
                    {
                        winMessage = string.Format("Tie!{0}Another Round?", Environment.NewLine);
                    }

                    isInterestedInAnotherGame(winMessage);
                }
            }
        }

        private void pcGameBoardMove()
        {
            ButtonWithLocation pcNewButton = null, pcOldButton;
            string winner = string.Empty, winMessage = string.Empty;
            char whoWon;
            Vector2 pcMoveNewLocation = new Vector2(-1, -1), pcMoveStartLocation = new Vector2(-1, -1);
            pcMoveNewLocation = Logic.GameLogic.MakePcRandomMove(m_GameBoard, ref m_IsPcHasAvailableMoves, ref m_HasAnotherEatMove, out pcMoveStartLocation);

            if (!m_IsPcHasAvailableMoves)
            {
                if (!Logic.GameLogic.IsATie(m_GameBoard))
                {
                    whoWon = GameLogic.WhoWin();
                    winner = whoWon == 'X' ? PlayerOneName.Text : PlayerTwoName.Text;
                    winMessage = string.Format("{0} Won!{1}Another Round?", winner, Environment.NewLine);
                }
                else
                {
                    winMessage = string.Format("Tie!{0}Another Round?", Environment.NewLine);
                }

                isInterestedInAnotherGame(winMessage);
            }
            else
            {
                pcNewButton = GameBoardButtons[pcMoveNewLocation.GetRow, pcMoveNewLocation.GetColumn];
                pcOldButton = GameBoardButtons[pcMoveStartLocation.GetRow, pcMoveStartLocation.GetColumn];
                pcOldButton.Click -= new EventHandler(GameBoardButtons_FirstClick);
                pcOldButton.Click += new EventHandler(GameBoardButtons_ClickOnEmptyCell);
                pcNewButton.Click += new EventHandler(GameBoardButtons_FirstClick);
                m_Pieces.Add(m_GameBoard[pcNewButton.VectorLocation]);
                GameBoardButtons[pcMoveStartLocation.GetRow, pcMoveStartLocation.GetColumn].Text = string.Empty;
                GameBoardButtons[pcMoveStartLocation.GetRow, pcMoveStartLocation.GetColumn].BackgroundImage = null;

                m_IsButtonClicked = false;
                Piece currentPiece = m_GameBoard[pcNewButton.VectorLocation];
                if (m_HasAnotherEatMove)
                {
                    while (GameLogic.ExeAllPcEatMovesAvailableToCurrentLocation(m_GameBoard, currentPiece))
                    {
                        pcNewButton.Click -= new EventHandler(GameBoardButtons_FirstClick);
                        pcNewButton.Click += new EventHandler(GameBoardButtons_ClickOnEmptyCell);
                        GameBoardButtons[pcMoveNewLocation.GetRow, pcMoveNewLocation.GetColumn].Text = string.Empty;
                        GameBoardButtons[pcMoveNewLocation.GetRow, pcMoveNewLocation.GetColumn].BackgroundImage = null;
                    }

                    m_Pieces.Add(m_GameBoard[pcNewButton.VectorLocation]);
                    pcNewButton = GameBoardButtons[currentPiece.GetLocation.GetRow, currentPiece.GetLocation.GetColumn];
                    pcNewButton.Click += new EventHandler(GameBoardButtons_FirstClick);
                    pcNewButton.Click += new EventHandler(GameBoardButtons_ClickOnEmptyCell);
                    m_IsPcMove = !m_IsPcMove;
                }

                if (!m_HasAnotherEatMove)
                {
                    if (m_IsMoveLegal)
                    {
                        m_IsPcMove = !m_IsPcMove;
                    }
                }
            }
        }

        private void isInterestedInAnotherGame(string i_Message)
        {
            string caption = string.Empty;
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult dialogResult;

            dialogResult = MessageBox.Show(i_Message, caption, buttons);
            if (dialogResult == DialogResult.Yes)
            {
                PlayerOneScore.Text = 0.ToString();
                PlayerTwoScore.Text = 0.ToString();
                m_GameBoard.CleanBoard();
                m_GameBoard.Initial();
                deleteGameBoardButtons();
                initGameBoardButtons(r_GameBoardSize);
                initData();
            }
            else
            {
                this.Close();
            }
        }

        private void deleteGameBoardButtons()
        {
            for(int i = 0; i < (int)r_GameBoardSize; i++)
            {
                for(int j = 0; j < (int)r_GameBoardSize; j++)
                {
                    if(GameBoardButtons[i, j] != null)
                    {
                        this.Controls.Remove(GameBoardButtons[i, j]);
                    }
                }
            }
        }

        private void initData()
        {
            m_IsMoveLegal = false;
            m_HasAnotherEatMove = false;
            m_IsPcMove = false;
            m_LastTurnGroup = 'X';
            m_IsPlayerOneTurn = true;
            m_IsButtonClicked = false;
            if (ClickedButton != null)
            {
                ClickedButton.BackColor = Color.White;
                ClickedButton.Click += new EventHandler(GameBoardButtons_FirstClick);
                ClickedButton.Click -= new EventHandler(GameBoardButtons_SecondClick);
                ClickedButton = null;
            }
        }

        private void printMustEatMessageBox()
        {
            string message = "You must eat! Try again.";
            string caption = string.Empty;
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult dialogResult;

            dialogResult = MessageBox.Show(message, caption, buttons);
        }

        private void printIllegalMoveMessageBox()
        {
            string message = "Invalid Move! Try again.";
            string caption = string.Empty;
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult dialogResult;

            dialogResult = MessageBox.Show(message, caption, buttons);
        }

        private void initBoardSize(GameBoard.eGameboardSizes i_GameBoardSize)
        {
            int sizeInPixels;

            switch (i_GameBoardSize)
            {
                case GameBoard.eGameboardSizes.Small:
                    sizeInPixels = k_SmallGameBoardFormSize;
                    break;
                case GameBoard.eGameboardSizes.Medium:
                    sizeInPixels = k_MediumGameBoardFormSize;
                    break;
                case GameBoard.eGameboardSizes.Large:
                    sizeInPixels = k_LargeGameBoardFormSize;
                    break;
                default:
                    sizeInPixels = k_LargeGameBoardFormSize;
                    break;
            }

            this.Size = new System.Drawing.Size(sizeInPixels, sizeInPixels);
        }

        private void scoreChanged()
        {
            PlayerOneScore.Text = GameLogic.XScore.ToString();
            PlayerTwoScore.Text = GameLogic.OScore.ToString();
        }
    }
}