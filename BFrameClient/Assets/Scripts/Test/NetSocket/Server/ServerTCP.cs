using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

class ServerTCP
{
    TcpClient client;
    Thread thread;
    RemoteClient remote;
    public ServerTCP()
    {
        thread = new Thread(MyThread);
        thread.Start();

        Debug.Log("ServerTCP Start!");
    }

    private void MyThread()
    {
        IPAddress address = IPAddress.Parse("127.0.0.1");
        int port = 8500;
        TcpListener listener = new TcpListener(address, port);
        listener.Start();
        while (true)
        {
            client = listener.AcceptTcpClient();
            remote = new RemoteClient(client);
        }
    }
    public void Close()
    {
        if (thread.IsAlive)
            thread.Abort();
    }

    public void SendMessage(string msg)
    {
        remote.SendMessage(msg);
    }

    class RemoteClient
    {
        public const int BuffseSize = 8192;
        NetworkStream stream;
        byte[] buffer;

        public RemoteClient(TcpClient client)
        {
            stream = client.GetStream();
            buffer = new byte[BuffseSize];

            stream.BeginRead(buffer, 0, BuffseSize, ReadAsync, null);
        }

        private void ReadAsync(IAsyncResult ar)
        {
            try
            {
                int readCount = stream.EndRead(ar);
                if (0 == readCount) throw new Exception("Read 0 Size!");

                string msg = Encoding.UTF8.GetString(buffer, 0, readCount);
                Debug.Log("Server Receive Message:" + msg);

                lock (stream)
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

        public void SendMessage(string msg)
        {
            byte[] temp = Encoding.UTF8.GetBytes(msg);
            stream.Write(temp, 0, temp.Length);
            Debug.Log("Server SendMessage:" + msg);
        }
    }
}

