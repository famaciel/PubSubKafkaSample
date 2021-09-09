using System;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace PubSub.Producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Producer Started!");

            string bootstrapServers = "localhost:9092";
            string nomeTopic = "new_activity";

            var command = string.Empty;

            while (!command.ToUpper().Equals("EXIT"))
            {
                Console.WriteLine("Type Message (or exit):");
                command = Console.ReadLine();

                // if (!command.ToUpper().Equals("EXIT"))
                //     Console.WriteLine($"Typed {command}");

                try
                {
                    var config = new ProducerConfig
                    {
                        BootstrapServers = bootstrapServers
                    };

                    using (var producer = new ProducerBuilder<Null, string>(config).Build())
                    {
                        var result = await producer.ProduceAsync(
                                nomeTopic,
                                new Message<Null, string>
                                { Value = command });

                        Console.WriteLine($"Status: { result.Status.ToString()}");
                    }

                    Console.WriteLine("Message Sent");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exceção: {ex.GetType().FullName} | " +
                                 $"Mensagem: {ex.Message}");
                }
            }
        }
    }
}
