using GameServer.Cache.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache
{
    /// <summary>
    /// 数据缓存单一模式
    /// </summary>
    public class Caches
    {
        public static MatchCache matchCache { get; set; }
        public static FightCache fightCache { get; set; }

        static Caches()
        {
            matchCache = new MatchCache();
            fightCache = new FightCache();
        }
    }
}
