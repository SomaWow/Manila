using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Cache;
using GameServer.Database;
using MyServer;
using Protocol.Code;
using Protocol.Code.DTO;

namespace GameServer.Logic
{
    public delegate void StartFight(List<ClientPeer> clientList, int roomType);
    public class MatchHandler : IHandler
    {
        //独一份的matchCache
        private MatchCache matchCache = Caches.matchCache;
        public StartFight startFight;
        public void Disconnect(ClientPeer client)
        {
            LeaveRoom(client, 0);
        }

        public void Receive(ClientPeer client, int subCode, object value)
        {
            switch(subCode)
            {
                case MatchCode.Enter_CREQ:
                    EnterRoom(client, (int)value);
                    break;
                case MatchCode.Leave_CREQ:
                    LeaveRoom(client, (int)value);
                    break;
                case MatchCode.Ready_CREQ:
                    Ready(client,(int)value);
                    break;
                case MatchCode.UnReady_CREQ:
                    UnReady(client,(int)value);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 客户端发来的准备请求
        /// </summary>
        /// <param name="client"></param>
        /// <param name="roomtype"></param>
        private void Ready(ClientPeer client, int roomType)
        {
            SingleExecute.Instance.Execute(()=> {
                if (matchCache.IsMatching(client.Id) == false) return;

                MatchRoom room = matchCache.GetRoom(client.Id);
                room.Ready(client.Id);
                room.Broadcast(OpCode.Match, MatchCode.Ready_BRO, client.Id);
                //所有玩家都准备就可以开始游戏了
                if (room.IsAllReady())
                {
                    startFight(room.clientList, roomType);

                    //通知房间中的所有玩家，开始游戏了
                    room.Broadcast(OpCode.Match, MatchCode.StartGame_BRO, null);
                    //销毁房间
                    matchCache.DestroyRoom(room);
                }
            });

        }
        /// <summary>
        /// 客户端传来的取消准备
        /// </summary>
        /// <param name="client"></param>
        /// <param name="roomType"></param>
        private void UnReady(ClientPeer client, int roomType)
        {
            SingleExecute.Instance.Execute(() => {
                if (matchCache.IsMatching(client.Id) == false) return;

                MatchRoom room = matchCache.GetRoom(client.Id);
                room.UnReady(client.Id);
                room.Broadcast(OpCode.Match, MatchCode.UnReady_BRO, client.Id);
            });
        }
        private void EnterRoom(ClientPeer client, int roomType) //此处的roomtype没有用到
        {
            SingleExecute.Instance.Execute(() =>
            {
                //判断一下当前客户端连接对象是不是在匹配房间里面，如果在，则忽略
                if (matchCache.IsMatching(client.Id)) return;
                MatchRoom room = matchCache.Enter(client);
                //构造UserDto用户数据传输模型
                UserDto userDto = DatabaseManager.CreateUserDto(client.Id);
                //广播给房间内所有玩家，除了自身，有新的玩家进来了，参数：新进来的用户的UserDto，广播给所有玩家除了自己
                room.Broadcast(OpCode.Match, MatchCode.Enter_BRO, userDto, client);

                //给客户端一个响应，参数：房间传输模型，包含房间内正在等待的玩家已经准备的玩家id集合
                client.SendMsg(OpCode.Match, MatchCode.Enter_SRES, MakeMatchRoomDto(room));
            });
        }
        /// <summary>
        /// 制作传输信息载体
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        private MatchRoomDto MakeMatchRoomDto(MatchRoom room)
        {
            MatchRoomDto dto = new MatchRoomDto();
            for(int i=0; i < room.clientList.Count; i++)
            {
                dto.Enter(DatabaseManager.CreateUserDto(room.clientList[i].Id));
            }
            dto.readyUserIdList = room.readyUIdList;
            return dto;
        }
        private void LeaveRoom(ClientPeer client, int roomType)
        {
            SingleExecute.Instance.Execute(()=> {
                //不在匹配房间，忽略掉
                if (matchCache.IsMatching(client.Id) == false) return;

                MatchRoom room = matchCache.Leave(client.Id);
                room.Broadcast(OpCode.Match, MatchCode.Leave_BRO, client.Id);
            });
        }
    }
}
