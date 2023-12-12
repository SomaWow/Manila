using Protocol.Code;
using Protocol.Code.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsurancePanel : MonoBehaviour {


    private int userId;
    private int mortgageNum;
    private int ClickNumber;
    private Dictionary<int, int> resultDic;

    private List<Button> btn_CardList;
    private List<Text> txt_NumList;
    private Button btn_Decide;
    private Button btn_Reset;
    private Button btn_Close;
    private Text txt_Hint;

    private void Awake()
    {
        EventCenter.AddListener<Dictionary<int, int>>(EventType.InsurancePart, InsurancePart);

        this.gameObject.SetActive(false);

        ClickNumber = 0;
        resultDic = new Dictionary<int, int> { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 } };

        btn_CardList = new List<Button>();
        btn_CardList.Add(transform.Find("doukou").GetComponent<Button>());
        btn_CardList.Add(transform.Find("silk").GetComponent<Button>());
        btn_CardList.Add(transform.Find("renshen").GetComponent<Button>());
        btn_CardList.Add(transform.Find("yushi").GetComponent<Button>());

        //值
        txt_NumList = new List<Text>();
        txt_NumList.Add(transform.Find("doukou/Text").GetComponent<Text>());
        txt_NumList.Add(transform.Find("silk/Text").GetComponent<Text>());
        txt_NumList.Add(transform.Find("renshen/Text").GetComponent<Text>());
        txt_NumList.Add(transform.Find("yushi/Text").GetComponent<Text>());
        //提示显示
        txt_Hint = transform.Find("Hint").GetComponent<Text>();
        //加上监听
        Addlistener();
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<Dictionary<int, int>>(EventType.InsurancePart, InsurancePart);

    }

    private void Addlistener()
    {
        //面板上的按钮们
        btn_CardList[0].onClick.AddListener(() => {
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


        //决定
        btn_Decide = transform.Find("btn_Decide").GetComponent<Button>();
        btn_Decide.onClick.AddListener(() => {
            if (ClickNumber == mortgageNum)
            {
                NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.InsurancePart_CREQ, resultDic);
                this.gameObject.SetActive(false);
            }
            else
            {
                EventCenter.Broadcast(EventType.Hint, "请选择要求的股票数");
            }
        });
        btn_Reset = transform.Find("btn_Reset").GetComponent<Button>();
        btn_Reset.onClick.AddListener(()=> {
            InitShow();
        });
    }

    /// <summary>
    /// 从这里开始，显示
    /// [-1] 里面是userId，[-2]里面是要选择几张牌
    /// </summary>
    /// <param name="userIdProfitDic"></param>
    private void InsurancePart(Dictionary<int, int> userIdProfitDic)
    {
        foreach (var key in userIdProfitDic.Keys)
        {
            Debug.Log("userIdProfitDic.Keys为" + key + "; userIdProfitDic.Values为" + userIdProfitDic[key]);
        }

        if (userIdProfitDic[-1] == Models.GameModel.userDto.UserId)
        {
            userId = userIdProfitDic[-1];
            mortgageNum = userIdProfitDic[-2];

            Debug.Log("mortgageNum被成功赋值了");
            txt_Hint.text = "你需要抵押" + mortgageNum.ToString() + "张股票还债";
            InitShow();
            this.gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// 填充卡片数量和设置交互
    /// </summary>
    private void InitShow()
    {
        for (int i = 0; i < 4; i++) resultDic[i] = 0;
        ClickNumber = 0;

        //拥有的股票
        PlayerDto dto = Models.GameModel.fightDto.GetPlayerDto(Models.GameModel.userDto.UserId);
        for (int i = 0; i < 4; i++)
        {
            txt_NumList[i].text = dto.cardDic[i].ToString();
            //设置可否交互
            if (dto.cardDic[i] == 0) btn_CardList[i].interactable = false;
            else btn_CardList[i].interactable = true;
        }
    }
    /// <summary>
    /// 点击卡片的响应
    /// </summary>
    /// <param name="goodCode"></param>
    private void ClickCardButton(int goodCode)
    {
        ClickNumber++;
        int num = int.Parse(txt_NumList[goodCode].text) - 1;
        txt_NumList[goodCode].text = num.ToString();
        resultDic[goodCode]++;
        if (num == 0) btn_CardList[goodCode].interactable = false;
    }
}
