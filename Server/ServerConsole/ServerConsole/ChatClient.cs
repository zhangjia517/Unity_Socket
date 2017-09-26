using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;

namespace ServerConsole
{
    internal class ChatClient
    {
        /// <summary>
        /// 客户列表
        /// </summary>
        public static Hashtable AllClients = new Hashtable();

        /// <summary>
        /// 客户端实体
        /// </summary>
        private readonly TcpClient _client;

        /// <summary>
        /// 消息数据
        /// </summary>
        private readonly byte[] _data;

        /// <summary>
        /// 客户端昵称
        /// </summary>
        private string _clientNick;

        /// <summary>
        /// 是否昵称
        /// </summary>
        private bool _receiveNick = true;

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIp;

        public ChatClient(TcpClient client)
        {
            _client = client;
            ClientIp = client.Client.RemoteEndPoint.ToString();
            // 把当前客户端实例添加到客户列表当中
            AllClients.Add(ClientIp, this);
            _data = new byte[_client.ReceiveBufferSize];
            // 从服务端获取消息
            client.GetStream().BeginRead(_data, 0, Convert.ToInt32(_client.ReceiveBufferSize), ReceiveMessage, null);
        }

        /// <summary>
        /// 从客戶端获取消息
        /// </summary>
        /// <param name="ar"></param>
        public void ReceiveMessage(IAsyncResult ar)
        {
            try
            {
                int bytesRead;
                lock (_client.GetStream())
                {
                    bytesRead = _client.GetStream().EndRead(ar);
                }
                if (bytesRead < 1)
                {
                    AllClients.Remove(ClientIp);
                    Broadcast(_clientNick + " has left the chat");
                    return;
                }

                var messageReceived = Encoding.UTF8.GetString(_data, 0, bytesRead);
                if (_receiveNick)
                {
                    _clientNick = messageReceived;
                    Broadcast(_clientNick + " has joined the chat");
                    _receiveNick = false;
                }
                else
                {
                    Broadcast(_clientNick + ">" + messageReceived);
                }

                lock (_client.GetStream())
                {
                    _client.GetStream().BeginRead(_data, 0, Convert.ToInt32(_client.ReceiveBufferSize), ReceiveMessage,
                        null);
                }
            }
            catch (Exception)
            {
                AllClients.Remove(ClientIp);
                Broadcast(_clientNick + " has left the chat");
            }
        }

        /// <summary>
        /// 向客戶端发送消息
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            try
            {
                NetworkStream ns;
                lock (_client.GetStream())
                {
                    ns = _client.GetStream();
                }
                // 对信息进行编码
                var bytesToSend = Encoding.UTF8.GetBytes(message);
                ns.Write(bytesToSend, 0, bytesToSend.Length);
                ns.Flush();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// 向客户端广播消息
        /// </summary>
        /// <param name="message"></param>
        public void Broadcast(string message)
        {
            Console.WriteLine(message);
            foreach (DictionaryEntry c in AllClients)
                ((ChatClient)c.Value).SendMessage(message + Environment.NewLine);
        }
    }
}