using System;
using System.Collections.Generic;

namespace Data;

public partial class IndikatorVerileri
{
    public int Id { get; set; }

    public string? CoinKodu { get; set; }

    public string? IndikatorAdi { get; set; }

    public decimal? IndikatorDegeri { get; set; }

    public string? IslemZamanBirimi { get; set; }

    public DateTime? OlusturmaZamani { get; set; }

    public DateTime? GelenVeriZamani { get; set; }

    public decimal? Sonuc { get; set; }
}
