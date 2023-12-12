using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code.DTO
{
    /// <summary>
    /// 匹配房间传输模型
    /// </summary>
    [Serializable]
    public class MatchRoomDto
    {
        /// <summary>
        /// 用户ID与该用户UserDto之间的映射字典
        /// </summary>
        public Dictionary<int, UserDto> userIdUserDtoDic { get; private set; }
        /// <summary>
        /// 准备的玩家ID
        /// </summary>
        public List<int> readyUserIdList { get; set; }
        /// <summary>
        /// 进入房间顺序的用户ID列表
        /// </summary>
        public List<int> enterOrderList { get; private set; }
        
        public MatchRoomDto()
        {
            userIdUserDtoDic = new Dictionary<int, UserDto>();
            readyUserIdList = new List<int>();
            enterOrderList = new List<int>();
        }
        /// <summary>
        /// 进入房间的方法，在客户端调用
        /// </summary>
        /// <param name="dto"></param>
        public void Enter(UserDto dto)
        {
            userIdUserDtoDic.Add(dto.UserId, dto);
            enterOrderList.Add(dto.UserId);
        }
        /// <summary>
        /// 离开房间的方法，在客户端调用
        /// </summary>
        /// <param name="dto"></param>
        public void Leave(int userId)
        {
            userIdUserDtoDic.Remove(userId);
            readyUserIdList.Remove(userId);
            enterOrderList.Remove(userId);
        }
        /// <summary>
        /// 准备
        /// </summary>
        /// <param name="userId"></param>
        public void Ready(int userId)
        {
            readyUserIdList.Add(userId);
        }
        /// <summary>
        /// 取消准备
        /// </summary>
        /// <param name="userId"></param>
        public void UnReady(int userId)
        {
            readyUserIdList.Remove(userId);
        }
        
    }
}
