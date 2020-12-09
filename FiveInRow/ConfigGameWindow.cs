using System;
using Gtk;

namespace FiveInRow
{
    public partial class ConfigGameWindow : Window
    { 
        public ConfigGameWindow() :
                base(WindowType.Toplevel)
        {
            this.Build();
            btnPlay.Clicked += new EventHandler(playGame);
            DeleteEvent += delegate { Application.Quit(); };
        }

        private void playGame(object sender, EventArgs e)
        {
            Board.SetConfigGameWindow(this);
            this.Hide();
            uint row, col;
            row = col = Convert.ToUInt32(sbBoardSize.Text, 10);
            _ = new Board(row, col);

        }

    }

}
