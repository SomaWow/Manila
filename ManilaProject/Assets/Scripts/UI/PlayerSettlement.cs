using Protocol.Code.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettlement : MonoBehaviour {

    private int PlayerIndex;
    private int UserId;

    private Image img_HeadIcon;
    private Text txt_UserName;
    private Text txt_Money;
    private Image img_Win;

    private void Awake()
    {
        EventCenter.AddListener<Dictionary<int, int>>(EventType.SettlementContext, SettlementContext);
        EventCenter.AddListener<int>(EventType.GameOverSettlement, GameOverSettlement);

        PlayerIndex = int.Parse(gameObject.tag);

        img_HeadIcon = transform.Find("headIcon").GetComponent<Image>();
        txt_UserName = transform.Find("txt_UserName").GetComponent<Text>();
        txt_Money = transform.Find("money").GetComponent<Text>();
        img_Win = transform.Find("win").GetComponent<Image>();
        img_Win.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.AddListener<Dictionary<int, int>>(EventType.SettlementContext, SettlementContext);
        EventCenter.AddListener<int>(EventType.GameOverSettlement, GameOverSettlement);
    }

    /// <summary>
    /// 填充内容
    /// </summary>
    /// <param name="userIdProfitDic"></param>
    private void SettlementContext(Dictionary<int, int> userIdProfitDic)
    {

        UserId = Models.GameModel.matchRoomDto.enterOrderList[PlayerIndex];
        UserDto userDto = Models.GameModel.matchRoomDto.userIdUserDtoDic[UserId];
        img_HeadIcon.sprite = ResourcesManager.GetSprite(userDto.IconName);
        txt_UserName.text = userDto.UserName;

        txt_Money.text = userIdProfitDic[UserId].ToString();
    }

    /// <summary>
    /// 把金钱显示一下
    /// </summary>
    private void GameOverSettlement(int winPlayerId)
    {
        txt_Money.text = Models.GameModel.fightDto.playerList[PlayerIndex].Money.ToString();
        if(winPlayerId == this.UserId)
        {
            img_Win.gameObject.SetActive(true);
        }
    }
}
