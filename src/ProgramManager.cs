using System.Windows;
using System;
using System.Text;
using Game;
using DamkaForm;
using pieces;
using Logic;

public static class ProgramManager
{
    public static void RunProgram()
    {
        FormGame formGame;
        FormLogin formLogin = new FormLogin();
        bool is2PlayersMode;
        GameBoard gameBoard;
        if (formLogin.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            is2PlayersMode = formLogin.Is2PlayersMode();
            gameBoard = new GameBoard((int)formLogin.GameBoardSize);
            formGame = new FormGame(formLogin.UserName1, formLogin.UserName2, formLogin.GameBoardSize, gameBoard, is2PlayersMode);
            formGame.ShowDialog();
        }
    }

    public static void ResetGame(GameBoard io_GameBoard, ref bool o_IsSingleGameOver, ref bool o_IsFirstTurn, ref bool o_IsPlayerOneTurn)
    {
        io_GameBoard.CleanBoard();
        io_GameBoard.Initial();
        o_IsSingleGameOver = false;
        o_IsFirstTurn = o_IsPlayerOneTurn = true;
        GameLogic.CurrentPlayerGroup = 'X';
    }
}
