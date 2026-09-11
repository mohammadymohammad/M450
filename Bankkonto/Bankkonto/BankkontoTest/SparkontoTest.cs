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
        var konto = new Bankkonto.Sparkonto();
        konto.Einzahlen(100m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => konto.Beziehen(101m));
    }

    [Fact]
    void Sparkonto_Bezug_BisZumGuthaben_Erlaubt()
    {
        // Arrange
        var konto = new Bankkonto.Sparkonto();
        konto.Einzahlen(100m);

        // Act
        konto.Beziehen(100m);

        // Assert
        Assert.Equal(0m, konto.Guthaben);
    }
}
