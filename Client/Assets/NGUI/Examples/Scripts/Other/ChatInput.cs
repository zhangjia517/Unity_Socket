using System;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;

[RequireComponent(typeof(UIInput))]
[AddComponentMenu("NGUI/Examples/Chat Input")]
public class ChatInput : MonoBehaviour
{
    public UITextList textList;
    public UIInput m_IptUsername;

    private UIInput mInput;
    private byte[] data;
    private TcpClient _client;
    private const int portNo = 5819;
    private Queue myQueue = new Queue();

    private void Start()
    {
        mInput = GetComponent<UIInput>();
        mInput.label.maxLineCount = 1;
    }

    public void OnSubmit()
    {
        SendMsg2Server(mInput.value);
    }

    public void OnConnect()
    {
        this._client = new TcpClient();
        this._client.Connect("182.92.8.213", portNo);
        data = new byte[this._client.ReceiveBufferSize];
        SendMsg2Server(m_IptUsername.value);
        this._client.GetStream().BeginRead(data, 0, System.Convert.ToInt32(this._client.ReceiveBufferSize), ReceiveMessage, null);
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

                if (myQueue.Count >= 16) //ÐÐÊý
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
            Debug.Log(ex);
        }
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
            Debug.Log(ex);
        }
    }

    public void RefreshMessage(IEnumerable myCollection)
    {
        string message = "";
        foreach (string str in myCollection)
        {
            message += str;
        }

        textList.textLabel.text = message;

        //if (textList != null)
        //{
        //    //string text = NGUIText.StripSymbols(message);

        //    if (!string.IsNullOrEmpty(message))
        //    {
        //        textList.Add(message);
        //        mInput.value = "";
        //        mInput.isSelected = false;
        //    }
        //}
    }
}