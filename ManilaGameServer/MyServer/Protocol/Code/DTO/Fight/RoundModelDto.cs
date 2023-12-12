using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Cache.Fight
{
    /// <summary>
    /// 回合管理类
    /// </summary>
    [Serializable]
    public class RoundModelDto
    {
        /// <summary>
        /// 当前操作的玩家
        /// </summary>
        public int CurrentOperatingUserId { get; set; }
        /// <summary>
        /// 当前所在的回合，取值1，2，3
        /// </summary>
        public int CurrentRound { get; set; }


        public RoundModelDto()
        {
            CurrentRound = 1;
            CurrentOperatingUserId = -1;
        }
        public void Init()
        {
            CurrentRound = 1;
            CurrentOperatingUserId = -1;
        }
        /// <summary>
        /// 开始操作
        /// </summary>
        /// <param name="userId"></param>
        public void Start(int userId)
        {
            CurrentOperatingUserId = userId;
        }
        /// <summary>
        /// 轮换操作
        /// </summary>
        public void Turn(int userId)
        {
            CurrentOperatingUserId = userId;
        }

        public void NextRound()
        {
            if (CurrentRound == 3) CurrentRound = 1;
            else CurrentRound++;
        }
    }
}
