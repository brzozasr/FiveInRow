using System;
using Gtk;

namespace FiveInRow
{
    public partial class ConfigGameWindow : Window
    {
        public RadioButton RbAi => rbAi;
        public RadioButton RbMultiplayer => rbMultiplayer;
        private uint _row;
        private uint _col;

        public uint Col
        {
            get => _col;
        }
        public uint Row
        {
            get => _row;
        }

        public ConfigGameWindow() :
                base(WindowType.Toplevel)
        {
            this.Build();
            btnPlay.Clicked += new EventHandler(PlayGame);
            DeleteEvent += delegate { Application.Quit(); };

            hboxButtons.Visible = false;
        }

        private void PlayGame(object sender, EventArgs e)
        {
            Board.SetConfigGameWindow(this);
            _row = _col = Convert.ToUInt32(sbBoardSize.Text, 10);

            if (rbAi.Active)
            {
                this.Hide();
                _ = new Board(_row, _col);
            }
            else if (rbMultiplayer.Active)
            {

            }

        }
    }

}
