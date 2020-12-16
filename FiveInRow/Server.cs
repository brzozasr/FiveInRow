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
        public TcpClient Client => _client;
        public TcpListener ServerListener => _server;

        private TcpListener _server;
        private Socket _socket;
        private StreamReader _reader;
        private StreamWriter _writer;
        private BackgroundWorker _messageReceiver;
        private BackgroundWorker _messageSender;
        private string _textToSend;


        public Server()
        {
            InitBackgroundWorkers();
            BackgroundWorkersAddLListener();
        }

        private void InitBackgroundWorkers()
        {
            _messageReceiver = new BackgroundWorker();
            _messageSender = new BackgroundWorker();
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
                _server = new TcpListener(IPAddress.Parse(_config.EntryIpServer.Text),
                    int.Parse(_config.EntryPortServer.Text));
                _server.Start();
                _client = _server.AcceptTcpClient();
                
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
            try
            {
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(_config.EntryIpClient.Text),
                    int.Parse(_config.EntryPortClient.Text));
                _client = new TcpClient();
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
            if(_client.Connected)
            {
                try
                {
                    string receive = _reader.ReadLine();
                    
                    Application.Invoke (delegate {
                        _config.EntryReceivedData.Text = receive;
                    });
                    receive = "";
                }
                catch(Exception ex)
                {
                    DialogWindow(ex.Message);
                }
            }
        }
        
        private void MessageSenderDoWork(object sender, DoWorkEventArgs e)
        {
            if(_client.Connected)
            {
                _writer.WriteLine(_textToSend);
                Application.Invoke (delegate {
                    _config.EntryReceivedData.Text = _textToSend;
                });
            }
            else
            {
                DialogWindow("Sending failed");
            }
            _messageSender.CancelAsync();
        }
        
        
        protected internal void RunSender()
        {
            // if(MessagetextBox.Text!="")
            // {
            _textToSend = "Test text :)";
                // _textToSend = _config.EntryReceivedData.Text;
                _messageSender.RunWorkerAsync();
            // }
            // MessagetextBox.Text = "";

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