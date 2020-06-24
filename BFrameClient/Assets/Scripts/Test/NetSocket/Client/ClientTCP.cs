using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ClientTCP
{
    SocketClient client;
    public ClientTCP()
    {
        client = new SocketClient();

        Debug.Log("ClientTCP Start!");
    }

    public void SendMessage(string msg)
    {
        client.SendMessage(msg);
    }

    public class SocketClient
    {
        string ip = "127.0.0.1";
        int prot = 8500;

        public const int BuffseSize = 8192;
        NetworkStream stream;
        byte[] buffer;

        public SocketClient()
        {
            TcpClient client = new TcpClient();
            client.Connect(ip, prot);

            stream = client.GetStream();

            buffer = new byte[8192];
            stream.BeginRead(buffer, 0, buffer.Length, ReadAsync, null);
        }

        void ReadAsync(IAsyncResult result)
        {
            try
            {
                int readCount = stream.EndRead(result);
                if (readCount == 0) throw new Exception("读取到0字节");

                string msg = Encoding.UTF8.GetString(buffer, 0, readCount);
                Debug.Log("Client Receive Message:" + msg);

                lock (stream) //再次开启读取
                {
                    Array.Clear(buffer, 0, buffer.Length);
                    stream.BeginRead(buffer, 0, BuffseSize, ReadAsync, null);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        //发送消息到服务器
        public void SendMessage(string msg)
        {
            byte[] temp = Encoding.UTF8.GetBytes(msg);

            stream.Write(temp, 0, temp.Length);
            Debug.Log("Client SendMessage:" + msg);
        }
    }
}


