using Protocol.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code.DTO.Fight
{
    [Serializable]
    public class WorkerDto
    {
        /// <summary>
        /// 目前放置到第几位工人
        /// </summary>
        public int WorkerIndex;
        /// <summary>
        /// 使用的工人的站位，站位用SiteCode
        /// </summary>
        public List<int> WorkerSiteList;
        /// <summary>
        /// 工人所在的船
        /// </summary>
        public List<int> WorkerShipList;

        public WorkerDto()
        {
            WorkerIndex = 0;
            WorkerSiteList = new List<int>() { SiteCode.InitPos, SiteCode.InitPos, SiteCode.InitPos };
            WorkerShipList = new List<int>() { ShipCode.NonShip, ShipCode.NonShip, ShipCode.NonShip };
        }
        //回到初始状态
        public void Init()
        {
            WorkerIndex = 0;
            WorkerSiteList = new List<int>() { SiteCode.InitPos, SiteCode.InitPos, SiteCode.InitPos };
            WorkerShipList = new List<int>() { ShipCode.NonShip, ShipCode.NonShip, ShipCode.NonShip };

        }
        /// <summary>
        /// 同伙干活
        /// </summary>
        /// <param name="site"></param>
        public void Work(int site, int ship)
        {
            WorkerSiteList[WorkerIndex] = site;
            WorkerShipList[WorkerIndex] = ship;
            WorkerIndex++;
        }
        /// <summary>
        /// 海盗登船
        /// </summary>
        public void PirateOnBoard(int pos, int ship) {
            for(int i = 0; i < WorkerSiteList.Count; i++)
            {
                if(WorkerSiteList[i] == SiteCode.Pirate1)
                {
                    WorkerSiteList[i] = pos;
                    WorkerShipList[i] = ship;
                }
            }
        }
        /// <summary>
        /// 海盗晋升
        /// </summary>
        public void PiratePromote()
        {
            for (int i = 0; i < WorkerSiteList.Count; i++)
            {
                if (WorkerSiteList[i] == SiteCode.Pirate2)
                {
                    WorkerSiteList[i] = SiteCode.Pirate1;
                }
            }
        }
        /// <summary>
        /// 如果某个工人的位置范围在这个之间，就把他改成24，初始位置
        /// </summary>
        /// <param name="goodCode"></param>
        public void RemoveOneShipWorker(int ship)
        {
            for(int i = 0; i < WorkerShipList.Count; i++)
            {
                if(WorkerShipList[i] == ship)
                {
                    WorkerSiteList[i] = SiteCode.InitPos;
                    WorkerShipList[i] = ShipCode.NonShip;
                    WorkerIndex--;
                }
            }
        }

        //把海盗头子放到第一个位置，同伙放到第二个位置
        public void PiratePlunder(int pos, int ship)
        {
            for(int i=0; i < WorkerSiteList.Count; i++)
            {
                if(WorkerSiteList[i] == 6)
                {
                    WorkerSiteList[i] = pos;
                    WorkerShipList[i] = ship;
                }
                else if (WorkerSiteList[i] == 7)
                {
                    WorkerSiteList[i] = pos + 1;
                    WorkerShipList[i] = ship;
                }
            }
        }

        
    }
}
