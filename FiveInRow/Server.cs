using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Gtk;
using Socket = System.Net.Sockets.Socket;


namespace FiveInRow
{
    public class Server
    {
        private Socket _socket;
        private BackgroundWorker _messageReceiver;
        private BackgroundWorker _messageSender;
        private TcpListener _listener;

        public TcpListener Listener => _listener;

        private TcpClient _client;

        public TcpClient Client => _client;

        private static ConfigGameWindow _config;

        public Server()
        {
            InitBackgroundWorkers();
            BackgroundWorkersAddLListener();
        }

        private void InitBackgroundWorkers()
        {
            _messageReceiver = new BackgroundWorker(); // 1
            _messageSender = new BackgroundWorker(); // 2
        }

        private void BackgroundWorkersAddLListener()
        {
            _messageReceiver.DoWork += MessageReceiverDoWork;
            _messageSender.DoWork += MessageSenderDoWork;
        }


        protected internal void StartServer()
        {
            _listener = new TcpListener(IPAddress.Parse(_config.EntryIpServer.Text),
                int.Parse(_config.EntryPortServer.Text));
            try
            {
                _listener.Start();
                Console.WriteLine("Wait for connection...");
                _socket = _listener.AcceptSocket();
                Console.WriteLine("Client connected...");
            }
            catch (Exception ex)
            {
                DialogWindow(ex.Message);
                Console.WriteLine(ex.Message);
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
                Console.WriteLine("Connected...");
                _messageReceiver.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                DialogWindow(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }


        protected internal void StopServer()
        {
            _messageReceiver.WorkerSupportsCancellation = true;
            _messageReceiver.CancelAsync();
            if (_listener != null)
            {
                _listener.Stop();
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

        protected internal void ReceiveMove()
        {
            byte[] messageReceived = new byte[1024]; 
            
            int byteRecv = _socket.Receive(messageReceived); 
            Console.WriteLine("Message from Server -> {0}",  
                Encoding.ASCII.GetString(messageReceived,  
                    0, byteRecv));
        }
        
        protected internal void SendMove(string move)
        {
            byte[] messageSent = Encoding.ASCII.GetBytes(move); 
            int byteSent = _socket.Send(messageSent); 
            _messageReceiver.RunWorkerAsync();
        }

        private void MessageReceiverDoWork(object sender, DoWorkEventArgs e)
        {
            ReceiveMove();
        }

        private void MessageSenderDoWork(object sender, DoWorkEventArgs e)
        {
            
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