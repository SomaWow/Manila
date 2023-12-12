using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyServer
{
    public class ClientPeer
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public Socket clientSocket { get; set; }
        private NetMsg msg;

        public ClientPeer() {
            msg = new NetMsg();
            ReceiveArgs = new SocketAsyncEventArgs();
            ReceiveArgs.UserToken = this;
            ReceiveArgs.SetBuffer(new byte[2048], 0, 2048);
        }
        #region 接收数据
        public SocketAsyncEventArgs ReceiveArgs { get; set; }
        /// <summary>
        /// 接收到消息后，存放到数据缓存区
        /// </summary>
        private List<byte> cache = new List<byte>();
        /// <summary>
        ///是否正在处理接收的数据
        /// </summary>
        private bool isProcessingReceive = false;
        /// <summary>
        /// 消息处理完成后的委托
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        public delegate void ReceiveCompleted(ClientPeer client, NetMsg msg);
        public ReceiveCompleted receiveCompleted;
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="packet"></param>
        public void ProcessReceive(byte[] packet)
        {
            cache.AddRange(packet);
            if(isProcessingReceive == false)
                ProcessData();
        }
        /// <summary>
        /// 处理接收数据
        /// </summary>
        /// <param name="cache"></param>
        private void ProcessData()
        {
            isProcessingReceive = true;
            //解析包，从缓存区取出一个完整的包
            byte[] packet = EncodeTool.DecodePacket(ref cache);
            if (packet == null)
            {
                isProcessingReceive = false;
                return;
            }
            NetMsg msg = EncodeTool.DecodeMsg(packet);
            if(receiveCompleted != null) //不等于空即有人注册
            {
                receiveCompleted(this, msg);
            }
            ProcessData();
        }
        #endregion

        #region 发送消息
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作码</param>
        /// <param name="value">参数</param>
        public void SendMsg(int opCode, int subCode, object value)
        {
            msg.Change(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);
            SendMsg(packet);
        }
        public void SendMsg(byte[] packet)
        {
            try
            {
                clientSocket.Send(packet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region 断开连接
        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            cache.Clear();
            isProcessingReceive = false;
            clientSocket.Shutdown(SocketShutdown.Both); //同时停止发送和接收
            clientSocket.Close();
            clientSocket = null;
        }
        #endregion

    }
}
