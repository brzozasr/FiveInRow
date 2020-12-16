using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using Gtk;
using Socket = System.Net.Sockets.Socket;


namespace FiveInRow
{
    public class Server
    {
        private static ConfigGameWindow _config;
        private TcpClient _client;
        public TcpClient Client => _client;
        public TcpListener ServerListener => _server;

        private TcpListener _server;
        private Socket _socket;
        private BackgroundWorker _messageReceiver;
        private BackgroundWorker _startingServer;


        public Server()
        {
            InitBackgroundWorkers();
            BackgroundWorkersAddLListener();
        }

        private void InitBackgroundWorkers()
        {
            _messageReceiver = new BackgroundWorker();
            _startingServer = new BackgroundWorker();
        }

        private void BackgroundWorkersAddLListener()
        {
            _messageReceiver.DoWork += MessageReceiverDoWork;
            _startingServer.DoWork += StartingServerDoWork;
            _startingServer.RunWorkerCompleted += StartingServerWorkerCompleted;
        }
        

        protected internal void StartServer()
        {
            try
            {
                _server = new TcpListener(IPAddress.Parse(_config.EntryIpServer.Text),
                    int.Parse(_config.EntryPortServer.Text));
                _server.Start();
                _socket = _server.AcceptSocket();
            }
            catch (SocketException ex)
            {
                DialogWindow(ex.Message);
            }
            finally
            {
                _server.Stop();
            }
        }


        protected internal void ConnectClient()
        {
            try
            {
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(_config.EntryIpClient.Text),
                    int.Parse(_config.EntryPortClient.Text));
                _client = new TcpClient();
                _client.Connect(ipEnd);
                _socket = _client.Client;
                _messageReceiver.RunWorkerAsync();

                if (_client.Connected)
                {
                    Console.WriteLine("Connected to server..." + "\n");
                }
            }
            catch (Exception ex)
            {
                DialogWindow(ex.Message);
                Console.WriteLine("Start server");
                Console.WriteLine(ex.Message);
            }
        }


        protected internal void StopServer()
        {
            _messageReceiver.WorkerSupportsCancellation = true;
            _messageReceiver.CancelAsync();
            if (_server != null)
            {
                _server.Stop();
                Console.WriteLine("Server stopped.");
            }
        }


        protected internal void DisconnectClient()
        {
            _messageReceiver.WorkerSupportsCancellation = true;
            _messageReceiver.CancelAsync();
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
            byte[] buffer = new byte[150];
            _socket.Receive(buffer);
            // TODO
            foreach (var val in buffer)
            {
                Console.Write(val + ", ");
            }
            
        }


        protected internal void SendMove(string move)
        {
            byte[] num = {1};
            _socket.Send(num);
            _messageReceiver.RunWorkerAsync();
        }


        private void MessageReceiverDoWork(object sender, DoWorkEventArgs e)
        {
            if (_client.Connected)
            {
                ReceiveMove();
            }
        }
        
        private void StartingServerDoWork(object sender, DoWorkEventArgs e)
        {

        }
        
        
        private void StartingServerWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Client connected to the server... :)" + "\n");
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