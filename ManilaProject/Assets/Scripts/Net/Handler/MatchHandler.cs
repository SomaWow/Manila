using Protocol.Code;
using Protocol.Code.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchHandler : BaseHandler
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case MatchCode.Enter_SRES:
                EnterRoomSRES(value as MatchRoomDto);
                break;
            case MatchCode.Enter_BRO:
                EnterRoomBRO(value as UserDto);
                break;
            case MatchCode.Leave_BRO:
                LeaveBRO((int)value);
                break;
            case MatchCode.Ready_BRO:
                ReadyBRO((int)value);
                break;
            case MatchCode.UnReady_BRO:
                UnReadyBRO((int)value);
                break;
            case MatchCode.StartGame_BRO:
                StartGame_BRO();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 开始游戏的广播
    /// </summary>
    private void StartGame_BRO()
    {
        EventCenter.Broadcast(EventType.Hint, "开始游戏");
        EventCenter.Broadcast(EventType.StartGame);

    }
    /// <summary>
    /// 客户端请求进入房间服务器的响应
    /// </summary>
    /// <param name="dto"></param>
    private void EnterRoomSRES(MatchRoomDto dto)
    {
        Models.GameModel.matchRoomDto = dto;
        //刷新界面玩家的UI显示
        EventCenter.Broadcast(EventType.RefreshUI);
    }
    /// <summary>
    /// 每次新来一个别的玩家给这边一个玩家信息的响应
    /// </summary>
    /// <param name="dto"></param>
    private void EnterRoomBRO(UserDto dto)
    {
        
        Models.GameModel.matchRoomDto.Enter(dto);
        //刷新界面玩家的UI显示
        EventCenter.Broadcast(EventType.RefreshUI);
        EventCenter.Broadcast(EventType.Hint,"玩家 "+dto.UserName+" 进入房间");
    }
    /// <summary>
    /// 有玩家离开服务器的广播
    /// </summary>
    private void LeaveBRO(int leaveUserId)
    {
        string userName = Models.GameModel.matchRoomDto.userIdUserDtoDic[leaveUserId].UserName;
        Models.GameModel.matchRoomDto.Leave(leaveUserId);
        EventCenter.Broadcast(EventType.RefreshUI);
        EventCenter.Broadcast(EventType.Hint, "玩家 " + userName + " 离开房间");
    }
    /// <summary>
    /// 有玩家准备，服务器发来广播
    /// </summary>
    /// <param name="readyUserId"></param>
    private void ReadyBRO(int readyUserId)
    {
        Models.GameModel.matchRoomDto.Ready(readyUserId);
        EventCenter.Broadcast(EventType.RefreshUI);
    }
    private void UnReadyBRO(int unReadyUserId)
    {
        Models.GameModel.matchRoomDto.UnReady(unReadyUserId);
        EventCenter.Broadcast(EventType.RefreshUI);
    }
}
