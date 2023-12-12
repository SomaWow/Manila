using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code
{
    public class AccountCode
    {
        public const int Register_CREQ = 0;
        public const int Register_SRES = 1;
        public const int Login_CREQ = 2;
        public const int Login_SRES = 3;
        public const int GetUserInfo_CREQ = 4;
        public const int GetUserInfo_SRES = 5;
        public const int ChooseHeadIcon_CREQ = 6;
    }
}
