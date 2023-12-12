using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code.DTO.Fight
{
    [Serializable]
    public class ShipManager
    {

        public ShipDto ShipDto;
        
        /// <summary>
        /// 被抢劫的船号
        /// </summary>
        public List<int> PlunderedShipList;

        //货物的价值
        public List<int> GoodIncomeList;
        //船容量
        public List<int> LocationNumList;
        //乘客数
        public List<int> WorkerNumList;
        //每艘船结算
        public Dictionary<int, int> ShipProfitDic;

        //固定值，每种货物对应的空位
        public Dictionary<int, int> GoodPosNumDic = new Dictionary<int, int>() { { 0, 3 }, { 1, 3 }, { 2, 3 }, { 3, 4 } };

        public ShipManager()
        {
            ShipDto = new ShipDto();

            PlunderedShipList = new List<int>();

            GoodIncomeList = new List<int>() { 0, 0, 0 };
            LocationNumList = new List<int>() { 0, 0, 0 };
            WorkerNumList = new List<int>() { 0, 0, 0 };

            ShipProfitDic = new Dictionary<int, int>();
        }

        public void Init()
        {
            ShipDto = new ShipDto();

            PlunderedShipList.Clear();

            GoodIncomeList = new List<int>() { 0, 0, 0 };
            LocationNumList = new List<int>() { 0, 0, 0 };
            WorkerNumList = new List<int>() { 0, 0, 0 };

            ShipProfitDic = new Dictionary<int, int>();
        }
        /// <summary>
        /// 货物上船
        /// </summary>
        /// <param name="goodCode"></param>
        public void GoodOnBoard(List<int> goodCodeList)
        {
            ShipDto.GoodOnBoard(goodCodeList);

            for (int i = 0; i < goodCodeList.Count; i++)
            {
                //每只船上的位置
                LocationNumList[i] = GoodPosNumDic[i];
                //每种货物的总价值
                switch (goodCodeList[i])
                {
                    case 0:
                        GoodIncomeList[i] = 24;
                        break;
                    case 1:
                        GoodIncomeList[i] = 30;
                        break;
                    case 2:
                        GoodIncomeList[i] = 18;
                        break;
                    case 3:
                        GoodIncomeList[i] = 36;
                        break;
                    default:
                        GoodIncomeList[i] = 0;
                        break;
                }
            }

        }
        /// <summary>
        /// 人员上船
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pos"></param>
        public void WorkerOnBoard(int shipCode)
        {
            WorkerNumList[shipCode]++;
        }
        /// <summary>
        ///根据获得的船的list获得货物的list
        /// </summary>
        public List<int> GetGoodCodeList(List<int> shipList)
        {
            List<int> goodCodeList = new List<int>();
            for (int i = 0; i < shipList.Count; i++)
            {
                goodCodeList.Add(ShipDto.OnBoardGoodList[shipList[i]]);
            }
            return goodCodeList;
        }
        /// <summary>
        /// 成功进港
        /// </summary>
        public void ReachPort(int shipCode)
        {
            ShipDto.ShipSiteList[shipCode] = ShipSiteCode.Port;
            ShipDto.PortShipList.Add(shipCode);
        }
        /// <summary>
        /// 失败维修
        /// </summary>
        /// <param name="index"></param>
        public void FailReach(int shipCode)
        {
            ShipDto.ShipSiteList[shipCode] = ShipSiteCode.Fix;
            ShipDto.FixShipList.Add(shipCode);
        }

        /// <summary>
        /// 剩下的船加到修理厂里
        /// </summary>
        public void FixRemainShip()
        {
            for(int i = 0; i < ShipDto.ShipSiteList.Count; i++)
            {
                if(ShipDto.ShipSiteList[i] < 13) 
                {
                    ShipDto.ShipSiteList[i] = ShipSiteCode.Fix;
                    ShipDto.FixShipList.Add(i); //把船的代号加到修理厂船只List里面
                }
            }
        }
        /// <summary>
        /// 返回的是货物的codelist
        /// </summary>
        /// <returns></returns>
        public bool IsThereShipOn13()
        {
            for(int i = 0; i < ShipDto.ShipSiteList.Count; i++)
            {
                if(ShipDto.ShipSiteList[i] == 13)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获得13上的空位
        /// </summary>
        /// <returns></returns>
        public bool IsThereVacantPosOn13()
        {
            for(int i = 0; i < 3; i++)
            {
                if(ShipDto.ShipSiteList[i] == 13 && GetVacantPosNum(i) > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获得空位置的数量
        /// </summary>
        /// <returns></returns>
        public int GetVacantPosNum(int shipCode)
        {
            return LocationNumList[shipCode] - WorkerNumList[shipCode];
        }
        /// <summary>
        /// 抢劫船只
        /// </summary>
        public void PlunderShip()
        {
            for (int i = 0; i < ShipDto.ShipSiteList.Count; i++)
            {
                if (ShipDto.ShipSiteList[i] == 13)
                {
                    PlunderedShipList.Add(i);
                }
            }
        }
        /// <summary>
        /// 海盗可登船的位点，有没有人在客户端考虑
        /// </summary>
        public List<int> GetPirateOnBoardSite()
        {
            List<int> onBoardSiteList = new List<int>();

            for (int i = 0; i < ShipDto.ShipSiteList.Count; i++)
            {
                if (ShipDto.ShipSiteList[i] == 13)
                {
                    foreach (var site in GetSiteList(i))
                    {
                        onBoardSiteList.Add(site);
                    }
                }
            }
            return onBoardSiteList;
        }
                /*
        /// <summary>
        /// 获得13位置上的空位置
        /// </summary>
        /// <returns></returns>
        public List<int> PirateOnBoardSite()
        {
            List<int> list = new List<int>();
            for(int i = 0; i < ShipDto.ShipSiteList.Count; i++)
            {
                if(ShipDto.ShipSiteList[i] == 13)
                {
                    int index = WorkerNumList[i];
                    for(int j = 0; j < (LocationNumList[i]-WorkerNumList[i]); j++)
                    {
                        list.Add(GetSiteList(i)[index]);
                        index++;
                    }
                }
            }
            return list;
        }
        */
        /// <summary>
        /// 获得这艘船上的位置代号
        /// </summary>
        /// <returns></returns>
        public List<int> GetSiteList(int shipCode)
        {
            switch (ShipDto.OnBoardGoodList[shipCode])
            {
                case 0:
                    return new List<int>() { 11, 12, 13 };
                case 1:
                    return new List<int>() { 14, 15, 16 };
                case 2:
                    return new List<int>() { 17, 18, 19 };
                case 3:
                    return new List<int>() { 20, 21, 22, 23 };
                default:
                    return null;
            }
        }
        /// <summary>
        /// 让位于13的船进港，用于第三轮没有海盗的情况
        /// </summary>
        public void OnPlace13ReachPort()
        {
            for (int i = 0; i < ShipDto.ShipSiteList.Count; i++)
            {
                if (ShipDto.ShipSiteList[i] == 13)
                {
                    ShipDto.ShipSiteList[i] = ShipSiteCode.Port;
                }
            }
        }

        /// <summary>
        /// 到港收益
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, int> LoadedWareProfit()
        {
            //货物收益
            foreach(int ship in ShipDto.PortShipList)
            {
                //如果被海盗劫持了不算收益
                if (PlunderedShipList.Contains(ship)) continue;

                ShipProfitDic.Add(ship, GetOneIncome(ship));
            }

            //海盗收益
            if(PlunderedShipList.Count > 0)
            {
                int goodSum = 0;
                int pirateSum = 0;

                foreach (var ship in PlunderedShipList)
                {
                    goodSum += GetAllIncome(ship);
                    pirateSum += WorkerNumList[ship];
                }
                ShipProfitDic.Add(PlunderedShipList[0], goodSum / pirateSum);
            }
            return ShipProfitDic;
        }
        /// <summary>
        /// 货物结算的时候一份收益的量
        /// </summary>
        /// <returns></returns>
        public int GetOneIncome(int shipCode)
        {
            if (WorkerNumList[shipCode] == 0)
                return GoodIncomeList[shipCode];
            else
                return GoodIncomeList[shipCode] / WorkerNumList[shipCode];
        }
        /// <summary>
        /// 货物总价值
        /// </summary>
        /// <returns></returns>
        public int GetAllIncome(int shipCode)
        {
            return GoodIncomeList[shipCode];
        }
        /// <summary>
        /// 获得到港货物
        /// </summary>
        /// <returns></returns>
        public List<int> GetPortGood()
        {
            List<int> goodList = new List<int>();
            foreach(var shipCode in ShipDto.PortShipList)
            {
                goodList.Add(ShipDto.OnBoardGoodList[shipCode]);
            }
            return goodList;
        }



        public void GetShipByPos(int pos)
        {

        }
    }
}
