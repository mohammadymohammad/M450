using Bankkonto;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankkontoTest;

public class KontoTest
{
    [Fact]
    void KontoNummer_WirdAutomatischErstellt()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, KontoStatus.Standard);

        // Act
        var kontoNummer = konto.KontoNummer;

        // Assert
        Assert.False(string.IsNullOrEmpty(kontoNummer));
    }

    [Fact]
    void Einzahlen_100_ErhoetGuthabenUm100()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, KontoStatus.Standard);
        decimal initialBalance = konto.Guthaben;

        // Act
        konto.Einzahlen(100m);

        // Assert
        Assert.Equal(initialBalance + 100m, konto.Guthaben);
    }

    [Fact]
    void Einzahlen_0_WirftArgumentException()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, KontoStatus.Standard);

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => konto.Einzahlen(0m));
    }

    [Fact]
    void Einzahlen_NegativerBetrag_WirftArgumentException()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, KontoStatus.Standard);

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => konto.Einzahlen(-100m));
    }

    [Fact]
    void Kontoabschliessen_SetztGuthabenAufNull()
    {
        // Arrange
        decimal initialBalance = 0m;
        var konto = new Bankkonto.Privatkonto(initialBalance, KontoStatus.Standard);
        // Act
        konto.Kontoabschliessen();

        // Assert
        Assert.Equal(0m, konto.Guthaben);
    }

    [Fact]
    void Kontoabschliessen_WirftInvalidOperationException_WennGuthabenNichtNull()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, KontoStatus.Standard);
        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => konto.Kontoabschliessen());
    }

    [Fact]
    void Kontoabschliessen_WirftInvalidOperationException_WennKontoBereitsGeschlossen()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, KontoStatus.Standard);
        konto.IstGeschlossen = true;
        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => konto.Kontoabschliessen());
    }

    [Fact]
    void Zinsgutschreibung_GuthabenUnter10000_VerwendetAktivZins()
    {
        // Arrange
        decimal initialBalance = 1000m;
        var konto = new Bankkonto.Privatkonto(initialBalance, KontoStatus.Standard);

        konto.Status = KontoStatus.Standard;
        konto.Einzahlen(4000m);

        Bankkonto.Konto.AktivZins = 0.01m;

        // Act
        konto.Zinsgutschreibung(365);

        // Assert
        Assert.Equal(5050m, konto.Guthaben);
    }
}
