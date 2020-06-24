using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ClientUDP
{
    SocketClient client;
    public ClientUDP()
    {
        client = new SocketClient();
        Debug.Log("ClientUDP Start!");
    }

    public void SendMessage(string msg)
    {
        client.SendMessage(msg);
    }


    class SocketClient
    {
        private UdpClient udpClient;
        private const int localPort = 6600;

        private const string remoteIP = "127.0.0.1";
        private const int remotePort = 5500;
        private IPEndPoint remotePoint;

        public SocketClient()
        {
            remotePoint = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);

            udpClient = new UdpClient(localPort);
            udpClient.Connect(IPAddress.Parse(remoteIP), remotePort);
            udpClient.BeginReceive(OnReceive, null);
        }

        void OnReceive(IAsyncResult result)
        {
            try
            {
                byte[] buffer = udpClient.EndReceive(result, ref remotePoint);
                OnMessage(buffer);

                lock (udpClient)
                {
                    udpClient.BeginReceive(OnReceive, null);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        void OnMessage(byte[] buffer)
        {
            string message = Encoding.Default.GetString(buffer);
            Debug.Log("Client Receive Message:" + message);
        }

        public void SendMessage(string message)
        {
            byte[] sendData = Encoding.Default.GetBytes(message);
            udpClient.Send(sendData, sendData.Length);
            Debug.Log("Client Send Message:" + message);
        }
    }
}

