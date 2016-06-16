using System;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    private const int portNo = 500;
    private TcpClient _client;
    private byte[] data;
    public string nickName = "";
    public string message = "";
    public string sendMsg = "";

    //public GUIStyle guiStyle = null;
    private Queue myQueue = new Queue();

    private void OnGUI()
    {
        nickName = GUI.TextField(new Rect(10, 10, 100, 20), nickName);
        message = GUI.TextArea(new Rect(10, 40, 300, 350), message);
        sendMsg = GUI.TextField(new Rect(10, 400, 210, 20), sendMsg);
        if (GUI.Button(new Rect(120, 10, 80, 20), "Connect"))
        {
            //Debug.Log("hello");
            this._client = new TcpClient();
            this._client.Connect("192.168.16.150", portNo);
            data = new byte[this._client.ReceiveBufferSize];
            //SendMessage(txtNick.Text);
            SendMessage(nickName);
            this._client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(this._client.ReceiveBufferSize), ReceiveMessage, null);
        };
        if (GUI.Button(new Rect(230, 400, 80, 20), "Send"))
        {
            SendMessage(sendMsg);
            sendMsg = "";
        };
    }

    public void SendMessage(string message)
    {
        try
        {
            NetworkStream ns = this._client.GetStream();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            ns.Write(data, 0, data.Length);
            ns.Flush();
        }
        catch (Exception ex)
        {
        }
    }

    public void ReceiveMessage(IAsyncResult ar)
    {
        try
        {
            int bytesRead;
            bytesRead = this._client.GetStream().EndRead(ar);
            if (bytesRead < 1)
            {
                return;
            }
            else
            {
                Debug.Log(System.Text.Encoding.UTF8.GetString(data, 0, bytesRead));

                if (myQueue.Count >= 15)
                {
                    myQueue.Enqueue(System.Text.Encoding.UTF8.GetString(data, 0, bytesRead));
                    myQueue.Dequeue();
                }
                else
                {
                    myQueue.Enqueue(System.Text.Encoding.UTF8.GetString(data, 0, bytesRead));
                }
                RefreshMessage(myQueue);
            }
            this._client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(this._client.ReceiveBufferSize), ReceiveMessage, null);
        }
        catch (Exception ex)
        {
        }
    }

    public void RefreshMessage(IEnumerable myCollection)
    {
        message = "";
        foreach (string str in myCollection)
        {
            message += str;
        }
    }
}