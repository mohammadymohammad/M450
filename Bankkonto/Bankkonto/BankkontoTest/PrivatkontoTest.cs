using System;
using System.Collections.Generic;
using System.Text;

namespace BankkontoTest;

public class PrivatkontoTest
{
    [Fact]
    void Privatkonto_Ueberziehung_BisZumLimit_Erlaubt()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, Bankkonto.KontoStatus.Standard);
        konto.Einzahlen(100m);

        // Act
        konto.Beziehen(1100m);

        // Assert
        Assert.Equal(-1000m, konto.Guthaben);
    }

    [Fact]
    void Privatkonto_Ueberziehung_UeberLimit_NichtErlaubt()
    {
        // Arrange
        var konto = new Bankkonto.Privatkonto(1000m, Bankkonto.KontoStatus.Standard);
        konto.Einzahlen(100m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => konto.Beziehen(1101m));
    }
}
