using System;
using System.Net.Sockets;

namespace SoketDemo
{
    internal class Program
    {
        // 设置连接端口
        private const int portNo = 500;

        private static void Main(string[] args)
        {
            // 初始化服务器IP
            System.Net.IPAddress localAdd = System.Net.IPAddress.Parse("192.168.16.150");
            //System.Net.IPAddress localAdd = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())[0];
            // 创建TCP侦听器
            TcpListener listener = new TcpListener(localAdd, portNo);
            listener.Start();
            // 显示服务器启动信息
            Console.WriteLine("Server is starting...");
            // 循环接受客户端的连接请求
            while (true)
            {
                ChatClient user = new ChatClient(listener.AcceptTcpClient());
                // 显示连接客户端的IP与端口
                Console.WriteLine(user._clientIP + " is joined");
            }
        }
    }
}