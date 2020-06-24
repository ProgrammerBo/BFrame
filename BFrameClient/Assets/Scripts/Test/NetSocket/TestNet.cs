using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNet 
{
    ServerTCP serverTCP;
    ClientTCP clientTCP;
    ClientUDP clientUDP;
    ServerUDP ServerUDP;

    public string key;

    public TestNet(string key)
    {
        this.key = key;
        if (key == "TCP")
        {
            serverTCP = new ServerTCP();
            clientTCP = new ClientTCP();
        }
        else if (key == "UDP")
        {
            clientUDP = new ClientUDP();
            ServerUDP = new ServerUDP();
        }
    }

    public void ClientSendMessage()
    {
        if (key == "TCP")
        {
            clientTCP.SendMessage("clientTCP.SendMessage");
        }
        else if (key == "UDP")
        {
            clientUDP.SendMessage("clientUDP.SendMessage");
        }
    }

    public void ServerSendMessage()
    {
        if (key == "TCP")
        {
            serverTCP.SendMessage("serverTCP.SendMessage");
        }
        else if (key == "UDP")
        {
            ServerUDP.SendMessage("ServerUDP.SendMessage");
        }
    }

    public void Close()
    {
        if (key == "TCP")
        {
            serverTCP.Close();
        }
    }
}
