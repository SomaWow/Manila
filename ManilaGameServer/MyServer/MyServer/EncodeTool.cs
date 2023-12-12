using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyServer
{
    public class EncodeTool
    {
        /// <summary>
        /// 构造包，包头+包尾
        /// </summary>
        /// <returns></returns>
        public static byte[] EncodePacket(byte[] data)
        {

            using (MemoryStream ms = new MemoryStream()) //不用手动释放内存流对象,自动释放
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(data.Length);
                    bw.Write(data);
                    byte[] packet = new byte[ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(), 0, packet, 0, (int)ms.Length);
                    return packet;

                }
            }
        }
        /// <summary>
        /// 解析包，从缓冲区里取出一个完整的包
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static byte[] DecodePacket(ref List<byte> cache)
        {
            if(cache.Count < 4) //代表缓冲区中的数据都不能构成一个整型
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream(cache.ToArray())) //把cache转换为数组
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int length = br.ReadInt32();
                    int remainLength = (int)(ms.Length - ms.Position);
                    if(length > remainLength) //缓存区里的内容不能构成一个包
                    {
                        return null;
                    }
                    byte[] data = br.ReadBytes(length);
                    //更新数据缓存
                    cache.Clear();
                    cache.AddRange(br.ReadBytes(remainLength));
                    return data;
                }
            }
        }
        /// <summary>
        /// 把NetMsg类转换成字节数组，发送出去
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] EncodeMsg(NetMsg msg)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(msg.opCode);
                    bw.Write(msg.subCode);
                    if(msg.value != null)
                    {
                        bw.Write(EncodeObj(msg.value));
                    }
                    byte[] data = new byte[ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, (int)ms.Length);
                    return data;
                }
            }
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static byte[] EncodeObj(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                byte[] data = new byte[ms.Length];
                Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, (int)ms.Length);
                return data;
            }
        }
        /// <summary>
        /// 将字节数组转换成 NetMsg 网络消息类
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static NetMsg DecodeMsg(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    NetMsg msg = new NetMsg();
                    msg.opCode = br.ReadInt32();
                    msg.subCode = br.ReadInt32();
                    if(ms.Length - ms.Position > 0)
                    {
                        object obj =DecodeObj(br.ReadBytes((int)(ms.Length - ms.Position)));
                        msg.value = obj;
                    }
                    return msg;
                }
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        private static object DecodeObj(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }


    }
}
