using System;
using System.Collections.Generic;
using Gtk;

namespace FiveInRow
{
    public class Board : Window
    {
        public Board(uint row, uint col) : base("Board")
        {
            SetDefaultSize(640, 600);
            SetPosition(WindowPosition.Center);
            DeleteEvent += delegate { Application.Quit(); };

            List<Button> buttonLists = new List<Button>();

            Table table = new Table(row, col, true);

            Console.WriteLine(buttonLists.Count);

            for (uint i = 0; i < row; i++)
            {
                for (uint j = 0; j < col; j++)
                {
                    Button cell = new Button($"{i}, {j}");
                    table.Attach(cell, j, j + 1, i, i + 1);
                    buttonLists.Add(cell);
                }
            }

            Console.WriteLine(buttonLists.Count);

            foreach (Button btn in buttonLists)
            {
                btn.Clicked += OnClick;
            }

            Add(table);
            ShowAll();
        }

        private void OnClick(object sender, EventArgs args)
        {
            Application.Quit();
        }
    }

}
