using System;
using System.ComponentModel;
using System.IO;
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
        private TcpListener _listener;
        public TcpListener Listener => _listener;

        private StreamReader _reader;
        private StreamWriter _writer;
        private string _receiveText;
        private string _textToSend;
        public TcpClient Client => _client;
        private BackgroundWorker _messageReceiver;
        private BackgroundWorker _messageSender;

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
            try
            {
                _listener = new TcpListener(IPAddress.Parse(_config.EntryIpServer.Text),
                    int.Parse(_config.EntryPortServer.Text));
                _listener.Start();
                _client = _listener.AcceptTcpClient();

                _reader = new StreamReader(_client.GetStream());
                _writer = new StreamWriter(_client.GetStream());
                _writer.AutoFlush = true;

                _messageReceiver.RunWorkerAsync();
                _messageSender.WorkerSupportsCancellation = true;
            }
            catch (SocketException ex)
            {
                DialogWindow(ex.Message);
            }
        }


        protected internal void ConnectClient()
        {
            _client = new TcpClient();
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(_config.EntryIpClient.Text),
                int.Parse(_config.EntryPortClient.Text));
            try
            {
                _client.Connect(ipEnd);

                if (_client.Connected)
                {
                    Console.WriteLine("Connected to server..." + "\n");
                    _writer = new StreamWriter(_client.GetStream());
                    _reader = new StreamReader(_client.GetStream());
                    _writer.AutoFlush = true;
                    _messageReceiver.RunWorkerAsync();
                    _messageSender.WorkerSupportsCancellation = true;
                }
            }
            catch (Exception ex)
            {
                DialogWindow(ex.Message);
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


        // private void ReceiveMove()
        // {
        //     byte[] buffer = new byte[150];
        //     _socket.Receive(buffer);
        //     // TODO
        //     foreach (var val in buffer)
        //     {
        //         Console.Write(val + ", ");
        //     }
        //     
        // }
        //
        //
        // protected internal void SendMove(string move)
        // {
        //     byte[] num = {1};
        //     _socket.Send(num);
        //     _messageReceiver.RunWorkerAsync();
        // }


        private void MessageReceiverDoWork(object sender, DoWorkEventArgs e)
        {
            while (_client.Connected)
            {
                try
                {
                    _receiveText = _reader.ReadLine();

                    if (!String.IsNullOrEmpty(_receiveText))
                    {
                        Console.WriteLine("_receiveText: " + _receiveText);

                        Application.Invoke(delegate
                        {
                            _config.EntryReceivedData.Text = _reader.ReadLine();
                        });
                        
                    }
                    _receiveText = "";
                }
                catch (Exception ex)
                {
                    DialogWindow(ex.Message);
                }
            }
        }

        private void MessageSenderDoWork(object sender, DoWorkEventArgs e)
        {
            if (_client.Connected)
            {
                _writer.WriteLine(_textToSend);

                // Console.WriteLine("_textToSend: " + _textToSend);
                //
                // Application.Invoke(delegate { _config.EntryReceivedData.Text = _textToSend; });
            }
            else
            {
                DialogWindow("Sending failed");
            }

            _messageSender.CancelAsync();
        }


        protected internal void RunSender(string text)
        {
            _textToSend = text;
            _messageSender.RunWorkerAsync();
        }


        protected internal string DataReceiver(string inputText)
        {
            return inputText;
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