using System;
using System.Collections.Generic;
using Gtk;

namespace FiveInRow
{
    public class Board : Window
    {
        private Table _table;  // Gtk widget
        private static uint[,] _boardArray; // board with status of game
        protected internal const uint EmptyCell = 0; // emty cell (=0) of the game
        protected internal const uint Player1Mark = 1; // cell with mark (=1) the Player1 
        protected internal const uint Player2Mark = 2; // cell with mark (=2) the Player2
        private string _player1Image = Stock.Apply; // Image for player 1
        private string _player2Image = Stock.Cancel; // Image for player 2
        private bool _turn = true; // turn = PLAYER1
        private string _imageState;
        private static ConfigGameWindow _configGameWindow; // handler to ConfigGameWindow


        public Board(uint row, uint col) : base("FIVE IN ONE ROW")
        {
            SetPosition(WindowPosition.Center);
            this.TypeHint = Gdk.WindowTypeHint.Dialog;
            this.Resizable = false;
            DeleteEvent += new DeleteEventHandler(DestroyBoard);

            List<Button> buttonLists = new List<Button>();
            _boardArray = new uint[row, col];

            _table = new Table(row, col, true);

            for (uint i = 0; i < row; i++)
            {
                for (uint j = 0; j < col; j++)
                {
                    Button cell = new Button($"{i},{j}");
                    cell.WidthRequest = 40;
                    cell.HeightRequest = 40;
                    cell.Children[0].ChildVisible = false;
                    _table.Attach(cell, j, j + 1, i, i + 1);
                    buttonLists.Add(cell);
                    _boardArray[i, j] = EmptyCell;
                }
            }

            foreach (Button btn in buttonLists)
            {
                btn.Clicked += OnClick;
            }

            Add(_table);
            ShowAll();
        }


        private void OnClick(object sender, EventArgs args)
        {
            Button btn = (Button) sender;

            string[] coords = btn.Label.Split(',');
            int x = Int32.Parse(coords[0]);
            int y = Int32.Parse(coords[1]);

            uint leftAttach = (uint) y;
            uint rightAttach = (uint) y + 1;
            uint topAttach = (uint) x;
            uint bottomAttach = (uint) x + 1;

            _table.Remove(btn);

            if (_turn == true)
            {
                _imageState = _player1Image;
            }
            else
            {
                _imageState = _player2Image;
            }

            Image image = new Image(_imageState, IconSize.Button);
            Button newBtn = new Button(image);
            newBtn.WidthRequest = 40;
            newBtn.HeightRequest = 40;

            _table.Attach(newBtn, leftAttach, rightAttach, topAttach, bottomAttach);

            ShowAll();

            if (_turn == true) // Player1
            {
                _boardArray[x, y] = Player1Mark;
                _turn = false;
            }
            else // Player2
            {
                _boardArray[x, y] = Player2Mark;
                _turn = true;
            }

            GameLogic.HasWon("PLAYER 1", "PLAYER 2");
            GameLogic.IsBoardFull();
            GameListener();
            
            // TODO delete
            GameLogic.FindMarkInDiagonalLeftLine();
            // GameLogic.FindMarkInDiagonalRightLine();
            // GameLogic.FindMarkInVerticalLine();
            // GameLogic.FindMarkInHorizontalLine();
        }


        private void GameListener()
        {
            foreach (Button btn in _table.Children)
            {
                // Console.WriteLine(btn.Label);
            }
        }


        private void DestroyBoard(object sender, DeleteEventArgs e)
        { 
            this.Destroy();
            _configGameWindow.Show();
            
        }


        // window ConfigGameWindow handler
        public static void SetConfigGameWindow(ConfigGameWindow configGameWindow)
        {
            _configGameWindow = configGameWindow;
        }


        /// <summary>Getter for Board</summary>
        public static uint[,] BoardArray    // get and set for _boardArray
        {
            get => _boardArray;
        }


        /// <summary>Getter for the Cell of Board</summary>
        public static uint GetCellOfBoardArray(int x, int y)
        {
            return _boardArray[x, y];
        }


        /// <summary>Setter for the Cell of Board</summary>
        public static void SetCellOfBoardArray(int x, int y, uint value)
        {
            _boardArray[x, y] = value;
        }
    }
}