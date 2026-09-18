using System;
using System.Collections.Generic;
using System.Text;

namespace BankkontoTest;

[TestClass] 
public class SparkontoTest
{
    [TestMethod]
    public void Sparkonto_Ueberziehung_NichtErlaubt()
    {
        // Arrange
        decimal initialBalance = 1000m;
        decimal withdrawAmount = 1001m;
        var konto = new Bankkonto.Sparkonto(initialBalance, Bankkonto.KontoArt.Standard);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => konto.Beziehen(withdrawAmount));
    }

    [TestMethod]
    public void Sparkonto_Bezug_BisZumGuthaben_Erlaubt()
    {
        // Arrange
        decimal initialBalance = 1000m;
        decimal withdrawAmount = 1000m;
        var konto = new Bankkonto.Sparkonto(initialBalance, Bankkonto.KontoArt.Standard);

        // Act
        konto.Beziehen(withdrawAmount);

        // Assert
        Assert.AreEqual(0m, konto.Guthaben);
    }
}
