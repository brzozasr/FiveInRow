using System;
using System.Net;
using System.Text;
using Gtk;

namespace FiveInRow
{
    public partial class ConfigGameWindow : Window
    {
        public RadioButton RbAi => rbAi;
        public RadioButton RbMultiplayer => rbMultiplayer;
        private uint _row;
        private uint _col;
        
        protected internal Entry EntryName
        {
            get => entryName;
        }

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
            rbAi.Clicked += OnClickRadioBtnAi;
            rbMultiplayer.Clicked += OnClickRadioBtnMultiplayer;
            btnServerShow.Clicked += OnClickBtnServerShow;
            btnClientShow.Clicked += OnClickBtnClientShow;
            
            entryPortServer.Text = "5533";
            entryPortClient.Text = "5533";

            if (rbAi.Active)
            {
                hboxButtons.Visible = false;
                frameServer.Visible = false;
                frameClient.Visible = false;
            }
            else if (rbMultiplayer.Active)
            {
                hboxButtons.Visible = true;
            }
        }

        private void OnClickBtnServerShow(object sender, EventArgs e)
        {
            frameServer.Visible = true;
            frameClient.Visible = false;
            entryIpServer.Text = GetIpAddress();
        }

        private void OnClickBtnClientShow(object sender, EventArgs e)
        {
            frameClient.Visible = true;
            frameServer.Visible = false;
        }

        private void OnClickRadioBtnAi(object sender, EventArgs e)
        {
            hboxButtons.Visible = false;
            frameServer.Visible = false;
            frameClient.Visible = false;
        }

        private void OnClickRadioBtnMultiplayer(object sender, EventArgs e)
        {
            hboxButtons.Visible = true;
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
        
        private string GetIpAddress()
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            // Get the IP  
            string myIP = Dns.GetHostEntry(hostName).AddressList[0].ToString();  
            return myIP;
        }
    }

}
