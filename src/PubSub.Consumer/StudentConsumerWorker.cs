using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PubSub.Consumer
{
    public class StudentConsumerWorker : BackgroundService
    {
        private readonly ILogger<StudentConsumerWorker> _logger;

        public StudentConsumerWorker(ILogger<StudentConsumerWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StudentConsumerWorker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);

                string bootstrapServers = "localhost:9092";
                string nomeTopic = "new_activity";

                var config = new ConsumerConfig
                {
                    BootstrapServers = bootstrapServers,
                    GroupId = $"{nomeTopic}-group-0",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                try
                {
                    using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                    {
                        consumer.Subscribe(nomeTopic);

                        try
                        {
                            while (true)
                            {
                                var cr = consumer.Consume(stoppingToken);
                                _logger.LogInformation($"New Message: {cr.Message.Value}");
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            consumer.Close();
                            _logger.LogError("Canceled the execution of the Consumer...");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception: {ex.GetType().FullName} | " +
                                 $"Message: {ex.Message}");
                }
            }
        }
    }
}