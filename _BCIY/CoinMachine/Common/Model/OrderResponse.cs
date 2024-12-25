using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class OrderResponse
    {
        public string Symbol { get; set; }
        public string OrderId { get; set; }
        public string ClientOrderId { get; set; }
        public string TransactTime { get; set; }
    }
}
