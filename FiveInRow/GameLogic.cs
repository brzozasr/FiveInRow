using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;

namespace FiveInRow
{
    public class GameLogic
    {
        private static int _inLine = 5;

        private static List<(int x, int y)> _fourMarksHorizontal;
        private static List<(int x, int y)> _threeMarksHorizontal;
        private static List<(int x, int y)> _twoMarksHorizontal;

        private static List<(int x, int y)> _fourMarksVertical;
        private static List<(int x, int y)> _threeMarksVertical;
        private static List<(int x, int y)> _twoMarksVertical;

        private static List<(int x, int y)> _fourMarksDiagonalRight;
        private static List<(int x, int y)> _threeMarksDiagonalRight;
        private static List<(int x, int y)> _twoMarksDiagonalRight;

        private static List<(int x, int y)> _fourMarksDiagonalLeft;
        private static List<(int x, int y)> _threeMarksDiagonalLeft;
        private static List<(int x, int y)> _twoMarksDiagonalLeft;

        private static List<(int, int)> _singleList;

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

        protected internal static (int, int) AiMove()
        {
            if (AiMoveToWin() != (-1, -1))  // Checking for AI win possibility
            {
                Console.WriteLine("WIN" + AiMoveToWin());
                return AiMoveToWin();
            }
            else
            {
                FindMarkInHorizontalLine();
                FindMarkInVerticalLine();
                FindMarkInDiagonalRightLine();
                FindMarkInDiagonalLeftLine();
                SingleCell();

                // Checking for 4 horizontal marks of Player
                if (_fourMarksHorizontal.Count > 0) 
                {
                    // Searching duplicates in list with 4 marks
                    // (it means that between line marks is one field gap)
                    var fourDuplicatesH = _fourMarksHorizontal.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();

                    // If there is a gap returns it
                    if (fourDuplicatesH.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(fourDuplicatesH));
                        return RandomElementFromList(fourDuplicatesH);
                    }
                    
                    // Checking for gap between 4 and 3 horizontal marks of Player
                    if (_fourMarksHorizontal.Count > 0 && _threeMarksHorizontal.Count > 0) 
                    {
                        // Searching duplicates in lists with 4 and 3 marks (it means that between lines is one field gap)
                        var fourAndThreeDuplicateH = _fourMarksHorizontal.Intersect(_threeMarksHorizontal).ToList();;

                        if (fourAndThreeDuplicateH.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(fourAndThreeDuplicateH));
                            return RandomElementFromList(fourAndThreeDuplicateH);
                        }
                    }
                    
                    // Checking for gap between 4 and 2 horizontal marks of Player
                    if (_fourMarksHorizontal.Count > 0 && _twoMarksHorizontal.Count > 0) 
                    {
                        // Searching duplicates in lists with 4 and 2 marks
                        // (it means that between lines is one field gap)
                        var fourAndTwoDuplicateH = _fourMarksHorizontal.Intersect(_twoMarksHorizontal).ToList();;

                        if (fourAndTwoDuplicateH.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(fourAndTwoDuplicateH));
                            return RandomElementFromList(fourAndTwoDuplicateH);
                        }
                    }
                    
                    // Checking for gap between 4 and 1 horizontal marks of Player
                    if (_fourMarksHorizontal.Count > 0 && _singleList.Count > 0) 
                    {
                        // Searching duplicates in lists with 4 and 1 marks
                        // (it means that between lines is one field gap)
                        var fourAndSingleDuplicateH = _fourMarksHorizontal.Intersect(_singleList).ToList();;

                        if (fourAndSingleDuplicateH.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(fourAndSingleDuplicateH));
                            return RandomElementFromList(fourAndSingleDuplicateH);
                        }
                    }
                    
                    Console.WriteLine(_fourMarksHorizontal.First());
                    return _fourMarksHorizontal.First();
                }
                
                // Checking for 4 vertical marks of Player
                if (_fourMarksVertical.Count > 0)  
                {
                    // Searching duplicates in list with 4 marks
                    // (it means that between line marks is one field gap)
                    var fourDuplicatesV = _fourMarksVertical.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();

                    // If there is a gap returns it
                    if (fourDuplicatesV.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(fourDuplicatesV));
                        return RandomElementFromList(fourDuplicatesV);
                    }
                    
                    // Checking for gap between 4 and 3 vertical marks of Player
                    if (_fourMarksVertical.Count > 0 && _threeMarksVertical.Count > 0) 
                    {
                        // Searching duplicates in lists with 4 and 3 marks (it means that between lines is one field gap)
                        var fourAndThreeDuplicateV = _fourMarksVertical.Intersect(_threeMarksVertical).ToList();;

                        if (fourAndThreeDuplicateV.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(fourAndThreeDuplicateV));
                            return RandomElementFromList(fourAndThreeDuplicateV);
                        }
                    }
                    
                    // Checking for gap between 4 and 2 vertical marks of Player
                    if (_fourMarksVertical.Count > 0 && _twoMarksVertical.Count > 0) 
                    {
                        // Searching duplicates in lists with 4 and 2 marks
                        // (it means that between lines is one field gap)
                        var fourAndTwoDuplicateV = _fourMarksVertical.Intersect(_twoMarksVertical).ToList();;

                        if (fourAndTwoDuplicateV.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(fourAndTwoDuplicateV));
                            return RandomElementFromList(fourAndTwoDuplicateV);
                        }
                    }
                    
                    // Checking for gap between 4 and 1 vertical marks of Player
                    if (_fourMarksVertical.Count > 0 && _singleList.Count > 0) 
                    {
                        // Searching duplicates in lists with 4 and 1 marks
                        // (it means that between lines is one field gap)
                        var fourAndSingleDuplicateV = _fourMarksVertical.Intersect(_singleList).ToList();;

                        if (fourAndSingleDuplicateV.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(fourAndSingleDuplicateV));
                            return RandomElementFromList(fourAndSingleDuplicateV);
                        }
                    }
                    
                    Console.WriteLine(_fourMarksVertical.First());
                    return _fourMarksVertical.First();
                }
                
                // Checking for 4 diagonal right marks of Player
                if (_fourMarksDiagonalRight.Count > 0)  
                {
                    // Searching duplicates in list with 4 marks
                    // (it means that between line marks is one field gap)
                    var fourDuplicatesDr = _fourMarksDiagonalRight.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();

                    // If there is a gap returns it
                    if (fourDuplicatesDr.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(fourDuplicatesDr));
                        return RandomElementFromList(fourDuplicatesDr);
                    }
                    
                    // Checking for gap between 4 and 3 diagonal right marks of Player
                    if (_fourMarksDiagonalRight.Count > 0 && _threeMarksDiagonalRight.Count > 0) 
                    {
                        // Searching duplicates in lists with 4 and 3 marks (it means that between lines is one field gap)
                        var fourAndThreeDuplicateDr = _fourMarksDiagonalRight.Intersect(_threeMarksDiagonalRight).ToList();;

                        if (fourAndThreeDuplicateDr.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(fourAndThreeDuplicateDr));
                            return RandomElementFromList(fourAndThreeDuplicateDr);
                        }
                    }
                    
                    // Checking for gap between 4 and 2 diagonal right marks of Player
                    if (_fourMarksDiagonalRight.Count > 0 && _twoMarksDiagonalRight.Count > 0) 
                    {
                        // Searching duplicates in lists with 4 and 2 marks
                        // (it means that between lines is one field gap)
                        var fourAndTwoDuplicateDr = _fourMarksDiagonalRight.Intersect(_twoMarksDiagonalRight).ToList();;

                        if (fourAndTwoDuplicateDr.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(fourAndTwoDuplicateDr));
                            return RandomElementFromList(fourAndTwoDuplicateDr);
                        }
                    }
                    
                    // Checking for gap between 4 and 1 diagonal right marks of Player
                    if (_fourMarksDiagonalRight.Count > 0 && _singleList.Count > 0) 
                    {
                        // Searching duplicates in lists with 4 and 1 marks
                        // (it means that between lines is one field gap)
                        var fourAndSingleDuplicateDr = _fourMarksDiagonalRight.Intersect(_singleList).ToList();;

                        if (fourAndSingleDuplicateDr.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(fourAndSingleDuplicateDr));
                            return RandomElementFromList(fourAndSingleDuplicateDr);
                        }
                    }
                    
                    Console.WriteLine(_fourMarksDiagonalRight.First());
                    return _fourMarksDiagonalRight.First();
                }
                
                // Checking for 4 diagonal left marks of Player
                if (_fourMarksDiagonalLeft.Count > 0)  
                {
                    // Searching duplicates in list with 4 marks
                    // (it means that between line marks is one field gap)
                    var fourDuplicatesDl = _fourMarksDiagonalLeft.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();

                    // If there is a gap returns it
                    if (fourDuplicatesDl.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(fourDuplicatesDl));
                        return RandomElementFromList(fourDuplicatesDl);
                    }
                    
                    // Checking for gap between 4 and 3 diagonal left marks of Player
                    if (_fourMarksDiagonalLeft.Count > 0 && _threeMarksDiagonalLeft.Count > 0) 
                    {
                        // Searching duplicates in lists with 4 and 3 marks (it means that between lines is one field gap)
                        var fourAndThreeDuplicateDl = _fourMarksDiagonalLeft.Intersect(_threeMarksDiagonalLeft).ToList();;

                        if (fourAndThreeDuplicateDl.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(fourAndThreeDuplicateDl));
                            return RandomElementFromList(fourAndThreeDuplicateDl);
                        }
                    }
                    
                    // Checking for gap between 4 and 2 diagonal left marks of Player
                    if (_fourMarksDiagonalLeft.Count > 0 && _twoMarksDiagonalLeft.Count > 0) 
                    {
                        // Searching duplicates in lists with 4 and 2 marks
                        // (it means that between lines is one field gap)
                        var fourAndTwoDuplicateDl = _fourMarksDiagonalLeft.Intersect(_twoMarksDiagonalLeft).ToList();;

                        if (fourAndTwoDuplicateDl.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(fourAndTwoDuplicateDl));
                            return RandomElementFromList(fourAndTwoDuplicateDl);
                        }
                    }
                    
                    // Checking for gap between 4 and 1 diagonal left marks of Player
                    if (_fourMarksDiagonalLeft.Count > 0 && _singleList.Count > 0) 
                    {
                        // Searching duplicates in lists with 4 and 1 marks
                        // (it means that between lines is one field gap)
                        var fourAndSingleDuplicateDl = _fourMarksDiagonalLeft.Intersect(_singleList).ToList();;

                        if (fourAndSingleDuplicateDl.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(fourAndSingleDuplicateDl));
                            return RandomElementFromList(fourAndSingleDuplicateDl);
                        }
                    }
                    
                    Console.WriteLine(_fourMarksDiagonalLeft.First());
                    return _fourMarksDiagonalLeft.First();
                }
                
                // Checking for gap between 3 and others horizontal marks of Player
                if (_threeMarksHorizontal.Count > 0)  
                {
                    // Searching duplicates in list with 3 marks
                    // (it means that between line marks is one field gap)
                    var threeDuplicatesH = _threeMarksHorizontal.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();

                    // If there is a gap returns it
                    if (threeDuplicatesH.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(threeDuplicatesH));
                        return RandomElementFromList(threeDuplicatesH);
                    }
                    
                    // Checking for gap between 3 and 2 horizontal marks of Player
                    if (_threeMarksHorizontal.Count > 0 && _twoMarksHorizontal.Count > 0) 
                    {
                        // Searching duplicates in lists with 3 and 2 marks (it means that between lines is one field gap)
                        var twoDuplicateH = _threeMarksHorizontal.Intersect(_twoMarksHorizontal).ToList();;

                        if (twoDuplicateH.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(twoDuplicateH));
                            return RandomElementFromList(twoDuplicateH);
                        }
                    }
                    
                    // Checking for gap between 3 and 1 horizontal marks of Player
                    if (_threeMarksHorizontal.Count > 0 && _singleList.Count > 0) 
                    {
                        // Searching duplicates in lists with 3 and 1 marks
                        // (it means that between lines is one field gap)
                        var singleDuplicateH = _threeMarksHorizontal.Intersect(_singleList).ToList();;

                        if (singleDuplicateH.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(singleDuplicateH));
                            return RandomElementFromList(singleDuplicateH);
                        }
                    }
                }
                
                // Checking for gap between 3 and others vertical marks of Player
                if (_threeMarksVertical.Count > 0)  
                {
                    // Searching duplicates in list with 3 marks
                    // (it means that between line marks is one field gap)
                    var threeDuplicatesV = _threeMarksVertical.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();

                    // If there is a gap returns it
                    if (threeDuplicatesV.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(threeDuplicatesV));
                        return RandomElementFromList(threeDuplicatesV);
                    }
                    
                    // Checking for gap between 3 and 2 vertical marks of Player
                    if (_threeMarksVertical.Count > 0 && _twoMarksVertical.Count > 0) 
                    {
                        // Searching duplicates in lists with 3 and 2 marks
                        // (it means that between lines is one field gap)
                        var twoDuplicateV = _threeMarksVertical.Intersect(_twoMarksVertical).ToList();;

                        if (twoDuplicateV.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(twoDuplicateV));
                            return RandomElementFromList(twoDuplicateV);
                        }
                    }
                    
                    // Checking for gap between 3 and 1 vertical marks of Player
                    if (_threeMarksVertical.Count > 0 && _singleList.Count > 0) 
                    {
                        // Searching duplicates in lists with 3 and 1 marks
                        // (it means that between lines is one field gap)
                        var singleDuplicateV = _threeMarksVertical.Intersect(_singleList).ToList();;

                        // If there is a gap returns it
                        if (singleDuplicateV.Count > 0)  
                        {
                            Console.WriteLine(RandomElementFromList(singleDuplicateV));
                            return RandomElementFromList(singleDuplicateV);
                        }
                    }
                }
                
                // Checking for gap between 3 and others diagonal right marks of Player
                if (_threeMarksDiagonalRight.Count > 0) 
                {
                    // Searching duplicates in list with 3 marks
                    // (it means that between line marks is one field gap)
                    var threeDuplicatesDr = _threeMarksDiagonalRight.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();

                    // If there is a gap returns it
                    if (threeDuplicatesDr.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(threeDuplicatesDr));
                        return RandomElementFromList(threeDuplicatesDr);
                    }
                    
                    // Checking for gap between 3 and 2 diagonal right marks of Player
                    if (_threeMarksDiagonalRight.Count > 0 && _twoMarksDiagonalRight.Count > 0) 
                    {
                        // Searching duplicates in lists with 3 and 2 marks
                        // (it means that between lines is one field gap)
                        var twoDuplicateDr = _threeMarksDiagonalRight.Intersect(_twoMarksDiagonalRight).ToList();;

                        if (twoDuplicateDr.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(twoDuplicateDr));
                            return RandomElementFromList(twoDuplicateDr);
                        }
                    }
                    
                    // Checking for gap between 3 and 1 diagonal right marks of Player
                    if (_threeMarksDiagonalRight.Count > 0 && _singleList.Count > 0) 
                    {
                        // Searching duplicates in lists with 3 and 1 marks
                        // (it means that between lines is one field gap)
                        var singleDuplicateDr = _threeMarksDiagonalRight.Intersect(_singleList).ToList();;

                        // If there is a gap returns it
                        if (singleDuplicateDr.Count > 0)  
                        {
                            Console.WriteLine(RandomElementFromList(singleDuplicateDr));
                            return RandomElementFromList(singleDuplicateDr);
                        }
                    }
                }
                
                // Checking for gap between 3 and others diagonal left marks of Player
                if (_threeMarksDiagonalLeft.Count > 0)  
                {
                    // Searching duplicates in list with 3 marks
                    // (it means that between line marks is one field gap)
                    var threeDuplicatesDl = _threeMarksDiagonalLeft.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();

                    // If there is a gap returns it
                    if (threeDuplicatesDl.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(threeDuplicatesDl));
                        return RandomElementFromList(threeDuplicatesDl);
                    }
                    
                    // Checking for gap between 3 and 2 diagonal left marks of Player
                    if (_threeMarksDiagonalLeft.Count > 0 && _twoMarksDiagonalLeft.Count > 0) 
                    {
                        // Searching duplicates in lists with 3 and 2 marks
                        // (it means that between lines is one field gap)
                        var twoDuplicateDl = _threeMarksDiagonalLeft.Intersect(_twoMarksDiagonalLeft).ToList();;

                        if (twoDuplicateDl.Count > 0)  // If there is a gap returns it
                        {
                            Console.WriteLine(RandomElementFromList(twoDuplicateDl));
                            return RandomElementFromList(twoDuplicateDl);
                        }
                    }
                    
                    // Checking for gap between 3 and 1 diagonal left marks of Player
                    if (_threeMarksDiagonalLeft.Count > 0 && _singleList.Count > 0) 
                    {
                        // Searching duplicates in lists with 3 and 1 marks
                        // (it means that between lines is one field gap)
                        var singleDuplicateDl = _threeMarksDiagonalLeft.Intersect(_singleList).ToList();;

                        // If there is a gap returns it
                        if (singleDuplicateDl.Count > 0)  
                        {
                            Console.WriteLine(RandomElementFromList(singleDuplicateDl));
                            return RandomElementFromList(singleDuplicateDl);
                        }
                    }
                }
                
                // Checkin that 2 and 2 (horizontal) are in the line with one gap
                if (_twoMarksHorizontal.Count > 0)
                {
                    var twoAndTwoDuplicatesH = _twoMarksHorizontal.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();

                    // If there is a gap returns it
                    if (twoAndTwoDuplicatesH.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(twoAndTwoDuplicatesH));
                        return RandomElementFromList(twoAndTwoDuplicatesH);
                    }
                }
                
                // Checkin that 2 and 2 (vertical) are in the line with one gap
                if (_twoMarksVertical.Count > 0)
                {
                    var twoAndTwoDuplicatesV = _twoMarksVertical.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();

                    // If there is a gap returns it
                    if (twoAndTwoDuplicatesV.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(twoAndTwoDuplicatesV));
                        return RandomElementFromList(twoAndTwoDuplicatesV);
                    }
                }
                
                // Checkin that 2 and 2 (diagonal right) are in the line with one gap
                if (_twoMarksDiagonalRight.Count > 0)
                {
                    var twoAndTwoDuplicatesDr = _twoMarksDiagonalRight.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();

                    // If there is a gap returns it
                    if (twoAndTwoDuplicatesDr.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(twoAndTwoDuplicatesDr));
                        return RandomElementFromList(twoAndTwoDuplicatesDr);
                    }
                }
                
                // Checkin that 2 and 2 (diagonal left) are in the line with one gap
                if (_twoMarksDiagonalLeft.Count > 0)
                {
                    var twoAndTwoDuplicatesDl = _twoMarksDiagonalLeft.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();

                    // If there is a gap returns it
                    if (twoAndTwoDuplicatesDl.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(twoAndTwoDuplicatesDl));
                        return RandomElementFromList(twoAndTwoDuplicatesDl);
                    }
                }
                
                // Checkin that 3 in line (horizontal) open (??)
                if (_threeMarksHorizontal.Count > 1)
                {
                    Console.WriteLine(RandomElementFromList(_threeMarksHorizontal));
                    return RandomElementFromList(_threeMarksHorizontal);
                }
                
                // Checkin that 3 in line (vertical) open (??)
                if (_threeMarksVertical.Count > 1)
                {
                    Console.WriteLine(RandomElementFromList(_threeMarksVertical));
                    return RandomElementFromList(_threeMarksVertical);
                }
                
                // Checkin that 3 in line (diagonal right) open (??)
                if (_threeMarksDiagonalRight.Count > 1)
                {
                    Console.WriteLine(RandomElementFromList(_threeMarksDiagonalRight));
                    return RandomElementFromList(_threeMarksDiagonalRight);
                }
                
                // Checkin that 3 in line (diagonal left) open (??)
                if (_threeMarksDiagonalLeft.Count > 1)
                {
                    Console.WriteLine(RandomElementFromList(_threeMarksDiagonalLeft));
                    return RandomElementFromList(_threeMarksDiagonalLeft);
                }
                
                // Checkin that 2 and 1 (horizontal) are in one line with one gap
                if (_twoMarksHorizontal.Count > 0 && _singleList.Count > 0)
                {
                    var twoAndSingleDuplicateH = _twoMarksHorizontal.Intersect(_singleList).ToList();;

                    // If there is a gap returns it
                    if (twoAndSingleDuplicateH.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(twoAndSingleDuplicateH));
                        return RandomElementFromList(twoAndSingleDuplicateH);
                    }
                }
                
                // Checkin that 2 and 1 (vertical) are in one line with one gap
                if (_twoMarksVertical.Count > 0 && _singleList.Count > 0)
                {
                    var twoAndSingleDuplicateV = _twoMarksVertical.Intersect(_singleList).ToList();;

                    // If there is a gap returns it
                    if (twoAndSingleDuplicateV.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(twoAndSingleDuplicateV));
                        return RandomElementFromList(twoAndSingleDuplicateV);
                    }
                }
                
                // Checkin that 2 and 1 (diagonal right) are in one line with one gap
                if (_twoMarksDiagonalRight.Count > 0 && _singleList.Count > 0)
                {
                    var twoAndSingleDuplicateDr = _twoMarksDiagonalRight.Intersect(_singleList).ToList();;

                    // If there is a gap returns it
                    if (twoAndSingleDuplicateDr.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(twoAndSingleDuplicateDr));
                        return RandomElementFromList(twoAndSingleDuplicateDr);
                    }
                }
                
                // Checkin that 2 and 1 (diagonal left) are in one line with one gap
                if (_twoMarksDiagonalLeft.Count > 0 && _singleList.Count > 0)
                {
                    var twoAndSingleDuplicateDl = _twoMarksDiagonalLeft.Intersect(_singleList).ToList();;

                    // If there is a gap returns it
                    if (twoAndSingleDuplicateDl.Count > 0)  
                    {
                        Console.WriteLine(RandomElementFromList(twoAndSingleDuplicateDl));
                        return RandomElementFromList(twoAndSingleDuplicateDl);
                    }
                }
                
                // Checkin 3 in line (horizontal)
                if (_threeMarksHorizontal.Count > 0)
                {
                    Console.WriteLine(RandomElementFromList(_threeMarksHorizontal));
                    return RandomElementFromList(_threeMarksHorizontal);
                }
                
                // Checkin 3 in line (vertical)
                if (_threeMarksVertical.Count > 0)
                {
                    Console.WriteLine(RandomElementFromList(_threeMarksVertical));
                    return RandomElementFromList(_threeMarksVertical);
                }
                
                // Checkin 3 in line (diagonal right)
                if (_threeMarksDiagonalRight.Count > 0)
                {
                    Console.WriteLine(RandomElementFromList(_threeMarksDiagonalRight));
                    return RandomElementFromList(_threeMarksDiagonalRight);
                }
                
                // Checkin 3 in line (diagonal left)
                if (_threeMarksDiagonalLeft.Count > 0)
                {
                    Console.WriteLine(RandomElementFromList(_threeMarksDiagonalLeft));
                    return RandomElementFromList(_threeMarksDiagonalLeft);
                }
                
                // Checking that 2 (horizontal) are in the line
                if (_twoMarksHorizontal.Count > 0)
                {
                    Console.WriteLine(RandomElementFromList(_twoMarksHorizontal));
                    return RandomElementFromList(_twoMarksHorizontal);
                }
                
                // Checking that 2 (vertical) are in the line
                if (_twoMarksVertical.Count > 0)
                {
                    Console.WriteLine(RandomElementFromList(_twoMarksVertical));
                    return RandomElementFromList(_twoMarksVertical);
                }
                
                // Checking that 2 (diagonal right) are in the line
                if (_twoMarksDiagonalRight.Count > 0)
                {
                    Console.WriteLine(RandomElementFromList(_twoMarksDiagonalRight));
                    return RandomElementFromList(_twoMarksDiagonalRight);
                }
                
                // Checking that 2 (diagonal left) are in the line
                if (_twoMarksDiagonalLeft.Count > 0)
                {
                    Console.WriteLine(RandomElementFromList(_twoMarksDiagonalLeft));
                    return RandomElementFromList(_twoMarksDiagonalLeft);
                }
            }

            if (_singleList.Count > 0)
            {
                Console.WriteLine(RandomElementFromList(_singleList));
                return RandomElementFromList(_singleList);
            }
            else
            {
                return GetRandomEmptyField();
            }
        }

        public static void Test()
        {
            List<(int, int)> l1 = new List<(int, int)>();
            List<(int, int)> l2 = new List<(int, int)>();

            l1.Add((0, 0));
            l1.Add((0, 1));
            l1.Add((0, 2));
            l1.Add((0, 3));
            l1.Add((0, 4));
            l1.Add((1, 3));
            l1.Add((4, 4));
            l1.Add((4, 5));
            
            l2.Add((1, 0));
            l2.Add((1, 1));
            l2.Add((1, 2));
            l2.Add((5, 3));
            l2.Add((1, 4));
            l2.Add((2, 3));

            // var duplicateKeys = l1.GroupBy(x => x)
            //     .Where(group => group.Count() > 1)
            //     .Select(group => group.Key).ToList();
            
            var duplicates = l1.Intersect(l2).ToList();

            foreach (var val in duplicates)
            {
                Console.Write(val + ", ");
            }

            Console.WriteLine();
        }

        private static void FindMarkInHorizontalLine()
        {
            _fourMarksHorizontal = new List<(int x, int y)>();
            _threeMarksHorizontal = new List<(int x, int y)>();
            _twoMarksHorizontal = new List<(int x, int y)>();

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
                                            _twoMarksHorizontal.Add((tmpSet.First().x, tmpSet.First().y - 1));
                                        }

                                        if (tmpSet.Last().y < board.GetLength(1) - 1 &&
                                            (uint) board.GetValue(tmpSet.Last().x, tmpSet.Last().y + 1) ==
                                            Board.EmptyCell)
                                        {
                                            _twoMarksHorizontal.Add((tmpSet.Last().x, tmpSet.Last().y + 1));
                                        }

                                        tmpSet.Clear();
                                        break;
                                    case 3:
                                        if (tmpSet.First().y > 0 &&
                                            (uint) board.GetValue(tmpSet.First().x, tmpSet.First().y - 1) ==
                                            Board.EmptyCell)
                                        {
                                            _threeMarksHorizontal.Add((tmpSet.First().x, tmpSet.First().y - 1));
                                        }

                                        if (tmpSet.Last().y < board.GetLength(1) - 1 &&
                                            (uint) board.GetValue(tmpSet.Last().x, tmpSet.Last().y + 1) ==
                                            Board.EmptyCell)
                                        {
                                            _threeMarksHorizontal.Add((tmpSet.Last().x, tmpSet.Last().y + 1));
                                        }

                                        tmpSet.Clear();
                                        break;
                                    case 4:
                                        if (tmpSet.First().y > 0 &&
                                            (uint) board.GetValue(tmpSet.First().x, tmpSet.First().y - 1) ==
                                            Board.EmptyCell)
                                        {
                                            _fourMarksHorizontal.Add((tmpSet.First().x, tmpSet.First().y - 1));
                                        }

                                        if (tmpSet.Last().y < board.GetLength(1) - 1 &&
                                            (uint) board.GetValue(tmpSet.Last().x, tmpSet.Last().y + 1) ==
                                            Board.EmptyCell)
                                        {
                                            _fourMarksHorizontal.Add((tmpSet.Last().x, tmpSet.Last().y + 1));
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
            
            // Console.WriteLine("twoMarks: ");
            // foreach (var tup in _twoMarksHorizontal)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();
            // Console.WriteLine("threeMarks: ");
            // foreach (var tup in _threeMarksHorizontal)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();
            // Console.WriteLine("fourMarks: ");
            // foreach (var tup in _fourMarksHorizontal)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();
        }


        private static void FindMarkInVerticalLine()
        {
            _fourMarksVertical = new List<(int x, int y)>();
            _threeMarksVertical = new List<(int x, int y)>();
            _twoMarksVertical = new List<(int x, int y)>();

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
                                            _twoMarksVertical.Add((tmpSet.First().x - 1, tmpSet.First().y));
                                        }

                                        if (tmpSet.Last().x < board.GetLength(0) - 1 &&
                                            (uint) board.GetValue(tmpSet.Last().x + 1, tmpSet.Last().y) ==
                                            Board.EmptyCell)
                                        {
                                            _twoMarksVertical.Add((tmpSet.Last().x + 1, tmpSet.Last().y));
                                        }

                                        tmpSet.Clear();
                                        break;
                                    case 3:
                                        if (tmpSet.First().x > 0 &&
                                            (uint) board.GetValue(tmpSet.First().x - 1, tmpSet.First().y) ==
                                            Board.EmptyCell)
                                        {
                                            _threeMarksVertical.Add((tmpSet.First().x - 1, tmpSet.First().y));
                                        }

                                        if (tmpSet.Last().x < board.GetLength(0) - 1 &&
                                            (uint) board.GetValue(tmpSet.Last().x + 1, tmpSet.Last().y) ==
                                            Board.EmptyCell)
                                        {
                                            _threeMarksVertical.Add((tmpSet.Last().x + 1, tmpSet.Last().y));
                                        }

                                        tmpSet.Clear();
                                        break;
                                    case 4:
                                        if (tmpSet.First().x > 0 &&
                                            (uint) board.GetValue(tmpSet.First().x - 1, tmpSet.First().y) ==
                                            Board.EmptyCell)
                                        {
                                            _fourMarksVertical.Add((tmpSet.First().x - 1, tmpSet.First().y));
                                        }

                                        if (tmpSet.Last().x < board.GetLength(0) - 1 &&
                                            (uint) board.GetValue(tmpSet.Last().x + 1, tmpSet.Last().y) ==
                                            Board.EmptyCell)
                                        {
                                            _fourMarksVertical.Add((tmpSet.Last().x + 1, tmpSet.Last().y));
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
            
            // Console.WriteLine("twoMarks: ");
            // foreach (var tup in _twoMarksVertical)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();
            // Console.WriteLine("threeMarks: ");
            // foreach (var tup in _threeMarksVertical)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();
            // Console.WriteLine("fourMarks: ");
            // foreach (var tup in _fourMarksVertical)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();
        }


        private static void FindMarkInDiagonalRightLine()
        {
            uint[,] board = Board.BoardArray;

            _fourMarksDiagonalRight = new List<(int x, int y)>();
            _threeMarksDiagonalRight = new List<(int x, int y)>();
            _twoMarksDiagonalRight = new List<(int x, int y)>();

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
                                                _twoMarksDiagonalRight.Add((tmpListPos.First().x + 1,
                                                    tmpListPos.First().y - 1));
                                            }

                                            if (tmpListPos.Last().x > 0 &&
                                                tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                                board[tmpListPos.Last().x - 1, tmpListPos.Last().y + 1] ==
                                                Board.EmptyCell)
                                            {
                                                _twoMarksDiagonalRight.Add((tmpListPos.Last().x - 1,
                                                    tmpListPos.Last().y + 1));
                                            }

                                            tmpListPos.Clear();
                                            break;
                                        case 3:
                                            if (tmpListPos.First().y > 0 &&
                                                tmpListPos.First().x < board.GetLength(0) - 1 &&
                                                board[tmpListPos.First().x + 1, tmpListPos.First().y - 1] ==
                                                Board.EmptyCell)
                                            {
                                                _threeMarksDiagonalRight.Add((tmpListPos.First().x + 1,
                                                    tmpListPos.First().y - 1));
                                            }

                                            if (tmpListPos.Last().x > 0 &&
                                                tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                                board[tmpListPos.Last().x - 1, tmpListPos.Last().y + 1] ==
                                                Board.EmptyCell)
                                            {
                                                _threeMarksDiagonalRight.Add((tmpListPos.Last().x - 1,
                                                    tmpListPos.Last().y + 1));
                                            }

                                            tmpListPos.Clear();
                                            break;
                                        case 4:
                                            if (tmpListPos.First().y > 0 &&
                                                tmpListPos.First().x < board.GetLength(0) - 1 &&
                                                board[tmpListPos.First().x + 1, tmpListPos.First().y - 1] ==
                                                Board.EmptyCell)
                                            {
                                                _fourMarksDiagonalRight.Add((tmpListPos.First().x + 1,
                                                    tmpListPos.First().y - 1));
                                            }

                                            if (tmpListPos.Last().x > 0 &&
                                                tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                                board[tmpListPos.Last().x - 1, tmpListPos.Last().y + 1] ==
                                                Board.EmptyCell)
                                            {
                                                _fourMarksDiagonalRight.Add((tmpListPos.Last().x - 1,
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
            
            // Console.WriteLine("twoMarks: ");
            // foreach (var tup in _twoMarksDiagonalRight)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();
            // Console.WriteLine("threeMarks: ");
            // foreach (var tup in _threeMarksDiagonalRight)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();
            // Console.WriteLine("fourMarks: ");
            // foreach (var tup in _fourMarksDiagonalRight)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();
        }


        private static void FindMarkInDiagonalLeftLine()
        {
            uint[,] board = Board.BoardArray;

            _fourMarksDiagonalLeft = new List<(int x, int y)>();
            _threeMarksDiagonalLeft = new List<(int x, int y)>();
            _twoMarksDiagonalLeft = new List<(int x, int y)>();

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
                                            if (tmpListPos.First().x > 0 &&
                                                tmpListPos.First().y > 0 &&
                                                board[tmpListPos.First().x - 1, tmpListPos.First().y - 1] ==
                                                Board.EmptyCell)
                                            {
                                                _twoMarksDiagonalLeft.Add((tmpListPos.First().x - 1,
                                                    tmpListPos.First().y - 1));
                                            }

                                            if (tmpListPos.Last().x < board.GetLength(0) - 1 &&
                                                tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                                board[tmpListPos.Last().x + 1, tmpListPos.Last().y + 1] ==
                                                Board.EmptyCell)
                                            {
                                                _twoMarksDiagonalLeft.Add((tmpListPos.Last().x + 1,
                                                    tmpListPos.Last().y + 1));
                                            }

                                            tmpListPos.Clear();
                                            break;
                                        case 3:
                                            if (tmpListPos.First().x > 0 &&
                                                tmpListPos.First().y > 0 &&
                                                board[tmpListPos.First().x - 1, tmpListPos.First().y - 1] ==
                                                Board.EmptyCell)
                                            {
                                                _threeMarksDiagonalLeft.Add((tmpListPos.First().x - 1,
                                                    tmpListPos.First().y - 1));
                                            }

                                            if (tmpListPos.Last().x < board.GetLength(0) - 1 &&
                                                tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                                board[tmpListPos.Last().x + 1, tmpListPos.Last().y + 1] ==
                                                Board.EmptyCell)
                                            {
                                                _threeMarksDiagonalLeft.Add((tmpListPos.Last().x + 1,
                                                    tmpListPos.Last().y + 1));
                                            }

                                            tmpListPos.Clear();
                                            break;
                                        case 4:
                                            if (tmpListPos.First().x > 0 &&
                                                tmpListPos.First().y > 0 &&
                                                board[tmpListPos.First().x - 1, tmpListPos.First().y - 1] ==
                                                Board.EmptyCell)
                                            {
                                                _fourMarksDiagonalLeft.Add((tmpListPos.First().x - 1,
                                                    tmpListPos.First().y - 1));
                                            }

                                            if (tmpListPos.Last().x < board.GetLength(0) - 1 &&
                                                tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                                board[tmpListPos.Last().x + 1, tmpListPos.Last().y + 1] ==
                                                Board.EmptyCell)
                                            {
                                                _fourMarksDiagonalLeft.Add((tmpListPos.Last().x + 1,
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
            
            // Console.WriteLine("twoMarks: ");
            // foreach (var tup in _twoMarksDiagonalLeft)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();
            // Console.WriteLine("threeMarks: ");
            // foreach (var tup in _threeMarksDiagonalLeft)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();
            // Console.WriteLine("fourMarks: ");
            // foreach (var tup in _fourMarksDiagonalLeft)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();
        }


        private static void SingleCell()
        {
            uint[,] board = Board.BoardArray;
            int boardHeight = board.GetLength(0);
            int boardWidth = board.GetLength(1);
            uint mark = Board.Player1Mark;
            uint empty = Board.EmptyCell;

            SortedSet<(int x, int y)> singleCell = new SortedSet<(int x, int y)>();

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if ((uint) board.GetValue(i, j) == Board.Player1Mark)
                    {
                        (int x, int y) cell = ((i, j));

                        // Top left corner
                        if (cell.x == 0 && cell.y == 0)
                        {
                            if ((uint) board.GetValue(i, j + 1) != mark &&
                                (uint) board.GetValue(i + 1, j + 1) != mark &&
                                (uint) board.GetValue(i + 1, j) != mark)
                            {
                                if ((uint) board.GetValue(i, j + 1) == empty)
                                {
                                    singleCell.Add((i, j + 1));
                                }

                                if ((uint) board.GetValue(i + 1, j + 1) == empty)
                                {
                                    singleCell.Add((i + 1, j + 1));
                                }

                                if ((uint) board.GetValue(i + 1, j) == empty)
                                {
                                    singleCell.Add((i + 1, j));
                                }
                            }
                        }

                        // Top edge
                        if (cell.x == 0 && cell.y > 0 && cell.y < boardWidth - 1)
                        {
                            if ((uint) board.GetValue(i, j - 1) != mark &&
                                (uint) board.GetValue(i + 1, j - 1) != mark &&
                                (uint) board.GetValue(i, j + 1) != mark &&
                                (uint) board.GetValue(i + 1, j + 1) != mark &&
                                (uint) board.GetValue(i + 1, j) != mark)
                            {
                                if ((uint) board.GetValue(i, j - 1) == empty)
                                {
                                    singleCell.Add((i, j - 1));
                                }

                                if ((uint) board.GetValue(i + 1, j - 1) == empty)
                                {
                                    singleCell.Add((i + 1, j - 1));
                                }

                                if ((uint) board.GetValue(i, j + 1) == empty)
                                {
                                    singleCell.Add((i, j + 1));
                                }

                                if ((uint) board.GetValue(i + 1, j + 1) == empty)
                                {
                                    singleCell.Add((i + 1, j + 1));
                                }

                                if ((uint) board.GetValue(i + 1, j) == empty)
                                {
                                    singleCell.Add((i + 1, j));
                                }
                            }
                        }

                        // Top right corner
                        if (cell.x == 0 && cell.y == boardWidth - 1)
                        {
                            if ((uint) board.GetValue(i, j - 1) != mark &&
                                (uint) board.GetValue(i + 1, j - 1) != mark &&
                                (uint) board.GetValue(i + 1, j) != mark)
                            {
                                if ((uint) board.GetValue(i, j - 1) == empty)
                                {
                                    singleCell.Add((i, j - 1));
                                }

                                if ((uint) board.GetValue(i + 1, j - 1) == empty)
                                {
                                    singleCell.Add((i + 1, j - 1));
                                }

                                if ((uint) board.GetValue(i + 1, j) == empty)
                                {
                                    singleCell.Add((i + 1, j));
                                }
                            }
                        }

                        // Left edge
                        if (cell.x > 0 && cell.x < boardHeight - 1 && cell.y == 0)
                        {
                            if ((uint) board.GetValue(i - 1, j) != mark &&
                                (uint) board.GetValue(i - 1, j + 1) != mark &&
                                (uint) board.GetValue(i, j + 1) != mark &&
                                (uint) board.GetValue(i + 1, j + 1) != mark &&
                                (uint) board.GetValue(i + 1, j) != mark)
                            {
                                if ((uint) board.GetValue(i - 1, j) == empty)
                                {
                                    singleCell.Add((i - 1, j));
                                }

                                if ((uint) board.GetValue(i - 1, j + 1) == empty)
                                {
                                    singleCell.Add((i - 1, j + 1));
                                }

                                if ((uint) board.GetValue(i, j + 1) == empty)
                                {
                                    singleCell.Add((i, j + 1));
                                }

                                if ((uint) board.GetValue(i + 1, j + 1) == empty)
                                {
                                    singleCell.Add((i + 1, j + 1));
                                }

                                if ((uint) board.GetValue(i + 1, j) == empty)
                                {
                                    singleCell.Add((i + 1, j));
                                }
                            }
                        }

                        // Bottom left corner
                        if (cell.x == boardHeight - 1 && cell.y == 0)
                        {
                            if ((uint) board.GetValue(i - 1, j) != mark &&
                                (uint) board.GetValue(i - 1, j + 1) != mark &&
                                (uint) board.GetValue(i, j + 1) != mark)
                            {
                                if ((uint) board.GetValue(i - 1, j) == empty)
                                {
                                    singleCell.Add((i - 1, j));
                                }

                                if ((uint) board.GetValue(i - 1, j + 1) == empty)
                                {
                                    singleCell.Add((i - 1, j + 1));
                                }

                                if ((uint) board.GetValue(i, j + 1) == empty)
                                {
                                    singleCell.Add((i, j + 1));
                                }
                            }
                        }

                        // Bottom edge
                        if (cell.x == boardHeight - 1 && cell.y > 0 && cell.y < boardWidth - 1)
                        {
                            if ((uint) board.GetValue(i, j - 1) != mark &&
                                (uint) board.GetValue(i - 1, j - 1) != mark &&
                                (uint) board.GetValue(i - 1, j) != mark &&
                                (uint) board.GetValue(i - 1, j + 1) != mark &&
                                (uint) board.GetValue(i, j + 1) != mark)
                            {
                                if ((uint) board.GetValue(i, j - 1) == empty)
                                {
                                    singleCell.Add((i, j - 1));
                                }

                                if ((uint) board.GetValue(i - 1, j - 1) == empty)
                                {
                                    singleCell.Add((i - 1, j - 1));
                                }

                                if ((uint) board.GetValue(i - 1, j) == empty)
                                {
                                    singleCell.Add((i - 1, j));
                                }

                                if ((uint) board.GetValue(i - 1, j + 1) == empty)
                                {
                                    singleCell.Add((i - 1, j + 1));
                                }

                                if ((uint) board.GetValue(i, j + 1) == empty)
                                {
                                    singleCell.Add((i, j + 1));
                                }
                            }
                        }

                        // Bottom right corner
                        if (cell.x == boardHeight - 1 && cell.y == boardWidth - 1)
                        {
                            if ((uint) board.GetValue(i, j - 1) != mark &&
                                (uint) board.GetValue(i - 1, j - 1) != mark &&
                                (uint) board.GetValue(i - 1, j) != mark)
                            {
                                if ((uint) board.GetValue(i, j - 1) == empty)
                                {
                                    singleCell.Add((i, j - 1));
                                }

                                if ((uint) board.GetValue(i - 1, j - 1) == empty)
                                {
                                    singleCell.Add((i - 1, j - 1));
                                }

                                if ((uint) board.GetValue(i - 1, j) == empty)
                                {
                                    singleCell.Add((i - 1, j));
                                }
                            }
                        }

                        // Right edge
                        if (cell.x > 0 && cell.x < boardHeight - 1 && cell.y == boardWidth - 1)
                        {
                            if ((uint) board.GetValue(i - 1, j) != mark &&
                                (uint) board.GetValue(i - 1, j - 1) != mark &&
                                (uint) board.GetValue(i, j - 1) != mark &&
                                (uint) board.GetValue(i + 1, j - 1) != mark &&
                                (uint) board.GetValue(i + 1, j) != mark)
                            {
                                if ((uint) board.GetValue(i - 1, j) == empty)
                                {
                                    singleCell.Add((i - 1, j));
                                }

                                if ((uint) board.GetValue(i - 1, j - 1) == empty)
                                {
                                    singleCell.Add((i - 1, j - 1));
                                }

                                if ((uint) board.GetValue(i, j - 1) == empty)
                                {
                                    singleCell.Add((i, j - 1));
                                }

                                if ((uint) board.GetValue(i + 1, j - 1) == empty)
                                {
                                    singleCell.Add((i + 1, j - 1));
                                }

                                if ((uint) board.GetValue(i + 1, j) == empty)
                                {
                                    singleCell.Add((i + 1, j));
                                }
                            }
                        }

                        // Middle cell of the board
                        if (cell.x > 0 && cell.x < boardHeight - 1 && cell.y > 0 && cell.y < boardWidth - 1)
                        {
                            if ((uint) board.GetValue(i - 1, j) != mark &&
                                (uint) board.GetValue(i - 1, j - 1) != mark &&
                                (uint) board.GetValue(i, j - 1) != mark &&
                                (uint) board.GetValue(i + 1, j - 1) != mark &&
                                (uint) board.GetValue(i + 1, j) != mark &&
                                (uint) board.GetValue(i + 1, j + 1) != mark &&
                                (uint) board.GetValue(i, j + 1) != mark &&
                                (uint) board.GetValue(i - 1, j + 1) != mark)
                            {
                                if ((uint) board.GetValue(i - 1, j) == empty)
                                {
                                    singleCell.Add((i - 1, j));
                                }

                                if ((uint) board.GetValue(i - 1, j - 1) == empty)
                                {
                                    singleCell.Add((i - 1, j - 1));
                                }

                                if ((uint) board.GetValue(i, j - 1) == empty)
                                {
                                    singleCell.Add((i, j - 1));
                                }

                                if ((uint) board.GetValue(i + 1, j - 1) == empty)
                                {
                                    singleCell.Add((i + 1, j - 1));
                                }

                                if ((uint) board.GetValue(i + 1, j) == empty)
                                {
                                    singleCell.Add((i + 1, j));
                                }

                                if ((uint) board.GetValue(i + 1, j + 1) == empty)
                                {
                                    singleCell.Add((i + 1, j + 1));
                                }

                                if ((uint) board.GetValue(i, j + 1) == empty)
                                {
                                    singleCell.Add((i, j + 1));
                                }

                                if ((uint) board.GetValue(i - 1, j + 1) == empty)
                                {
                                    singleCell.Add((i - 1, j + 1));
                                }
                            }
                        }
                    }
                }
            }

            _singleList = singleCell.ToList();

            // foreach (var tup in _singleList)
            // {
            //     Console.Write(tup + ", ");
            // }
            //
            // Console.WriteLine();
        }


        private static (int, int) AiMoveToWin()
        {
            if (AiCheckToWinHorizontal().Count > 0)
            {
                return AiCheckToWinHorizontal().First();
            }
            else if (AiCheckToWinVertical().Count > 0)
            {
                return AiCheckToWinVertical().First();
            }
            else if (AiCheckToWinDiagonalRight().Count > 0)
            {
                return AiCheckToWinDiagonalRight().First();
            }
            else if (AiCheckToWinDiagonalLeft().Count > 0)
            {
                return AiCheckToWinDiagonalLeft().First();
            }
            else
            {
                return (-1, -1);
            }
        }
        
        
        private static List<(int, int)> AiCheckToWinHorizontal()
        {
            List<(int x, int y)> aiCheckToWinHorizontal = new List<(int x, int y)>();

            SortedSet<(int x, int y)> tmpSet = new SortedSet<(int x, int y)>();

            uint[,] board = Board.BoardArray;

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board.GetLength(1) - 1 > j)
                    {
                        if ((uint) board.GetValue(i, j) == Board.Player2Mark &&
                            (uint) board.GetValue(i, j + 1) == Board.Player2Mark)
                        {
                            tmpSet.Add((i, j));
                            tmpSet.Add((i, j + 1));
                        }

                        if ((uint) board.GetValue(i, j + 1) != Board.Player2Mark || board.GetLength(1) - 2 == j)
                        {
                            if (tmpSet.Count > 0)
                            {
                                if (tmpSet.Count == _inLine - 1)
                                {
                                    if (tmpSet.First().y > 0 &&
                                        (uint) board.GetValue(tmpSet.First().x, tmpSet.First().y - 1) ==
                                        Board.EmptyCell)
                                    {
                                        aiCheckToWinHorizontal.Add((tmpSet.First().x, tmpSet.First().y - 1));
                                    }

                                    if (tmpSet.Last().y < board.GetLength(1) - 1 &&
                                        (uint) board.GetValue(tmpSet.Last().x, tmpSet.Last().y + 1) ==
                                        Board.EmptyCell)
                                    {
                                        aiCheckToWinHorizontal.Add((tmpSet.Last().x, tmpSet.Last().y + 1));
                                    }
                                    tmpSet.Clear();
                                }
                                tmpSet.Clear();
                            }
                        }
                    }
                }
            }

            // Console.WriteLine("aiMoveToWinHorizontal: ");
            // foreach (var tup in aiCheckToWinHorizontal)
            // {
            //     Console.Write(tup + " ");
            // }
            // Console.WriteLine();

            return aiCheckToWinHorizontal;
        }
        
        
        private static List<(int, int)> AiCheckToWinVertical()
        {
            List<(int x, int y)> aiCheckToWinVertical = new List<(int x, int y)>();

            SortedSet<(int x, int y)> tmpSet = new SortedSet<(int x, int y)>();

            uint[,] board = Board.BoardArray;

            for (int i = 0; i < board.GetLength(1); i++)
            {
                for (int j = 0; j < board.GetLength(0); j++)
                {
                    if (board.GetLength(0) - 1 > j)
                    {
                        if ((uint) board.GetValue(j, i) == Board.Player2Mark &&
                            (uint) board.GetValue(j + 1, i) == Board.Player2Mark)
                        {
                            tmpSet.Add((j, i));
                            tmpSet.Add((j + 1, i));
                        }

                        if ((uint) board.GetValue(j + 1, i) != Board.Player2Mark || board.GetLength(0) - 2 == j)
                        {
                            if (tmpSet.Count > 0)
                            {
                                if (tmpSet.Count == _inLine - 1)
                                {
                                    if (tmpSet.First().x > 0 &&
                                        (uint) board.GetValue(tmpSet.First().x - 1, tmpSet.First().y) ==
                                        Board.EmptyCell)
                                    {
                                        aiCheckToWinVertical.Add((tmpSet.First().x - 1, tmpSet.First().y));
                                    }

                                    if (tmpSet.Last().x < board.GetLength(0) - 1 &&
                                        (uint) board.GetValue(tmpSet.Last().x + 1, tmpSet.Last().y) ==
                                        Board.EmptyCell)
                                    {
                                        aiCheckToWinVertical.Add((tmpSet.Last().x + 1, tmpSet.Last().y));
                                    }

                                    tmpSet.Clear();
                                }
                                tmpSet.Clear();
                            }
                        }
                    }
                }
            }
            
            // Console.WriteLine("aiCheckToWinVertical: ");
            // foreach (var tup in aiCheckToWinVertical)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();

            return aiCheckToWinVertical;
        }


        private static List<(int, int)> AiCheckToWinDiagonalRight()
        {
            uint[,] board = Board.BoardArray;

            List<(int x, int y)> aiCheckToWinDiagonalRight = new List<(int x, int y)>();

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
                            if (tmpList.ElementAt(i).value == Board.Player2Mark &&
                                tmpList.ElementAt(i + 1).value == Board.Player2Mark)
                            {
                                tmpListPos.Add((tmpList.ElementAt(i).x, tmpList.ElementAt(i).y));
                                tmpListPos.Add((tmpList.ElementAt(i + 1).x, tmpList.ElementAt(i + 1).y));
                            }

                            if (tmpList.ElementAt(i + 1).value != Board.Player2Mark || tmpList.Count - 2 == i)
                            {
                                tmpListPos = tmpListPos.Distinct().ToList();


                                if (tmpListPos.Count > 0)
                                {
                                    if (tmpListPos.Count == _inLine - 1)
                                    {
                                        if (tmpListPos.First().y > 0 &&
                                            tmpListPos.First().x < board.GetLength(0) - 1 &&
                                            board[tmpListPos.First().x + 1, tmpListPos.First().y - 1] ==
                                            Board.EmptyCell)
                                        {
                                            aiCheckToWinDiagonalRight.Add((tmpListPos.First().x + 1,
                                                tmpListPos.First().y - 1));
                                        }

                                        if (tmpListPos.Last().x > 0 &&
                                            tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                            board[tmpListPos.Last().x - 1, tmpListPos.Last().y + 1] ==
                                            Board.EmptyCell)
                                        {
                                            aiCheckToWinDiagonalRight.Add((tmpListPos.Last().x - 1,
                                                tmpListPos.Last().y + 1));
                                        }

                                        tmpListPos.Clear();
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
            
            // Console.WriteLine("aiCheckToWinDiagonalRight: ");
            // foreach (var tup in aiCheckToWinDiagonalRight)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();

            return aiCheckToWinDiagonalRight;
        }


        private static List<(int, int)> AiCheckToWinDiagonalLeft()
        {
            uint[,] board = Board.BoardArray;

            List<(int x, int y)> aiCheckToWinDiagonalLeft = new List<(int x, int y)>();

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
                            if (tmpList.ElementAt(k).value == Board.Player2Mark &&
                                tmpList.ElementAt(k + 1).value == Board.Player2Mark)
                            {
                                tmpListPos.Add((tmpList.ElementAt(k).x, tmpList.ElementAt(k).y));
                                tmpListPos.Add((tmpList.ElementAt(k + 1).x, tmpList.ElementAt(k + 1).y));
                            }

                            if (tmpList.ElementAt(k + 1).value != Board.Player2Mark || tmpList.Count - 2 == k)
                            {
                                tmpListPos = tmpListPos.Distinct().ToList();

                                if (tmpListPos.Count > 0)
                                {
                                    if (tmpListPos.Count == _inLine - 1)
                                    {
                                        if (tmpListPos.First().x > 0 &&
                                            tmpListPos.First().y > 0 &&
                                            board[tmpListPos.First().x - 1, tmpListPos.First().y - 1] ==
                                            Board.EmptyCell)
                                        {
                                            aiCheckToWinDiagonalLeft.Add((tmpListPos.First().x - 1,
                                                tmpListPos.First().y - 1));
                                        }

                                        if (tmpListPos.Last().x < board.GetLength(0) - 1 &&
                                            tmpListPos.Last().y < board.GetLength(1) - 1 &&
                                            board[tmpListPos.Last().x + 1, tmpListPos.Last().y + 1] ==
                                            Board.EmptyCell)
                                        {
                                            aiCheckToWinDiagonalLeft.Add((tmpListPos.Last().x + 1,
                                                tmpListPos.Last().y + 1));
                                        }

                                        tmpListPos.Clear();
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

            // Console.WriteLine("aiCheckToWinDiagonalLeft: ");
            // foreach (var tup in aiCheckToWinDiagonalLeft)
            // {
            //     Console.Write(tup + " ");
            // }
            //
            // Console.WriteLine();

            return aiCheckToWinDiagonalLeft;
        }


        private static (int, int) GetRandomEmptyField()
        {
            uint[,] board = Board.BoardArray;
            List<(int, int)> emptyFields = new List<(int, int)>();

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if ((uint) board.GetValue(i, j) == Board.EmptyCell)
                    {
                        emptyFields.Add((i, j));
                    }
                }
            }
            
            return RandomElementFromList(emptyFields);
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


        private static (int, int) RandomElementFromList(List<(int, int)> list)
        {
            var random = new Random();
            int index = random.Next(list.Count);
            return list.ElementAt(index);
        }
    }
}