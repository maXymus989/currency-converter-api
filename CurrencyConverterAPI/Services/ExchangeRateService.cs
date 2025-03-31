using System.Text.Json;

public class ExchangeRateService : BackgroundService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExchangeRateService> _logger;
    private readonly Dictionary<string, double> _exchangeRates = new();
    private const string API_URL = "https://api.exchangerate-api.com/v4/latest/USD";

    public ExchangeRateService(HttpClient httpClient, ILogger<ExchangeRateService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public virtual Dictionary<string, double> GetRates()
    {
        _logger.LogInformation($"Видача курсів валют: {JsonSerializer.Serialize(_exchangeRates)}");
        return _exchangeRates;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdateExchangeRates();
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task UpdateExchangeRates()
    {

        try
        {
            var response = await _httpClient.GetStringAsync(API_URL);

            var data = JsonSerializer.Deserialize<ExchangeRateResponse>(response);

            if (data != null && data.rates.Count > 0)
            {
                lock (_exchangeRates)
                {
                    _exchangeRates.Clear();
                    foreach (var rate in data.rates)
                    {
                        _exchangeRates[rate.Key] = rate.Value;
                    }
                }
            }
            else
            {
                _logger.LogWarning("Recieved empty list of currency rates.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
        }
    }


    private class ExchangeRateResponse
    {
        public Dictionary<string, double> rates { get; set; }
    }
}
