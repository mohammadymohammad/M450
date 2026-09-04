using System;
using System.Collections.Generic;
using System.Text;

namespace BankkontoTest;

public class BankkontoTest
{
    [Fact]
    void Deposit_0_or_Negative_Amount_Throws_ArgumentException()
    {
        // Arrange
        var konto = new Bankkonto.Bankkonto();
        decimal depositAmount = 0m;
        // Act & Assert
        Assert.Throws<ArgumentException>(() => konto.Einzahlen(depositAmount));
    }

    [Fact]
    void Deposit_100_Amount_IncreasesBalanceBy100()
    {
        // Arrange
        var konto = new Bankkonto.Bankkonto();
        decimal initialBalance = konto.Guthaben;
        decimal depositAmount = 100m;
        // Act
        konto.Einzahlen(depositAmount);
        // Assert
        Assert.Equal(initialBalance + depositAmount, konto.Guthaben);
    }
}
