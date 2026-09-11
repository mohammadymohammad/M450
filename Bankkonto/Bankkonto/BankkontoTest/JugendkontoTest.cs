using System;
using System.Collections.Generic;
using System.Text;

namespace BankkontoTest;

public class JugendkontoTest
{
    [Fact]
    void Jugendkonto_Ueberziehung_NichtErlaubt()
    {
        // Arrange
        decimal initialBalance = 1000m;
        var konto = new Bankkonto.Jugendkonto(initialBalance, Bankkonto.KontoStatus.Standard);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => konto.Beziehen(1001m));
    }

    [Fact]
    void Jugendkonto_Ueberziehung_UeberLimit_NichtErlaubt()
    {
        // Arrange
        decimal initialBalance = 1000m;
        decimal withdrawalAmount = 0m;
        var konto = new Bankkonto.Jugendkonto(initialBalance, Bankkonto.KontoStatus.Standard);

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => konto.Beziehen(withdrawalAmount));
    }

    [Fact]
    void Jugendkonto_Bezug_BisZumGuthaben_Erlaubt()
    {
        // Arrange
        decimal initialBalance = 1000m;
        var konto = new Bankkonto.Jugendkonto(initialBalance, Bankkonto.KontoStatus.Standard);

        // Act
        konto.Beziehen(100m);

        // Assert
        Assert.Equal(900m, konto.Guthaben);
    }
}
