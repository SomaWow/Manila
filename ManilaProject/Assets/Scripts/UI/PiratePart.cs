using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiratePart : MonoBehaviour {

    private List<Button> btn_ReachButtonList;
    private List<Button> btn_FailButtonList;
    private List<int> resultList;
    private Color InitColor = new Color(0.96f, 0.77f, 0);
    private Color ClickColor = new Color(0.78f, 0.82f, 1f);
    private int clickNum = 0;
    private int shipNum = 0;

    private void Awake()
    {
        this.gameObject.SetActive(false);

        EventCenter.AddListener<int>(EventType.PirateChoose, PirateChoose);

        resultList = new List<int> { 0, 0, 0 };

        btn_ReachButtonList = new List<Button>();
        btn_ReachButtonList.Add(transform.Find("btn_A_ReachPort").GetComponent<Button>());
        btn_ReachButtonList.Add(transform.Find("btn_B_ReachPort").GetComponent<Button>());
        btn_ReachButtonList.Add(transform.Find("btn_C_ReachPort").GetComponent<Button>());

        btn_ReachButtonList[0].onClick.AddListener(()=> {
            btn_ReachButtonList[0].image.color = ClickColor;
            btn_ReachButtonList[0].interactable = false;
            btn_FailButtonList[0].interactable = false;
            resultList[0] = ShipSiteCode.Port;
            Judge();
        });
        btn_ReachButtonList[1].onClick.AddListener(()=> {
            btn_ReachButtonList[1].image.color = ClickColor;
            btn_ReachButtonList[1].interactable = false;
            btn_FailButtonList[1].interactable = false;
            resultList[1] = ShipSiteCode.Port;
            Judge();
        });
        btn_ReachButtonList[2].onClick.AddListener(()=> {
            btn_ReachButtonList[2].image.color = ClickColor;
            btn_ReachButtonList[2].interactable = false;
            btn_FailButtonList[2].interactable = false;
            resultList[2] = ShipSiteCode.Port;
            Judge();
        });

        btn_FailButtonList = new List<Button>();
        btn_FailButtonList.Add(transform.Find("btn_A_FailReach").GetComponent<Button>());
        btn_FailButtonList.Add(transform.Find("btn_B_FailReach").GetComponent<Button>());
        btn_FailButtonList.Add(transform.Find("btn_C_FailReach").GetComponent<Button>());
        btn_FailButtonList[0].onClick.AddListener(() => {
            btn_FailButtonList[0].image.color = ClickColor;
            btn_FailButtonList[0].interactable = false;
            btn_ReachButtonList[0].interactable = false;
            resultList[0] = ShipSiteCode.Fix;
            Judge();
        });
        btn_FailButtonList[1].onClick.AddListener(() => {
            btn_FailButtonList[1].image.color = ClickColor;
            btn_FailButtonList[1].interactable = false;
            btn_ReachButtonList[1].interactable = false;
            resultList[1] = ShipSiteCode.Fix;
            Judge();
        });
        btn_FailButtonList[2].onClick.AddListener(() => {
            btn_FailButtonList[2].image.color = ClickColor;
            btn_FailButtonList[2].interactable = false;
            btn_ReachButtonList[2].interactable = false;
            resultList[2] = ShipSiteCode.Fix;
            Judge();
        });
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<int>(EventType.PirateChoose, PirateChoose);
    }

    private void PirateChoose(int userId)
    {
        Debug.Log("面板收到了信息"+userId);
        if(Models.GameModel.userDto.UserId == userId)
        {
            List<int> shipOn13List = new List<int>();
            shipOn13List = Models.GameModel.fightDto.ShipDto.GetShipOn13();
            shipNum = shipOn13List.Count;
            clickNum = 0;
            for (int i = 0; i < 3; i++)
            {
                btn_ReachButtonList[i].image.color = InitColor;
                btn_FailButtonList[i].image.color = InitColor;
                if (shipOn13List.Contains(i))
                {
                    btn_ReachButtonList[i].interactable = true;
                    btn_FailButtonList[i].interactable = true;
                }
                else
                {
                    btn_ReachButtonList[i].interactable = false;
                    btn_FailButtonList[i].interactable = false;
                }

            }
            this.gameObject.SetActive(true);
        }


    }
    private void Judge()
    {
        clickNum++;
        if(clickNum == shipNum)
        {
            this.gameObject.SetActive(false);
            NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.PirateChoose_CREQ, resultList);
        }
    }
}
