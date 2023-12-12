using DG.Tweening;
using Protocol.Code;
using Protocol.Code.DTO;
using Protocol.Code.DTO.Fight;
using Protocol.Constant;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private int PlayerIndex;
    private int UserId;
    //临时使用的变量
    private int BiddingPrice;
    //颜色们
    private List<int> ColorList;
    private Color color;
    private string colorStr;
    

    /// <summary>
    ///组件们
    /// </summary>
    private Image img_HeadIcon;
    private Text txt_UserName;
    private Text txt_Money;
    private Text txt_OwnCardNum;
    private Text txt_MortgageCardNum;
    private GameObject img_CountDown;
    private List<Image> img_WorkerList;
    private GameObject img_HarbourMaster;
    private Text txt_Hint;
    private Image img_Coin;
    private Image img_Card;

    //竞价选船老大
    private Button btn_Pass;
    private InputField input_Bidding;
    private Button btn_Bdding;
    /// <summary>
    /// 3D部分
    /// </summary>
    //

    private List<Transform> cap_WorkerList;
    //所有的位置 最后一个是起始位置
    private List<Transform> allSitesList;

    private void Awake()
    {
        EventCenter.AddListener(EventType.NewMoveRound, NewMoveRound);
        EventCenter.AddListener(EventType.RefreshCard, RefreshCard);
        //开放检测，只是当前玩家操作的指示灯打开
        EventCenter.AddListener<int>(EventType.CurrentPlayer, IndicateCurrentPlayer);
        EventCenter.AddListener(EventType.RefreshWorker, RefreshWorker);
        EventCenter.AddListener(EventType.RefreshMoney, RefreshMoney);
        //设置船老大
        EventCenter.AddListener(EventType.SetHarbourMaster, SetHarbourMaster);
        //更新竞价结果提示
        EventCenter.AddListener(EventType.RefreshBiddingResult, RefreshBiddingResult);
        //开始游戏，随机玩家开始竞价，<第一个开始玩家的位次，第一个开始玩家的UserId>
        EventCenter.AddListener<int>(EventType.Auction, Auction);
        EventCenter.AddListener(EventType.RefreshUI, RefreshUI);

        ColorList = new List<int> { ColorCode.Red, ColorCode.Yello, ColorCode.Blue, ColorCode.Black};
        PlayerIndex = int.Parse(gameObject.tag);
        switch (ColorList[PlayerIndex])
        {
            case ColorCode.Red:
                this.color = Color.red;
                colorStr = "red";
                break;
            case ColorCode.Yello:
                this.color = Color.yellow;
                colorStr = "yellow";
                break;
            case ColorCode.Blue:
                this.color = Color.blue;
                colorStr = "blue";
                break;
            case ColorCode.Black:
                this.color = Color.black;
                colorStr = "black";
                break;
        }

        Init();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.NewMoveRound, NewMoveRound);
        EventCenter.RemoveListener(EventType.RefreshCard, RefreshCard);
        EventCenter.RemoveListener<int>(EventType.CurrentPlayer, IndicateCurrentPlayer);
        EventCenter.RemoveListener(EventType.RefreshWorker, RefreshWorker);
        EventCenter.RemoveListener(EventType.RefreshMoney, RefreshMoney);
        EventCenter.RemoveListener(EventType.SetHarbourMaster, SetHarbourMaster);
        EventCenter.RemoveListener(EventType.RefreshBiddingResult, RefreshBiddingResult);
        EventCenter.RemoveListener<int>(EventType.Auction, Auction);
        EventCenter.RemoveListener(EventType.RefreshUI, RefreshUI);
    }
    /// <summary>
    /// Init
    /// </summary>
    private void Init()
    {
        img_HeadIcon = transform.Find("headIcon").GetComponent<Image>();
        txt_UserName = transform.Find("txt_userName").GetComponent<Text>();
        txt_Money = transform.Find("txt_money").GetComponent<Text>();
        img_Coin = transform.Find("coin").GetComponent<Image>();
        txt_OwnCardNum = transform.Find("Card/own").GetComponent<Text>();
        txt_MortgageCardNum = transform.Find("Card/mortgage").GetComponent<Text>();
        img_Card = transform.Find("Card").GetComponent<Image>();
        //图片标志们 
        img_WorkerList = new List<Image>();
        img_WorkerList.Add(transform.Find("worker_1").GetComponent<Image>());
        img_WorkerList.Add(transform.Find("worker_2").GetComponent<Image>());
        img_WorkerList.Add(transform.Find("worker_3").GetComponent<Image>());
        foreach(var sign in img_WorkerList)
        {
            sign.color = this.color;
        }
        //3d胶囊工人们
        cap_WorkerList = new List<Transform>();
        cap_WorkerList.Add(transform.Find("/Workers/"+colorStr+"_0"));
        cap_WorkerList.Add(transform.Find("/Workers/"+colorStr+"_1"));
        cap_WorkerList.Add(transform.Find("/Workers/"+colorStr+"_2"));
        //所有的位置
        allSitesList = new List<Transform>();
        allSitesList.Add(transform.Find("/WorkerSites/0"));
        allSitesList.Add(transform.Find("/WorkerSites/1"));
        allSitesList.Add(transform.Find("/WorkerSites/2"));
        allSitesList.Add(transform.Find("/WorkerSites/3"));
        allSitesList.Add(transform.Find("/WorkerSites/4"));
        allSitesList.Add(transform.Find("/WorkerSites/5"));
        allSitesList.Add(transform.Find("/WorkerSites/6"));
        allSitesList.Add(transform.Find("/WorkerSites/7"));
        allSitesList.Add(transform.Find("/WorkerSites/8"));
        allSitesList.Add(transform.Find("/WorkerSites/9"));
        allSitesList.Add(transform.Find("/WorkerSites/10"));
        allSitesList.Add(transform.Find("/Ware/doukou/11/site"));
        allSitesList.Add(transform.Find("/Ware/doukou/12/site"));
        allSitesList.Add(transform.Find("/Ware/doukou/13/site"));
        allSitesList.Add(transform.Find("/Ware/silk/14/site"));
        allSitesList.Add(transform.Find("/Ware/silk/15/site"));
        allSitesList.Add(transform.Find("/Ware/silk/16/site"));
        allSitesList.Add(transform.Find("/Ware/renshen/17/site"));
        allSitesList.Add(transform.Find("/Ware/renshen/18/site"));
        allSitesList.Add(transform.Find("/Ware/renshen/19/site"));
        allSitesList.Add(transform.Find("/Ware/yushi/20/site"));
        allSitesList.Add(transform.Find("/Ware/yushi/21/site"));
        allSitesList.Add(transform.Find("/Ware/yushi/22/site"));
        allSitesList.Add(transform.Find("/Ware/yushi/23/site"));
        allSitesList.Add(transform.Find("/Workers/" + colorStr + "_InitPos"));

        img_CountDown = transform.Find("countdown").gameObject;
        img_HarbourMaster = transform.Find("harbourMaster").gameObject;
        txt_Hint = transform.Find("txt_Hint").GetComponent<Text>();

        //竞价部分
        btn_Pass = transform.Find("Bidding/btn_Pass").GetComponent<Button>();
        //监听Pass按钮
        btn_Pass.onClick.AddListener(() =>
        {
            if (Models.GameModel.bidDto.bidPassList.Count == 3)
            {
                EventCenter.Broadcast(EventType.Hint, "大家都Pass了，只能你出价啦");
            }
            else
            {
                NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.Bidding_CREQ, -1);
                btn_Pass.gameObject.SetActive(false);
                input_Bidding.gameObject.SetActive(false);
                btn_Bdding.gameObject.SetActive(false);
            }

        });
        input_Bidding = transform.Find("Bidding/input_Bidding").GetComponent<InputField>();
        btn_Bdding = transform.Find("Bidding/btn_Bidding").GetComponent<Button>();
        //监听竞价按钮
        btn_Bdding.onClick.AddListener(() =>
        {

            if (input_Bidding.text == null || input_Bidding.text == "")
                EventCenter.Broadcast(EventType.Hint, "请输入竞价");
            else if (int.Parse(input_Bidding.text) > Models.GameModel.fightDto.playerList[PlayerIndex].Money)
            {
                EventCenter.Broadcast(EventType.Hint, "不能超过你拥有的钱数");
            }
            else if(int.Parse(input_Bidding.text) <= 0)
            {
                EventCenter.Broadcast(EventType.Hint, "出价至少要大于0");
            }
            else if (int.Parse(input_Bidding.text) <= Models.GameModel.bidDto.HighestBid)
            {
                EventCenter.Broadcast(EventType.Hint, "要比最高竞价高或者可以选择Pass");
            }
            else
            {
                BiddingPrice = int.Parse(input_Bidding.text);
                Debug.Log("发送竞价结果" + BiddingPrice);
                NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.Bidding_CREQ, BiddingPrice); //发送目前最高报价

                btn_Pass.gameObject.SetActive(false);
                input_Bidding.gameObject.SetActive(false);
                btn_Bdding.gameObject.SetActive(false);
            }
        });
        HideObj();
    }
    private void HideObj()
    {
        img_HeadIcon.gameObject.SetActive(false);
        txt_UserName.gameObject.SetActive(false);
        txt_Money.gameObject.SetActive(false);
        img_Coin.gameObject.SetActive(false);
        img_Card.gameObject.SetActive(false);

        foreach (var w in img_WorkerList) w.gameObject.SetActive(false);

        img_CountDown.SetActive(false);
        img_HarbourMaster.SetActive(false);
        txt_Hint.gameObject.SetActive(false);
        //竞价船老大的部分
        btn_Pass.gameObject.SetActive(false);
        input_Bidding.gameObject.SetActive(false);
        btn_Bdding.gameObject.SetActive(false);
    }

    /// <summary>
    /// 当有新玩家进来时，有玩家离开时或自身玩家进来时调用这个方法，刷新一下UI界面
    /// </summary>
    private void RefreshUI()
    {
        MatchRoomDto room = Models.GameModel.matchRoomDto;
        if (PlayerIndex <= room.enterOrderList.Count - 1)
        {
            //每次刷新的时候刷新一下UserId，后面就可以一直用啦
            UserId = Models.GameModel.matchRoomDto.enterOrderList[PlayerIndex];
            //从matchRoom中取出对应位置的玩家信息，并更新UI
            UserDto userDto = room.userIdUserDtoDic[UserId];
            img_HeadIcon.sprite = ResourcesManager.GetSprite(userDto.IconName);
            txt_UserName.text = userDto.UserName;

            img_HeadIcon.gameObject.SetActive(true);
            txt_UserName.gameObject.SetActive(true);
            txt_Money.gameObject.SetActive(true);
            img_Coin.gameObject.SetActive(true);
            img_Card.gameObject.SetActive(true);

            foreach (var w in img_WorkerList) w.gameObject.SetActive(true);
            //如果玩家正在准备中，就把“已准备”打开
            if (room.readyUserIdList.Contains(UserId))
            {
                txt_Hint.text = "已准备";
                txt_Hint.gameObject.SetActive(true);
            }
            else
            {
                txt_Hint.gameObject.SetActive(false);
            }
            //说明这个玩家是我们自己
            if (UserId == Models.GameModel.userDto.UserId)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                transform.GetComponent<Image>().color = new Color(0.99f, 0.65f, 0.65f);
            }
            else
            {
                transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                transform.GetComponent<Image>().color = new Color(0.85f, 0.74f, 0.68f);
            }
        }
        else
        {
            img_HeadIcon.gameObject.SetActive(false);
            txt_UserName.gameObject.SetActive(false);
            txt_Money.gameObject.SetActive(false);
            img_Coin.gameObject.SetActive(false);
            img_Card.gameObject.SetActive(false);

            foreach (var w in img_WorkerList) w.gameObject.SetActive(false);

            txt_Hint.gameObject.SetActive(false);

            transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
            transform.GetComponent<Image>().color = new Color(0.85f, 0.74f, 0.68f);
        }
    }
    /// <summary>
    /// 刷新拥有的牌数
    /// </summary>
    private void RefreshCard()
    {
        PlayerDto dto = Models.GameModel.fightDto.playerList[PlayerIndex];
        txt_OwnCardNum.text = dto.cardNum.ToString();
        txt_MortgageCardNum.text = dto.mortgageNum.ToString();
    }
    /// <summary>
    /// 开始游戏，开始竞价
    /// </summary>
    private void Auction(int userId)
    {

        //以示区分
        if (userId == this.UserId)
        {
            //各个界面的指示打开，指示该玩家正在操作
            img_CountDown.gameObject.SetActive(true);
            if (userId == Models.GameModel.userDto.UserId)
            {
                //当这个玩家是自己的时候，激活竞价按钮,尚不显示竞价结果的txt
                btn_Pass.gameObject.SetActive(true);
                input_Bidding.gameObject.SetActive(true);
                btn_Bdding.gameObject.SetActive(true);
            }
        }
        else
        {
            img_CountDown.gameObject.SetActive(false);
            btn_Pass.gameObject.SetActive(false);
            input_Bidding.gameObject.SetActive(false);
            btn_Bdding.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 更新竞价结果，显示Pass和目前出价最高的银
    /// </summary>
    private void RefreshBiddingResult()
    {
        BidDto dto = Models.GameModel.bidDto;
        txt_Hint.gameObject.SetActive(false);
        //有最高价的显示最高出价
        if (dto.HighestBidId == UserId)
        {
            txt_Hint.text = dto.HighestBid.ToString();
            txt_Hint.gameObject.SetActive(true);
        }
        else
        {
            txt_Hint.gameObject.SetActive(false);
        }
        //如果有Pass的，显示Pass
        foreach (var passPlayer in dto.bidPassList)
        {
            if (passPlayer == UserId)
            {
                txt_Hint.text = "Pass";
                txt_Hint.gameObject.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 设置船老大，扣掉钱，并让他进行上船选择。。。
    /// </summary>
    private void SetHarbourMaster()
    {
        img_CountDown.gameObject.SetActive(false);
        btn_Pass.gameObject.SetActive(false);
        input_Bidding.gameObject.SetActive(false);
        btn_Bdding.gameObject.SetActive(false);
        txt_Hint.gameObject.SetActive(false);

        BidDto dto = Models.GameModel.bidDto;

        if (this.UserId == dto.HarbourMasterId)
        {
            img_HarbourMaster.SetActive(true);
            img_CountDown.SetActive(true);
        }
    }
    /// <summary>
    /// 刷新金钱数量
    /// </summary>
    private void RefreshMoney()
    {
        PlayerDto player = Models.GameModel.fightDto.playerList[PlayerIndex];
        txt_Money.text = player.Money.ToString();
    }
    /// <summary>
    /// 放置工人
    /// </summary>
    private void RefreshWorker()
    {
        WorkerDto wokerDto = Models.GameModel.fightDto.playerList[PlayerIndex].Worker;

        for(int i = 0; i < wokerDto.WorkerSiteList.Count; i++)
        {
            //获得位置
            int pos = wokerDto.WorkerSiteList[i];

            if (pos <= SiteCode.Insurance)
            {
                //先变大再移动
                img_WorkerList[i].gameObject.SetActive(false);
                cap_WorkerList[i].localScale = allSitesList[pos].localScale;
                cap_WorkerList[i].DOLocalMove(allSitesList[pos].localPosition, 0.5f);
                cap_WorkerList[i].localRotation = allSitesList[pos].localRotation;
            }
            else if (pos == SiteCode.InitPos)
            {
                img_WorkerList[i].gameObject.SetActive(true);

                //回到初始位点
                cap_WorkerList[i].parent = allSitesList[pos].parent;
                cap_WorkerList[i].DOLocalMove(allSitesList[pos].localPosition, 0.5f);
                cap_WorkerList[i].localRotation = allSitesList[pos].localRotation;
                cap_WorkerList[i].localScale = allSitesList[pos].localScale;
            }
            else
            {
                //先变大再移动
                img_WorkerList[i].gameObject.SetActive(false);
                //船上的位置，要和船一起移动，设置父节点
                cap_WorkerList[i].parent = allSitesList[pos].parent;
                cap_WorkerList[i].localScale = allSitesList[pos].localScale;
                cap_WorkerList[i].DOLocalMove(allSitesList[pos].localPosition,1f);
                cap_WorkerList[i].localRotation = allSitesList[pos].localRotation;

                /*
                cap_WorkerList[i].localPosition = allSitesList[pos].localPosition;
                cap_WorkerList[i].localRotation = allSitesList[pos].localRotation;
                cap_WorkerList[i].localScale = allSitesList[pos].localScale;
                */
            }
        }
    }

    /// <summary>
    /// 指示当前操作玩家的图标
    /// </summary>
    private void IndicateCurrentPlayer(int userId)
    {
        if(userId == this.UserId)
        {
            img_CountDown.SetActive(true);
        }
        else
        {
            img_CountDown.SetActive(false);
        }
    }

    /// <summary>
    /// 在新的回合把一些东西重置
    /// </summary>
    private void NewMoveRound()
    {
        img_HarbourMaster.SetActive(false);
    }
}
