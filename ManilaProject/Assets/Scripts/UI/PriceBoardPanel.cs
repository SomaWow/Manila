using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriceBoardPanel : MonoBehaviour {

    private List<Image> PiecesList; //股价表上的棋子,棋子向前移动的距离是28.5
    private int InitY = -36;

    private List<int> CurrentPriceList;

    private void Awake()
    {
        EventCenter.AddListener(EventType.RefreshPriceBoard, IncreasePrice);
        CurrentPriceList = new List<int> { 0, 0, 0, 0 };

        PiecesList = new List<Image>();
        PiecesList.Add(transform.Find("piece1").GetComponent<Image>());
        PiecesList.Add(transform.Find("piece2").GetComponent<Image>());
        PiecesList.Add(transform.Find("piece3").GetComponent<Image>());
        PiecesList.Add(transform.Find("piece4").GetComponent<Image>());
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.RefreshPriceBoard, IncreasePrice);
    }
    /// <summary>
    /// 刷新价格表
    /// </summary>
    private void IncreasePrice()
    {
        Dictionary<int, int> dic = Models.GameModel.fightDto.goodDto.GoodShareDic;
        for(int i = 0; i < 4; i++)
        {
            Debug.Log("收到的货物的价格为" + dic[i]);
            if (CurrentPriceList[i] == dic[i])
                continue;
            CurrentPriceList[i] = dic[i];
            var v = PiecesList[i].transform.localPosition;
            v.y = v.y + 28.5f;
            PiecesList[i].transform.localPosition = v;
        }
    }
}
