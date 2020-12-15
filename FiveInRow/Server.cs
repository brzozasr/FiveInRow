using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace FiveInRow
{
    class Server
    {
        private TcpClient client;
        public StreamReader streamToReader;
        public StreamWriter streamToWriter;
        public string recieve;
        public string TextToSend;
        private BackgroundWorker _backgroundWorker1;
        private BackgroundWorker _backgroundWorker2;
    }
}