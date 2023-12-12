using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PilotPanel : MonoBehaviour
{

    private int model;
    private int clickCount;
    /// <summary>
    /// 用于传输结果
    /// </summary>
    private List<int> moveList;
    /// <summary>
    /// 用于临时存放
    /// </summary>
    private List<int> showNumList;

    private List<Button> btn_DirectionList;
    private List<Text> txt_StepNumList;
    private Button btn_Decide;
    private Button btn_Reset;
    private Button btn_Close;
    private Text txt_Hint;

    private void Awake()
    {
        this.gameObject.SetActive(false);

        EventCenter.AddListener(EventType.SmallPilotOperation, SmallPilotOperation);
        EventCenter.AddListener(EventType.LargePilotOperation, LargePilotOperation);

        model = 0;
        clickCount = 0;
        moveList = new List<int>() { 0, 0, 0 };
        showNumList = new List<int>() { 0, 0, 0, 0, 0, 0 };

        btn_Decide = transform.Find("btn_Move").GetComponent<Button>();
        btn_Decide.onClick.AddListener(() =>
        {
            if (clickCount == 0)
            {
                EventCenter.Broadcast(EventType.Hint, "请选择");
                return;
            }
            NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.Pilot_CREQ, moveList);
            this.gameObject.SetActive(false);
            //整理一下信息提交啦

        });
        btn_Reset = transform.Find("btn_Reset").GetComponent<Button>();
        btn_Reset.onClick.AddListener(ResetShow);
        btn_Close = transform.Find("btn_Close").GetComponent<Button>();
        btn_Close.onClick.AddListener(() =>
        {
            NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.Pilot_CREQ, new List<int>() { 0, 0, 0});
            this.gameObject.SetActive(false);
        });

        btn_DirectionList = new List<Button>();
        btn_DirectionList.Add(transform.Find("A_up").GetComponent<Button>());
        btn_DirectionList.Add(transform.Find("B_up").GetComponent<Button>());
        btn_DirectionList.Add(transform.Find("C_up").GetComponent<Button>());
        btn_DirectionList.Add(transform.Find("A_down").GetComponent<Button>());
        btn_DirectionList.Add(transform.Find("B_down").GetComponent<Button>());
        btn_DirectionList.Add(transform.Find("C_down").GetComponent<Button>());

        //加监听
        btn_DirectionList[0].onClick.AddListener(() =>
        {
            ClickDirectionButton(0, 1);
        });
        btn_DirectionList[1].onClick.AddListener(() =>
        {
            ClickDirectionButton(1, 1);
        });
        btn_DirectionList[2].onClick.AddListener(() =>
        {
            ClickDirectionButton(2, 1);
        });
        btn_DirectionList[3].onClick.AddListener(() =>
        {
            ClickDirectionButton(3, -1);
        });
        btn_DirectionList[4].onClick.AddListener(() =>
        {
            ClickDirectionButton(4, -1);
        });
        btn_DirectionList[5].onClick.AddListener(() =>
        {
            ClickDirectionButton(5, -1);
        });

        txt_StepNumList = new List<Text>();
        txt_StepNumList.Add(transform.Find("A_up/Text").GetComponent<Text>());
        txt_StepNumList.Add(transform.Find("B_up/Text").GetComponent<Text>());
        txt_StepNumList.Add(transform.Find("C_up/Text").GetComponent<Text>());
        txt_StepNumList.Add(transform.Find("A_down/Text").GetComponent<Text>());
        txt_StepNumList.Add(transform.Find("B_down/Text").GetComponent<Text>());
        txt_StepNumList.Add(transform.Find("C_down/Text").GetComponent<Text>());

        txt_Hint = transform.Find("Hint").GetComponent<Text>();
        //Test
        //LargePilotOperation();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.SmallPilotOperation, SmallPilotOperation);
        EventCenter.RemoveListener(EventType.LargePilotOperation, LargePilotOperation);
    }

    private void SmallPilotOperation()
    {
        model = 1;
        txt_Hint.text = "小领航员移动1格";
        ResetShow();
    }

    private void LargePilotOperation()
    {
        model = 2;
        txt_Hint.text = "大领航员移动2格";
        ResetShow();
    }
    /// <summary>
    /// 重置显示
    /// </summary>
    private void ResetShow()
    {
        for (int i = 0; i < moveList.Count; i++) moveList[i] = 0;
        for (int i = 0; i < showNumList.Count; i++) showNumList[i] = 0;
        clickCount = 0;

        for (int i = 0; i < 6; i++)
        {
            txt_StepNumList[i].text = "";
            btn_DirectionList[i].interactable = true;
        }

        //到港船只不允许操作
        foreach (var shipCode in Models.GameModel.fightDto.ShipDto.PortShipList)
        {
            btn_DirectionList[shipCode].interactable = false;
            btn_DirectionList[shipCode + 3].interactable = false;
        }

        this.gameObject.SetActive(true);
    }

    private void ClickDirectionButton(int index, int step)
    {
        moveList[index % 3] += step;
        showNumList[index] += 1;
        txt_StepNumList[index].text = showNumList[index].ToString();
        clickCount++;
        switch (model)
        {
            case 1:
                if (clickCount == 1)
                {
                    for (int i = 0; i < 6; i++) btn_DirectionList[i].interactable = false;
                }

                break;
            case 2:
                if (clickCount == 2)
                {
                    for (int i = 0; i < 6; i++) btn_DirectionList[i].interactable = false;
                }
                break;
            default:
                break;
        }

    }
}
