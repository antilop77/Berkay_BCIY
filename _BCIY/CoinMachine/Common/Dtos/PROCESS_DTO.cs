using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class PROCESS_DTO
    {
        public int Id { get; set; }

        public string? Status { get; set; }

        public string? Symbol { get; set; }

        public string? OrderSide { get; set; }

        /// <summary>
        /// LIMIT, MARKET VS
        /// </summary>
        public string? OrderType { get; set; }

        public string? IntervalType { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? OpenPrice { get; set; }
        public decimal? StopPrice { get; set; }
        public decimal? ClosePrice { get; set; }
        public decimal? ClosedPrice { get; set; }

        public string? CloseType { get; set; }

        public decimal? OpenedPrice { get; set; }

        public decimal? ExecutedQuantity { get; set; }

        public string? SignalType { get; set; }
        public decimal? CallBackRate { get; set; }

        public string? BinanceOrderId { get; set; }

        public string? BinanceCloseOrderId { get; set; }
        public string? BinanceTrailingOrderId { get; set; }
        public string? BINANCE_TP_ORDER_ID { get; set; }
        public string? BINANCE_SL_ORDER_ID { get; set; }

        public string? BinanceStatus { get; set; }

        public DateTime? InsertDatetime { get; set; }

        public DateTime? UpdateDatetime { get; set; }

        public DateTime? ClosedDatetime { get; set; }
    }
}
