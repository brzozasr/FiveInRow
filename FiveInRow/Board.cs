﻿using System;
using System.Collections.Generic;
using Gtk;
using System.Threading.Tasks;
using Gdk;
using Image = Gtk.Image;
using Window = Gtk.Window;


namespace FiveInRow
{
    public class Board : Window
    {
        private Table _table; // Gtk widget
        private static uint[,] _boardArray; // board with status of game
        protected internal const uint EmptyCell = 0; // emty cell (=0) of the game
        protected internal const uint Player1Mark = 1; // cell with mark (=1) the Player1 
        protected internal const uint Player2Mark = 2; // cell with mark (=2) the Player2
        private string _player1Image = Stock.Apply; // Image for player 1
        private string _player2Image = Stock.Cancel; // Image for player 2
        private bool _turn = true; // turn = PLAYER1
        private string _imageState;
        private static ConfigGameWindow _configGameWindow; // handler to ConfigGameWindow
        private Label _lbPlayer1;
        private Label _lbPlayer2;
        private Label _lbTurn;

        public Board(uint row, uint col) : base("FIVE IN ONE ROW")
        {
            SetPosition(WindowPosition.Center);
            this.TypeHint = Gdk.WindowTypeHint.Dialog;
            this.Resizable = false;
            DeleteEvent += DestroyBoard;

            List<Button> buttonLists = new List<Button>();
            _boardArray = new uint[row, col];

            VBox vBoxMain = new VBox(false, 0);
            HBox hBoxTopBar = new HBox(false, 0);
            _lbPlayer1 = new Label("Player 1");
            _lbPlayer2 = new Label("Player 2");
            _lbTurn = new Label("<= TURN");

            Pango.FontDescription fontDescription = Pango.FontDescription.FromString("Arial");
            fontDescription.Size = 13000;
            fontDescription.Weight = Pango.Weight.Bold;
            Color red = new Color(255, 0, 0);
            Color blue = new Color(0, 0, 255);
            Color green = new Color(0, 130, 0);
            _lbTurn.ModifyFont(fontDescription);
            _lbTurn.ModifyFg(StateType.Normal, blue);
            _lbPlayer1.ModifyFont(fontDescription);
            _lbPlayer1.ModifyFg(StateType.Normal, green);
            _lbPlayer2.ModifyFont(fontDescription);
            _lbPlayer2.ModifyFg(StateType.Normal, red);


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

            vBoxMain.PackStart(hBoxTopBar, true, true, 10);
            vBoxMain.PackEnd(_table, true, true, 0);

            hBoxTopBar.PackStart(_lbPlayer1, true, true, 10);
            hBoxTopBar.PackStart(_lbTurn, true, true, 10);
            hBoxTopBar.PackEnd(_lbPlayer2, true, true, 10);

            Add(vBoxMain);
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

            _boardArray[x, y] = Player1Mark;
            _turn = false;

            string hasWon = GameLogic.HasWon("PLAYER 1", "PLAYER 2");
            bool isBoardFull = GameLogic.IsBoardFull();

            GameStatus("PLAYER 1");

            if (_configGameWindow.RbAi.Active)
            {
                if (hasWon == null && !isBoardFull)
                {
                    BotMove();
                    // Task.Delay(250);
                    LbTurn.Text = "TURN =>";
                    ButtonsLocked(true);
                }
                else
                {
                    Console.WriteLine($"{hasWon} won!!!");
                    // TODO popup
                }
            }
            else if (_configGameWindow.RbMultiplayer.Active)
            {
                if (hasWon == null && !isBoardFull)
                {
                    PlayerMove();
                }
                else
                {
                    Console.WriteLine($"{hasWon} won!!!");
                    // TODO popup
                }
            }
        }


        private void BotMove()
        {
            (int x, int y) coordinates = GameLogic.AiMove();
            Console.WriteLine(coordinates);
            SetCellOfBoardArray(coordinates.x, coordinates.y, Player2Mark);
            _turn = true;
            LbTurn.Text = "<= TURN";

            ReplaceButton(coordinates);
            ReloadBoard(_configGameWindow.Row, _configGameWindow.Col);
            ButtonsLocked(false);

            // loop for updating GUI
            while (Gtk.Application.EventsPending())
            {
                Gtk.Application.RunIteration();
            }
        }


        private void ReplaceButton((int x, int y) coordinates)
        {
            string strLabel = $"{coordinates.x},{coordinates.y}";

            uint leftAttach = (uint) coordinates.y;
            uint rightAttach = (uint) coordinates.y + 1;
            uint topAttach = (uint) coordinates.x;
            uint bottomAttach = (uint) coordinates.x + 1;


            foreach (Button btn in _table.Children)
            {
                if (btn.Label == strLabel)
                {
                    _table.Remove(btn);
                }
            }

            Image image = new Image(_player2Image, IconSize.Button);
            Button newBtn = new Button(image);
            newBtn.WidthRequest = 40;
            newBtn.HeightRequest = 40;

            _table.Attach(newBtn, leftAttach, rightAttach, topAttach, bottomAttach);

            ShowAll();
        }


        private void ReloadBoard(uint row, uint col)
        {
            List<Button> buttonLists = new List<Button>();

            while (_table.Children.Length > 0)
            {
                _table.Remove(_table.Children[0]);
                _table.Children[0].Destroy();

                if (_table.Children.Length == 1)
                {
                    _table.Attach(new Button(), 1, 2, 1, 2);
                }
            }

            for (uint i = 0; i < row; i++)
            {
                for (uint j = 0; j < col; j++)
                {
                    Button cell;

                    if (_boardArray[i, j] == Player1Mark)
                    {
                        Image image = new Image(_player1Image, IconSize.Button);
                        cell = new Button(image);
                    }
                    else if (_boardArray[i, j] == Player2Mark)
                    {
                        Image image = new Image(_player2Image, IconSize.Button);
                        cell = new Button(image);
                    }
                    else
                    {
                        cell = new Button($"{i},{j}");
                        cell.Children[0].ChildVisible = false;
                    }

                    cell.WidthRequest = 40;
                    cell.HeightRequest = 40;
                    _table.Attach(cell, j, j + 1, i, i + 1);
                    buttonLists.Add(cell);
                }
            }

            foreach (Button btn in buttonLists)
            {
                if (btn.Label != null)
                {
                    btn.Clicked += OnClick;
                }
            }

            ShowAll();
        }


        private void ButtonsLocked(bool isLocked)
        {
            foreach (var btn in _table.Children)
            {
                btn.Sensitive = isLocked;
            }
        }


        private void GameStatus(string playerName, bool isDraw = false)
        {
            string message = "";
            
            if (isDraw)
            {
                message = "Nobody won is a draw.";
            }
            else
            {
                message = $"Player {playerName} won the game.";
            }
            
            MessageDialog md = new MessageDialog(this,
                DialogFlags.DestroyWithParent, MessageType.Info,
                ButtonsType.Close, message);
            md.Run();
            md.Destroy();
            
            md.Close += DestroyBoard;
        }

        private void DestroyBoard(object sender, EventArgs e)
        {
            this.Destroy();
            _configGameWindow.Show();
        }


        private void PlayerMove()
        {
        }


        private void GameListener()
        {
            foreach (var btn in _table.Children)
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
        public static uint[,] BoardArray // get and set for _boardArray
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

        protected internal Label LbPlayer1
        {
            get => _lbPlayer1;
            set => _lbPlayer1 = value;
        }

        protected internal Label LbPlayer2
        {
            get => _lbPlayer2;
            set => _lbPlayer2 = value;
        }

        protected internal Label LbTurn
        {
            get => _lbTurn;
            set => _lbTurn = value;
        }
    }
}