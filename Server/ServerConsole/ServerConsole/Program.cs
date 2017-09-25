using System;
using System.Net;
using System.Net.Sockets;

namespace ServerConsole
{
    internal class Program
    {
        /// <summary>
        /// 连接端口
        /// </summary>
        private const int PortNo = 5819;

        private static void Main()
        {
            // 初始化服务器IP
            var localAdd = IPAddress.Parse("127.0.0.1");
            // 创建TCP侦听器
            var listener = new TcpListener(localAdd, PortNo);
            listener.Start();
            // 显示服务器启动信息
            Console.WriteLine("Server is starting...");
            // 循环接受客户端的连接请求
            while (true)
            {
                var user = new ChatClient(listener.AcceptTcpClient());
                // 显示连接客户端的IP与端口
                Console.WriteLine(user.ClientIp + " is joined");
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}