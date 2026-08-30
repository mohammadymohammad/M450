using System;
using System.Collections.Generic;
using System.Text;

namespace M450.UnitTesting.App.Tests;

/// <summary>
/// Test to check the calculation of the total price of an order after an optional percentage discount.
/// </summary>
public class PriceCalculatorTests
{
    [Fact]
    void CalculateTotal_WithDiscount_ReturnsExpectedTotal()
    {
        // Arrange
        PriceCalculator calculator = new PriceCalculator();
        decimal unitPrice = 10.0m;
        int quantity = 2;
        decimal discount = 25m;
        decimal expected = 15.0m;

        // Act
        decimal total = calculator.CalculateTotal(unitPrice, quantity, discount);

        // Assert
        Assert.Equal(expected, total);
    }

    [Fact]
    void CalculateTotal_WithoutDiscount_ReturnExpectedTotal()
    {
        //Arrange
        PriceCalculator calculator = new PriceCalculator();
        decimal unitPrice = 5.0m;
        int quantity = 3;
        decimal expected = 15.0m;

        //Act
        decimal total = calculator.CalculateTotal(unitPrice, quantity);

        //Assert
        Assert.Equal(expected, total);
    }

    [Fact]
    void CalculateTotal_WithZeroQuantity_ReturnZeroAsTotal()
    {
        //Arrange
        PriceCalculator calculator = new PriceCalculator();
        decimal unitPrice = 5.0m;
        int quantity = 0;
        decimal discount = 25m;
        decimal expected = 0m;

        //Act
        decimal total = calculator.CalculateTotal(unitPrice, quantity, discount);

        //Assert
        Assert.Equal(expected, total);
    }

    [Fact]
    void CalculateTotal_With100Discount_ReturnZeroAsTotal()
    {
        //Arrange
        PriceCalculator calculator = new PriceCalculator();
        decimal unitPrice = 15.0m;
        int quantity = 3;
        decimal discount = 100m;
        decimal expected = 0m;

        //Act
        decimal total = calculator.CalculateTotal(unitPrice, quantity, discount);

        //Assert
        Assert.Equal(expected, total);
    }

    [Fact]
    void CalculateTotal_WithNigativeUnitPrice_ThrowsArgumentOutOfRangeException()
    {
        //Arrange
        PriceCalculator calculator = new PriceCalculator();
        decimal unitPrice = -15.0m;
        int quantity = 3;
        decimal discount = 100m;

        //Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.CalculateTotal(unitPrice, quantity, discount) );
    }

    [Fact]
    void CalculateTotal_WithNegativeQuantity_ThrowsArgumentOutOfRangeException()
    {
        //Arrange
        PriceCalculator calculator = new PriceCalculator();
        decimal unitPrice = 15.0m;
        int quantity = -3;
        decimal discount = 100m;

        //Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.CalculateTotal(unitPrice, quantity, discount));
    }

    [Fact]
    void CalculateTotal_WithInvalidDiscount_ThrowsArgumentOutOfRangeException()
    {
        //Arrange
        PriceCalculator calculator = new PriceCalculator();
        decimal unitPrice = 15.0m;
        int quantity = 3;
        decimal discount = 101m; // Disount can't be greater than 100m.

        //Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => calculator.CalculateTotal(unitPrice, quantity, discount));
    }
}
