using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cm.consoleAppSellApp.Model
{

    public class balanceModel
    {
        public Class1[] Property1 { get; set; }
    }

    public class Class1
    {
        public string accountAlias { get; set; }
        public string asset { get; set; }
        public string balance { get; set; }
        public string crossWalletBalance { get; set; }
        public string crossUnPnl { get; set; }
        public string availableBalance { get; set; }
        public string maxWithdrawAmount { get; set; }
        public bool marginAvailable { get; set; }
        public long updateTime { get; set; }
    }
}