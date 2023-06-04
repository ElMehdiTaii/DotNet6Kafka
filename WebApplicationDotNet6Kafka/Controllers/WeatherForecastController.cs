using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace WebApplicationDotNet6Kafka.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IConfiguration _configuration;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IActionResult> Get()
    {
        SendRequest(
        JsonSerializer.Serialize(Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })));
        return Ok();
    }
    private void SendRequest(string message)
    {
        ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = _configuration["Kafka:BrokerUrl"],
            ClientId = Dns.GetHostName()
        };

        using (var producer =
             new ProducerBuilder<Null, string>(config).Build())
        {
            try
            {
                var task = producer.ProduceAsync(_configuration["Kafka:Topics:MyTopic"],
                    new Message<Null, string> { Value = message })
                    ;

                if (!task.Wait(TimeSpan.FromSeconds(10)))
                {
                    producer.Poll(TimeSpan.FromSeconds(30));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Oops, something went wrong: {e}");
            }
        }
    }
}