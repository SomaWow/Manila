using Protocol.Code;
using Protocol.Code.DTO;
using Protocol.Code.DTO.Fight;
using Protocol.Constant;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightHandler : BaseHandler
{
    public override void OnReceive(int subCode, object value)
    {
        switch (subCode)
        {
            case FightCode.StartFight_BRO:
                Debug.Log("接收到StartFight_BRO");
                StartFight_BRO(value as BidDto);
                break;
            case FightCode.RefreshCard_BRO:
                Debug.Log("接收到RefreshCard_BRO");
                RefreshCard_BRO(value as FightDto);
                break;
            case FightCode.Bidding_BRO:
                Debug.Log("接收到Bidding_BRO");
                Bidding_BRO((int) value);
                break;
            case FightCode.BiddingResult_BRO:
                Debug.Log("接收到BiddingResult_BRO");
                BiddingResult_BRO(value as BidDto);
                break;
            case FightCode.SetHarbourMaster_BRO:
                Debug.Log("接收到SetHarbourMaster_BRO");
                SetHarbourMaster_BRO(value as BidDto);
                break;
            case FightCode.RefreshMoney_BRO:
                Debug.Log("接收到RefreshMoney_BRO");
                RefreshMoney_BRO(value as List<PlayerDto>);
                break;
            case FightCode.OnBoard_BRO:
                Debug.Log("接收到OnBoard_BRO");
                OnBoard_BRO(value as List<int>);
                break;
            case FightCode.MoveShip_BRO:
                MoveShip_BRO(value as ShipDto);
                break;
            //有人放置了一个工人，刷新
            case FightCode.SetWorker_BRO: 
                SetWorker_BRO(value as List<PlayerDto>);
                break;
            //下一个人可以放工人了
            case FightCode.NextSetWorker_BRO: 
                NextSetWorker_BRO((int)value);
                break;
            //接收投骰子的结果
            case FightCode.CastDice_BRO:
                CastDice_BRO(value as List<int>);
                break;
            case FightCode.SmallPilot_BRO:
                SmallPilot_BRO((int)value);
                break;
            case FightCode.LargePilot_BRO:
                LargePilot_BRO((int)value);
                break;
            case FightCode.Round2Pirate_BRO:
                Round2Pirate_BRO(value as List<int>);
                break;
            case FightCode.Round3Pirate_BRO:
                Round3Pirate_BRO((int)value);
                break;
            case FightCode.Settlement_BRO:
                Settlement_BRO(value as Dictionary<int, int>);
                break;
            case FightCode.SettlementShow_BRO:
                SettlementShow_BRO();
                break;
            case FightCode.ValueRise_BRO:
                ValueRise_BRO(value as GoodDto);
                break;
            case FightCode.GameOver_BRO:
                GameOver_BRO((int)value);
                break;
            case FightCode.NewMoveRound_BRO:
                NewMoveRound_BRO(value as FightDto);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 将id转换为Indexs
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private int IdToIndex(int userId)
    {
        List<int> playerIdList = Models.GameModel.matchRoomDto.enterOrderList;
        for (int i = 0; i < playerIdList.Count; i++)
        {
            if (userId == playerIdList[i])
            {
                return i;
            }
        }
        return -1;
    }
    
    /// <summary>
    /// 开始游戏广播，应该开始竞价啦
    /// </summary>
    /// <param name="playerList"></param>
    private void StartFight_BRO(BidDto dto)
    {
        EventCenter.Broadcast(EventType.StartGame);
        Models.GameModel.bidDto = dto;
        EventCenter.Broadcast(EventType.Auction, dto.FirstPlayerId);
    }
    /// <summary>
    /// 发牌
    /// </summary>
    private void RefreshCard_BRO(FightDto dto)
    {
        Models.GameModel.fightDto = dto;
        EventCenter.Broadcast(EventType.RefreshCard);
    }

    /// <summary>
    /// 轮流竞价
    /// </summary>
    /// <param name="userId"></param>
    private void Bidding_BRO(int userId)
    {
        EventCenter.Broadcast(EventType.Auction, userId);
    }
    /// <summary>
    /// 更新竞价结果，显示Pass和目前出价最高的银
    /// </summary>
    private void BiddingResult_BRO(BidDto dto)
    {
        Models.GameModel.bidDto = dto;
        EventCenter.Broadcast(EventType.RefreshBiddingResult);
    }
    /// <summary>
    /// 设置船老大
    /// </summary>
    /// <param name="dto"></param>
    private void SetHarbourMaster_BRO(BidDto dto)
    {
        Models.GameModel.bidDto = dto;
        EventCenter.Broadcast(EventType.SetHarbourMaster);
    }
    /// <summary>
    /// 刷新金钱
    /// </summary>
    /// <param name="playerList"></param>
    private void RefreshMoney_BRO(List<PlayerDto> playerList)
    {
        Models.GameModel.fightDto.playerList = playerList;
        EventCenter.Broadcast(EventType.RefreshMoney);
    }
    /// <summary>
    /// 货物上船，要把位点们加进去
    /// </summary>
    private void OnBoard_BRO(List<int> onBoardList)
    {
        Models.GameModel.fightDto.ShipDto.GoodOnBoard(onBoardList);
        
        EventCenter.Broadcast(EventType.OnBoard, onBoardList);
    }
    /// <summary>
    /// 刷新船只的显示
    /// </summary>
    private void MoveShip_BRO(ShipDto dto)
    {
        Models.GameModel.fightDto.ShipDto = dto;
        EventCenter.Broadcast(EventType.MoveShip);
    }
    /// <summary>
    /// 有人放置了个工人广播
    /// </summary>
    private void SetWorker_BRO(List<PlayerDto> playerList)
    {
        //把改动的那位玩家的信息赋值到gamemoel
        Models.GameModel.fightDto.playerList = playerList;
        EventCenter.Broadcast(EventType.RefreshWorker);
        EventCenter.Broadcast(EventType.RefreshMoney);
    }
    /// <summary>
    /// 该下一个人放同伙了
    /// </summary>
    private void NextSetWorker_BRO(int userId)
    {
        if(userId == Models.GameModel.userDto.UserId)
        {
            EventCenter.Broadcast(EventType.OpenDetection);
            EventCenter.Broadcast(EventType.ShareButtonAvailable);
        }
        EventCenter.Broadcast(EventType.CurrentPlayer, userId);

        EventCenter.Broadcast(EventType.Hint, "第"+Models.GameModel.fightDto.roundModelDto.CurrentRound+"轮，" + "轮到 " + Models.GameModel.fightDto.GetPlayerDto(userId).UserName + " 放置同伙");

    }
    private void CastDice_BRO(List<int> diceList)
    {
        Debug.Log("收到了骰子的结果");
        //按照骰子的结果修改船的位置
        Models.GameModel.fightDto.ShipDto.MoveShip(diceList);
        //回合数更新
        Models.GameModel.fightDto.roundModelDto.NextRound();
        EventCenter.Broadcast(EventType.CastDice, diceList);
    }
    /// <summary>
    /// 小领航员开始操作
    /// </summary>
    /// <param name="userId"></param>
    private void SmallPilot_BRO(int userId)
    {
        EventCenter.Broadcast(EventType.Hint, "小领航员操作");
        EventCenter.Broadcast(EventType.CurrentPlayer, userId);
        if(userId == Models.GameModel.userDto.UserId)
        {
            EventCenter.Broadcast(EventType.SmallPilotOperation);
            EventCenter.Broadcast(EventType.CurrentPlayer, userId);
        }
    }    
    /// <summary>
    /// 大领航员开始操作
    /// </summary>
    /// <param name="userId"></param>
    private void LargePilot_BRO(int userId)
    {
        EventCenter.Broadcast(EventType.Hint, "大领航员操作");
        EventCenter.Broadcast(EventType.CurrentPlayer, userId);
        if (userId == Models.GameModel.userDto.UserId)
        {
            EventCenter.Broadcast(EventType.LargePilotOperation);
            EventCenter.Broadcast(EventType.CurrentPlayer, userId);
        }
    }
    /// <summary>
    /// 海盗登船,第一个是
    /// </summary>
    private void Round2Pirate_BRO(List<int> list)
    {
        //指示当前在操作的玩家
        EventCenter.Broadcast(EventType.CurrentPlayer, list[0]);

        if (list[0] == Models.GameModel.userDto.UserId)
        {
            EventCenter.Broadcast(EventType.Hint, "请选择位于13上的空位登船");
            list.RemoveAt(0);
            EventCenter.Broadcast(EventType.PirateOnBoardOpen, list);
        }
        else
        {
            EventCenter.Broadcast(EventType.Hint, "海盗登船操作");
        }


    }
    /// <summary>
    /// 海盗抢劫
    /// </summary>
    private void Round3Pirate_BRO(int userId)
    {
        Debug.Log("海盗船长的Id为"+userId);
        EventCenter.Broadcast(EventType.Hint, "海盗抢劫，海盗船长选择");
        //海盗船长的选择
        EventCenter.Broadcast(EventType.PirateChoose, userId);
    }

    /// <summary>
    /// 结算，还包含了保险员的功能
    /// </summary>
    /// <param name="userIdProfitDic"></param>
    private void Settlement_BRO(Dictionary<int,int> userIdProfitDic)
    {
        Debug.Log("收到结算信息");
        //填充内容
        EventCenter.Broadcast(EventType.SettlementContext, userIdProfitDic);
        
        if (userIdProfitDic.ContainsKey(-1))
        {
            if (userIdProfitDic.ContainsKey(-2))
            {
                EventCenter.Broadcast(EventType.Hint, "保险员需要抵押" + userIdProfitDic[-2] + "张股票还债");

                //如果需要抵押股票，显示抵押股票的页面
                EventCenter.Broadcast(EventType.InsurancePart, userIdProfitDic);
                //当前操作玩家指示
                EventCenter.Broadcast(EventType.CurrentPlayer, userIdProfitDic[-1]);
                return;
            }
            else
            {
                EventCenter.Broadcast(EventType.Hint, "保险员为了还债抵押掉了所有的股票");
            }

        }
        EventCenter.Broadcast(EventType.SettlementShow);
    }
    /// <summary>
    /// 因为保险员要进行选择，没有显示的结算界面现在显示
    /// </summary>
    private void SettlementShow_BRO()
    {
        EventCenter.Broadcast(EventType.SettlementShow);
    }
    /// <summary>
    /// 货物升值
    /// </summary>
    private void ValueRise_BRO(GoodDto goodDto)
    {
        Models.GameModel.fightDto.goodDto = goodDto;
        EventCenter.Broadcast(EventType.RefreshPriceBoard);
    }
    /// <summary>
    /// 游戏结束
    /// </summary>
    private void GameOver_BRO(int winPlayerId)
    {
        EventCenter.Broadcast(EventType.GameOverSettlement, winPlayerId);
    }
    /// <summary>
    /// 开始新的一轮移动回合
    /// </summary>
    private void NewMoveRound_BRO(FightDto dto)
    {
        Models.GameModel.fightDto = dto;
        EventCenter.Broadcast(EventType.OffBoard);
        EventCenter.Broadcast(EventType.ClickReset);
        EventCenter.Broadcast(EventType.RefreshCard);
        EventCenter.Broadcast(EventType.RefreshWorker);
        EventCenter.Broadcast(EventType.MoveShip);
        EventCenter.Broadcast(EventType.NewMoveRound);
    }
}
