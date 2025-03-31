using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Xunit;

public class ConverterControllerTests
{
    private readonly Mock<ExchangeRateService> _exchangeRateServiceMock;
    private readonly ConverterController _controller;

    public ConverterControllerTests()
    {
        _exchangeRateServiceMock = new Mock<ExchangeRateService>(null, null);
        _controller = new ConverterController(_exchangeRateServiceMock.Object);
    }

    [Fact]
    public void ConvertCurrency_ValidData_ReturnsOkResult()
    {
        // Arrange
        var rates = new Dictionary<string, double>
    {
        { "USD", 1.0 },
        { "EUR", 0.85 }
    };

        _exchangeRateServiceMock.Setup(service => service.GetRates()).Returns(rates);

        double amount = 100;
        string from = "USD";
        string to = "EUR";
        double expectedResult = amount * (rates[to] / rates[from]);

        // Act
        var result = _controller.ConvertCurrency(amount, from, to) as OkObjectResult;

        // Assert
        var responseDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(
        JsonConvert.SerializeObject(result.Value));

        Assert.NotNull(responseDict);
        Assert.Equal(from, responseDict["from"].ToString());
        Assert.Equal(to, responseDict["to"].ToString());
        Assert.Equal(amount, Convert.ToDouble(responseDict["amount"]));
        Assert.Equal(expectedResult, Convert.ToDouble(responseDict["result"]));
    }


    [Fact]
    public void ConvertCurrency_InvalidCurrency_ReturnsBadRequest()
    {
        // Arrange
        var rates = new Dictionary<string, double>
        {
            { "USD", 1.0 },
            { "EUR", 0.85 }
        };

        _exchangeRateServiceMock.Setup(service => service.GetRates()).Returns(rates);

        double amount = 100;
        string from = "USD";
        string to = "GBP"; // Немає в словнику

        // Act
        var result = _controller.ConvertCurrency(amount, from, to) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Exchange rate not found.", result.Value);
    }

    [Fact]
    public void GetRates_ReturnsRatesSuccessfully()
    {
        // Arrange
        var rates = new Dictionary<string, double>
        {
            { "USD", 1.0 },
            { "EUR", 0.85 }
        };

        _exchangeRateServiceMock.Setup(service => service.GetRates()).Returns(rates);

        // Act
        var result = _controller.GetRates() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(rates, result.Value);
    }
}
