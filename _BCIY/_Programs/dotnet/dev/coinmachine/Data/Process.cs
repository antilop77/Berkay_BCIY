using System;
using System.Collections.Generic;

namespace Data;

public partial class Process
{
    public int Id { get; set; }

    /// <summary>
    /// NEW - PENDING (ORDER GERÇEKLEŞMESİ BEKLENİYOR) - OPENED - CLOSED - RENEW - ERROR - CANCELED
    /// </summary>
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

    public decimal? ClosePrice { get; set; }

    public bool? ClosePriceUpdated { get; set; }

    /// <summary>
    /// EĞER VERİ VARSA KAPATMA EMRİ VER BURDAKİ TİPİ KULLAN. MARKET EMRİ GİBİ
    /// </summary>
    public string? CloseType { get; set; }

    public decimal? OpenedPrice { get; set; }

    public decimal? StopPrice { get; set; }

    public bool? StopPriceUpdated { get; set; }

    public decimal? CallBackRate { get; set; }

    public decimal? ExecutedQuantity { get; set; }

    public decimal? ClosedPrice { get; set; }

    /// <summary>
    /// BU ISLEM ICIN CALISMIS OLAN INDIKATOR
    /// </summary>
    public string? SignalType { get; set; }

    public decimal? IndicatorValue { get; set; }

    public string? BinanceOrderId { get; set; }

    /// <summary>
    /// KAPANIŞ EMRİNİN ID Sİ
    /// </summary>
    public string? BinanceCloseOrderId { get; set; }

    public string? BinanceTrailingOrderId { get; set; }

    public string? BinanceTpOrderId { get; set; }

    public string? BinanceSlOrderId { get; set; }

    public string? BinanceStatus { get; set; }

    public DateTime? InsertDatetime { get; set; }

    public DateTime? UpdateDatetime { get; set; }

    public DateTime? ClosedDatetime { get; set; }
}
