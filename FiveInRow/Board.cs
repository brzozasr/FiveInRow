using System;
using System.Collections.Generic;
using Gtk;

namespace FiveInRow
{
    public class Board : Window
    {
        uint[,] _boardArray;
        const uint EMPTY = 0;
        const uint PLAYER1 = 1;
        const uint PLAYER2 = 2;

        public Board(uint row, uint col) : base("Board")
        {
            SetDefaultSize(640, 600);
            SetPosition(WindowPosition.Center);
            DeleteEvent += delegate { Application.Quit(); };

            List<Button> buttonLists = new List<Button>();
            _boardArray = new uint[row, col];

            Table table = new Table(row, col, true);

            for (uint i = 0; i < row; i++)
            {
                for (uint j = 0; j < col; j++)
                {
                    Button cell = new Button($"{i},{j}");
                    cell.Children[0].ChildVisible = false;
                    table.Attach(cell, j, j + 1, i, i + 1);
                    buttonLists.Add(cell);
                    _boardArray[i, j] = EMPTY;
                }
            }

            foreach (Button btn in buttonLists)
            {
                btn.Clicked += OnClick;
            }

            Add(table);
            ShowAll();
        }

        private void OnClick(object sender, EventArgs args)
        {
            //Application.Quit();
            Button btn = (Button)sender;

            Console.WriteLine(btn.Label);
            Image image = new Image(Stock.Apply, IconSize.Button);
            //Image image = Image.LoadFromResource("sBallRed.png");
            //image.WidthRequest = 10;
            //image.HeightRequest = 10;
            btn.Remove(btn.Children[0]);
            //Image image = new Image("gtk-print", IconSize.Button);


            //btn.Image = image;
            btn.ChildVisible = true;
            btn.Add(image);
            Console.WriteLine(btn);


            Console.WriteLine(btn.Label);
            //btn.Label = "TEST";
        }
    }

}
