using Protocol.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol
{
    public class SiteLibrary
    {
        private int Price;
        private int Income;

        public SiteLibrary()
        {
            Price = 0;
            Income = 0;
        }

        public int GetPosPrice(int siteCode)
        {
            switch (siteCode)
            {
                case SiteCode.Port1:
                    Price = 4;
                    break;
                case SiteCode.Port2:
                    Price = 3;
                    break;
                case SiteCode.Port3:
                    Price = 2;
                    break;
                case SiteCode.Fix1:
                    Price = 4;
                    break;
                case SiteCode.Fix2:
                    Price = 3;
                    break;
                case SiteCode.Fix3:
                    Price = 2;
                    break;
                case SiteCode.Pirate1:
                    Price = 5;
                    break;
                case SiteCode.Pirate2:
                    Price = 5;
                    break;
                case SiteCode.pilot2:
                    Price = 2;
                    break;
                case SiteCode.pilot5:
                    Price = 5;
                    break;
                case SiteCode.Insurance:
                    Price = -10;
                    break;
                case SiteCode.Doukou1:
                    Price = 2;
                    break;
                case SiteCode.Doukou2:
                    Price = 3;
                    break;
                case SiteCode.Doukou3:
                    Price = 4;
                    break;
                case SiteCode.Silk1:
                    Price = 3;
                    break;
                case SiteCode.Silk2:
                    Price = 4;
                    break;
                case SiteCode.Silk3:
                    Price = 5;
                    break;
                case SiteCode.Renshen1:
                    Price = 1;
                    break;
                case SiteCode.Renshen2:
                    Price = 2;
                    break;
                case SiteCode.Renshen3:
                    Price = 3;
                    break;
                case SiteCode.Yushi1:
                    Price = 3;
                    break;
                case SiteCode.Yushi2:
                    Price = 4;
                    break;
                case SiteCode.Yushi3:
                    Price = 5;
                    break;
                case SiteCode.Yushi4:
                    Price = 5;
                    break;
            }
            return Price;
        }
        public int GetPosIncome(int siteCode)
        {
            switch (siteCode)
            {
                case SiteCode.Port1:
                    Income = 6;
                    break;
                case SiteCode.Port2:
                    Income = 8;
                    break;
                case SiteCode.Port3:
                    Income = 15;
                    break;
                case SiteCode.Fix1:
                    Income = 6;
                    break;
                case SiteCode.Fix2:
                    Income = 8;
                    break;
                case SiteCode.Fix3:
                    Income = 15;
                    break;
                case SiteCode.Pirate1:
                    Income = 0;
                    break;
                case SiteCode.Pirate2:
                    Income = 0;
                    break;
                case SiteCode.pilot2:
                    Income = 0;
                    break;
                case SiteCode.pilot5:
                    Income = 0;
                    break;
                case SiteCode.Insurance:
                    Income = 0;
                    break;
                case SiteCode.Doukou1:
                    Income = 24;
                    break;
                case SiteCode.Doukou2:
                    Income = 24;
                    break;
                case SiteCode.Doukou3:
                    Income = 24;
                    break;
                case SiteCode.Silk1:
                    Income = 30;
                    break;
                case SiteCode.Silk2:
                    Income = 30;
                    break;
                case SiteCode.Silk3:
                    Income = 30;
                    break;
                case SiteCode.Renshen1:
                    Income = 18;
                    break;
                case SiteCode.Renshen2:
                    Income = 18;
                    break;
                case SiteCode.Renshen3:
                    Income = 18;
                    break;
                case SiteCode.Yushi1:
                    Income = 36;
                    break;
                case SiteCode.Yushi2:
                    Income = 36;
                    break;
                case SiteCode.Yushi3:
                    Income = 36;
                    break;
                case SiteCode.Yushi4:
                    Income = 36;
                    break;
            }
            return Income;
        }
        
    }
}
