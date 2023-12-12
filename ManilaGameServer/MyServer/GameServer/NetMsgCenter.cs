using GameServer.Logic;
using MyServer;
using Protocol.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class NetMsgCenter : IApplication
    {
        private AccountHandler accountHandler = new AccountHandler();
        private ChatHandler chatHandler = new ChatHandler();
        private MatchHandler matchHandler = new MatchHandler();
        private FightHandler fightHandler = new FightHandler();

        public NetMsgCenter()
        {
            //把这个方法注册进去，当startFight被调用，就会调用被注册的方法
            matchHandler.startFight += fightHandler.StartFight;
        }

        //倒着断开
        public void Disconnect(ClientPeer client)
        {
            fightHandler.Disconnect(client);
            matchHandler.Disconnect(client);
            chatHandler.Disconnect(client);
            accountHandler.Disconnect(client);
        }

        public void Receive(ClientPeer client, NetMsg msg)
        {
            switch(msg.opCode)
            {
                case OpCode.Account:
                    accountHandler.Receive(client, msg.subCode, msg.value);
                    break;
                case OpCode.Chat:
                    chatHandler.Receive(client, msg.subCode, msg.value);
                    break;
                case OpCode.Match:
                    matchHandler.Receive(client, msg.subCode, msg.value);
                    break;
                case OpCode.Fight:
                    fightHandler.Receive(client, msg.subCode, msg.value);
                    break;
                default:
                    break;
            }
        }
    }
}
