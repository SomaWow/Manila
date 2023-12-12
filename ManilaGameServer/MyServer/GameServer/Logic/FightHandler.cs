using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Cache;
using GameServer.Cache.Fight;
using GameServer.Database;
using MyServer;
using Protocol.Code;
using Protocol.Code.DTO;
using Protocol.Constant;

namespace GameServer.Logic
{
    public class FightHandler : IHandler
    {
        private FightCache fightCache = Caches.fightCache;
        public void Disconnect(ClientPeer client)
        {
            LeaveRoom(client);
        }

        public void Receive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case FightCode.Bidding_CREQ:
                    Bidding_CREQ(client, (int)value); //值为BiddingPrice，-1为pass
                    break;
                case FightCode.OnBoard_CREQ:
                    OnBoard_CREQ(client, value as List<int>);
                    break;
                case FightCode.MoveShip_CREQ:
                    MoveShip_CREQ(client, value as List<int>);
                    break;
                case FightCode.ClickPos_CREQ:
                    ClickPos_CREQ(client, value as List<int>); //点击位置，放置工人
                    break;
                case FightCode.EndOperation_CREQ:
                    EndOperation_CREQ(client);
                    break;
                case FightCode.BuyShare_CREQ:
                    BuyShare_CREQ(client, value as Dictionary<int, int>);
                    break;
                case FightCode.MortgageShare_CREQ:
                    MortgageShare_CREQ(client, value as Dictionary<int, int>);
                    break;
                case FightCode.RedemptionShare_CREQ:
                    RedemptionShare_CREQ(client, value as Dictionary<int, int>);
                    break;
                case FightCode.NextRound_CREQ:
                    PiratePart(client);
                    break;
                case FightCode.Pilot_CREQ:
                    Pilot_CREQ(client, value as List<int>);
                    break;
                case FightCode.PirateOnBoard_CREQ:
                    PirateOnBoard_CREQ(client, value as List<int>);
                    break;
                case FightCode.PiratePass_CREQ:
                    PiratePass_CREQ(client);
                    break;
                case FightCode.PirateChoose_CREQ:
                    PirateChoose_CREQ(client, value as List<int>);
                    break;
                case FightCode.InsurancePart_CREQ:
                    InsurancePart_CREQ(client, value as Dictionary<int, int>);
                    break;
                case FightCode.SettlementComplete_CREQ:
                    TransferOrGameOver(client);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 开始战斗，这个方法被委托到MatchHandler的startFight方法里面了，所以在本脚本中没有被调用
        /// </summary>
        /// <param name="clientList"></param>
        /// <param name="roomType"></param>
        public void StartFight(List<ClientPeer> clientList, int roomType)
        {
            SingleExecute.Instance.Execute(() =>
            {
                //要找这个room只能用userId从fightCache中找了
                FightRoom room = fightCache.CreateRoom(clientList);
                //给大家发牌
                room.DealCard();
                room.Broadcast(OpCode.Fight, FightCode.RefreshCard_BRO, MakeFightDto(room));
                //选择第一个开始的人
                ClientPeer firstPlayer = room.SetFirstPlayer();

                BidDto dto = room.BidDto;
                room.Broadcast(OpCode.Fight, FightCode.StartFight_BRO, dto);
            });
        }

        /// <summary>
        /// 客户端离开请求的处理
        /// </summary>
        /// <param name="client"></param>
        private void LeaveRoom(ClientPeer client)
        {
            SingleExecute.Instance.Execute(() =>
            {
                //不在战斗房间，忽略
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                room.Broadcast(OpCode.Fight, FightCode.Leave_BRO, client.Id);
                //TODO这里没写客户端部分，客户端所有人退到第二个界面
                fightCache.DestroyRoom(room);
            });
        }
        /// <summary>
        /// 制作传输信息载体
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        private FightDto MakeFightDto(FightRoom room)
        {
            FightDto dto = new FightDto();
            dto.playerList = room.PlayerList;
            dto.roundModelDto = room.RoundModel;
            dto.bankCardDic = room.CardLibrary.BankCardDic;
            dto.goodDto = room.GoodDto;
            dto.ShipDto = room.ShipManager.ShipDto;
            return dto;
        }
        /// <summary>
        /// 轮换处理
        /// </summary>
        /// <param name="client"></param>
        private void BidTurn(ClientPeer client)
        {
            if (fightCache.IsFighting(client.Id) == false) return;

            FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
            int nextId = room.Turn();

            if (room.BidDto.IsBidPass(nextId))
            {
                BidTurn(client);
            }
            else
            {
                room.Broadcast(OpCode.Fight, FightCode.Bidding_BRO, nextId);
            }
            //TODO 定时器任务
        }
        /// <summary>
        /// 竞价情趣
        /// </summary>
        /// <param name="client"></param>
        /// <param name="biddingprice"></param>
        private void Bidding_CREQ(ClientPeer client, int biddingprice)
        {
            SingleExecute.Instance.Execute(() =>
            {
                //不在战斗房间，忽略
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                BidDto dto = room.BidDto;
                Console.WriteLine("接收到报价：" + biddingprice);
                if (biddingprice == -1)
                {
                    //加入Pass阵营
                    dto.AddBidPass(client.Id);
                    if (dto.bidPassList.Count == 3 && dto.HighestBid != 0)
                    {
                        //结束竞价,之前价格最高的当船长
                        room.SetHarbourMaster(dto.HighestBidId);
                        //设置船老大为当前操作的玩家
                        room.SetPlayer(dto.HighestBidId);
                        room.Broadcast(OpCode.Fight, FightCode.RefreshMoney_BRO, room.PlayerList);
                        room.Broadcast(OpCode.Fight, FightCode.SetHarbourMaster_BRO, dto);
                    }
                    else
                    {
                        //转换下一个玩家操作
                        room.Broadcast(OpCode.Fight, FightCode.BiddingResult_BRO, dto);
                        BidTurn(client);
                    }
                }
                else
                {

                    //设置当前的最高价格，并播报出去
                    dto.HighestBidId = client.Id;
                    dto.HighestBid = biddingprice;
                    if (dto.bidPassList.Count == 3 && dto.HighestBid != 0)
                    {
                        //结束竞价,之前价格最高的当船长
                        room.SetHarbourMaster(dto.HighestBidId);
                        //设置船老大为当前操作的玩家
                        room.SetPlayer(dto.HighestBidId);
                        room.Broadcast(OpCode.Fight, FightCode.RefreshMoney_BRO, room.PlayerList);
                        room.Broadcast(OpCode.Fight, FightCode.SetHarbourMaster_BRO, dto);
                    }
                    else
                    {
                        room.Broadcast(OpCode.Fight, FightCode.BiddingResult_BRO, dto);
                        //转换下一位玩家操作
                        BidTurn(client);
                    }

                }

            });

        }
        /// <summary>
        /// 传输来了上船的货物
        /// </summary>
        /// <param name="client"></param>
        /// <param name="onBoardList"></param>
        private void OnBoard_CREQ(ClientPeer client, List<int> onBoardList)
        {
            SingleExecute.Instance.Execute(() =>
            {
                //不在战斗房间，忽略
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);

                room.ShipManager.GoodOnBoard(onBoardList);
                
                room.Broadcast(OpCode.Fight, FightCode.OnBoard_BRO, onBoardList);
            });
        }
        /// <summary>
        /// 广播给大家移动船只
        /// </summary>
        /// <param name="client"></param>
        /// <param name="moveShipList"></param>
        private void MoveShip_CREQ(ClientPeer client, List<int> moveShipList)
        {
            SingleExecute.Instance.Execute(() =>
            {
                //不在战斗房间，忽略
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                room.ShipManager.ShipDto.MoveShip(moveShipList);
                room.Broadcast(OpCode.Fight, FightCode.MoveShip_BRO, room.ShipManager.ShipDto);
            });
        }
        /// <summary>
        /// 客户端发来的放置一个工人的请求
        /// </summary>
        private void ClickPos_CREQ(ClientPeer client, List<int> PosShip)
        {
            SingleExecute.Instance.Execute(() =>
            {
                //不在战斗房间，忽略
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                PlayerDto playerDto = room.GetPlayerDto(client.Id);
                //减去占位花掉的钱
                playerDto.Money -= room.SiteLibrary.GetPosPrice(PosShip[0]);
                //修改玩家的工人记录
                Console.WriteLine("玩家" + client.UserName + "在" + PosShip[0] + "号位置上放置一个同伙");
                playerDto.SetWorker(PosShip[0], PosShip[1]);
       
                if(PosShip[0] <= 10)
                {
                    room.StaffAssignment.WorkerStatistics(client.Id, PosShip[0]);
                }

                room.Broadcast(OpCode.Fight, FightCode.SetWorker_BRO, room.PlayerList);
            });
        }
        /// <summary>
        /// 下一位放置同伙
        /// </summary>
        /// <param name="client"></param>
        private void EndOperation_CREQ(ClientPeer client)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                //轮到下一位玩家放工人
                SetWorkerTurn(client, room);
            });
        }

        /// <summary>
        /// 放置同伙的轮换方法
        /// </summary>
        /// <param name="client"></param>
        private void SetWorkerTurn(ClientPeer client, FightRoom room)
        {

            int nextId = room.Turn();
            //说明一轮结束了
            if (nextId == room.BidDto.HarbourMasterId)
            {
                PilotPart(client, room);
            }
            else
            {
                room.Broadcast(OpCode.Fight, FightCode.NextSetWorker_BRO, nextId);
            }
        }
        /// <summary>
        /// 投色子
        /// </summary>
        /// <param name="client"></param>
        private void CastDice(ClientPeer client)
        {
            FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
            List<int> diceList = room.DiceLibrary.GetDiceResult();
            //更新船的位置
            room.ShipManager.ShipDto.MoveShip(diceList);
            //传递骰子的结果
            room.Broadcast(OpCode.Fight, FightCode.CastDice_BRO, diceList);
        }
        /// <summary>
        /// 收到领航员玩家的操作
        /// </summary>
        private void Pilot_CREQ(ClientPeer client, List<int> moveList)
        {
            SingleExecute.Instance.Execute(() =>
            {
                //不在战斗房间，忽略
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                room.ShipManager.ShipDto.MoveShip(moveList);
                room.Broadcast(OpCode.Fight, FightCode.MoveShip_BRO, room.ShipManager.ShipDto);

                PilotPart(client, room);
            });
        }
        /// <summary>
        /// 领航员部分，被调用
        /// </summary>
        private void PilotPart(ClientPeer client, FightRoom room)
        {
            if (room.RoundModel.CurrentRound == 3)
            {
                if (room.StaffAssignment.SmallPilot != -1)
                {
                    room.Broadcast(OpCode.Fight, FightCode.SmallPilot_BRO, room.StaffAssignment.SmallPilot);
                    room.StaffAssignment.SmallPilot = -1;
                    return;
                }
                if (room.StaffAssignment.LargePilot != -1)
                {
                    room.Broadcast(OpCode.Fight, FightCode.LargePilot_BRO, room.StaffAssignment.LargePilot);
                    room.StaffAssignment.LargePilot = -1;
                    return;
                }
            }
            //投骰子
            CastDice(client);

        }
        /// <summary>
        /// 海盗部分
        /// </summary>
        private void PiratePart(ClientPeer client)
        {
            SingleExecute.Instance.Execute(() =>
            {
                //不在战斗房间，忽略
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                //回合2
                if (room.RoundModel.CurrentRound == 2)
                {
                    //有海盗
                    if (room.StaffAssignment.PirateCaptain != -1)
                    {
                        //13，有空位
                        if (room.Round2PirateJudge())
                        {
                            List<int> list = new List<int>();
                            list.Add(room.StaffAssignment.PirateCaptain);
                            foreach(var site in room.GetPirateOnBoardSite())
                            {
                                list.Add(site);
                            }
                            //走到这里说明海盗功能开启，传递海盗头头的userId和可以登船的位点们
                            room.Broadcast(OpCode.Fight, FightCode.Round2Pirate_BRO, list);
                            return;
                        }
                    }
                }
                //回合3
                if (room.RoundModel.CurrentRound == 3)
                {
                    //如果有停在13的船并且有海盗
                    if (room.Round3PirateJudge() && room.StaffAssignment.PirateCaptain != -1)
                    {
                        room.Plunder();



                        //刷新工人
                        room.Broadcast(OpCode.Fight, FightCode.SetWorker_BRO, room.PlayerList);
                        room.Broadcast(OpCode.Fight, FightCode.Round3Pirate_BRO, room.StaffAssignment.PirateCaptain);
                        return;
                    }
                    //没海盗的话如果有13就进港
                    room.ShipManager.OnPlace13ReachPort();
                    //结算
                    Settlement(room);
                    return;
                }
                //下一回合了
                NextRound(client);
                room.RoundModel.CurrentRound++;

            });

        }
        /// <summary>
        /// 可以开始下一个moveRound
        /// </summary>
        private void NextRound(ClientPeer client)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                room.Broadcast(OpCode.Fight, FightCode.NextSetWorker_BRO, room.BidDto.HarbourMasterId);
            });
        }
        /// <summary>
        /// 从客户端传来的海盗选择的登船位置
        /// </summary>
        /// <param name="client"></param>
        /// <param name="posCode"></param>
        private void PirateOnBoard_CREQ(ClientPeer client, List<int>PosShip)
        {
            SingleExecute.Instance.Execute(() =>
            {
                //不在战斗房间，忽略
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);

                //改一下当前海盗的位置
                PlayerDto playerDto1 = room.GetPlayerDto(client.Id);
                playerDto1.PirateOnBoard(PosShip[0], PosShip[1]);

                if (room.StaffAssignment.PirateAssociate != -1)
                {
                    //下一位海盗晋升
                    room.StaffAssignment.PirateCaptain = room.StaffAssignment.PirateAssociate;
                    room.StaffAssignment.PirateAssociate = -1;
                    PlayerDto playerDto2 = room.GetPlayerDto(room.StaffAssignment.PirateCaptain);
                    playerDto2.PiratePromote();
                }
                else
                {
                    room.StaffAssignment.PirateCaptain = -1;
                }
                //刷新同伙
                room.Broadcast(OpCode.Fight, FightCode.SetWorker_BRO, room.PlayerList);

            });
            PiratePart(client);
        }
        /// <summary>
        /// 海盗没有登船，点击了Pass，期待抢劫
        /// </summary>
        /// <param name="client"></param>
        private void PiratePass_CREQ(ClientPeer client)
        {
            SingleExecute.Instance.Execute(() =>
            {
                //不在战斗房间，忽略
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                room.RoundModel.NextRound();
                room.Broadcast(OpCode.Fight, FightCode.NextSetWorker_BRO, room.BidDto.HarbourMasterId);
            });
        }
        /// <summary>
        /// 抢劫完毕海盗船长的选择
        /// </summary>
        private void PirateChoose_CREQ(ClientPeer client, List<int> decideList)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                room.ShipManager.ShipDto.PirateChoose(decideList);
                room.Broadcast(OpCode.Fight, FightCode.MoveShip_BRO, room.ShipManager.ShipDto);

                Settlement(room);
            });

        }
        /// <summary>
        /// 结算
        /// </summary>
        private void Settlement(FightRoom room)
        {
            //剩下的进修理厂
            room.ShipManager.FixRemainShip();
            //更新船只信息
            room.Broadcast(OpCode.Fight, FightCode.MoveShip_BRO, room.ShipManager.ShipDto);
            //货物结算，包括船上的货物，
            Dictionary<int, int> userIdProfit = room.GoodSettlement();
            room.Broadcast(OpCode.Fight, FightCode.Settlement_BRO, userIdProfit);
            //发送结算结果
            room.Broadcast(OpCode.Fight, FightCode.RefreshMoney_BRO, room.PlayerList);
            room.Broadcast(OpCode.Fight, FightCode.RefreshCard_BRO, MakeFightDto(room));

            //自带发送消息了
            room.ValueRise();

        }

        private void TransferOrGameOver(ClientPeer client)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);

                //游戏结束
                if (room.IsEnd == true)
                {
                    room.GameOverSettlement();
                    int winPlayerId = room.WinPlayerId();
                    foreach (PlayerDto player in room.PlayerList)
                    {
                        if (player.UserId == winPlayerId)
                        {
                            DatabaseManager.Win(winPlayerId);
                        }
                        else
                        {
                            DatabaseManager.Lose(player.UserId);
                        }
                    }
                    room.Broadcast(OpCode.Fight, FightCode.RefreshMoney_BRO, room.PlayerList);
                    room.Broadcast(OpCode.Fight, FightCode.GameOver_BRO, winPlayerId);
                    //销毁房间
                    fightCache.DestroyRoom(room);

                    
                }
                //下一轮游戏开始
                else
                {
                    room.NewMoveRound();
                    room.Broadcast(OpCode.Fight, FightCode.NewMoveRound_BRO, MakeFightDto(room));

                    //选择第一个开始的人
                    ClientPeer firstPlayer = room.SetFirstPlayer();
                    BidDto dto = room.BidDto;
                    room.Broadcast(OpCode.Fight, FightCode.StartFight_BRO, dto);

                }
            });
        }

        /// <summary>
        /// 购买股票
        /// </summary>
        private void BuyShare_CREQ(ClientPeer client, Dictionary<int, int> buyCardDic)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                PlayerDto playerDto = room.GetPlayerDto(client.Id);

                //扣钱，广播刷新钱
                foreach(var goodCode in buyCardDic.Keys)
                {
                    if(buyCardDic[goodCode] != 0)
                        playerDto.SpendMoney(room.GoodDto.GetPrice(goodCode));
                }

                room.Broadcast(OpCode.Fight, FightCode.RefreshMoney_BRO, room.PlayerList);

                //买股票
                playerDto.AddCard(buyCardDic);
                room.CardLibrary.BuyCard(buyCardDic);
                room.Broadcast(OpCode.Fight, FightCode.RefreshCard_BRO, MakeFightDto(room));

            });
        }
        /// <summary>
        /// 抵押股票
        /// </summary>
        /// <param name="client"></param>
        /// <param name="mortgageCardDic"></param>
        private void MortgageShare_CREQ(ClientPeer client, Dictionary<int, int> mortgageCardDic)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                PlayerDto playerDto = room.GetPlayerDto(client.Id);

                //抵押股票
                playerDto.MortgageCard(mortgageCardDic);
                room.Broadcast(OpCode.Fight, FightCode.RefreshCard_BRO, MakeFightDto(room));

                //得到金钱,每张股票12块，广播刷新钱
                int num = 0;
                foreach (int i in mortgageCardDic.Values)
                {
                    num += i;
                }
                playerDto.GetMoney(12 * num);
                room.Broadcast(OpCode.Fight, FightCode.RefreshMoney_BRO, room.PlayerList);
            });
        }
        /// <summary>
        /// 赎回股票
        /// </summary>
        /// <param name="client"></param>
        /// <param name="redemptionCardDic"></param>
        private void RedemptionShare_CREQ(ClientPeer client, Dictionary<int, int> redemptionCardDic)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                PlayerDto playerDto = room.GetPlayerDto(client.Id);

                //抵押股票
                playerDto.RedemptionCard(redemptionCardDic);
                room.Broadcast(OpCode.Fight, FightCode.RefreshCard_BRO, MakeFightDto(room));

                //得到金钱,每张股票12块，广播刷新钱
                int num = 0;
                foreach (int i in redemptionCardDic.Values)
                {
                    num += i;
                }
                playerDto.SpendMoney(15 * num);
                room.Broadcast(OpCode.Fight, FightCode.RefreshMoney_BRO, room.PlayerList);
            });
        }
        /// <summary>
        /// 保险员抵押股票还账
        /// </summary>
        /// <param name="client"></param>
        /// <param name="mortgageCardDic"></param>
        private void InsurancePart_CREQ(ClientPeer client, Dictionary<int, int> mortgageCardDic)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (fightCache.IsFighting(client.Id) == false) return;

                FightRoom room = fightCache.GetFightRoomByUserId(client.Id);
                PlayerDto playerDto = room.GetPlayerDto(client.Id);

                //抵押股票
                playerDto.MortgageCard(mortgageCardDic);
                room.Broadcast(OpCode.Fight, FightCode.RefreshCard_BRO, MakeFightDto(room));
                room.Broadcast(OpCode.Fight, FightCode.SettlementShow_BRO, null);
            });
        }
    }
}
