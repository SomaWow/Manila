using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code.DTO
{
    /// <summary>
    /// 用户信息传输模型
    /// </summary>
    [Serializable]
    public class UserDto
    {
        public int UserId;
        public string UserName;
        public string IconName;
        public int Win;
        public int Lose;

        public UserDto(int UserId, string UserName, string IconName, int Win, int Lose)
        {
            this.UserId = UserId;
            this.UserName = UserName;
            this.IconName = IconName;
            this.Win = Win;
            this.Lose = Lose;
        }
        public void Change(int UserId, string UserName, string IconName, int Win, int Lose)
        {
            this.UserId = UserId;
            this.UserName = UserName;
            this.IconName = IconName;
            this.Win = Win;
            this.Lose = Lose;
        }
    }
}
