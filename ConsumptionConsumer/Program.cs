﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Newtonsoft.Json;

namespace EnergyBuyer.ConsumptionConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            // The Kafka endpoint address
            const string kafkaEndpoint = "127.0.0.1:9092";

            // The Kafka topic i'll be using
            const string kafkaTopic = "consumptionTransformed";

            // Create the consumer configuration
            var consumerConfig = new Dictionary<string, object>
            {
                { "group.id", "energyBuyer"},
                { "bootstrap.servers", kafkaEndpoint }
            };

            // Create the consumer
            using (var consumer = new Consumer<Null, string>(consumerConfig, null, new StringDeserializer(Encoding.UTF8)))
            {
                // Subscribe to the OnMessage event
                consumer.OnMessage += (obj, msg) => {
                    Console.WriteLine($"Received: {msg.Value}");
                    var thing = JsonConvert.DeserializeObject<ConsumptionTransformed>(msg.Value);
                };

                // Subscribe to the Kafka topic
                consumer.Subscribe(new List<string> { kafkaTopic });

                // Handle Cancel Keypress
                var cancelled = false;
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; // Prevent the process from terminating
                    cancelled = true;
                };

                Console.WriteLine("Ctrl-C to exit");

                // Poll for messages
                while (!cancelled)
                {
                    consumer.Poll(-1);
                }
            }

        }
    }
}
