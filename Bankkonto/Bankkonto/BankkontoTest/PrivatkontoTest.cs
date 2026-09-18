using System;
using System.Collections.Generic;
using System.Text;

namespace BankkontoTest;

[TestClass]
public class PrivatkontoTest
{
    [TestMethod]
    public void Privatkonto_Beziehen_VerringertGuthaben()
    {
        // Arrange
        decimal initialBalance = 1000m;
        var konto = new Bankkonto.Privatkonto(initialBalance, Bankkonto.KontoArt.Standard);
        // Act
        konto.Beziehen(200m);
        // Assert
        Assert.AreEqual(800m, konto.Guthaben);
    }

    [TestMethod]
    public void Privatkonto_Ueberziehung_BisZumLimit_Erlaubt()
    {
        // Arrange
        decimal initialBalance = 1000m;
        decimal withdrawalAmount = 1001m;
        var konto = new Bankkonto.Privatkonto(initialBalance, Bankkonto.KontoArt.Standard);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => konto.Beziehen(withdrawalAmount));
    }

    [TestMethod]
    public void Privatkonto_Ueberziehung_UeberLimit_NichtErlaubt()
    {
        // Arrange
        decimal initialBalance = 1000m;
        decimal withdrawalAmount = 1001m;
        var konto = new Bankkonto.Privatkonto(initialBalance, Bankkonto.KontoArt.Standard);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => konto.Beziehen(withdrawalAmount));
    }
}
