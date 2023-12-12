using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code
{
    /// <summary>
    /// 战斗模块自操作码
    /// </summary>
    public class FightCode
    {
        public const int StartFight_BRO = 0;
        public const int Leave_BRO = 1;
        //竞价
        public const int Bidding_CREQ = 2;
        public const int Bidding_BRO = 3;
        public const int BiddingResult_BRO = 4;
        public const int SetHarbourMaster_BRO = 5;
        //刷新牌组
        public const int RefreshCard_BRO = 6;
        //上船的货物
        public const int OnBoard_CREQ = 7;
        public const int OnBoard_BRO = 8;
        //刷新客户端的金钱
        public const int RefreshMoney_BRO = 9;
        //移动船只
        public const int MoveShip_CREQ = 10;
        public const int MoveShip_BRO = 11;
        //检测到有位点被点击
        public const int ClickPos_CREQ = 12;
        public const int SetWorker_BRO = 13;
        //轮到下一位放置工人了
        public const int NextSetWorker_BRO = 14;
        //投骰子
        public const int CastDice_BRO = 15;
        //股票购买，贷款，赎回部分
        public const int BuyShare_CREQ = 16;
        public const int MortgageShare_CREQ = 17;
        public const int RedemptionShare_CREQ = 18;
        //某位玩家的回合结束，轮到下一位玩家
        public const int EndOperation_CREQ = 19;
        public const int NextRound_CREQ = 20;

        /// <summary>
        /// 同伙职能判定
        /// </summary>
        public const int SmallPilot_BRO = 21;
        public const int LargePilot_BRO = 22;
        public const int Pilot_CREQ = 23;
        public const int Round2Pirate_BRO = 24;
        public const int Round3Pirate_BRO = 25;
        public const int PirateOnBoard_CREQ = 26;
        public const int PiratePass_CREQ = 27;
        public const int PirateChoose_CREQ = 28;

        /// <summary>
        /// 结算部分
        /// </summary>
        public const int Settlement_BRO = 29;
        public const int SettlementShow_BRO = 30;
        public const int InsurancePart_CREQ = 31;
        public const int ValueRise_BRO = 32;
        public const int SettlementComplete_CREQ = 33;
        public const int GameOver_BRO = 34;
        public const int NewMoveRound_BRO = 35;
        public const int InsuranceChoose_BRO = 36;
    }
}
