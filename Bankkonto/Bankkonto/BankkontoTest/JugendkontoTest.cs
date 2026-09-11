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
        var konto = new Bankkonto.Jugendkonto();
        konto.Einzahlen(100m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => konto.Beziehen(101m));
    }

    [Fact]
    void Jugendkonto_Bezug_BisZumGuthaben_Erlaubt()
    {
        // Arrange
        var konto = new Bankkonto.Jugendkonto();
        konto.Einzahlen(100m);

        // Act
        konto.Beziehen(100m);

        // Assert
        Assert.Equal(0m, konto.Guthaben);
    }
}
