using System;
using System.Collections;
using System.Collections.Generic;

namespace FiveInRow
{
    public class GameLogic
    {
        private static int _inLine = 5;

        public GameLogic()
        {
        }

        protected internal static string HasWon(string player1Name, string player2Name)
        {
            uint[,] board = Board.BoardArray;
            string searchPlayer1 = MultiplyString(Convert.ToString(Board.Player1Mark), _inLine);
            string searchPlayer2 = MultiplyString(Convert.ToString(Board.Player2Mark), _inLine);

            string row = "";

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    row += Convert.ToString(board.GetValue(i, j));
                    if (row.Contains(searchPlayer1))
                    {
                        Console.WriteLine("Player 1");
                        return player1Name;
                    }
                    else if (row.Contains(searchPlayer2))
                    {
                        Console.WriteLine("Player 2");
                        return player2Name;
                    }
                }

                row = "";
            }

            string col = "";

            for (int i = 0; i < board.GetLength(1); i++)
            {
                for (int j = 0; j < board.GetLength(0); j++)
                {
                    col += Convert.ToString(board.GetValue(j, i));
                    if (col.Contains(searchPlayer1))
                    {
                        Console.WriteLine("Player 1");
                        return player1Name;
                    }
                    else if (col.Contains(searchPlayer2))
                    {
                        Console.WriteLine("Player 2");
                        return player2Name;
                    }
                }

                col = "";
            }

            int heightRow = board.GetLength(0);
            int widthCol = board.GetLength(1);

            string diagonalRight = "";

            // looping thru two dimensional array diagonally (/ direction)
            for (int k = 0; k <= heightRow + widthCol - 2; k++)
            {
                for (int j = 0; j <= k; j++)
                {
                    int i = k - j;
                    if (i < heightRow && j < widthCol)
                    {
                        diagonalRight += Convert.ToString(board.GetValue(i, j));
                        if (diagonalRight.Contains(searchPlayer1))
                        {
                            Console.WriteLine("Player 1");
                            return player1Name;
                        }
                        else if (diagonalRight.Contains(searchPlayer2))
                        {
                            Console.WriteLine("Player 2");
                            return player2Name;
                        }
                    }
                }

                diagonalRight = "";
            }

            string diagonalLeft = "";

            // looping thru two dimensional array diagonally (\ direction)
            for (int n = -heightRow; n <= widthCol; n++)
            {
                for (int i = 0; i < heightRow; i++)
                {
                    if ((i - n >= 0) && (i - n < widthCol))
                    {
                        diagonalLeft += Convert.ToString(board.GetValue(i, i - n));
                        if (diagonalLeft.Contains(searchPlayer1))
                        {
                            Console.WriteLine("Player 1");
                            return player1Name;
                        }
                        else if (diagonalLeft.Contains(searchPlayer2))
                        {
                            Console.WriteLine("Player 2");
                            return player2Name;
                        }
                    }
                }
                diagonalLeft = "";
            }

            return null;
        }

        protected internal static bool IsBoardFull()
        {
            uint[,] board = Board.BoardArray;
            foreach (var cell in board)
            {
                if (cell == 0)
                {
                    return false;
                }
            }

            return true;
        }

        protected internal static (int, int) AiMove()
        {
            List<(int x, int y)> fourOpen = new List<(int x, int y)>();
            List<(int x, int y)> fourHalfOpen = new List<(int x, int y)>();
            List<(int x, int y)> threeOpen = new List<(int x, int y)>();
            List<(int x, int y)> threeHalfOpen = new List<(int x, int y)>();
            List<(int x, int y)> twoOpen = new List<(int x, int y)>();
            List<(int x, int y)> twoHalfOpen = new List<(int x, int y)>();
            List<(int x, int y)> one = new List<(int x, int y)>();
            
            return (0, 0);
        }

        private static (int, int) GetFourOpen()
        {
            
            return (0, 0);
        }
        
        /// <summary>
        /// Searching empty fields (EmptyCell) in line where AI can put they mark.
        /// </summary>
        /// <param name="noMarks">number marks in line,</param>
        /// <param name="mark">kind of mark.</param>
        /// <returns>Returns list of fields (EmptyCell) where AI can put they mark.</returns>
        private static List<(int, int)> FindMarkInLine(int noMarks, int mark)
        {
            
            
            List<(int x, int y)> test = new List<(int x, int y)>();
            return test;
        }
        
        protected internal static (int, int) AiMoveToWin()
        {
            
            return (0, 0);
        }

        private static string MultiplyString(string multiplier, int multiplicand)
        {
            string tmpStirng = "";

            for (int i = 0; i < multiplicand; i++)
            {
                tmpStirng += multiplier;
            }

            return tmpStirng;
        }
    }
}