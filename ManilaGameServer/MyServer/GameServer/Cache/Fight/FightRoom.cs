using MyServer;
using Protocol;
using Protocol.Code;
using Protocol.Code.DTO;
using Protocol.Code.DTO.Fight;
using Protocol.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Fight
{
    public class FightRoom
    {
        /// <summary>
        /// 房间ID,唯一标识符
        /// </summary>
        public int RoomId { get; set; }

        public bool IsEnd;
        /// <summary>
        /// 玩家列表
        /// </summary>
        public List<PlayerDto> PlayerList;
        /// <summary>
        /// 回合管理类
        /// </summary>
        public RoundModelDto RoundModel;
        /// <summary>
        /// 竞价
        /// </summary>
        public BidDto BidDto;
        /// <summary>
        /// 管理货物股价和上船的货物
        /// </summary>
        public GoodDto GoodDto;
        /// <summary>
        /// 管理船的信息
        /// </summary>
        public ShipManager ShipManager;
        public int PortShipNum;
        public int FixShipNum;
        /// <summary>
        /// 管理公共卡片和发牌
        /// </summary>
        public CardLibrary CardLibrary;
        /// <summary>
        /// 固定值，管理所有的位置
        /// </summary>
        public SiteLibrary SiteLibrary;
        /// <summary>
        /// 给出骰子结果
        /// </summary>
        public DiceLibrary DiceLibrary;
        /// <summary>
        /// 管理人员分配
        /// </summary>
        public StaffAssignment StaffAssignment;

        /// <summary>
        /// 构造方法，初始化玩家
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="clientList"></param>
        public FightRoom(int roomId, List<ClientPeer> clientList)
        {
            this.RoomId = roomId;
            IsEnd = false;

            PlayerList = new List<PlayerDto>();
            ShipManager = new ShipManager();
            Init(clientList);

            RoundModel = new RoundModelDto();
            BidDto = new BidDto();
            GoodDto = new GoodDto();
            CardLibrary = new CardLibrary();
            SiteLibrary = new SiteLibrary();
            DiceLibrary = new DiceLibrary();
            StaffAssignment = new StaffAssignment();

            PortShipNum = 0;
            FixShipNum = 0;
        }
        /// <summary>
        /// 起始
        /// </summary>
        /// <param name="clientList"></param>
        public void Init(List<ClientPeer> clientList)
        {
            IsEnd = false;

            PlayerList.Clear();
            List<int> tempList = new List<int>();
            //把玩家加到room的list中
            foreach (var client in clientList)
            {
                PlayerDto dto = new PlayerDto(client.Id, client.UserName);
                PlayerList.Add(dto);

                tempList.Add(client.Id);
            }

            ShipManager.Init();
        }
        /// <summary>
        /// 新的一次移动回合
        /// </summary>
        public void NewMoveRound()
        {
            foreach(var player in PlayerList)
            {
                player.Worker.Init();
            }
            RoundModel.Init() ;
            StaffAssignment.Init();
            ShipManager.Init();
            BidDto.Init();
        }
        /// <summary>
        /// 给出userId获得PlayerDto
        /// </summary>
        /// <returns></returns>
        public PlayerDto GetPlayerDto(int userId)
        {
            foreach (var player in PlayerList)
            {
                if (userId == player.UserId)
                    return player;
            }
            return null;
        }
        /// <summary>
        /// 花钱
        /// </summary>
        public void SpendMoney(int userId, int money)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList[i].UserId == userId)
                {
                    PlayerList[i].SpendMoney(money);
                }
            }
        }
        /// <summary>
        /// 收益
        /// </summary>
        public void GetMoney(int userId, int money)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList[i].UserId == userId)
                {
                    PlayerList[i].GetMoney(money);
                }

            }
        }
        /// <summary>
        /// 发牌
        /// </summary>
        public void DealCard()
        {
            foreach (var player in PlayerList)
            {
                //给每个人随机发两张牌
                player.AddCard(CardLibrary.DealCard());
            }
        }
        /// <summary>
        /// 广播发消息
        /// </summary>
        public void Broadcast(int opCode, int subCode, object value, ClientPeer exceptClient = null)
        {
            NetMsg msg = new NetMsg(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);
            foreach (var player in PlayerList)
            {
                //获取客户端连接对象
                ClientPeer client = Database.DatabaseManager.GetClientPeerByUserId(player.UserId);
                if (client == exceptClient)
                    continue;
                client.SendMsg(packet);
            }
        }
        /// <summary>
        /// 下次操作的玩家Id
        /// </summary>
        /// <returns></returns>
        public int Turn()
        {
            int currentUserId = RoundModel.CurrentOperatingUserId;
            int nextUserId = GetNextUserId(currentUserId);
            RoundModel.Turn(nextUserId);
            return nextUserId;
        }
        /// <summary>
        /// 获得下次操作的玩家Id
        /// </summary>
        /// <param name="currentId"></param>
        /// <returns></returns>
        private int GetNextUserId(int currentId)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList[i].UserId == currentId)
                {
                    //i = 3,到了最后一位玩家了
                    if (i == PlayerList.Count - 1)
                    {
                        return PlayerList[0].UserId;
                    }
                    else
                    {
                        return PlayerList[i + 1].UserId;
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// 随机选择第一个人
        /// </summary>
        public ClientPeer SetFirstPlayer()
        {
            Random ran = new Random();
            int ranIndex = ran.Next(0, PlayerList.Count);
            //int ranIndex = 0; //测试，都搞成从第一位开始
            //随机到的人的Id
            int userId = PlayerList[ranIndex].UserId;
            BidDto.FirstPlayerId = userId;
            //playerList[ranIndex].Identity = Identity.FirstPlayer;

            //设置这人为当前操作的人
            RoundModel.Start(userId);
            ClientPeer firstPlayer = Database.DatabaseManager.GetClientPeerByUserId(userId);
            string userName = firstPlayer.UserName;
            Console.WriteLine("第一个开始的玩家名是 " + userName);
            return firstPlayer;
        }
        /// <summary>
        /// 设置当前开始的玩家
        /// </summary>
        /// <returns></returns>
        public void SetPlayer(int userId)
        {
            RoundModel.Start(userId);
        }
        /// <summary>
        /// 设置船老大，并把他钱扣掉
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public void SetHarbourMaster(int userId)
        {
            BidDto.HarbourMasterId = userId;
            SpendMoney(userId, BidDto.HighestBid);

            /**
            ClientPeer harbourMasterPlayer = Database.DatabaseManager.GetClientPeerByUserId(userId);
            string userName = harbourMasterPlayer.UserName;
            Console.WriteLine("恭喜 " + userName + " 成为海港负责人");
            return harbourMasterPlayer;
    **/
        }
        /// <summary>
        /// round2条件：13 && 有空位  返回0，不能上，返回1，能上一个，返回2，能上两个
        /// </summary>
        public bool Round2PirateJudge()
        {
            if (ShipManager.IsThereVacantPosOn13()) return true;
            else return false;
        }
        /// <summary>
        /// 获得可让海盗登船的位点的list
        /// </summary>
        public List<int> GetPirateOnBoardSite()
        {
            return ShipManager.GetPirateOnBoardSite();
        }
        /// <summary>
        /// round3条件：13
        /// </summary>
        public bool Round3PirateJudge()
        {
            if (ShipManager.IsThereShipOn13()) return true;
            else return false;
        }
        /// <summary>
        /// 抢劫船只
        /// </summary>
        public void Plunder()
        {
            //把劫持的船加到List记录中去
            ShipManager.PlunderShip();


            
            //把被劫持船上的工人全部移走，并且船上人数记录清零
            for(int i = 0; i < PlayerList.Count; i++)
            {
                foreach(var plunderedShip in ShipManager.PlunderedShipList)
                {
                    PlayerList[i].RemoveOneShipWorker(plunderedShip);
                    ShipManager.WorkerNumList[plunderedShip] = 0;
                }
            }

            int ship = ShipManager.PlunderedShipList[0];
            int pos = ShipManager.GetSiteList(ship)[0];
            //海盗们选第一个货物上的第一和第二个空位坐上去
            if(StaffAssignment.PirateCaptain != -1)
            {
                ShipManager.WorkerNumList[ShipManager.PlunderedShipList[0]]++;
                if(StaffAssignment.PirateAssociate != -1)
                {
                    ShipManager.WorkerNumList[ShipManager.PlunderedShipList[0]]++;
                }
            }
            foreach (var player in PlayerList)
            {
                player.PiratePlunder(pos, ship);
            }

        }
        /// <summary>
        /// 结算，游戏结束的话返回true
        /// </summary>
        public Dictionary<int, int> GoodSettlement()
        {
            //货物结算，劫持判定
            Dictionary<int, int> shipProfit = ShipManager.LoadedWareProfit();

            Dictionary<int, int> userIdProfit = new Dictionary<int, int>();

            foreach(var player in PlayerList)
            {
                int income = 0;
                foreach (var workerShip in player.Worker.WorkerShipList)
                {
                    if (shipProfit.Keys.Contains(workerShip))
                    {
                        income += shipProfit[workerShip];
                    }
                }
                userIdProfit.Add(player.UserId, income);
            }
            //占位结算 0-5
            //入港和维修的船只
            for (int i = 0; i < PlayerList.Count; i++)
            {
                foreach (var site in PlayerList[i].Worker.WorkerSiteList)
                {
                    if (site >= 0 && site <= (ShipManager.ShipDto.PortShipList.Count - 1))
                    {
                        userIdProfit[PlayerList[i].UserId] += SiteLibrary.GetPosIncome(site);
                    }
                    if (site >= 3 && site <= (ShipManager.ShipDto.FixShipList.Count + 2))
                    {
                        userIdProfit[PlayerList[i].UserId] += SiteLibrary.GetPosIncome(site);
                    }
                }
            }
            //保险员结算，-1是userId，-2是需要选择抵押的股票数，-3是通知一下要把他的股票全抵押了
            int pay = 0;
            int supposeMoney = 0;
            int mortgageNum = 0;
            int tempMoney = 0;
            Console.WriteLine("StaffAssignment.InsuranceAgent = " + StaffAssignment.InsuranceAgent);
            if (StaffAssignment.InsuranceAgent != -1)
            {
                tempMoney = userIdProfit[StaffAssignment.InsuranceAgent];
                int fixShipNum = ShipManager.ShipDto.FixShipList.Count;
                //赔钱
                if (fixShipNum != 0)
                {
                    for (int i = 0; i < fixShipNum; i++)
                    {
                        //要赔钱的位置为3，4，5，所以从3开始
                        pay += SiteLibrary.GetPosIncome(i + 3);
                    }

                    PlayerDto player = GetPlayerDto(StaffAssignment.InsuranceAgent);

                    //不用卖股票，把钱扣掉
                    supposeMoney = player.Money + userIdProfit[StaffAssignment.InsuranceAgent];
                    if (supposeMoney >= pay)
                    {
                        userIdProfit[StaffAssignment.InsuranceAgent] -= pay;
                    }
                    //要卖股票
                    else
                    {
                        //卖股票知道能还钱或卖完股票
                        for (int i = 0; i < player.cardNum; i++)
                        {
                            mortgageNum++;
                            supposeMoney += 12;
                            if (supposeMoney >= pay)
                            {
                                break;
                            }
                        }
                        //卖了股票能还钱
                        if (supposeMoney >= pay)
                        {
                            //卖完了才能还上，不用选择
                            if (mortgageNum == player.cardNum)
                            {
                                userIdProfit[StaffAssignment.InsuranceAgent] = supposeMoney - player.Money - pay;
                                //卡片全部扣掉
                                player.MortgageAllCard();
                                userIdProfit.Add(-1, StaffAssignment.InsuranceAgent);
                                userIdProfit.Add(-3, 0);
                            }
                            else
                            {
                                //需要选择股票
                                userIdProfit[StaffAssignment.InsuranceAgent] = supposeMoney - player.Money - pay;
                                userIdProfit.Add(-1, StaffAssignment.InsuranceAgent);
                                userIdProfit.Add(-2, mortgageNum);
                            }
                        }
                        //卖完了也还不上
                        else
                        {
                            //钱全扣完，卡片全卖掉
                            userIdProfit[StaffAssignment.InsuranceAgent] = -player.Money;
                            player.MortgageAllCard();
                            userIdProfit.Add(-1, StaffAssignment.InsuranceAgent);
                            userIdProfit.Add(-3, 0);
                        }
                    }
                }
            }
            //获得的钱收入各位人的口袋
            foreach (int userId in userIdProfit.Keys)
            {
                for (int i = 0; i < PlayerList.Count; i++)
                {
                    if (PlayerList[i].UserId == userId)
                        PlayerList[i].Money += userIdProfit[userId];
                }
            }
            //更新完金钱后，改成要发给客户端的金钱
            if (StaffAssignment.InsuranceAgent != -1)
            {
                userIdProfit[StaffAssignment.InsuranceAgent] = tempMoney - pay;
            }
            return userIdProfit;
        }

        /// <summary>
        /// 货物涨价
        /// </summary>
        /// <returns></returns>
        public void ValueRise()
        {
            GoodDto.ValueRise(ShipManager.GetPortGood());
            Broadcast(OpCode.Fight, FightCode.ValueRise_BRO, GoodDto);
            IsEnd = GoodDto.WhetherToEnd();
        }
        /// <summary>
        /// 游戏结束的最终结算,把股票转成钱即可
        /// </summary>
        public void GameOverSettlement()
        {
            foreach(var player in PlayerList)
            {
                //抵押的股票
                player.SpendMoney(player.mortgageNum * 15);
                //拥有的股票
                foreach (var goodCode in player.cardDic.Keys)
                {
                    player.GetMoney(player.cardDic[goodCode] * GoodDto.SettlementPrice(goodCode));
                }
            }
        }
        /// <summary>
        /// 返回胜利的人的Id
        /// </summary>
        /// <returns></returns>
        public int WinPlayerId()
        {
            int userId = -1;
            int tempMoney = 0;
            for(int i = 0; i < PlayerList.Count; i++)
            {
                if(tempMoney < PlayerList[i].Money)
                {
                    userId = PlayerList[i].UserId;
                    tempMoney = PlayerList[i].Money;
                }
            }
            return userId;
        }
        /// <summary>
        /// 销毁房间，重置房间数据
        /// </summary>
        public void Destroy()
        {
            PlayerList.Clear();
            RoundModel.Init();
            BidDto.Init();
            CardLibrary.Init();
            GoodDto.Init();
            PortShipNum = 0;
            FixShipNum = 0;
            StaffAssignment.Init();

        }
    }
}
