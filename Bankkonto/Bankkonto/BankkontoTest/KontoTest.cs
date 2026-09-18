using Bankkonto;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace BankkontoTest;

[TestClass]
public class KontoTest
{
    [TestMethod]
    public void KontoNummer_WirdAutomatischErstellt()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, KontoArt.Standard);

        // Act
        var kontoNummer = konto.KontoNummer;

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(kontoNummer));
    }

    [TestMethod]
    public void ZweiKonten_HabenUnterschiedlicheKontoNummern()
    {
        // Arrange
        var konto1 = new Bankkonto.Privatkonto(1000m, KontoArt.Standard);
        var konto2 = new Bankkonto.Privatkonto(2000m, KontoArt.Standard);
        // Act
        var kontoNummer1 = konto1.KontoNummer;
        var kontoNummer2 = konto2.KontoNummer;
        // Assert
        Assert.AreNotEqual(kontoNummer1, kontoNummer2);
    }

    [TestMethod]
    public void Einzahlen_100_ErhoetGuthabenUm100()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, KontoArt.Standard);
        decimal initialBalance = konto.Guthaben;

        // Act
        konto.Einzahlen(100m);

        // Assert
        Assert.AreEqual(initialBalance + 100m, konto.Guthaben);
    }

    [TestMethod]
    public void Einzahlen_0_WirftArgumentException()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, KontoArt.Standard);

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => konto.Einzahlen(0m));
    }

    [TestMethod]
    public void Einzahlen_NegativerBetrag_WirftArgumentException()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, KontoArt.Standard);

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => konto.Einzahlen(-100m));
    }

    [TestMethod]
    public void SetzeBezugslimite_SpeichertLimiteUndGueltigkeitsdatum()
    {
        var konto = new Bankkonto.Privatkonto(1000m, KontoArt.Standard);
        var gueltigAb = new DateTime(2026, 10, 1);

        konto.setzeBezugslimite(5000m, gueltigAb);

        Assert.AreEqual(5000m, konto.Bezugslimite);
        Assert.AreEqual(gueltigAb, konto.BezugslimiteGueltigAb);
    }

    [TestMethod]
    [DataRow(-1, DisplayName = "Bezugslimite weniger als Standardlimiten")]
    [DataRow(10001, DisplayName = "Bezugslimite über den Standardlimiten")]
    public void SetzeBezugslimite_AusserhalbDerStandardlimiten_WirftException(double bezugslimite)
    {
        var konto = new Bankkonto.Privatkonto(1000m, KontoArt.Standard);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => konto.setzeBezugslimite((decimal)bezugslimite, DateTime.Today));
    }

    [TestMethod]
    public void Kontoabschliessen_SetztGuthabenAufNull()
    {
        // Arrange
        decimal initialBalance = 0m;
        var konto = new Bankkonto.Privatkonto(initialBalance, KontoArt.Standard);
        // Act
        konto.Kontoabschliessen();

        // Assert
        Assert.AreEqual(0m, konto.Guthaben);
    }

    [TestMethod]
    public void Kontoabschliessen_WirftInvalidOperationException_WennGuthabenNichtNull()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, KontoArt.Standard);
        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => konto.Kontoabschliessen());
    }

    [TestMethod]
    public void Kontoabschliessen_WirftInvalidOperationException_WennKontoBereitsGeschlossen()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, KontoArt.Standard);
        konto.IstGeschlossen = true;
        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => konto.Kontoabschliessen());
    }

    [TestMethod]
    [DataRow(KontoArt.Standard, 5050.0, 0.01, 365, 1000.0, 4000.0, DisplayName = "Guthaben unter 10000")]
    [DataRow(KontoArt.VIP, 14172.60, 0.01, 300, 10000.0, 4000.0, DisplayName = "Guthaben zwischen 10000 und 50000")]
    [DataRow(KontoArt.VIP, 55350.00, 0.01, 365, 50000.0, 4000.0, DisplayName = "Guthaben über 50000 für VIP-Kunden")]
    [DataRow(KontoArt.Standard, 54945.00, 0.01, 365, 50000.0, 4000.0, DisplayName = "Guthaben über 50000 für Standard-Kunden")]
    public void Zinsgutschreibung_Guthaben_VerwendetAktivZins(
        KontoArt kontoArt,
        double expected, 
        double zins, 
        int tage, 
        double initialBalance, 
        double einzahlung )
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto((decimal)initialBalance, kontoArt);
        konto.Einzahlen((decimal)einzahlung);

        Bankkonto.Konto.AktivZins = (decimal)zins;

        // Act
        konto.Zinsgutschreibung(tage);

        // Assert
        Assert.AreEqual((decimal)expected, konto.Guthaben);
    }
}
