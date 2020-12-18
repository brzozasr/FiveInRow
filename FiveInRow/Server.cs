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
        private Socket _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

        public Socket SocketListener => _socket;

        private BackgroundWorker _messageReceiverFirst;
        private BackgroundWorker _messageReceiverSecond;
        private BackgroundWorker _messageReceiverThird;
        private BackgroundWorker _messageReceiverFourth;
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
        }

        private void InitBackgroundWorkers()
        {
            _messageReceiverFirst = new BackgroundWorker();
            _messageReceiverSecond = new BackgroundWorker();
            _messageReceiverThird = new BackgroundWorker();
            _messageReceiverFourth = new BackgroundWorker();
            _waitForConnection = new BackgroundWorker();
        }

        private void BackgroundWorkersAddLListener()
        {
            _messageReceiverFirst.DoWork += MessageReceiverFirstDoWork;
            _messageReceiverFirst.RunWorkerCompleted += MessageReceiverFirstWorkerCompleted;
            _messageReceiverSecond.DoWork += MessageReceiverSecondDoWork;
            _messageReceiverSecond.RunWorkerCompleted += MessageReceiverSecondWorkerCompleted;
            _messageReceiverThird.DoWork += MessageReceiverThirdDoWork;
            _messageReceiverThird.RunWorkerCompleted += MessageReceiverThirdWorkerCompleted;
            _messageReceiverFourth.DoWork += MessageReceiverFourthDoWork;
            _messageReceiverFourth.RunWorkerCompleted += MessageReceiverFourthWorkerCompleted;
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
                _messageReceiverFirst.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                DialogWindow(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }


        protected internal void StopServer()
        {
            _waitForConnection.WorkerSupportsCancellation = true;
            _waitForConnection.CancelAsync();
            _waitForConnection.Dispose();
            try
            {
                if (_socket.Connected)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                }

                _listener.Stop();
                _socket.Close();

                Console.WriteLine("Server stopped.");
            }
            catch (Exception ex)
            {
                DialogWindow(ex.Message);
                Console.WriteLine(ex);
            }
        }


        protected internal void DisconnectClient()
        {
            _messageReceiverFirst.WorkerSupportsCancellation = true;
            _messageReceiverFirst.CancelAsync();
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
            _config.EntryReceivedData.Text = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);
        }

        protected internal void SendMove(string move)
        {
            byte[] messageSent = Encoding.ASCII.GetBytes(move);
            int byteSent = _socket.Send(messageSent);

            if (!_messageReceiverFirst.IsBusy)
            {
                _messageReceiverFirst.RunWorkerAsync();
            }
            else if (!_messageReceiverSecond.IsBusy)
            {
                _messageReceiverSecond.RunWorkerAsync();
            }
            else if (!_messageReceiverThird.IsBusy)
            {
                _messageReceiverThird.RunWorkerAsync();
            }
            else if (!_messageReceiverFourth.IsBusy)
            {
                _messageReceiverFourth.RunWorkerAsync();
            }
            else
            {
                DialogWindow("This BackgroundWorker is currently busy and cannot run multiple tasks concurrently.");
            }
        }

        private void MessageReceiverFirstDoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("First");
            ReceiveMove();
        }

        private void MessageReceiverFirstWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _messageReceiverFirst.WorkerSupportsCancellation = true;
            _messageReceiverFirst.CancelAsync();
        }

        private void MessageReceiverSecondDoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("Second");
            ReceiveMove();
        }

        private void MessageReceiverSecondWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _messageReceiverSecond.WorkerSupportsCancellation = true;
            _messageReceiverSecond.CancelAsync();
        }

        private void MessageReceiverThirdDoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("Third");
            ReceiveMove();
        }


        private void MessageReceiverThirdWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _messageReceiverThird.WorkerSupportsCancellation = true;
            _messageReceiverThird.CancelAsync();
        }

        private void MessageReceiverFourthDoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("Fourth");
            ReceiveMove();
        }

        private void MessageReceiverFourthWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _messageReceiverFourth.WorkerSupportsCancellation = true;
            _messageReceiverFourth.CancelAsync();
        }

        private void WaitForConnectionDoWork(object sender, DoWorkEventArgs e)
        {
            _socket = _listener.AcceptSocket();
        }

        private void WaitForConnectionWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("Client connected...");
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