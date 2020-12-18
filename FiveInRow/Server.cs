using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            try
            {
                _waitForConnection.WorkerSupportsCancellation = true;
                _waitForConnection.CancelAsync();
                _waitForConnection.Dispose();
                
                if (_socket != null)
                {
                    if (_socket.Connected)
                    {
                        _socket.Shutdown(SocketShutdown.Both);
                    }
                    
                    _socket.Close();
                    _socket.Dispose();
                    
                }

                if (_listener != null)
                {
                    _listener.Stop();
                }
                
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
            _socket = _listener.AcceptSocket();
        }

        private void WaitForConnectionWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Client connected...");
            _messageReceiver.RunWorkerAsync();
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