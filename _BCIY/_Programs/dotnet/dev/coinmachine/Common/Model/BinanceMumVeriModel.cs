using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class BinanceMumVeriModel
    {
        public DateTime openTime { get; set; }
        public DateTime closeDateTime { get; set; }
        public string mumTipi { get; set; }
        public decimal? open { get; set; }
        public decimal? high { get; set; }
        public decimal? low { get; set; }
        public decimal? close { get; set; }
        public decimal? volume { get; set; }
    }
}