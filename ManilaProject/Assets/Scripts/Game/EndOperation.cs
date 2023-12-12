using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndOperation : MonoBehaviour {

    private Button btn_EndOperation;

    private void Awake()
    {
        EventCenter.AddListener(EventType.OpenDetection, EndThisRound);

        this.gameObject.SetActive(false);

        btn_EndOperation = this.transform.GetComponent<Button>();
        btn_EndOperation.onClick.AddListener(()=> {
            //传递结束该玩家回合的信息，隐藏按钮
            NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.EndOperation_CREQ, null);
            this.gameObject.SetActive(false);
            EventCenter.Broadcast(EventType.ShareButtonUnAvaliable);

        });
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.OpenDetection, EndThisRound);
    }

    /// <summary>
    /// 出现
    /// </summary>
    private void EndThisRound()
    {
        this.gameObject.SetActive(true);
    }
}
