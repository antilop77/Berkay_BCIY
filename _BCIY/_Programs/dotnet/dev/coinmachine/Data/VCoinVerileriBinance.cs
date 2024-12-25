using System;
using System.Collections.Generic;

namespace Data;

public partial class VCoinVerileriBinance
{
    public int Id { get; set; }

    public string? CoinKodu { get; set; }

    public string? IslemZamanBirimi { get; set; }

    public DateTime? OlusturmaZamani { get; set; }

    public DateTime? GelenVeriZamani { get; set; }

    public decimal? Acilis { get; set; }

    public decimal? Yuksek { get; set; }

    public decimal? Dusuk { get; set; }

    public decimal? Kapanis { get; set; }

    public decimal? Hacim { get; set; }
}
