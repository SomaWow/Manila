using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Fight
{
    /// <summary>
    /// 给出投掷骰子的结果
    /// </summary>
    public class DiceLibrary
    {
        public List<int> GetDiceList;
        public Random ran;

        public DiceLibrary()
        {
            GetDiceList = new List<int>();
            ran = new Random();
        }

        public List<int> GetDiceResult()
        {
            GetDiceList.Clear();
            //Test
            /*
            GetDiceList.Add(1);
            GetDiceList.Add(1);
            GetDiceList.Add(3);
            */

            GetDiceList.Add(ran.Next(1, 7));
            GetDiceList.Add(ran.Next(1, 7));
            GetDiceList.Add(ran.Next(1, 7));

            return GetDiceList;
        }
    }
}
