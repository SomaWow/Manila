using GameServer.Database;
using MyServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache
{
    /// <summary>
    /// 匹配缓存层
    /// </summary>
    public class MatchCache
    {
        /// <summary>
        /// 正在匹配的用户ID与房间ID的映射字典
        /// </summary>
        public Dictionary<int, int> userIdRoomIdDic = new Dictionary<int, int>();
        /// <summary>
        /// 正在匹配的房间ID与之对应的房间数据模型之间的映射字典
        /// </summary>
        public Dictionary<int, MatchRoom> roomIdModelDic = new Dictionary<int, MatchRoom>();
        public Queue<MatchRoom> roomQueue = new Queue<MatchRoom>();
        /// <summary>
        /// 线程安全的整数
        /// </summary>
        private ThreadSafeInt roomId = new ThreadSafeInt(-1);
        /// <summary>
        /// 进入匹配房间的方法
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public MatchRoom Enter(ClientPeer client)
        {
            //遍历正在匹配的房间数据模型字典中有没有未满的房间
            foreach(var mr in roomIdModelDic.Values)
            {
                if (mr.IsFull())
                    continue;
                mr.Enter(client);
                userIdRoomIdDic.Add(client.Id, mr.roomId);
                return mr;
            }
            //如果执行到这里，说明没有空位置，再新开一个房间
            MatchRoom room = null;
            if (roomQueue.Count > 0)
                room = roomQueue.Dequeue();
            else
                room = new MatchRoom(roomId.Add_Get());
            room.Enter(client);
            roomIdModelDic.Add(room.roomId, room);
            userIdRoomIdDic.Add(client.Id, room.roomId);
            return room;
        }
        /// <summary>
        /// 离开匹配房间
        /// </summary>
        /// <param name="userId"></param>
        public MatchRoom Leave(int userId)
        {
            int roomId = userIdRoomIdDic[userId];
            MatchRoom room = roomIdModelDic[roomId];
            room.Leave(DatabaseManager.GetClientPeerByUserId(userId));
            userIdRoomIdDic.Remove(userId); //把userId和room对删掉
            //如果房间为空了，将房间放到房间重用队列，从正在匹配的房间队列中移除
            if (room.IsEmpty())
            {
                roomIdModelDic.Remove(roomId);
                roomQueue.Enqueue(room);
            }
            return room;
        }
        /// <summary>
        /// 判断是否在匹配房间里面
        /// </summary>
        /// <returns></returns>
        public bool IsMatching(int userId)
        {
            return userIdRoomIdDic.ContainsKey(userId);
        }
        /// <summary>
        /// 获取玩家所在的房间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MatchRoom GetRoom(int userId)
        {
            int roomId = userIdRoomIdDic[userId];
            return roomIdModelDic[roomId];
        }
        /// <summary>
        /// 销毁房间，游戏开始时调用
        /// </summary>
        /// <param name="room"></param>
        public void DestroyRoom(MatchRoom room)
        {
            roomIdModelDic.Remove(room.roomId);
            foreach(var item in room.clientList)
            {
                userIdRoomIdDic.Remove(item.Id);
            }
            room.clientList.Clear();
            room.readyUIdList.Clear();
            roomQueue.Enqueue(room);
        }

    }
}
