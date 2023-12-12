using Protocol;
using Protocol.Code.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModel
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public UserDto userDto { get; set; }
    /// <summary>
    /// 匹配房间传输模型
    /// </summary>
    public MatchRoomDto matchRoomDto { get; set; }

    /// <summary>
    /// 战斗情况
    /// </summary>
    public FightDto fightDto { get; set; }
    //Models.GameModel.fightDto.GetPlayerDto(Models.GameModel.userDto.UserId)

    /// <summary>
    /// 竞价选船长
    /// </summary>
    public BidDto bidDto { get; set; }
    /// <summary>
    /// 所有位点的开销和收益
    /// </summary>
    public SiteLibrary siteLibrary;

    public GameModel()
    {
        siteLibrary = new SiteLibrary();
    }
}