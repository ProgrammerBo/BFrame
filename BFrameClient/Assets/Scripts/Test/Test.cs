using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    //TestNet testNet;
    ClientSocket mSocket;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {

        //testNet = new TestNet("TCP");


        mSocket = new ClientSocket();
        mSocket.ConnectServer("127.0.0.1", 8088);  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            //testNet.ClientSendMessage();
            mSocket.SendMessage("Hello！");
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            //testNet.ServerSendMessage();
        }
    }


    void OnDestroy()
    {
        //testNet.Close();
    }
}
