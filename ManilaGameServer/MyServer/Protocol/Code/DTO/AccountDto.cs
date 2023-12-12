using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code.DTO
{
    [Serializable] //加了标签才可以对类进行序列化和反序列化
    public class AccountDto
    {
        public string userName;
        public string password;

        public AccountDto(string userName, string password)
        {
            this.userName = userName;
            this.password = password;
        }
        public void Change(string userName, string password)
        {
            this.userName = userName;
            this.password = password;
        }
    }
}
