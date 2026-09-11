using System;
using System.Collections.Generic;
using System.Text;

namespace BankkontoTest;

public class PrivatkontoTest
{
    [Fact]
    void Privatkonto_Beziehen_VerringertGuthaben()
    {
        // Arrange
        decimal initialBalance = 1000m;
        var konto = new Bankkonto.Privatkonto(initialBalance, Bankkonto.KontoStatus.Standard);
        // Act
        konto.Beziehen(200m);
        // Assert
        Assert.Equal(800m, konto.Guthaben);
    }

    [Fact]
    void Privatkonto_Ueberziehung_BisZumLimit_Erlaubt()
    {
        // Arrange
        decimal initialBalance = 1000m;
        decimal withdrawalAmount = 1001m;
        var konto = new Bankkonto.Privatkonto(initialBalance, Bankkonto.KontoStatus.Standard);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => konto.Beziehen(withdrawalAmount));
    }

    [Fact]
    void Privatkonto_Ueberziehung_UeberLimit_NichtErlaubt()
    {
        // Arrange
        decimal initialBalance = 1000;
        decimal withdrawalAmount = 0m;
        var konto = new Bankkonto.Privatkonto(initialBalance, Bankkonto.KontoStatus.Standard);

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => konto.Beziehen(withdrawalAmount));
    }
}
