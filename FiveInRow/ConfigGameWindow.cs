using System;
using System.Net;
using Gdk;
using Gtk;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace FiveInRow
{
    public partial class ConfigGameWindow : Window
    {
        public RadioButton RbAi => rbAi;
        public RadioButton RbMultiplayer => rbMultiplayer;
        public Entry EntryIpServer => entryIpServer;
        public Entry EntryPortServer => entryPortServer;
        public Entry EntryIpClient => entryIpClient;
        public Entry EntryPortClient => entryPortClient;
        private Entry _entryReceivedData;
        private Server _server;

        public Entry EntryReceivedData
        {
            get => _entryReceivedData;
            set => _entryReceivedData = value;
        }

        public Label LbConnectionInfo
        {
            get => lbConnectionInfo;
            set => lbConnectionInfo = value;
        }

        public HBox HBoxInfoLabel
        {
            get => hboxInfoLabel;
            set => hboxInfoLabel = value;
        }

        public Button BtnStartServer
        {
            get => btnStartServer;
            set => btnStartServer = value;
        }

        private uint _row;
        private uint _col;
        protected internal Entry EntryName { get; private set; }

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
            this.Resize(520,275);
            this.Build();
            
            Board.SetConfigGameWindow(this);
            Server.SetConfigGameWindow(this);
            
            _entryReceivedData = new Entry();
            vboxMain.PackEnd(_entryReceivedData, true, true, 0);
            // entryReceivedData.Visibility = false;
            _entryReceivedData.ShowNow();
            
            Pango.FontDescription fontDescription = Pango.FontDescription.FromString("Arial");
            fontDescription.Size = 13000;
            fontDescription.Weight = Pango.Weight.Bold;
            Color red = new Color(255, 0, 0);
            lbConnectionInfo.ModifyFont(fontDescription);
            lbConnectionInfo.ModifyFg(StateType.Normal, red);
            
            btnPlay.Clicked += PlayGame;
            // DeleteEvent += delegate { Application.Quit(); };
            DeleteEvent += AppQuit;
            rbAi.Clicked += OnClickRadioBtnAi;
            rbMultiplayer.Clicked += OnClickRadioBtnMultiplayer;
            btnServerShow.Clicked += OnClickBtnServerShow;
            btnClientShow.Clicked += OnClickBtnClientShow;
            btnStartServer.Clicked += OnClickBtnStartServer;
            btnConnectClient.Clicked += OnClickBtnConnectClient;
            _entryReceivedData.Changed += OnChangeReceivedData;
            
            entryPortServer.Text = "5533";
            entryPortClient.Text = "5533";

            if (rbAi.Active)
            {
                hboxButtons.Visible = false;
                frameServer.Visible = false;
                frameClient.Visible = false;
                btnPlay.Sensitive = true;
                hboxInfoLabel.Visible = false;
            }
            else if (rbMultiplayer.Active)
            {
                hboxButtons.Visible = true;
                btnPlay.Sensitive = false;
                hboxInfoLabel.Visible = false;
            }
        }

        private void AppQuit(object o, DeleteEventArgs args)
        {
            Application.Quit();
        }

        private void OnClickBtnStartServer(object sender, EventArgs e)
        {
            Button btn = (Button) sender;
            _server = new Server();
            
            if (btn.Label == "START")
            {
                _server.StartServer();
                btn.Label = "STOP";
            }
            else if (btn.Label == "STOP")
            {
                Console.WriteLine("stop aaa");
                _server.StopServer();
                btn.Label = "START";
            }
        }

        private void OnClickBtnConnectClient(object sender, EventArgs e)
        {
            Button btn = (Button) sender;
            _server = new Server();
            
            if (btn.Label == "CONNECT")
            {
                _server.ConnectClient();
                btn.Label = "DISCONNECT";
            }
            else if (btn.Label == "DISCONNECT")
            {
                _server.DisconnectClient();
                btn.Label = "CONNECT";
            }
        }

        private void OnChangeReceivedData(object sender, EventArgs e)
        {
            _server.SendMove(_entryReceivedData.Text);
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
            btnPlay.Sensitive = true;
        }

        private void OnClickRadioBtnMultiplayer(object sender, EventArgs e)
        {
            hboxButtons.Visible = true;
            btnPlay.Sensitive = false;
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
            else if (rbMultiplayer.Active && frameServer.Visible == true)
            {
                
            }
            else  if (rbMultiplayer.Active && frameClient.Visible == true)
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
