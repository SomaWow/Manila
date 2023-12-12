using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetection : MonoBehaviour
{
    public bool IsOpen;
    public bool IsPort;
    public bool PirateAvailable;
    public int PosIndex;
    public List<int> PosShip;

    private void Awake()
    {
        IsOpen = false;
        PirateAvailable = false;
        IsPort = false;

        PosIndex = int.Parse(this.gameObject.name);
        PosShip = new List<int>();
        PosShip.Add(PosIndex);

        EventCenter.AddListener(EventType.ClickReset, Init);
        EventCenter.AddListener<List<int>>(EventType.PirateOnBoardOpen, PirateOnBoardOpen);
        EventCenter.AddListener(EventType.PirateOnBoardClose, PirateOnBoardClose);
        EventCenter.AddListener(EventType.OpenDetection, OpenDetection);
        EventCenter.AddListener(EventType.CloseDetection, CloseDetection);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ClickReset, Init);
        EventCenter.RemoveListener<List<int>>(EventType.PirateOnBoardOpen, PirateOnBoardOpen);
        EventCenter.RemoveListener(EventType.PirateOnBoardClose, PirateOnBoardClose);
        EventCenter.RemoveListener(EventType.OpenDetection, OpenDetection);
        EventCenter.RemoveListener(EventType.CloseDetection, CloseDetection);
    }
    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        IsOpen = false;
        PirateAvailable = false;
        IsPort = false;

        PosShip.Clear();
        PosShip.Add(PosIndex);
    }
    /// <summary>
    /// 检测鼠标点击
    /// </summary>
    private void OnMouseDown()
    {


        if (IsOpen && !IsPort)
        {
            //如果钱不够提示一下
            int price = Models.GameModel.siteLibrary.GetPosPrice(PosIndex);
            int ownMoney = Models.GameModel.fightDto.GetPlayerDto(Models.GameModel.userDto.UserId).Money;
            if (ownMoney < price)
            {
                EventCenter.Broadcast(EventType.Hint, "金钱不足，请换个位置放置");
                return;
            }
            if (PosIndex >= 11 && PosIndex <= 23)
            {
                //已上船，parent的parent就是船，获取tag
                PosShip.Add(int.Parse(transform.parent.parent.tag));
            }
            else
            {
                PosShip.Add(ShipCode.NonShip);
            }

            //检测到点击的时候广播个啥，EventCenter.Broadcast(EventType.);
            EventCenter.Broadcast(EventType.CloseDetection);

            //发送位置的编号
            NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.ClickPos_CREQ, PosShip);


        }
        if(PirateAvailable)
        {
            PosShip.Add(int.Parse(transform.parent.parent.tag));

            NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.PirateOnBoard_CREQ, PosShip);
            EventCenter.Broadcast(EventType.PirateOnBoardClose);

        }
    }
    /// <summary>
    /// 全部开放检测
    /// </summary>
    public void OpenDetection()
    {
        IsOpen = true;
        //若有到港的船有这艘船的名字
        int ship = -1;
        if (int.TryParse(transform.parent.parent.tag, out ship))
        {
            if (Models.GameModel.fightDto.ShipDto.PortShipList.Contains(ship))
            {
                IsPort = true;
            }
        }
        
    }
    /// <summary>
    /// 全部关闭检测
    /// </summary>
    public void CloseDetection()
    {
        IsOpen = false;
    }
    /// <summary>
    /// 开放海盗可上船的位点
    /// </summary>
    private void PirateOnBoardOpen(List<int> availableSite)
    {
        //开放13位置上的点即可
        if (availableSite.Contains(PosIndex)) PirateAvailable = true;
    }
    /// <summary>
    /// 关掉海盗登船点
    /// </summary>
    private void PirateOnBoardClose()
    {
        PirateAvailable = false;
    }
}
