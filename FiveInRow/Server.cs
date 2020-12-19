using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gtk;
using Socket = System.Net.Sockets.Socket;


namespace FiveInRow
{
    public class Server
    {
        private Socket _socket;

        public Socket SocketListener => _socket;

        private BackgroundWorker _messageReceiver;
        private BackgroundWorker _waitForConnection;
        private TcpListener _listener;

        public TcpListener Listener => _listener;

        private TcpClient _client;

        public TcpClient Client => _client;

        private static ConfigGameWindow _config;

        public Server()
        {
            InitBackgroundWorkers();
            BackgroundWorkersAddLListener();
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        }

        private void InitBackgroundWorkers()
        {
            _messageReceiver = new BackgroundWorker();
            _waitForConnection = new BackgroundWorker();

            _messageReceiver.WorkerSupportsCancellation = true;
        }

        private void BackgroundWorkersAddLListener()
        {
            _messageReceiver.DoWork += MessageReceiverDoWork;
            _messageReceiver.RunWorkerCompleted += MessageReceiverWorkerCompleted;
            _waitForConnection.DoWork += WaitForConnectionDoWork;
            _waitForConnection.RunWorkerCompleted += WaitForConnectionWorkerCompleted;
        }


        protected internal void StartServer()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Parse(_config.EntryIpServer.Text),
                    int.Parse(_config.EntryPortServer.Text));
                _listener.Start();
                Console.WriteLine("Wait for connection...");
                _config.HBoxInfoLabel.Visible = true;
                _waitForConnection.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                DialogWindow(ex.Message);
                Console.WriteLine(ex);
            }
        }


        protected internal void ConnectClient()
        {
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(_config.EntryIpClient.Text),
                int.Parse(_config.EntryPortClient.Text));
            try
            {
                _client = new TcpClient();
                _client.Connect(ipEnd);
                _socket = _client.Client;
                _messageReceiver.RunWorkerAsync();
                
                if (_client.Connected)
                {
                    Console.WriteLine("Connected...");
                    SendMove(ServerConfig());
                }
            }
            catch (Exception ex)
            {
                DialogWindow(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }


        protected internal void StopServer()
        {
            try
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket.Dispose();
                }

                if (_listener != null)
                {
                    _listener.Stop();
                }
                
                if (_waitForConnection.CancellationPending)
                {
                    _waitForConnection.WorkerSupportsCancellation = true;
                    _waitForConnection.CancelAsync();
                    _waitForConnection.Dispose();
                }

                GC.SuppressFinalize(this);
                GC.Collect();

                Console.WriteLine("Server stopped...");
            }
            catch (Exception ex)
            {
                DialogWindow(ex.Message);
                Console.WriteLine(ex);
            }
        }


        protected internal void DisconnectClient()
        {
            _messageReceiver.WorkerSupportsCancellation = true;
            _messageReceiver.CancelAsync();
            _messageReceiver.Dispose();
            if (_client != null)
            {
                try
                {
                    _client.Client.Shutdown(SocketShutdown.Both);
                    _client.Client.Close();
                    _client.Close();
                    GC.Collect();
                }
                catch (ObjectDisposedException ex)
                {
                    DialogWindow(GetType().FullName + "\n" + ex.Message);
                }
            }
        }

        private void ReceiveMove()
        {
            while (true)
            {
                byte[] messageReceived = new byte[1024];

                if (messageReceived.Length > 0)
                {
                    int byteRecv = _socket.Receive(messageReceived);
                    Console.WriteLine("{0}", Encoding.ASCII.GetString(messageReceived,
                        0, byteRecv));

                    _config.EntryReceivedData.Text = Encoding.ASCII.GetString(messageReceived, 0,
                        byteRecv);
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }

        protected internal void SendMove(string move)
        {
            byte[] messageSent = Encoding.ASCII.GetBytes(move);
            int byteSent = _socket.Send(messageSent);
        }

        private void MessageReceiverDoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("Receiver DoWork...");
            ReceiveMove();
        }

        private void MessageReceiverWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Receiver Cancellation...");
            _messageReceiver.CancelAsync();
        }

        private void WaitForConnectionDoWork(object sender, DoWorkEventArgs e)
        {
            // if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            // {
            //     _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            // }
            
            _socket = _listener.AcceptSocket();
        }

        private void WaitForConnectionWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Client connected...");
            SendMove(ClientConfig());
            _messageReceiver.RunWorkerAsync();
        }


        private string ServerConfig()
        {
            string playerName;
            if (string.IsNullOrEmpty(_config.EntryName.Text))
            {
                playerName = "EMPTY_NAME";
            }
            else
            {
                playerName = _config.EntryName.Text;
            }

            return $"CONFIG<|>PLAYER1<|>{playerName}";
        }
        
        private string ClientConfig()
        {
            string playerName;
            if (string.IsNullOrEmpty(_config.EntryName.Text))
            {
                playerName = "EMPTY_NAME";
            }
            else
            {
                playerName = _config.EntryName.Text;
            }
            
            return $"CONFIG<|>PLAYER2<|>{playerName}<|>{_config.SbBoardSize.Text}<|>{_config.SbBoardSize.Text}";
        }
        
        
        private void DialogWindow(string message)
        {
            MessageDialog md = new MessageDialog(_config,
                DialogFlags.DestroyWithParent, MessageType.Info,
                ButtonsType.Close, message);
            md.Run();
            md.Destroy();
        }

        public static void SetConfigGameWindow(ConfigGameWindow configGameWindow)
        {
            _config = configGameWindow;
        }
    }
}