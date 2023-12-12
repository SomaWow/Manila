using DG.Tweening;
using Protocol.Code;
using Protocol.Code.DTO.Fight;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {
    /// <summary>
    /// 这个船的代号
    /// </summary>
    private int ShipIndex;

    /// <summary>
    /// 货物相关
    /// </summary>
    //固定值，所有货物
    private Dictionary<int, Transform> wareDic;
    //要上这艘船的货物
    private int goodCode;
    //上船的位置
    private Transform BoardPos;
    //货物的初始父节点
    private Transform GoodInitParent;
    //货物的初始位置
    private Transform GoodInitPos;

    /// <summary>
    /// 船移动相关
    /// </summary>
    //固定值，三艘船的初始位置
    private List<Transform> ShipInitPosList;
    //固定值 0-13的点
    private List<float> Site0_13List = new List<float>() { -40, -37.5f, -35, -32.5f, -30, -27.5f, -25, -22.5f, -20, -17.5f, -15, -12.5f, -10, -7.5f };
    //固定值，港口和修船场的位置
    private List<Transform> PortPosList;
    private List<Transform> FixPosList;
    
    /// <summary>
    /// 组件
    /// </summary>
    //显示船的位置
    private Transform spr_ShipPos;

    private void Awake()
    {
        EventCenter.AddListener(EventType.MoveShip, RefreshShipPosition);
        EventCenter.AddListener<List<int>>(EventType.OnBoard, OnBoard);
        EventCenter.AddListener(EventType.OffBoard, OffBoard);

        ShipIndex = int.Parse(this.gameObject.tag);

        goodCode = -1;
        BoardPos = transform.Find("BoardPos").GetComponent<Transform>();
        GoodInitParent = transform.Find("/Ware").GetComponent<Transform>();
        GoodInitPos = transform.Find("/Ware/offBoardPos").GetComponent<Transform>();
        //货物卡片们
        wareDic = new Dictionary<int, Transform>();
        wareDic.Add(GoodCode.Doukou, transform.Find("/Ware/doukou").GetComponent<Transform>());
        wareDic.Add(GoodCode.Silk, transform.Find("/Ware/silk").GetComponent<Transform>());
        wareDic.Add(GoodCode.Renshen, transform.Find("/Ware/renshen").GetComponent<Transform>());
        wareDic.Add(GoodCode.Yushi, transform.Find("/Ware/yushi").GetComponent<Transform>());

        //船的位置
        //船的初始位置
        ShipInitPosList = new List<Transform>();
        ShipInitPosList.Add(transform.Find("/ShipSites/initPos_A"));
        ShipInitPosList.Add(transform.Find("/ShipSites/initPos_B"));
        ShipInitPosList.Add(transform.Find("/ShipSites/initPos_C"));
        //入港位置
        PortPosList = new List<Transform>();
        PortPosList.Add(transform.Find("/ShipSites/port_A"));
        PortPosList.Add(transform.Find("/ShipSites/port_B"));
        PortPosList.Add(transform.Find("/ShipSites/port_C"));
        //修理厂位置
        FixPosList = new List<Transform>();
        FixPosList.Add(transform.Find("/ShipSites/fix_A"));
        FixPosList.Add(transform.Find("/ShipSites/fix_B"));
        FixPosList.Add(transform.Find("/ShipSites/fix_C"));

        spr_ShipPos = transform.Find("Pos");
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.MoveShip, RefreshShipPosition);
        EventCenter.RemoveListener<List<int>>(EventType.OnBoard, OnBoard);
        EventCenter.AddListener(EventType.OffBoard, OffBoard);
    }
    /// <summary>
    /// 目前还没调用，先放着 TODO
    /// </summary>
    private void Init() {

    }
    /// <summary>
    /// 货物上船
    /// </summary>
    /// <param name="onBoard"></param>
    private void OnBoard(List<int> onBoard)
    {
        goodCode = onBoard[ShipIndex];
        wareDic[goodCode].parent = this.transform;
        wareDic[goodCode].localPosition = BoardPos.localPosition;
        wareDic[goodCode].localRotation = BoardPos.localRotation;
        wareDic[goodCode].localScale = BoardPos.localScale;
    }
    /// <summary>
    /// 船上清空
    /// </summary>
    private void OffBoard()
    {
        wareDic[goodCode].parent = GoodInitParent;
        wareDic[goodCode].localPosition = GoodInitPos.localPosition;
        wareDic[goodCode].localRotation = GoodInitPos.localRotation;
        wareDic[goodCode].localScale = GoodInitPos.localScale;
    }
    /// <summary>
    /// 刷新船的位置
    /// </summary>
    private void RefreshShipPosition()
    {
        ShipDto dto = Models.GameModel.fightDto.ShipDto;
        int pos = dto.ShipSiteList[ShipIndex];

        if (pos == 0)
        {
            //等于0的时候可能是回到初始点了，把数字显示出来
            spr_ShipPos.gameObject.SetActive(true);
            spr_ShipPos.GetComponent<SpriteRenderer>().sprite = ResourcesManager.GetNumSprite(pos);
            //回到初始位点
            transform.localPosition = ShipInitPosList[ShipIndex].localPosition;
            transform.localRotation = ShipInitPosList[ShipIndex].localRotation;
        }
        else if(pos <= 13)
        {
            //修改数字
            spr_ShipPos.GetComponent<SpriteRenderer>().sprite = ResourcesManager.GetNumSprite(pos);
            //移船
            Vector3 v = new Vector3(ShipInitPosList[ShipIndex].localPosition.x, 8, Site0_13List[pos]);
            transform.DOMove(v, 1.5f);
            /*
            var v = this.transform.localPosition;
            v.z = v.z + (2.5f * (pos - ShipPos));
            this.transform.localPosition = v;
            */
        }
        else if(pos == ShipSiteCode.Port)
        {
            //入港
            for(int i = 0; i < dto.PortShipList.Count; i++)
            {
                if (dto.PortShipList[i] == ShipIndex)
                {
                    this.transform.localPosition = PortPosList[i].localPosition;
                    this.transform.localRotation = PortPosList[i].localRotation;
                }
            }
            //设置数字不可见
            spr_ShipPos.gameObject.SetActive(false);
        }
        else if(pos == ShipSiteCode.Fix)
        {
            //去修理
            for (int i = 0; i < dto.FixShipList.Count; i++)
            {
                if (dto.FixShipList[i] == ShipIndex)
                {
                    this.transform.localPosition = FixPosList[i].localPosition;
                    this.transform.localRotation = FixPosList[i].localRotation;
                }
            }
            //设置数字不可见
            spr_ShipPos.gameObject.SetActive(false);
        }
    }
}
