using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code.DTO.Fight
{   
    [Serializable]
    public class GoodDto
    {
        //每种货物棋子的落点，具体的价格索引ShareList，0-0,1-5,2-10,3-20,4-30
        public Dictionary<int, int> GoodShareDic;
        public List<int> ShareList;
        public List<int> HarbourMasterPriceList;
        

        public GoodDto()
        {
            GoodShareDic = new Dictionary<int, int>();
            GoodShareDic.Add(GoodCode.Doukou, 0);
            GoodShareDic.Add(GoodCode.Silk, 0);
            GoodShareDic.Add(GoodCode.Renshen, 0);
            GoodShareDic.Add(GoodCode.Yushi, 0);

            //固定值
            ShareList = new List<int> { 0, 5, 10, 20, 30};
            HarbourMasterPriceList = new List<int> { 5, 5, 10, 20, 30 };

        }

        public void Init()
        {

            //股价表回归到初始状态
            GoodShareDic.Clear();
            GoodShareDic.Add(GoodCode.Doukou, 0);
            GoodShareDic.Add(GoodCode.Silk, 0);
            GoodShareDic.Add(GoodCode.Renshen, 0);
            GoodShareDic.Add(GoodCode.Yushi, 0);
        }

        /// <summary>
        /// 货物涨价，本轮结束
        /// </summary>
        public void ValueRise(List<int> reachPortList)
        {
            foreach(var good in reachPortList)
            {
                this.GoodShareDic[good]++;
            }
        }
        /// <summary>
        /// 当有货物到达5的时候结束游戏
        /// </summary>
        public bool WhetherToEnd()
        {
            foreach(var value in GoodShareDic.Values)
            {
                if (value == 4)
                    return true;
            }
            return false;
        }

        public int SettlementPrice(int goodCode)
        {
            return ShareList[GoodShareDic[goodCode]];
        }

        public int HarbourMasterPrice(int goodCode)
        {
            return HarbourMasterPriceList[GoodShareDic[goodCode]];
        }

        /// <summary>
        /// 船长购买股票需要花的钱
        /// </summary>
        /// <param name="goodCode"></param>
        /// <returns></returns>
        public int GetPrice(int goodCode)
        {
            return HarbourMasterPriceList[GoodShareDic[goodCode]];
        }
    }
}
