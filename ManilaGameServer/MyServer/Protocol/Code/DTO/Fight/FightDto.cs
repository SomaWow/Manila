using GameServer.Cache.Fight;
using Protocol.Code.DTO.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code.DTO
{
    /// <summary>
    /// 用来传递战况
    /// </summary>
    [Serializable]
    public class FightDto
    {
        /// <summary>
        /// 玩家信息
        /// </summary>
        public List<PlayerDto> playerList;
        /// <summary>
        /// 回合信息
        /// </summary>
        public RoundModelDto roundModelDto;
        /// <summary>
        /// 公共股票库
        /// </summary>
        public Dictionary<int, int> bankCardDic;
        /// <summary>
        /// 管理货物股价和上船的货物
        /// </summary>
        public GoodDto goodDto;
        public ShipDto ShipDto;




        public FightDto()
        {
            playerList = new List<PlayerDto>();
            roundModelDto = new RoundModelDto();
            bankCardDic = new Dictionary<int, int>();
            goodDto = new GoodDto();
            ShipDto = new ShipDto();
        }
 
        /// <summary>
        /// 根据userId获得PlayerDto
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PlayerDto GetPlayerDto(int userId) {
            for(int i = 0; i < playerList.Count; i++)
            {
                if (userId == playerList[i].UserId)
                    return playerList[i];
            }
            return null;
        }
        
    }

}
