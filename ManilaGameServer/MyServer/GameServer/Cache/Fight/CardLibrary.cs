using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code.DTO.Fight
{
    /// <summary>
    /// 这个类的工作就是，游戏开始的时候给大家发牌
    /// </summary>
    public class CardLibrary
    {
        public Dictionary<int, int> BankCardDic { get; set; }
        public Dictionary<int, int> GetCardDic { get; set; }
        /// <summary>
        /// 抽卡池子
        /// </summary>
        private List<int> Pool;
        private List<int> SparePool;
 
        public CardLibrary()
        {
            BankCardDic = new Dictionary<int, int>();
            BankCardDic.Add(GoodCode.Doukou, 5);
            BankCardDic.Add(GoodCode.Silk, 5);
            BankCardDic.Add(GoodCode.Renshen, 5);
            BankCardDic.Add(GoodCode.Yushi, 5);

            GetCardDic = new Dictionary<int, int>();
            GetCardDic.Add(GoodCode.Doukou, 0);
            GetCardDic.Add(GoodCode.Silk, 0);
            GetCardDic.Add(GoodCode.Renshen, 0);
            GetCardDic.Add(GoodCode.Yushi, 0);

            //每种股票抽取3张
            Pool = new List<int> { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };
            SparePool = new List<int> { 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3 };
        }

        public void Init()
        {
            BankCardDic.Clear();
            BankCardDic.Add(GoodCode.Doukou, 5);
            BankCardDic.Add(GoodCode.Silk, 5);
            BankCardDic.Add(GoodCode.Renshen, 5);
            BankCardDic.Add(GoodCode.Yushi, 5);

            GetCardDic.Clear();
            GetCardDic.Add(GoodCode.Doukou, 0);
            GetCardDic.Add(GoodCode.Silk, 0);
            GetCardDic.Add(GoodCode.Renshen, 0);
            GetCardDic.Add(GoodCode.Yushi, 0);

            Pool.Clear();
            SparePool.ForEach(i=>Pool.Add(i));

        }

        public Dictionary<int, int> DealCard()
        {
            //清空要给出的卡牌
            int[] keyArr = GetCardDic.Keys.ToArray<int>();
            for(int i = 0; i < keyArr.Length; i++)
            {
                GetCardDic[keyArr[i]] = 0;
            }

            Random ran = new Random();//随机范围包括min，不包括max
            //第一次抽牌
            int ranIndex = ran.Next(0, Pool.Count);
            int card = Pool[ranIndex];
            Pool.RemoveAt(ranIndex);
            GetCardDic[card]++;
            BankCardDic[card]--;
            //第二次抽牌
            ranIndex = ran.Next(0, Pool.Count);
            card = Pool[ranIndex];
            Pool.RemoveAt(ranIndex);
            GetCardDic[card]++;
            BankCardDic[card]--;

            return GetCardDic;
        }
        /// <summary>
        /// 从银行中去掉买的卡片
        /// </summary>
        /// <param name="buyCardDic"></param>
        public void BuyCard(Dictionary<int, int> buyCardDic)
        {
            for(int i = 0; i < 4; i++)
            {
                BankCardDic[i] -= buyCardDic[i];
            }
        }
    }
}
