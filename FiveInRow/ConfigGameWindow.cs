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
            uint row, col;
            row = col = Convert.ToUInt32(sbBoardSize.Text, 10);

            if (rbAi.Active)
            {
                this.Hide();
                _ = new Board(row, col);
            }
            else if (rbMultiplayer.Active)
            {

            }

        }

    }

}
