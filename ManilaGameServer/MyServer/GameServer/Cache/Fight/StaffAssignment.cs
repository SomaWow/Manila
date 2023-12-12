using Protocol.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Fight
{
    public class StaffAssignment
    {
        //海盗头子
        public int PirateCaptain { get; set; }
        //海盗同伙
        public int PirateAssociate { get; set; }
        //小领航员
        public int SmallPilot { get; set; }
        //大领航员
        public int LargePilot { get; set; }
        //保险代理
        public int InsuranceAgent { get; set; }

        /// <summary>
        /// 初始化，传进来userIdList
        /// </summary>
        /// <param name="userIdList"></param>
        public StaffAssignment()
        {
            PirateCaptain = -1;
            PirateAssociate = -1;
            SmallPilot = -1;
            LargePilot = -1;
            InsuranceAgent = -1;


        }

        public void Init()
        {
            PirateCaptain = -1;
            PirateAssociate = -1;
            SmallPilot = -1;
            LargePilot = -1;
            InsuranceAgent = -1;


        }
        /// <summary>
        /// 人员统计
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pos"></param>
        public void WorkerStatistics(int userId, int pos)
        {
            if (pos == 6)
                PirateCaptain = userId;
            else if (pos == 7)
                PirateAssociate = userId;
            else if (pos == 8)
                SmallPilot = userId;
            else if (pos == 9)
                LargePilot = userId;
            else if (pos == 10)
            {
                Console.WriteLine("收到了保险员的站位");
                InsuranceAgent = userId;
            }
        }

    }
}
