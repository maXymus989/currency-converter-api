using Microsoft.AspNetCore.Mvc;

[Route("api/converter")]
[ApiController]
public class ConverterController : ControllerBase
{
    private readonly ExchangeRateService _exchangeRateService;

    public ConverterController(ExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
    }

    [HttpGet]
    public IActionResult ConvertCurrency([FromQuery] double amount, [FromQuery] string from, [FromQuery] string to)
    {
        var rates = _exchangeRateService.GetRates();

        if (!rates.ContainsKey(to.ToUpper()) || !rates.ContainsKey(from.ToUpper()))
            return BadRequest("Exchange rate not found.");

        double rate = rates[to.ToUpper()] / rates[from.ToUpper()];
        double result = amount * rate;

        Console.WriteLine(rates);

        return Ok(new { from, to, amount, result });

    }

    [HttpGet("rates")]
    public IActionResult GetRates()
    {
        var rates = _exchangeRateService.GetRates();
        return Ok(rates);
    }
}
