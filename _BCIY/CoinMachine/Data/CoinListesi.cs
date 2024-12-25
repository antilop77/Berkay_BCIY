using System;
using System.Collections.Generic;

namespace Data;

public partial class CoinListesi
{
    public int SiraNo { get; set; }

    public string? CoinKodu { get; set; }

    public string? MinMiktar { get; set; }

    public int? AktifDurumu { get; set; }

    public string? FiyatAdimi { get; set; }

    public DateTime? _5m { get; set; }

    public DateTime? _15m { get; set; }

    public DateTime? _30m { get; set; }

    public DateTime? _1h { get; set; }

    public DateTime? _4h { get; set; }

    public DateTime? _1d { get; set; }

    public DateTime? _1m { get; set; }

    public short SinyalVermeAcKapa { get; set; }

    public short Rsi { get; set; }

    public short Wr { get; set; }

    public short Bb { get; set; }
}
