using MyServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Fight
{
    public class FightCache
    {
        /// <summary>
        /// 玩家id和房间id映射
        /// </summary>
        public Dictionary<int, int> userIdRoomIdDic = new Dictionary<int, int>();
        /// <summary>
        /// 房间Id与房间的映射
        /// </summary>
        public Dictionary<int, FightRoom> roomIdModelDic = new Dictionary<int, FightRoom>();
        /// <summary>
        /// 战斗房间队列
        /// </summary>
        public Queue<FightRoom> roomQueue = new Queue<FightRoom>();
        public ThreadSafeInt roomId = new ThreadSafeInt(-1);
        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="clientList"></param>
        /// <returns></returns>
        public FightRoom CreateRoom(List<ClientPeer> clientList)
        {
            FightRoom room = null;
            if(roomQueue.Count > 0)
            {
                room = roomQueue.Dequeue();
                room.Init(clientList);
            }
            else
            {
                room = new FightRoom(roomId.Add_Get(), clientList);
            }
            foreach(var client in clientList)
            {
                userIdRoomIdDic.Add(client.Id, room.RoomId);
            }
            roomIdModelDic.Add(room.RoomId, room);
            return room;
        }
        /// <summary>
        /// 销毁房间
        /// </summary>
        /// <param name="room"></param>
        public void DestroyRoom(FightRoom room)
        {
            roomIdModelDic.Remove(room.RoomId);
            foreach(var player in room.PlayerList)
            {
                userIdRoomIdDic.Remove(player.UserId);
            }
            //初始化房间数据
            room.Destroy();
            roomQueue.Enqueue(room);
        }
        /// <summary>
        /// 获取玩家是否正在战斗
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsFighting(int userId)
        {
            return userIdRoomIdDic.ContainsKey(userId);
        }
        public FightRoom GetFightRoomByUserId(int userId)
        {
            int roomId = userIdRoomIdDic[userId];
            return roomIdModelDic[roomId];
        }
    }
}
