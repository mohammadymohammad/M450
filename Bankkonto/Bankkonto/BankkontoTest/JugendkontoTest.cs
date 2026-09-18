using System;
using System.Collections.Generic;
using System.Text;

namespace BankkontoTest;

[TestClass]
public class JugendkontoTest
{
    [TestMethod]
    public void Jugendkonto_Ueberziehung_NichtErlaubt()
    {
        // Arrange
        decimal blance = 1000m;
        decimal withdraw = 1001m;
        var konto = new Bankkonto.Jugendkonto(blance, Bankkonto.KontoArt.Standard);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => konto.Beziehen(withdraw));
    }

    [TestMethod]
    public void Jugendkonto_Ueberziehung_UeberLimit_NichtErlaubt()
    {
        // Arrange
        decimal initialBalance = 1000m;
        decimal withdrawalAmount = 0m;
        var konto = new Bankkonto.Jugendkonto(initialBalance, Bankkonto.KontoArt.Standard);

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => konto.Beziehen(withdrawalAmount));
    }

    [TestMethod]
    public void Jugendkonto_Bezug_BisZumGuthaben_Erlaubt()
    {
        // Arrange
        decimal initialBalance = 1000m;
        var konto = new Bankkonto.Jugendkonto(initialBalance, Bankkonto.KontoArt.Standard);

        // Act
        konto.Beziehen(1000m);

        // Assert
        Assert.AreEqual(0m, konto.Guthaben);
    }
}
