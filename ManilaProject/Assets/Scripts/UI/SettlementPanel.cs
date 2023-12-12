using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettlementPanel : MonoBehaviour {

    private Button btn_RollOut;
    private Text txt_Title;
    private bool IsGameOver;

    private void Awake()
    {
        EventCenter.AddListener(EventType.SettlementShow, SettlementShow);
        EventCenter.AddListener<int>(EventType.GameOverSettlement, GameOverSettlement);

        this.gameObject.SetActive(false);
        IsGameOver = false;

        btn_RollOut = transform.Find("btn_RollOut").GetComponent<Button>();
        btn_RollOut.onClick.AddListener(()=> {
            SceneManager.LoadScene("2.PersonalInterface");
            //服务器那边已经退出战斗房间了，这里退出匹配房间
            NetMsgCenter.Instance.SendMsg(OpCode.Match, MatchCode.Leave_CREQ, 0);//房间类型暂且用不到
            //更新一下输赢信息
            NetMsgCenter.Instance.SendMsg(OpCode.Account, AccountCode.GetUserInfo_CREQ, null);
        });

        txt_Title = transform.Find("Title").GetComponent<Text>();
        //Test
        //SettlementShow();
    }
    private void OnDestroy()
    {
        EventCenter.AddListener(EventType.SettlementShow, SettlementShow);
        EventCenter.AddListener<int>(EventType.GameOverSettlement, GameOverSettlement);
    }

    /// <summary>
    /// 展示结算面板
    /// </summary>
    private void SettlementShow()
    {
        txt_Title.text = "本轮盈利结果";
        btn_RollOut.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
        transform.localScale = new Vector3(0, 0, 0);
        StartCoroutine(ShowDelay());
    }

    private void GameOverMsg()
    {
        IsGameOver = true;
    }
    /// <summary>
    /// 展示结束游戏结算面板
    /// </summary>
    private void GameOverSettlement(int winPlayerId)
    {
        txt_Title.text = "游戏结束";
        this.gameObject.SetActive(true);
        btn_RollOut.gameObject.SetActive(true);
    }

    IEnumerator ShowDelay()
    {
        yield return new WaitForSeconds(5);
        transform.localScale = new Vector3(1, 1, 1);
        StartCoroutine(SettlementCompleteDelay());
    }

    IEnumerator SettlementCompleteDelay()
    {
        yield return new WaitForSeconds(5);
        this.gameObject.SetActive(false);

        NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.SettlementComplete_CREQ, null);
    }
}
