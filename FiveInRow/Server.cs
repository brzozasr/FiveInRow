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
        private TcpListener _server;
        private Socket _socket;
        private BackgroundWorker _messageReceiver;
        

        public Server()
        {
            InitBackgroundWorkers();
            BackgroundWorkersAddLListener();
        }

        private void InitBackgroundWorkers()
        {
            _messageReceiver = new BackgroundWorker();
        }

        private void BackgroundWorkersAddLListener()
        {
            _messageReceiver.DoWork += MessageReceiverDoWork;
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
            catch(SocketException ex)
            {
                DialogWindow(ex.Message);
            }
            finally
            {
                _server.Stop();
            }
        }


        protected internal void StopServer()
        {
            _messageReceiver.WorkerSupportsCancellation = true;
            _messageReceiver.CancelAsync();
            if (_server != null)
                _server.Stop();
        }


        protected internal void ConnectClient()
        {
            try
            {
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(_config.EntryIpClient.Text),
                    int.Parse(_config.EntryPortClient.Text));
                _client = new TcpClient(ipEnd);
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
            }
            finally
            {
                _client.Close();
            }
        }
        
        
        private void ReceiveMove()
        {
            byte[] buffer = new byte[1];
            _socket.Receive(buffer);
            // TODO
        }
        
        
        private void SendMove(string move)
        {
            byte[] num = { 1 };
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