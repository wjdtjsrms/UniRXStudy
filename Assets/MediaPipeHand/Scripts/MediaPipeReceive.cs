using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Anipen.Subsystem.MeidaPipeHand
{
    public class MediaPipeReceive
    {
        private string data = "";
        private int port;
        private bool startRecieving = true;
        private Thread receiveThread;
        private UdpClient client;

        public string Data => data;


        public void Start(int port)
        {
            this.port = port;
            startRecieving = true;
            receiveThread = new Thread(new ThreadStart(ReceiveData))
            {
                IsBackground = true
            };
            receiveThread.Start();
        }

        public void Stop()
        {
            client?.Close();

            data = "";
            startRecieving = false;
            client = null;
        }

        private void ReceiveData()
        {
            client = new UdpClient(port);
            while (startRecieving)
            {
                try
                {
                    IPEndPoint anyIP = new(IPAddress.Any, 0);
                    byte[] dataByte = client.Receive(ref anyIP);
                    data = Encoding.UTF8.GetString(dataByte);
                }
                catch (Exception)
                {
                    Stop();
                }
            }
        }
    }
}