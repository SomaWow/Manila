using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Protocol;

public class ManilaManager : MonoBehaviour
{

    private Button btn_Ready;
    private Button btn_UnReady;
    private Button btn_Back;

    private Button btn_DoukouWare;
    private Button btn_SilkWare;
    private Button btn_RenshenWare;
    private Button btn_YushiWare;

    //股票牌们
    private Dictionary<int, Text> txt_BankCardDic;
    private Dictionary<int, Text> txt_ownCardDic;
    private Dictionary<int, Text> txt_mortgageCardDic;

    /// <summary>
    /// 船选择界面
    /// </summary>
    //船老大功能部分
    private GameObject harbourMasterPanel;
    private InputField input_A;
    private InputField input_B;
    private InputField input_C;
    private Button btn_hmMove;
    private Button btn_hmReset;


    //临时变量使用的
    private List<int> onBoardList;

    private void Awake()
    {
        //发送进入房间的请求
        if (NetMsgCenter.Instance != null)
        {
            NetMsgCenter.Instance.SendMsg(OpCode.Match, MatchCode.Enter_CREQ, 0); //这个0之后可以改成房间类型使用
            Init();
        }
        EventCenter.AddListener(EventType.SetHarbourMaster, SetHarhourMaster);
        EventCenter.AddListener(EventType.RefreshCard, RefreshCard);
        EventCenter.AddListener(EventType.StartGame, StartGame);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.SetHarbourMaster, SetHarhourMaster);
        EventCenter.RemoveListener(EventType.RefreshCard, RefreshCard);
        EventCenter.RemoveListener(EventType.StartGame, StartGame);
    }
    private void Init()
    {
        onBoardList = new List<int>();

        //准备的按钮部分
        btn_Ready = transform.Find("/Canvas/btn_Ready").GetComponent<Button>();
        btn_Ready.onClick.AddListener(delegate { OnReadyButtonClick(); });
        btn_UnReady = transform.Find("/Canvas/btn_UnReady").GetComponent<Button>();
        btn_UnReady.gameObject.SetActive(false); //设置不可见
        btn_UnReady.onClick.AddListener(delegate { OnUnReadyButtonClick(); });

        //上船货物,设置为点击后禁用和变灰
        btn_DoukouWare = transform.Find("/Canvas/Ware/doukou").GetComponent<Button>();
        btn_DoukouWare.gameObject.SetActive(false);
        btn_DoukouWare.onClick.AddListener(()=> {
            OnBoard(GoodCode.Doukou);
            btn_DoukouWare.interactable = false;
        });

        btn_SilkWare = transform.Find("/Canvas/Ware/silk").GetComponent<Button>();
        btn_SilkWare.gameObject.SetActive(false);
        btn_SilkWare.onClick.AddListener(()=> {
            OnBoard(GoodCode.Silk);
            btn_SilkWare.interactable = false;
        });

        btn_RenshenWare = transform.Find("/Canvas/Ware/renshen").GetComponent<Button>();
        btn_RenshenWare.gameObject.SetActive(false);
        btn_RenshenWare.onClick.AddListener(()=> {
            OnBoard(GoodCode.Renshen);
            btn_RenshenWare.interactable = false;
        });
        btn_YushiWare = transform.Find("/Canvas/Ware/yushi").GetComponent<Button>();
        btn_YushiWare.gameObject.SetActive(false);
        btn_YushiWare.onClick.AddListener(()=> {
            OnBoard(GoodCode.Yushi);
            btn_YushiWare.interactable = false;
        });

        //返回按钮
        btn_Back = transform.Find("/Canvas/btn_Leave").GetComponent<Button>();
        btn_Back.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("2.PersonalInterface");
            //向服务器发送离开房间的请求
            NetMsgCenter.Instance.SendMsg(OpCode.Match, MatchCode.Leave_CREQ, 0);//房间类型暂且用不到
        });
        //股票牌们
        txt_BankCardDic = new Dictionary<int, Text>();
        txt_BankCardDic.Add(GoodCode.Doukou, transform.Find("/Canvas/Bank/bank_doukou/number").GetComponent<Text>());
        txt_BankCardDic.Add(GoodCode.Silk, transform.Find("/Canvas/Bank/bank_silk/number").GetComponent<Text>());
        txt_BankCardDic.Add(GoodCode.Renshen, transform.Find("/Canvas/Bank/bank_renshen/number").GetComponent<Text>());
        txt_BankCardDic.Add(GoodCode.Yushi, transform.Find("/Canvas/Bank/bank_yushi/number").GetComponent<Text>());

        txt_ownCardDic = new Dictionary<int, Text>();
        txt_ownCardDic.Add(GoodCode.Doukou, transform.Find("/Canvas/Card/self_doukou/own").GetComponent<Text>());
        txt_ownCardDic.Add(GoodCode.Silk, transform.Find("/Canvas/Card/self_silk/own").GetComponent<Text>());
        txt_ownCardDic.Add(GoodCode.Renshen, transform.Find("/Canvas/Card/self_renshen/own").GetComponent<Text>());
        txt_ownCardDic.Add(GoodCode.Yushi, transform.Find("/Canvas/Card/self_yushi/own").GetComponent<Text>());

        txt_mortgageCardDic = new Dictionary<int, Text>();
        txt_mortgageCardDic.Add(GoodCode.Doukou, transform.Find("/Canvas/Card/self_doukou/mortgage").GetComponent<Text>());
        txt_mortgageCardDic.Add(GoodCode.Silk, transform.Find("/Canvas/Card/self_silk/mortgage").GetComponent<Text>());
        txt_mortgageCardDic.Add(GoodCode.Renshen, transform.Find("/Canvas/Card/self_renshen/mortgage").GetComponent<Text>());
        txt_mortgageCardDic.Add(GoodCode.Yushi, transform.Find("/Canvas/Card/self_yushi/mortgage").GetComponent<Text>());

        //船选择界面
        //船老大选择界面
        harbourMasterPanel = transform.Find("/Canvas/ShipSelectionPanel/harbourmaster_part").gameObject;
        harbourMasterPanel.SetActive(false);
        input_A = transform.Find("/Canvas/ShipSelectionPanel/harbourmaster_part/input_A").GetComponent<InputField>();
        input_B = transform.Find("/Canvas/ShipSelectionPanel/harbourmaster_part/input_B").GetComponent<InputField>();
        input_C = transform.Find("/Canvas/ShipSelectionPanel/harbourmaster_part/input_C").GetComponent<InputField>();
        btn_hmMove = transform.Find("/Canvas/ShipSelectionPanel/harbourmaster_part/btn_Move").GetComponent<Button>();
        btn_hmMove.onClick.AddListener(()=> {
            hmMoveButton();
        });
        btn_hmReset = transform.Find("/Canvas/ShipSelectionPanel/harbourmaster_part/btn_Reset").GetComponent<Button>();
        btn_hmReset.onClick.AddListener(()=> {
            input_A.text = "";
            input_B.text = "";
            input_C.text = "";
        });

    }
    /// <summary>
    /// 准备按钮点击
    /// </summary>
    private void OnReadyButtonClick()
    {
        btn_Ready.gameObject.SetActive(false);
        NetMsgCenter.Instance.SendMsg(OpCode.Match, MatchCode.Ready_CREQ, 0);
        btn_UnReady.gameObject.SetActive(true);
    }
    /// <summary>
    /// 取消准备按钮
    /// </summary>
    private void OnUnReadyButtonClick()
    {
        btn_UnReady.gameObject.SetActive(false);
        btn_Ready.gameObject.SetActive(true);
        NetMsgCenter.Instance.SendMsg(OpCode.Match, MatchCode.UnReady_CREQ, 0);
    }
    /// <summary>
    /// 开始游戏
    /// </summary>
    private void StartGame()
    {
        btn_Ready.gameObject.SetActive(false);
        btn_UnReady.gameObject.SetActive(false);
        btn_Back.gameObject.SetActive(false);
    }
    /// <summary>
    /// 更新牌的显示
    /// </summary>
    private void RefreshCard()
    {
        Dictionary<int, int> dic = Models.GameModel.fightDto.bankCardDic;
        foreach (var key in dic.Keys)
        {
            txt_BankCardDic[key].text = dic[key].ToString();
        }
        //循环这些玩家，如果等于当前玩家都把牌的信息展示一下
        foreach (var player in Models.GameModel.fightDto.playerList)
        {
            if (player.UserId == Models.GameModel.userDto.UserId)
            {
                foreach (var key in player.cardDic.Keys)
                {
                    txt_ownCardDic[key].text = player.cardDic[key].ToString();
                }
                foreach (var key in player.mortgageCardDic.Keys)
                {
                    txt_mortgageCardDic[key].text = player.mortgageCardDic[key].ToString();
                }
            }
        }
    }
    /// <summary>
    /// 设置船长
    /// </summary>
    private void SetHarhourMaster()
    {
        onBoardList.Clear();
        //如果该客户端玩家为船老大，就让他选择上船的货物
        if (Models.GameModel.bidDto.HarbourMasterId == Models.GameModel.userDto.UserId)
        {
            EventCenter.Broadcast(EventType.ShareButtonAvailable);

            btn_DoukouWare.gameObject.SetActive(true);
            btn_SilkWare.gameObject.SetActive(true);
            btn_RenshenWare.gameObject.SetActive(true);
            btn_YushiWare.gameObject.SetActive(true);

            btn_DoukouWare.interactable = true;
            btn_SilkWare.interactable = true;
            btn_RenshenWare.interactable = true;
            btn_YushiWare.interactable = true;
        }
    }
    /// <summary>
    /// 货物上船
    /// </summary>
    private void OnBoard(int goodCode)
    {
        onBoardList.Add(goodCode);
        //如果==3，这里完事了搞下一步
        if(onBoardList.Count == 3)
        {
            onBoardList.Sort();
            NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.OnBoard_CREQ, onBoardList);
            onBoardList.Clear();

            btn_DoukouWare.gameObject.SetActive(false);
            btn_SilkWare.gameObject.SetActive(false);
            btn_RenshenWare.gameObject.SetActive(false);
            btn_YushiWare.gameObject.SetActive(false);

            //显示选择移动步数的页面
            harbourMasterPanel.SetActive(true);
        }
    }
    /// <summary>
    /// 在点击事件里调用，船长移动船只
    /// </summary>
    private void hmMoveButton()
    {
        if (input_A.text == null || input_A.text == "" || input_B.text == null || input_B.text == "" || input_C.text == null || input_C.text == "")
        {
            EventCenter.Broadcast(EventType.Hint, "不能为空");
            return;
        }
        int Amove = int.Parse(input_A.text);
        int Bmove = int.Parse(input_B.text);
        int Cmove = int.Parse(input_C.text);
        
        if(Amove < 0 || Amove > 5 || Bmove < 0 || Bmove > 5 || Cmove < 0 || Cmove > 5)
        {
            EventCenter.Broadcast(EventType.Hint, "单个船只移动范围为0-5");
            return;
        }
        if((Amove + Bmove + Cmove) != 9)
        {
            EventCenter.Broadcast(EventType.Hint, "三只船总共的移动距离之和为9");
            return;
        }
       
        //恭喜闯过了重重if来到这里，给服务器发送信息广播给所有玩家
        NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.MoveShip_CREQ, new List<int>{ Amove, Bmove, Cmove});
        //面板消失
        harbourMasterPanel.SetActive(false);
        //重置面板内容
        input_A.text = "";
        input_B.text = "";
        input_C.text = "";

        SetWorker(Models.GameModel.bidDto.HarbourMasterId);
    }
    /// <summary>
    /// 开放点击，放工人,这里只管海盗船长的
    /// </summary>
    private void SetWorker(int userId)
    {
        //只有在自己的客户端才能开放放工人功能
        if(userId == Models.GameModel.userDto.UserId)
        {
            //结束回合的按钮出现
            EventCenter.Broadcast(EventType.OpenDetection);
            EventCenter.Broadcast(EventType.Hint, "请放置一个同伙");
        }
    }
}
