using System;
using System.Net;
using System.Runtime.InteropServices;
using Gdk;
using Gtk;
using Window = Gtk.Window;
using WindowType = Gtk.WindowType;

namespace FiveInRow
{
    public partial class ConfigGameWindow : Window
    {
        private string _multiplayerRole;
        public string MultiplayerRole => _multiplayerRole;
        public RadioButton RbAi => rbAi;
        public RadioButton RbMultiplayer => rbMultiplayer;
        public Entry EntryIpServer => entryIpServer;
        public Entry EntryPortServer => entryPortServer;
        public Entry EntryIpClient => entryIpClient;
        public Entry EntryPortClient => entryPortClient;
        private Entry _entryReceivedData;
        private Server _server;
        private Board _board;
        private (int x, int y) _coord;

        public Server ServerRunning => _server;

        private string _typeOfReceiveData;
        
        public SpinButton SbBoardSize => sbBoardSize;

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
            _entryReceivedData.Visibility = false;
            // _entryReceivedData.ShowNow();
            
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
                _multiplayerRole = "PLAYER1";
                btn.Label = "STOP";
            }
            else if (btn.Label == "STOP")
            {
                _server.StopServer();
                _multiplayerRole = null;
                btn.Label = "START";
                HBoxInfoLabel.Visible = false;

                if (_server != null)
                {
                    _server.Dispose();
                }
            }
        }

        private void OnClickBtnConnectClient(object sender, EventArgs e)
        {
            Button btn = (Button) sender;
            _server = new Server();
            
            if (btn.Label == "CONNECT")
            {
                _server.ConnectClient();
                _multiplayerRole = "PLAYER2";
                btn.Label = "DISCONNECT";
            }
            else if (btn.Label == "DISCONNECT")
            {
                _server.DisconnectClient();
                _multiplayerRole = null;
                btn.Label = "CONNECT";
            }
        }

        private void OnChangeReceivedData(object sender, EventArgs e)
        {
            Entry dataGame = (Entry) sender;
            
            ReaderReceivedConfig(dataGame.Text);

            if (_typeOfReceiveData == "CONFIG")
            {
                Gtk.Application.Invoke (delegate {
                    this.Hide();
                    HBoxInfoLabel.Visible = false;
                    _board = new Board(_row, _col);
                });
            }

            if (_typeOfReceiveData == "MOVE")
            {
                Gtk.Application.Invoke (delegate {
                    _board.OpponentMove(_coord);
                });
            }
        }


        private void ReaderReceivedConfig(string data)
        {
            string[] splitArray = data.Split(new string[]{ "<|>" }, StringSplitOptions.None);

            if (splitArray.Length > 0)
            {
                _typeOfReceiveData = splitArray[0];
                
                if (_typeOfReceiveData == "CONFIG")
                {
                    if (splitArray[1] == "PLAYER1")
                    {
                        if (!string.IsNullOrEmpty(EntryName.Text))
                        {
                            Board.Player1Name = EntryName.Text;
                        }
                        
                        if (splitArray[2] != "EMPTY_NAME")
                        {
                            Board.Player2Name = splitArray[2];
                        }
                        
                        _row = _col = Convert.ToUInt32(sbBoardSize.Text, 10);
                    }
                    else if (splitArray[1] == "PLAYER2")
                    {
                        if (!string.IsNullOrEmpty(EntryName.Text))
                        {
                            Board.Player2Name = EntryName.Text;
                        }
                        
                        if (splitArray[2] != "EMPTY_NAME")
                        {
                            Board.Player1Name = splitArray[2];
                        }

                        _row = Convert.ToUInt32(splitArray[3], 10);
                        _col = Convert.ToUInt32(splitArray[4], 10);
                    }
                }
                else if (_typeOfReceiveData == "MOVE")
                {
                    _coord = ReaderReceivedMove(splitArray[1]);
                }
            }
        }

        private (int x, int y) ReaderReceivedMove(string move)
        {
            string[] coords = move.Split(',');
            int x = Int32.Parse(coords[0]);
            int y = Int32.Parse(coords[1]);

            return (x, y);
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
