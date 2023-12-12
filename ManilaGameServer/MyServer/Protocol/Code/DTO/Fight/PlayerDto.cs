using Protocol.Code.DTO.Fight;
using Protocol.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code.DTO
{
    /// <summary>
    /// 玩家传输模型
    /// </summary>
    [Serializable]
    public class PlayerDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int Money { get; set; }
        public Dictionary<int, int> cardDic; //自己的股票牌
        public int cardNum;
        public Dictionary<int, int> mortgageCardDic;
        public int mortgageNum;
        public WorkerDto Worker { get; set; } //工人

        public Dictionary<int, int> PosGoodDic = new Dictionary<int, int>() { { 11, 0 }, { 12, 0 }, { 13, 0 }, { 14, 1 }, { 15, 1 }, { 16, 1 }, { 17, 2 }, { 18, 2 }, { 19, 2 }, { 20, 3 }, { 21, 3 }, { 22, 3 }, { 23, 3 } };

        
            
        public PlayerDto(int userId, string userName)
        {
            this.UserId = userId;
            this.UserName = userName;
            Money = 30;
            cardNum = 0;
            mortgageNum = 0;
            //自己拥有的股票牌
            cardDic = new Dictionary<int, int>();
            cardDic.Add(GoodCode.Doukou, 0);
            cardDic.Add(GoodCode.Silk, 0);
            cardDic.Add(GoodCode.Renshen, 0);
            cardDic.Add(GoodCode.Yushi, 0);
            //用于抵押的股票牌
            mortgageCardDic = new Dictionary<int, int>();
            mortgageCardDic.Add(GoodCode.Doukou, 0);
            mortgageCardDic.Add(GoodCode.Silk, 0);
            mortgageCardDic.Add(GoodCode.Renshen, 0);
            mortgageCardDic.Add(GoodCode.Yushi, 0);

            Worker = new WorkerDto();
        }
        /// <summary>
        /// 得到牌，重载方法，好像不会失去牌，所以没有失去牌的方法
        /// </summary>
        //得到一张牌
        public void AddCard(int goodCode)
        {
            this.cardDic[goodCode]++;
            cardNum++;
        }
        //所有牌的变化
        public void AddCard(Dictionary<int, int> addCardDic)
        {
            foreach(var key in addCardDic.Keys)
            {
                this.cardDic[key] += addCardDic[key];
                cardNum += addCardDic[key];
            }
        }
        /// <summary>
        /// 抵押牌，重载方法
        /// </summary>
        public void MortgageCard(int goodCode)
        {
            mortgageCardDic[goodCode] += 1;
            mortgageNum++;
            cardDic[goodCode] -= 1;
            cardNum--;
        }
        public void MortgageCard(Dictionary<int, int> addcardDic)
        {
            foreach (var key in addcardDic.Keys)
            {
                this.mortgageCardDic[key] += addcardDic[key];
                this.mortgageNum += addcardDic[key];
                this.cardDic[key] -= addcardDic[key];
                this.cardNum -= addcardDic[key];
            }
        }
        //卡片全部扣掉，用于抵债
        public void MortgageAllCard()
        {
            for (int i = 0; i < 4; i++)
            {
                mortgageCardDic[i] += cardDic[i];
                cardDic[i] = 0;
            }
            mortgageNum += cardNum;
            cardNum = 0;
        }

        /// <summary>
        /// 赎回牌，重载方法
        /// </summary>
        public void RedemptionCard(int goodCode)
        {
            mortgageCardDic[goodCode] -= 1;
            mortgageNum--;
            cardDic[goodCode] += 1;
            cardNum++;
        }
        public void RedemptionCard(Dictionary<int, int> addcardDic)
        {
            foreach (var key in addcardDic.Keys)
            {
                this.mortgageCardDic[key] -= addcardDic[key];
                this.mortgageNum -= addcardDic[key];
                this.cardDic[key] += addcardDic[key];
                this.cardNum += addcardDic[key];
            }
        }

        /// <summary>
        /// 放置一个工人
        /// </summary>
        /// <param name="dto"></param>
        public void SetWorker(int position, int ship)
        {
            Worker.Work(position, ship);
        }
        /// <summary>
        /// 海盗登船
        /// </summary>
        public void PirateOnBoard(int position, int ship)
        {
            Worker.PirateOnBoard(position, ship);
        }
        /// <summary>
        /// 海盗晋升
        /// </summary>
        public void PiratePromote()
        {
            Worker.PiratePromote();
        }
        /// <summary>
        /// 重置所有工人位置
        /// </summary>
        /// <param name="dto"></param>
        public void ReSetWorker()
        {
            Worker.Init();
        }
        /// <summary>
        /// 花钱
        /// </summary>
        public void SpendMoney(int mon)
        {
            Money -= mon;
            if (Money < 0) Money = 0;
        }
        /// <summary>
        /// 收益
        /// </summary>
        public void GetMoney(int mon)
        {
            Money += mon;
            if (Money < 0) Money = 0;
        }

        public void RemoveOneShipWorker(int ship)
        {
            Worker.RemoveOneShipWorker(ship);
        }
        /// <summary>
        /// 海盗上船（抢劫）
        /// </summary>
        /// <param name="goodCode"></param>
        public void PiratePlunder(int pos, int ship)
        {
            Worker.PiratePlunder(pos, ship);
        }
    }
}
