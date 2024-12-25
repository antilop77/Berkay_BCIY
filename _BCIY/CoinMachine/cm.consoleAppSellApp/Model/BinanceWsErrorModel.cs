using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cm.consoleAppSellApp.Model
{
    public class BinanceWsErrorModel
    {
        public string id { get; set; }
        public int status { get; set; }
        public Error error { get; set; }
        public Ratelimit[] rateLimits { get; set; }
    }

    public class Error
    {
        public int code { get; set; }
        public string msg { get; set; }
    }

    public class Ratelimit
    {
        public string rateLimitType { get; set; }
        public string interval { get; set; }
        public int intervalNum { get; set; }
        public int limit { get; set; }
        public int count { get; set; }
    }
}

