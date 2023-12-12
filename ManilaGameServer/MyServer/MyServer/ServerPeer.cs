using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyServer
{
    public class ServerPeer
    {
        private Socket serverSocket;
        private Semaphore semaphore; //计量器，线程的运算控制
        private ClientPeerPool clientPeerPool; //对象连接池
        private IApplication application;
        /// <summary>
        /// 设置应用层
        /// </summary>
        /// <param name="application"></param>
        public void SetApplication(IApplication application)
        {
            this.application = application;
        }

        /// <summary>
        /// 开启服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="maxClient"></param>
        public void StartServer(string ip, int port, int maxClient)
        {
            try
            {
                clientPeerPool = new ClientPeerPool(maxClient);
                semaphore = new Semaphore(maxClient, maxClient);
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //填满客户端对象连接池
                for (int i = 0; i < maxClient; i++)
                {
                    ClientPeer temp = new ClientPeer();
                    temp.receiveCompleted = ReceiveProcessCompleted; //接受且解析完数据，处理结果
                    temp.ReceiveArgs.Completed += ReceiveArgs_Completed;
                    clientPeerPool.Enqueue(temp);
                }
                serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port)); //绑定到进程
                serverSocket.Listen(maxClient); //设置最大监听数
                Console.WriteLine("服务器启动了");
                StartAccept(null);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
            
        }


        #region 接收客户端的连接请求
        /// <summary>
        /// 接受客户端的连接
        /// </summary>
        /// <param name="e"></param>
        private void StartAccept(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += E_Completed;
            }
            //如果result为true ，代表正在接收连接，连接成功之后会出发Completed事件
            //如果result为false，代表接收成功
            bool result = serverSocket.AcceptAsync(e);
            if (result == false)
            {
                ProcessAccept(e); //接受成功，直接调用；正在接受，接收完了回调该函数
            }
        }
        private void E_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }
        /// <summary>
        /// 处理连接
        /// </summary>
        /// <param name="e"></param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            semaphore.WaitOne(); //如果满了，直到有释放的才允许继续连接
            ClientPeer client = clientPeerPool.Dequeue();
            client.clientSocket = e.AcceptSocket;
            Console.WriteLine(client.clientSocket.RemoteEndPoint + "客户端连接成功");
            //接受消息TODO
            StartReceive(client);

            e.AcceptSocket = null;
            StartAccept(e); //循环监听客户端连接
        }
        #endregion


        #region 接收数据
        private void StartReceive(ClientPeer client)
        {
            try
            {
                bool result = client.clientSocket.ReceiveAsync(client.ReceiveArgs);
                if (result == false)
                {
                    ProcessReceive(client.ReceiveArgs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }

        /// <summary>
        /// 异步接收数据完成后调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceiveArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessReceive(e);
        }
        /// <summary>
        /// 处理数据的接收
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            ClientPeer client = e.UserToken as ClientPeer;
            if (client.ReceiveArgs.SocketError == SocketError.Success && client.ReceiveArgs.BytesTransferred > 0)
            {
                byte[] packet = new byte[client.ReceiveArgs.BytesTransferred];
                Buffer.BlockCopy(client.ReceiveArgs.Buffer, 0, packet, 0, client.ReceiveArgs.BytesTransferred);
                client.ProcessReceive(packet);
                StartReceive(client);
            }
            //断开连接
            else {
                //没有传输的字节数，就代表断开连接了
                if (client.ReceiveArgs.BytesTransferred == 0)
                {
                    //客户端主动断开连接
                    if (client.ReceiveArgs.SocketError == SocketError.Success)
                    {
                        Disconnect(client, "客户端主动端开连接");
                    }
                    //因为网络异常被动断开连接
                    else
                    {
                        Disconnect(client, client.ReceiveArgs.SocketError.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// 一条消息完成后的回调
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        private void ReceiveProcessCompleted(ClientPeer client, NetMsg msg)
        {
            //交给应用层处理
            application.Receive(client, msg);
        }

        #endregion

        #region 断开连接
        /// <summary>
        /// 客户端断开连接
        /// </summary>
        /// <param name="client"></param>
        /// <param name="reasion"></param>
        private void Disconnect(ClientPeer client, string reason)
        {
            try
            {
                if (client == null)
                {
                    throw new Exception("客户端为空，无法断开连接");
                }
                Console.WriteLine(client.clientSocket.RemoteEndPoint + "客户端断开连接，原因" + reason);
                application.Disconnect(client);
                //让客户端处理断开连接
                client.Disconnect();

                clientPeerPool.Enqueue(client); //断开的对象再放回去
                semaphore.Release();//释放一下
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

    }
}
