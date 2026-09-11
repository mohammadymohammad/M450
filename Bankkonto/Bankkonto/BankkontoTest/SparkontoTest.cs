using System;
using System.Collections.Generic;
using System.Text;

namespace BankkontoTest;

public class SparkontoTest
{
    [Fact]
    void Sparkonto_Ueberziehung_NichtErlaubt()
    {
        // Arrange
        decimal initialBalance = 1000m;
        decimal withdrawAmount = 1001m;
        var konto = new Bankkonto.Sparkonto(initialBalance, Bankkonto.KontoStatus.Standard);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => konto.Beziehen(withdrawAmount));
    }

    [Fact]
    void Sparkonto_Bezug_BisZumGuthaben_Erlaubt()
    {
        // Arrange
        decimal initialBalance = 1000m;
        decimal withdrawAmount = 1000m;
        var konto = new Bankkonto.Sparkonto(initialBalance, Bankkonto.KontoStatus.Standard);

        // Act
        konto.Beziehen(withdrawAmount);

        // Assert
        Assert.Equal(0m, konto.Guthaben);
    }
}
