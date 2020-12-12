using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gtk;

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

            // looping thru two dimensional array horizontally
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

            // looping thru two dimensional array vertically
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
            for (int i = -heightRow; i <= widthCol; i++)
            {
                for (int j = 0; j < heightRow; j++)
                {
                    if ((j - i >= 0) && (j - i < widthCol))
                    {
                        diagonalLeft += Convert.ToString(board.GetValue(j, j - i));
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

        protected internal static (int, int) FindMarkInHorizontalLine()
        {
            List<(int x, int y)> fourMarksHorizontal = new List<(int x, int y)>();
            List<(int x, int y)> threeMarksHorizontal = new List<(int x, int y)>();
            List<(int x, int y)> twoMarksHorizontal = new List<(int x, int y)>();

            SortedSet<(int x, int y)> tmpSet = new SortedSet<(int x, int y)>();

            uint[,] board = Board.BoardArray;

            // Possible positions to move in horizontal line
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board.GetLength(1) - 1 > j)
                    {
                        if ((uint) board.GetValue(i, j) == Board.Player1Mark &&
                            (uint) board.GetValue(i, j + 1) == Board.Player1Mark)
                        {
                            tmpSet.Add((i, j));
                            tmpSet.Add((i, j + 1));
                        }

                        if ((uint) board.GetValue(i, j + 1) != Board.Player1Mark || board.GetLength(1) - 2 == j)
                        {
                            if (tmpSet.Count > 0)
                            {
                                switch (tmpSet.Count)
                                {
                                    case 2:
                                        if (tmpSet.First().y > 0 &&
                                            (uint) board.GetValue(tmpSet.First().x, tmpSet.First().y - 1) ==
                                            Board.EmptyCell)
                                        {
                                            twoMarksHorizontal.Add((tmpSet.First().x, tmpSet.First().y - 1));
                                        }

                                        if (tmpSet.Last().y < board.GetLength(1) - 1 &&
                                            (uint) board.GetValue(tmpSet.Last().x, tmpSet.Last().y + 1) ==
                                            Board.EmptyCell)
                                        {
                                            twoMarksHorizontal.Add((tmpSet.Last().x, tmpSet.Last().y + 1));
                                        }

                                        tmpSet.Clear();
                                        break;
                                    case 3:
                                        if (tmpSet.First().y > 0 &&
                                            (uint) board.GetValue(tmpSet.First().x, tmpSet.First().y - 1) ==
                                            Board.EmptyCell)
                                        {
                                            threeMarksHorizontal.Add((tmpSet.First().x, tmpSet.First().y - 1));
                                        }

                                        if (tmpSet.Last().y < board.GetLength(1) - 1 &&
                                            (uint) board.GetValue(tmpSet.Last().x, tmpSet.Last().y + 1) ==
                                            Board.EmptyCell)
                                        {
                                            threeMarksHorizontal.Add((tmpSet.Last().x, tmpSet.Last().y + 1));
                                        }

                                        tmpSet.Clear();
                                        break;
                                    case 4:
                                        if (tmpSet.First().y > 0 &&
                                            (uint) board.GetValue(tmpSet.First().x, tmpSet.First().y - 1) ==
                                            Board.EmptyCell)
                                        {
                                            fourMarksHorizontal.Add((tmpSet.First().x, tmpSet.First().y - 1));
                                        }

                                        if (tmpSet.Last().y < board.GetLength(1) - 1 &&
                                            (uint) board.GetValue(tmpSet.Last().x, tmpSet.Last().y + 1) ==
                                            Board.EmptyCell)
                                        {
                                            fourMarksHorizontal.Add((tmpSet.Last().x, tmpSet.Last().y + 1));
                                        }

                                        tmpSet.Clear();
                                        break;
                                    default:
                                        Console.WriteLine("Nothing to do.");
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            // Console.WriteLine("twoOpen " + twoMarks.Count);
            Console.WriteLine("twoMarks: ");
            foreach (var tup in twoMarksHorizontal)
            {
                Console.Write(tup + " ");
            }

            Console.WriteLine();
            Console.WriteLine("threeMarks: ");
            foreach (var tup in threeMarksHorizontal)
            {
                Console.Write(tup + " ");
            }

            Console.WriteLine();
            Console.WriteLine("fourMarks: ");
            foreach (var tup in fourMarksHorizontal)
            {
                Console.Write(tup + " ");
            }

            Console.WriteLine();

            return (0, 0);
        }


        protected internal static (int, int) FindMarkInVerticalLine()
        {
            List<(int x, int y)> fourMarksVertical = new List<(int x, int y)>();
            List<(int x, int y)> threeMarksVertical = new List<(int x, int y)>();
            List<(int x, int y)> twoMarksVertical = new List<(int x, int y)>();

            SortedSet<(int x, int y)> tmpSet = new SortedSet<(int x, int y)>();

            uint[,] board = Board.BoardArray;

            // Possible positions to move in vertical line
            for (int i = 0; i < board.GetLength(1); i++)
            {
                for (int j = 0; j < board.GetLength(0); j++)
                {
                    if (board.GetLength(0) - 1 > j)
                    {
                        if ((uint) board.GetValue(j, i) == Board.Player1Mark &&
                            (uint) board.GetValue(j + 1, i) == Board.Player1Mark)
                        {
                            tmpSet.Add((j, i));
                            tmpSet.Add((j + 1, i));
                        }

                        if ((uint) board.GetValue(j + 1, i) != Board.Player1Mark || board.GetLength(0) - 2 == j)
                        {
                            if (tmpSet.Count > 0)
                            {
                                switch (tmpSet.Count)
                                {
                                    case 2:
                                        if (tmpSet.First().x > 0 &&
                                            (uint) board.GetValue(tmpSet.First().x - 1, tmpSet.First().y) ==
                                            Board.EmptyCell)
                                        {
                                            twoMarksVertical.Add((tmpSet.First().x - 1, tmpSet.First().y));
                                        }

                                        if (tmpSet.Last().x < board.GetLength(0) - 1 &&
                                            (uint) board.GetValue(tmpSet.Last().x + 1, tmpSet.Last().y) ==
                                            Board.EmptyCell)
                                        {
                                            twoMarksVertical.Add((tmpSet.Last().x + 1, tmpSet.Last().y));
                                        }

                                        tmpSet.Clear();
                                        break;
                                    case 3:
                                        if (tmpSet.First().x > 0 &&
                                            (uint) board.GetValue(tmpSet.First().x - 1, tmpSet.First().y) ==
                                            Board.EmptyCell)
                                        {
                                            threeMarksVertical.Add((tmpSet.First().x - 1, tmpSet.First().y));
                                        }

                                        if (tmpSet.Last().x < board.GetLength(0) - 1 &&
                                            (uint) board.GetValue(tmpSet.Last().x + 1, tmpSet.Last().y) ==
                                            Board.EmptyCell)
                                        {
                                            threeMarksVertical.Add((tmpSet.Last().x + 1, tmpSet.Last().y));
                                        }

                                        tmpSet.Clear();
                                        break;
                                    case 4:
                                        if (tmpSet.First().x > 0 &&
                                            (uint) board.GetValue(tmpSet.First().x - 1, tmpSet.First().y) ==
                                            Board.EmptyCell)
                                        {
                                            fourMarksVertical.Add((tmpSet.First().x - 1, tmpSet.First().y));
                                        }

                                        if (tmpSet.Last().x < board.GetLength(0) - 1 &&
                                            (uint) board.GetValue(tmpSet.Last().x + 1, tmpSet.Last().y) ==
                                            Board.EmptyCell)
                                        {
                                            fourMarksVertical.Add((tmpSet.Last().x + 1, tmpSet.Last().y));
                                        }

                                        tmpSet.Clear();
                                        break;
                                    default:
                                        Console.WriteLine("Nothing to do.");
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            // Console.WriteLine("twoOpen " + twoMarks.Count);
            Console.WriteLine("twoMarks: ");
            foreach (var tup in twoMarksVertical)
            {
                Console.Write(tup + " ");
            }

            Console.WriteLine();
            Console.WriteLine("threeMarks: ");
            foreach (var tup in threeMarksVertical)
            {
                Console.Write(tup + " ");
            }

            Console.WriteLine();
            Console.WriteLine("fourMarks: ");
            foreach (var tup in fourMarksVertical)
            {
                Console.Write(tup + " ");
            }

            Console.WriteLine();

            return (0, 0);
        }


        protected internal static (int, int) FindMarkInDiagonalRightLine()
        {
            uint[,] board = Board.BoardArray;

            List<(int x, int y)> fourMarksDiagonalRight = new List<(int x, int y)>();
            List<(int x, int y)> threeMarksDiagonalRight = new List<(int x, int y)>();
            List<(int x, int y)> twoMarksDiagonalRight = new List<(int x, int y)>();

            List<(int x, int y, uint value)> tmpList = new List<(int x, int y, uint value)>();
            List<(int x, int y)> tmpListPos = new List<(int x, int y)>();

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
                        tmpList.Add((i, j, (uint) board.GetValue(i, j)));
                    }
                }

                if (diagonalRight.Length > _inLine - 1)
                {
                    for (int i = 0; i < tmpList.Count; i++)
                    {
                        if (tmpList.Count - 1 > i)
                        {
                            if (tmpList.ElementAt(i).value == Board.Player1Mark &&
                                tmpList.ElementAt(i + 1).value == Board.Player1Mark)
                            {
                                tmpListPos.Add((tmpList.ElementAt(i).x, tmpList.ElementAt(i).y));
                                tmpListPos.Add((tmpList.ElementAt(i + 1).x, tmpList.ElementAt(i + 1).y));
                            }

                            if (tmpList.ElementAt(i + 1).value != Board.Player1Mark || tmpList.Count - 2 == i)
                            {
                                tmpListPos = tmpListPos.Distinct().ToList();


                                if (tmpListPos.Count > 0)
                                {
                                    switch (tmpListPos.Count)
                                    {
                                        case 2:
                                            if (tmpListPos.First().y > 0 &&
                                                tmpListPos.First().x < board.GetLength(0) - 1 &&
                                                board[tmpListPos.First().x + 1, tmpListPos.First().y - 1] ==
                                                Board.EmptyCell)
                                            {
                                                twoMarksDiagonalRight.Add((tmpListPos.First().x + 1,
                                                    tmpListPos.First().y - 1));
                                            }

                                            if (tmpListPos.Last().x > 0 &&
                                                tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                                board[tmpListPos.Last().x - 1, tmpListPos.Last().y + 1] ==
                                                Board.EmptyCell)
                                            {
                                                twoMarksDiagonalRight.Add((tmpListPos.Last().x - 1,
                                                    tmpListPos.Last().y + 1));
                                            }

                                            tmpListPos.Clear();
                                            break;
                                        case 3:
                                            Console.WriteLine(tmpListPos.First().y);
                                            if (tmpListPos.First().y > 0 &&
                                                tmpListPos.First().x < board.GetLength(0) - 1 &&
                                                board[tmpListPos.First().x + 1, tmpListPos.First().y - 1] ==
                                                Board.EmptyCell)
                                            {
                                                threeMarksDiagonalRight.Add((tmpListPos.First().x + 1,
                                                    tmpListPos.First().y - 1));
                                            }

                                            if (tmpListPos.Last().x > 0 &&
                                                tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                                board[tmpListPos.Last().x - 1, tmpListPos.Last().y + 1] ==
                                                Board.EmptyCell)
                                            {
                                                threeMarksDiagonalRight.Add((tmpListPos.Last().x - 1,
                                                    tmpListPos.Last().y + 1));
                                            }

                                            tmpListPos.Clear();
                                            break;
                                        case 4:
                                            Console.WriteLine(tmpListPos.First().y);
                                            if (tmpListPos.First().y > 0 &&
                                                tmpListPos.First().x < board.GetLength(0) - 1 &&
                                                board[tmpListPos.First().x + 1, tmpListPos.First().y - 1] ==
                                                Board.EmptyCell)
                                            {
                                                fourMarksDiagonalRight.Add((tmpListPos.First().x + 1,
                                                    tmpListPos.First().y - 1));
                                            }

                                            if (tmpListPos.Last().x > 0 &&
                                                tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                                board[tmpListPos.Last().x - 1, tmpListPos.Last().y + 1] ==
                                                Board.EmptyCell)
                                            {
                                                fourMarksDiagonalRight.Add((tmpListPos.Last().x - 1,
                                                    tmpListPos.Last().y + 1));
                                            }

                                            tmpListPos.Clear();
                                            break;
                                        default:
                                            Console.WriteLine("Nothing to do.");
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    
                    tmpListPos.Clear();
                }

                diagonalRight = "";
                tmpList.Clear();
            }

            // Console.WriteLine("twoOpen " + twoMarks.Count);
            Console.WriteLine("twoMarks: ");
            foreach (var tup in twoMarksDiagonalRight)
            {
                Console.Write(tup + " ");
            }

            Console.WriteLine();
            Console.WriteLine("threeMarks: ");
            foreach (var tup in threeMarksDiagonalRight)
            {
                Console.Write(tup + " ");
            }

            Console.WriteLine();
            Console.WriteLine("fourMarks: ");
            foreach (var tup in fourMarksDiagonalRight)
            {
                Console.Write(tup + " ");
            }

            Console.WriteLine();

            return (0, 0);
        }
        
        
        protected internal static (int, int) FindMarkInDiagonalLeftLine()
        {
            uint[,] board = Board.BoardArray;

            List<(int x, int y)> fourMarksDiagonalLeft = new List<(int x, int y)>();
            List<(int x, int y)> threeMarksDiagonalLeft = new List<(int x, int y)>();
            List<(int x, int y)> twoMarksDiagonalLeft = new List<(int x, int y)>();

            List<(int x, int y, uint value)> tmpList = new List<(int x, int y, uint value)>();
            List<(int x, int y)> tmpListPos = new List<(int x, int y)>();

            int heightRow = board.GetLength(0);
            int widthCol = board.GetLength(1);

            string diagonalLeft = "";
            
            for (int i = -heightRow; i <= widthCol; i++)
            {
                for (int j = 0; j < heightRow; j++)
                {
                    if ((j - i >= 0) && (j - i < widthCol))
                    {
                        diagonalLeft += Convert.ToString(board.GetValue(j, j - i));
                        tmpList.Add((j, j - i, (uint) board.GetValue(j, j - i)));
                    }
                }
                
                if (diagonalLeft.Length > _inLine - 1)
                {
                    for (int k = 0; k < tmpList.Count; k++)
                    {
                        if (tmpList.Count - 1 > k)
                        {
                            if (tmpList.ElementAt(k).value == Board.Player1Mark &&
                                tmpList.ElementAt(k + 1).value == Board.Player1Mark)
                            {
                                tmpListPos.Add((tmpList.ElementAt(k).x, tmpList.ElementAt(k).y));
                                tmpListPos.Add((tmpList.ElementAt(k + 1).x, tmpList.ElementAt(k + 1).y));
                            }

                            if (tmpList.ElementAt(k + 1).value != Board.Player1Mark || tmpList.Count - 2 == k)
                            {
                                tmpListPos = tmpListPos.Distinct().ToList();

                                if (tmpListPos.Count > 0)
                                {
                                    switch (tmpListPos.Count)
                                    {
                                        case 2:
                                            Console.WriteLine("Poz: " + tmpListPos.First().x + ", " + tmpListPos.First().y);
                                            if (tmpListPos.First().x > 0 &&
                                                tmpListPos.First().y > 0 &&
                                                board[tmpListPos.First().x - 1, tmpListPos.First().y - 1] ==
                                                Board.EmptyCell)
                                            {
                                                twoMarksDiagonalLeft.Add((tmpListPos.First().x - 1,
                                                    tmpListPos.First().y - 1));
                                            }
                
                                            if (tmpListPos.Last().x < board.GetLength(0) - 1 &&
                                                tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                                board[tmpListPos.Last().x + 1, tmpListPos.Last().y + 1] ==
                                                Board.EmptyCell)
                                            {
                                                twoMarksDiagonalLeft.Add((tmpListPos.Last().x + 1,
                                                    tmpListPos.Last().y + 1));
                                            }
                
                                            tmpListPos.Clear();
                                            break;
                                        case 3:
                                            Console.WriteLine("Poz: " + tmpListPos.First().x + ", " + tmpListPos.First().y);
                                            if (tmpListPos.First().x > 0 &&
                                                tmpListPos.First().y > 0 &&
                                                board[tmpListPos.First().x - 1, tmpListPos.First().y - 1] ==
                                                Board.EmptyCell)
                                            {
                                                threeMarksDiagonalLeft.Add((tmpListPos.First().x - 1,
                                                    tmpListPos.First().y - 1));
                                            }
                
                                            if (tmpListPos.Last().x < board.GetLength(0) - 1 &&
                                                tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                                board[tmpListPos.Last().x + 1, tmpListPos.Last().y + 1] ==
                                                Board.EmptyCell)
                                            {
                                                threeMarksDiagonalLeft.Add((tmpListPos.Last().x + 1,
                                                    tmpListPos.Last().y + 1));
                                            }
                
                                            tmpListPos.Clear();
                                            break;
                                        case 4:
                                            Console.WriteLine("Poz: " + tmpListPos.First().x + ", " + tmpListPos.First().y);
                                            if (tmpListPos.First().x > 0 &&
                                                tmpListPos.First().y > 0 &&
                                                board[tmpListPos.First().x - 1, tmpListPos.First().y - 1] ==
                                                Board.EmptyCell)
                                            {
                                                fourMarksDiagonalLeft.Add((tmpListPos.First().x - 1,
                                                    tmpListPos.First().y - 1));
                                            }
                
                                            if (tmpListPos.Last().x < board.GetLength(0) - 1 &&
                                                tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                                board[tmpListPos.Last().x + 1, tmpListPos.Last().y + 1] ==
                                                Board.EmptyCell)
                                            {
                                                fourMarksDiagonalLeft.Add((tmpListPos.Last().x + 1,
                                                    tmpListPos.Last().y + 1));
                                            }
                
                                            tmpListPos.Clear();
                                            break;
                                        default:
                                            Console.WriteLine("Nothing to do.");
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    
                    tmpListPos.Clear();
                }

                diagonalLeft = "";
                tmpList.Clear();
            }
            
            // Console.WriteLine("twoOpen " + twoMarks.Count);
            Console.WriteLine("twoMarks: ");
            foreach (var tup in twoMarksDiagonalLeft)
            {
                Console.Write(tup + " ");
            }

            Console.WriteLine();
            Console.WriteLine("threeMarks: ");
            foreach (var tup in threeMarksDiagonalLeft)
            {
                Console.Write(tup + " ");
            }

            Console.WriteLine();
            Console.WriteLine("fourMarks: ");
            foreach (var tup in fourMarksDiagonalLeft)
            {
                Console.Write(tup + " ");
            }

            Console.WriteLine();
            
            return (0, 0);
        }


        /// <summary>
        /// Searching empty fields (EmptyCell) in line where AI can put they mark.
        /// </summary>
        /// <param name="noMarks">number of marks in line,</param>
        /// <param name="mark">kind of mark.</param>
        /// <returns>Returns list of fields (EmptyCell) where AI can put they mark.</returns>
        public static List<(int, int)> FindMarkInLine(int noMarks, int mark)
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