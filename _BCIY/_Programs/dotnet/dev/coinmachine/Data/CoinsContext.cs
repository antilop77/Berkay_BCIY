using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class CoinsContext : DbContext
{
    public CoinsContext()
    {
    }

    public CoinsContext(DbContextOptions<CoinsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CoinListesi> CoinListesis { get; set; }

    public virtual DbSet<CoinVerileriBinance> CoinVerileriBinances { get; set; }

    public virtual DbSet<IndikatorVerileri> IndikatorVerileris { get; set; }

    public virtual DbSet<Parametreler> Parametrelers { get; set; }

    public virtual DbSet<Process> Processes { get; set; }

    public virtual DbSet<ProcessLog> ProcessLogs { get; set; }

    public virtual DbSet<SeriLog> SeriLogs { get; set; }

    public virtual DbSet<VCoinVerileriBinance> VCoinVerileriBinances { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        //=> optionsBuilder.UseSqlServer("Server=51.12.247.74;Database=coins;User Id=bciy;Password=bciy!957;MultipleActiveResultSets=true;TrustServerCertificate=True");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoinListesi>(entity =>
        {
            entity.HasKey(e => e.SiraNo);

            entity.ToTable("CoinListesi");

            entity.Property(e => e.Bb).HasColumnName("BB");
            entity.Property(e => e.CoinKodu).HasMaxLength(50);
            entity.Property(e => e.FiyatAdimi).HasMaxLength(10);
            entity.Property(e => e.MinMiktar).HasMaxLength(10);
            entity.Property(e => e.Rsi).HasColumnName("RSI");
            entity.Property(e => e.SinyalVermeAcKapa).HasDefaultValueSql("((1))");
            entity.Property(e => e.Wr).HasColumnName("WR");
            entity.Property(e => e._15m)
                .HasColumnType("smalldatetime")
                .HasColumnName("15m");
            entity.Property(e => e._1d)
                .HasColumnType("smalldatetime")
                .HasColumnName("1d");
            entity.Property(e => e._1h)
                .HasColumnType("smalldatetime")
                .HasColumnName("1h");
            entity.Property(e => e._1m)
                .HasColumnType("smalldatetime")
                .HasColumnName("1m");
            entity.Property(e => e._30m)
                .HasColumnType("smalldatetime")
                .HasColumnName("30m");
            entity.Property(e => e._4h)
                .HasColumnType("smalldatetime")
                .HasColumnName("4h");
            entity.Property(e => e._5m)
                .HasColumnType("smalldatetime")
                .HasColumnName("5m");
        });

        modelBuilder.Entity<CoinVerileriBinance>(entity =>
        {
            entity.ToTable("CoinVerileriBinance");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Acilis).HasColumnType("decimal(18, 10)");
            entity.Property(e => e.CoinKodu)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Dusuk).HasColumnType("decimal(18, 10)");
            entity.Property(e => e.GelenVeriZamani).HasColumnType("datetime");
            entity.Property(e => e.Hacim).HasColumnType("decimal(18, 10)");
            entity.Property(e => e.IslemZamanBirimi)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Kapanis).HasColumnType("decimal(18, 10)");
            entity.Property(e => e.OlusturmaZamani).HasColumnType("datetime");
            entity.Property(e => e.Yuksek).HasColumnType("decimal(18, 10)");
        });

        modelBuilder.Entity<IndikatorVerileri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_IndikatorVerileri2");

            entity.ToTable("IndikatorVerileri");

            entity.HasIndex(e => e.CoinKodu, "NUX_CoinKodu");

            entity.HasIndex(e => new { e.CoinKodu, e.IndikatorAdi, e.IslemZamanBirimi }, "NUX_CoinKodu_IndikatorAdi_IslemZamaniBirimi");

            entity.HasIndex(e => new { e.IndikatorAdi, e.IslemZamanBirimi }, "NUX_IndikatorAdi_IslemZamanBirimi");

            entity.HasIndex(e => e.Sonuc, "NUX_SONUC");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CoinKodu).HasMaxLength(10);
            entity.Property(e => e.GelenVeriZamani).HasColumnType("smalldatetime");
            entity.Property(e => e.IndikatorAdi).HasMaxLength(100);
            entity.Property(e => e.IndikatorDegeri).HasColumnType("decimal(18, 10)");
            entity.Property(e => e.IslemZamanBirimi).HasMaxLength(10);
            entity.Property(e => e.OlusturmaZamani).HasColumnType("smalldatetime");
            entity.Property(e => e.Sonuc).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Parametreler>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Parametreler");

            entity.Property(e => e.Aciklama)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Deger)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Kod)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Process>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PROCESS__3214EC27707FBF41");

            entity.ToTable("PROCESS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BinanceCloseOrderId)
                .HasMaxLength(50)
                .HasComment("KAPANIŞ EMRİNİN ID Sİ")
                .HasColumnName("BINANCE_CLOSE_ORDER_ID");
            entity.Property(e => e.BinanceOrderId)
                .HasMaxLength(50)
                .HasColumnName("BINANCE_ORDER_ID");
            entity.Property(e => e.BinanceSlOrderId)
                .HasMaxLength(50)
                .HasColumnName("BINANCE_SL_ORDER_ID");
            entity.Property(e => e.BinanceStatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BINANCE_STATUS");
            entity.Property(e => e.BinanceTpOrderId)
                .HasMaxLength(50)
                .HasColumnName("BINANCE_TP_ORDER_ID");
            entity.Property(e => e.BinanceTrailingOrderId)
                .HasMaxLength(50)
                .HasColumnName("BINANCE_TRAILING_ORDER_ID");
            entity.Property(e => e.CallBackRate)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("CALL_BACK_RATE");
            entity.Property(e => e.ClosePrice)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("CLOSE_PRICE");
            entity.Property(e => e.ClosePriceUpdated)
                .HasDefaultValueSql("((0))")
                .HasColumnName("CLOSE_PRICE_UPDATED");
            entity.Property(e => e.CloseType)
                .HasMaxLength(50)
                .HasComment("EĞER VERİ VARSA KAPATMA EMRİ VER BURDAKİ TİPİ KULLAN. MARKET EMRİ GİBİ")
                .HasColumnName("CLOSE_TYPE");
            entity.Property(e => e.ClosedDatetime)
                .HasColumnType("datetime")
                .HasColumnName("CLOSED_DATETIME");
            entity.Property(e => e.ClosedPrice)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("CLOSED_PRICE");
            entity.Property(e => e.ExecutedQuantity)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("EXECUTED_QUANTITY");
            entity.Property(e => e.IndicatorValue)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("INDICATOR_VALUE");
            entity.Property(e => e.InsertDatetime)
                .HasColumnType("datetime")
                .HasColumnName("INSERT_DATETIME");
            entity.Property(e => e.IntervalType)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("INTERVAL_TYPE");
            entity.Property(e => e.OpenPrice)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("OPEN_PRICE");
            entity.Property(e => e.OpenedPrice)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("OPENED_PRICE");
            entity.Property(e => e.OrderSide)
                .HasMaxLength(4)
                .HasColumnName("ORDER_SIDE");
            entity.Property(e => e.OrderType)
                .HasMaxLength(50)
                .HasComment("LIMIT, MARKET VS")
                .HasColumnName("ORDER_TYPE");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("QUANTITY");
            entity.Property(e => e.SignalType)
                .HasMaxLength(50)
                .HasComment("BU ISLEM ICIN CALISMIS OLAN INDIKATOR")
                .HasColumnName("SIGNAL_TYPE");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasComment("NEW - PENDING (ORDER GERÇEKLEŞMESİ BEKLENİYOR) - OPENED - CLOSED - RENEW - ERROR - CANCELED")
                .HasColumnName("STATUS");
            entity.Property(e => e.StopPrice)
                .HasColumnType("decimal(18, 8)")
                .HasColumnName("STOP_PRICE");
            entity.Property(e => e.StopPriceUpdated)
                .HasDefaultValueSql("((0))")
                .HasColumnName("STOP_PRICE_UPDATED");
            entity.Property(e => e.Symbol)
                .HasMaxLength(50)
                .HasColumnName("SYMBOL");
            entity.Property(e => e.UpdateDatetime)
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATETIME");
        });

        modelBuilder.Entity<ProcessLog>(entity =>
        {
            entity.ToTable("ProcessLog");

            entity.Property(e => e.InsertDateTime).HasColumnType("datetime");
            entity.Property(e => e.LogType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReqBody).IsUnicode(false);
            entity.Property(e => e.ReqPath).IsUnicode(false);
            entity.Property(e => e.ResBody).IsUnicode(false);
        });

        modelBuilder.Entity<SeriLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SeriLog");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Level).HasMaxLength(128);
        });

        modelBuilder.Entity<VCoinVerileriBinance>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vCoinVerileriBinance");

            entity.Property(e => e.Acilis).HasColumnType("decimal(18, 10)");
            entity.Property(e => e.CoinKodu)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Dusuk).HasColumnType("decimal(18, 10)");
            entity.Property(e => e.GelenVeriZamani).HasColumnType("datetime");
            entity.Property(e => e.Hacim).HasColumnType("decimal(18, 10)");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.IslemZamanBirimi)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Kapanis).HasColumnType("decimal(18, 10)");
            entity.Property(e => e.OlusturmaZamani).HasColumnType("datetime");
            entity.Property(e => e.Yuksek).HasColumnType("decimal(18, 10)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
