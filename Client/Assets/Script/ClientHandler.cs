using System;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    public string nickName = "";
    public string message = "";
    public string sendMsg = "";
    private const int portNo = 500;
    private TcpClient _client;
    private byte[] data;
    private Queue myQueue = new Queue();

    private void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 150, 5, 500, 20), "Please input your name and Connect :)");
        nickName = GUI.TextField(new Rect(Screen.width / 2 - 150, 30, 210, 20), nickName);
        message = GUI.TextArea(new Rect(Screen.width / 2 - 150, 60, 300, 250), message);
        sendMsg = GUI.TextField(new Rect(Screen.width / 2 - 150, 320, 210, 20), sendMsg);

        if (GUI.Button(new Rect(Screen.width / 2 + 230 - 155, 30, 75, 20), "Connect"))
        {
            this._client = new TcpClient();
            this._client.Connect("192.168.16.150", portNo);
            data = new byte[this._client.ReceiveBufferSize];
            SendMsg2Server(nickName);
            this._client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(this._client.ReceiveBufferSize), ReceiveMessage, null);
        };

        if (GUI.Button(new Rect(Screen.width / 2 + 230 - 155, 320, 75, 20), "Send"))
        {
            SendMsg2Server(sendMsg);
            sendMsg = "";
        };
    }

    public void SendMsg2Server(string message)
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

                if (myQueue.Count >= 16) //行数
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