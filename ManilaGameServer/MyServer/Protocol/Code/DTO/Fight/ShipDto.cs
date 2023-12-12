using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code.DTO.Fight
{
    [Serializable]
    public class ShipDto
    {
        //船的位置，0-13代表船在航行，14代表入港，15代表进入修理厂
        public List<int> ShipSiteList;
        public List<int> OnBoardGoodList;

        public List<int> PortShipList;
        public List<int> FixShipList;

        public ShipDto()
        {
            ShipSiteList = new List<int>() { 0, 0, 0 };
            OnBoardGoodList = new List<int>() { -1, -1, -1};

            PortShipList = new List<int>();
            FixShipList = new List<int>();
        }
        public void GoodOnBoard(List<int> goodCodeList)
        {
            for (int i = 0; i < goodCodeList.Count; i++)
            {
                OnBoardGoodList[i] = goodCodeList[i];
            }
        }
        /// <summary>
        /// 移动船只
        /// </summary>
        /// <param name="moveList"></param>
        public void MoveShip(List<int> moveList)
        {
            for (int i = 0; i < moveList.Count; i++)
            {
                //已经入港忽略
                if (PortShipList.Contains(i))
                    continue;
                ShipSiteList[i] += moveList[i];
                if (ShipSiteList[i] > 13)
                {
                    ShipSiteList[i] = ShipSiteCode.Port;  //ShipSiteCode.Port = 14
                    PortShipList.Add(i); //把船的代号加到到港的船只List里面
                }

            }
        }
        /// <summary>
        /// 船长的选择，里面包括14为入港，15为进入修船厂，0为不做移动
        /// </summary>
        /// <param name="moveList"></param>
        public void PirateChoose(List<int> decideList)
        {
            for(int i = 0; i < decideList.Count; i++)
            {
                if (decideList[i] == 0)
                    continue;
                else if(decideList[i] == ShipSiteCode.Port)
                {
                    ShipSiteList[i] = decideList[i];
                    PortShipList.Add(i);
                }
                else if(decideList[i] == ShipSiteCode.Fix)
                {
                    ShipSiteList[i] = decideList[i];
                    FixShipList.Add(i);
                }
            }
        }

        public List<int> GetShipOn13()
        {
            List<int> shipOn13List = new List<int>();
            for(int i = 0; i < ShipSiteList.Count; i++)
            {
                if(ShipSiteList[i] == 13)
                {
                    shipOn13List.Add(i);
                }
            }
            return shipOn13List;
        }
    }
}
