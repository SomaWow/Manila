using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiratePassButton: MonoBehaviour {

    private Button btn_Pass;

    private void Awake()
    {
        this.gameObject.SetActive(false);

        EventCenter.AddListener<List<int>>(EventType.PirateOnBoardOpen, PiratePass);
        EventCenter.AddListener(EventType.PirateOnBoardClose, PiratePassClose);

        btn_Pass = this.gameObject.GetComponent<Button>();
        btn_Pass.onClick.AddListener(()=> {

            NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.PiratePass_CREQ, null);

            gameObject.SetActive(false);
            EventCenter.Broadcast(EventType.PirateOnBoardClose);
        });
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<List<int>>(EventType.PirateOnBoardOpen, PiratePass);
        EventCenter.AddListener(EventType.PirateOnBoardClose, PiratePassClose);
    }
    private void PiratePass(List<int> unUserlessList)
    {
        gameObject.SetActive(true);
    }
    private void PiratePassClose()
    {
        gameObject.SetActive(false);
    }

}
