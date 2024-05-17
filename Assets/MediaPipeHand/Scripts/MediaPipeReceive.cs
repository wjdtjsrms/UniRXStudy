using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class MediaPipeReceive 
{
    public int port;
    public bool startRecieving = true;
    public bool printToConsole = false;
    public string data = "";

    private Thread receiveThread;
    private UdpClient client;

    public void Start(int port)
    {
        this.port = port;
        startRecieving = true;
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    public void Stop()
    {
        startRecieving = false;
    }

    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (startRecieving)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref anyIP);
                data = Encoding.UTF8.GetString(dataByte);
            }
            catch (Exception err)
            {
                Debug.Log(err.ToString());
            }
        }
    }
}
