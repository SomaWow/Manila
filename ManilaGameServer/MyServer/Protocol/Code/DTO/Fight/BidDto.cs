using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code.DTO
{
    [Serializable]
    public class BidDto
    {
        public int FirstPlayerId { get; set; }
        public int HighestBidId { get; set; }
        public int HighestBid { get; set; }
        public List<int> bidPassList { get; set; }
        public int HarbourMasterId { get; set; }

        public BidDto()
        {
            FirstPlayerId = -1;
            HighestBid = 0;
            HighestBidId = -1;
            HarbourMasterId = -1;
            bidPassList = new List<int>();
        }
        public void Init()
        {
            FirstPlayerId = -1;
            HighestBid = 0;
            HighestBidId = -1;
            HarbourMasterId = -1;
            bidPassList.Clear();
        }
        /// <summary>
        /// 是否已经Pass竞价
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsBidPass(int userId)
        {
            return bidPassList.Contains(userId);
        }
        /// <summary>
        /// 竞价的时候Pass
        /// </summary>
        /// <param name="userId"></param>
        public void AddBidPass(int userId)
        {
            bidPassList.Add(userId);
        }
    }
}
