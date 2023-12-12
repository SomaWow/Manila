using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMsgCenter : MonoBehaviour
{
    public static NetMsgCenter Instance;

    private ClientPeer clientPeer;
    private void Awake()
    {
        Instance = this;
        clientPeer = new ClientPeer();

        DontDestroyOnLoad(gameObject);
        clientPeer.Connect("47.92.2.97", 6666);
    }
    private void FixedUpdate()
    {
        if (clientPeer == null) return;
        //如果消息队列不为空，一直执行处理的方法
        while(clientPeer.netMsgQueue.Count>0)
        {
            NetMsg msg = clientPeer.netMsgQueue.Dequeue();
            ProcessServerSendMsg(msg);
        }
    }

    #region 处理服务器发来的方法
    AccountHandler accountHandler = new AccountHandler();
    ChatHandler chatHandler = new ChatHandler();
    MatchHandler matchHandler = new MatchHandler();
    FightHandler fightHandler = new FightHandler();

    /// <summary>
    /// 处理服务器发来的消息
    /// </summary>
    /// <param name="msg"></param>
    private void ProcessServerSendMsg(NetMsg msg)
    {
        switch (msg.opCode)
        {
            case OpCode.Account:
                accountHandler.OnReceive(msg.subCode, msg.value);
                break;
            case OpCode.Chat:
                break;
            case OpCode.Match:
                matchHandler.OnReceive(msg.subCode, msg.value);
                break;
            case OpCode.Fight:
                fightHandler.OnReceive(msg.subCode, msg.value);
                break;
            default:
                break;
        }

    }
    #endregion

    #region 发送消息
    public void SendMsg(int opCode, int subCode, object value)
    {
        clientPeer.SendMsg(opCode, subCode, value);
    }
    public void SendMsg(NetMsg msg)
    {
        clientPeer.SendMsg(msg);
    }
    #endregion




}
