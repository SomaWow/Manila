using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour {

    private List<Image> img_diceList;

    private void Awake()
    {
        EventCenter.AddListener<List<int>>(EventType.CastDice, CastDice);

        this.gameObject.SetActive(false);

        img_diceList = new List<Image>();
        img_diceList.Add(transform.Find("A").GetComponent<Image>());
        img_diceList.Add(transform.Find("B").GetComponent<Image>());
        img_diceList.Add(transform.Find("C").GetComponent<Image>());
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener<List<int>>(EventType.CastDice, CastDice);
    }
    /// <summary>
    /// 接收骰子数目，展示出来，并调用移动船只的方法
    /// </summary>
    private void CastDice(List<int> dicList) {

        //显示投骰子的动画
        //显示投骰子的结果
        for (int i = 0; i < img_diceList.Count; i++)
        {
            img_diceList[i].sprite = ResourcesManager.GetDiceSprite(dicList[i]);
        }
        this.gameObject.SetActive(true);
        StartCoroutine(DiceDelay());

    }

    IEnumerator DiceDelay()
    {
        yield return new WaitForSeconds(2);
        //广播移动船只
        EventCenter.Broadcast(EventType.MoveShip);
        StartCoroutine(ShipMoveDelay());
    }
    IEnumerator ShipMoveDelay()
    {
        yield return new WaitForSeconds(2);
        //只让船长发一次就好了
        if (Models.GameModel.userDto.UserId == Models.GameModel.bidDto.HarbourMasterId)
        {
            NetMsgCenter.Instance.SendMsg(OpCode.Fight, FightCode.NextRound_CREQ, null);
        }
        this.gameObject.SetActive(false);
    }
}
