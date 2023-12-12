using Protocol.Code;
using Protocol.Code.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SharePart : MonoBehaviour {

    private Button btn_Diya;
    private Button btn_Shuhui;
    private Button btn_Buy;

    private GameObject sharePanel;
    private List<Button> btn_CardList;
    private List<Text> txt_NumList;
    private Button btn_Decide;
    private Button btn_Reset;
    private Button btn_Close;
    private Text txt_Hint;
    /// <summary>
    /// 1-购买
    /// 2-抵押
    /// 3-赎回
    /// </summary>
    private int model;
    /// <summary>
    /// 结果
    /// </summary>
    private Dictionary<int, int> resultDic;
    /// <summary>
    /// 是否可发送确定
    /// </summary>
    private bool isOk;

    private void Awake()
    {
        model = 0;
        isOk = false;
        resultDic = new Dictionary<int, int> { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 } };

        //监听
        EventCenter.AddListener(EventType.ShareButtonAvailable, ShareButtonAvailable);
        EventCenter.AddListener(EventType.ShareButtonUnAvaliable, ShareButtonUnAvaliable);

        //主界面按钮
        btn_Diya = transform.Find("btn_diya").GetComponent<Button>();
        btn_Shuhui = transform.Find("btn_shuhui").GetComponent<Button>();
        btn_Buy = transform.Find("btn_buy").GetComponent<Button>();
        
        //面板
        sharePanel = transform.Find("sharePanel").gameObject;

        txt_Hint = transform.Find("sharePanel/Text").GetComponent<Text>();
        //卡片
        btn_CardList = new List<Button>();
        btn_CardList.Add(transform.Find("sharePanel/doukou").GetComponent<Button>());
        btn_CardList.Add(transform.Find("sharePanel/silk").GetComponent<Button>());
        btn_CardList.Add(transform.Find("sharePanel/renshen").GetComponent<Button>());
        btn_CardList.Add(transform.Find("sharePanel/yushi").GetComponent<Button>());
        //值
        txt_NumList = new List<Text>();
        txt_NumList.Add(transform.Find("sharePanel/doukou/Text").GetComponent<Text>());
        txt_NumList.Add(transform.Find("sharePanel/silk/Text").GetComponent<Text>());
        txt_NumList.Add(transform.Find("sharePanel/renshen/Text").GetComponent<Text>());
        txt_NumList.Add(transform.Find("sharePanel/yushi/Text").GetComponent<Text>());
        //决定
        btn_Decide = transform.Find("sharePanel/btn_Decide").GetComponent<Button>();
        btn_Reset = transform.Find("sharePanel/btn_Reset").GetComponent<Button>();
        btn_Close = transform.Find("sharePanel/btn_Close").GetComponent<Button>();

        HideObj();
        AddListener();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShareButtonAvailable, ShareButtonAvailable);
        EventCenter.AddListener(EventType.ShareButtonUnAvaliable, ShareButtonUnAvaliable);

    }

    private void HideObj()
    {
        this.gameObject.SetActive(false);
        sharePanel.SetActive(false);
    }

    private void AddListener() {
        btn_Buy.onClick.AddListener(() => {
            ClickButtonBuy();
        });
        btn_Diya.onClick.AddListener(() => {
            ClickButtonDiya();
        });
        btn_Shuhui.onClick.AddListener(() => {
            ClickButtonShuhui();
        });

        //面板上的按钮们
        btn_CardList[0].onClick.AddListener(()=> {
            ClickCardButton(GoodCode.Doukou);
        });
        btn_CardList[1].onClick.AddListener(() => {
            ClickCardButton(GoodCode.Silk);
        });
        btn_CardList[2].onClick.AddListener(() => {
            ClickCardButton(GoodCode.Renshen);
        });
        btn_CardList[3].onClick.AddListener(() => {
            ClickCardButton(GoodCode.Yushi);
        });
        //确定
        btn_Decide.onClick.AddListener(()=> {
            if (isOk != true) EventCenter.Broadcast(EventType.Hint, "请点击卡片进行选择");
            else {
                switch (model)
                {
                    case 1:
                        btn_Buy.interactable = false;
                        NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.BuyShare_CREQ, resultDic);
                        break;
                    case 2:
                        NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.MortgageShare_CREQ, resultDic);
                        break;
                    case 3:
                        NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.RedemptionShare_CREQ, resultDic);
                        break;
                    default:
                        break;
                }
                sharePanel.SetActive(false);
            }
        });
        //重置
        btn_Reset.onClick.AddListener(()=> {
            switch (model)
            {
                case 1:
                    ClickButtonBuy();
                    break;
                case 2:
                    ClickButtonDiya();
                    break;
                case 3:
                    ClickButtonShuhui();
                    break;
                default:
                    break;
            }
        });
        //关闭
        btn_Close.onClick.AddListener(()=> {
            sharePanel.SetActive(false);
        });
    }
    /// <summary>
    /// 股票相关按钮可用
    /// </summary>
    private void ShareButtonAvailable()
    {
        this.gameObject.SetActive(true);

        PlayerDto dto = Models.GameModel.fightDto.GetPlayerDto(Models.GameModel.userDto.UserId);

        //如果是船长局且是第一回合，显示买股票按钮
        if (Models.GameModel.userDto.UserId == Models.GameModel.bidDto.HarbourMasterId && Models.GameModel.fightDto.roundModelDto.CurrentRound == 1)
        {
            btn_Buy.interactable = true;
        }
        else
        {
            btn_Buy.interactable = false;
        }
        //如果有股票，显示卖股票按钮
        if (dto.cardNum > 0)
        {
            btn_Diya.interactable = true;
        }
        else
        {
            btn_Diya.interactable = false;
        }
        //如果有抵押的股票，且金额大于15，显示赎回股票按钮
        if (dto.mortgageNum > 0 && dto.Money > 15)
        {
            btn_Shuhui.interactable = true;
        }
        else
        {
            btn_Shuhui.interactable = false;
        }
    }

    private void ShareButtonUnAvaliable()
    {
        this.gameObject.SetActive(false);

    }
    /// <summary>
    /// 购买
    /// </summary>
    private void ClickButtonBuy()
    {
        model = 1;
        txt_Hint.text = "海港负责人可以按照价格表（最少金币5）\n购买一张股票或放弃购买".ToString();
        InitShow(Models.GameModel.fightDto.bankCardDic);
    }
    /// <summary>
    /// 抵押
    /// </summary>
    private void ClickButtonDiya()
    {
        model = 2;
        txt_Hint.text = "抵押股票";
        PlayerDto dto = Models.GameModel.fightDto.GetPlayerDto(Models.GameModel.userDto.UserId);
        InitShow(dto.cardDic);
    }
    /// <summary>
    /// 赎回
    /// </summary>
    private void ClickButtonShuhui()
    {
        model = 3;
        txt_Hint.text = "赎回股票";
        PlayerDto dto = Models.GameModel.fightDto.GetPlayerDto(Models.GameModel.userDto.UserId);
        InitShow(dto.mortgageCardDic);
    }
    /// <summary>
    /// 初始化展示
    /// </summary>
    /// <param name="dic"></param>
    private void InitShow(Dictionary<int, int> dic)
    {
        for (int i = 0; i < 4; i++) resultDic[i] = 0;
        isOk = false;
        for (int i = 0; i < 4; i++)
        {
            txt_NumList[i].text = dic[i].ToString();
            //设置可否交互
            if (dic[i] == 0) btn_CardList[i].interactable = false;
            else btn_CardList[i].interactable = true;
        }
        sharePanel.SetActive(true);

    }
    /// <summary>
    /// 点击卡牌的响应
    /// </summary>
    /// <param name="goodCode"></param>
    private void ClickCardButton(int goodCode)
    {
        switch (model)
        {
            case 1:
                int price = Models.GameModel.fightDto.goodDto.GetPrice(goodCode);
                int ownMoney = Models.GameModel.fightDto.GetPlayerDto(Models.GameModel.userDto.UserId).Money;
                if (ownMoney < price)
                {
                    EventCenter.Broadcast(EventType.Hint, "金钱不足");
                    return;
                }
                btn_CardList[0].interactable = false;
                btn_CardList[1].interactable = false;
                btn_CardList[2].interactable = false;
                btn_CardList[3].interactable = false;
                txt_NumList[goodCode].text = (int.Parse(txt_NumList[goodCode].text) - 1).ToString();
                resultDic[goodCode]++;
                //可以点确定了
                isOk = true;
                break;
            case 2:
            case 3:
                int num = int.Parse(txt_NumList[goodCode].text) - 1;
                txt_NumList[goodCode].text = num.ToString();
                resultDic[goodCode]++;
                if (num == 0) btn_CardList[goodCode].interactable = false;
                isOk = true;
                break;
            default:
                break;
        }
    }
}
