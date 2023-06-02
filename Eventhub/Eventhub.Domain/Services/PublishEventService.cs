﻿using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Eventhub.Domain.Interfaces;
using Eventhub.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Eventhub.Domain.Services
{
    public class PublishEventService<T> : IPublishEventService<T> where T : class
    {
        private readonly string SnsTopicArn;
        private readonly string SnsTopicRegion;
        private readonly AmazonSimpleNotificationServiceClient _sqsClient;
        private readonly ILogger<T> _logger;

        public PublishEventService(IConfiguration configuration,
                            ILogger<T> logger)
        {
            SnsTopicArn = configuration["SnsTopicArn"]!;
            SnsTopicRegion = configuration["SnsTopicRegion"]!;
            _sqsClient = new AmazonSimpleNotificationServiceClient(RegionEndpoint.GetBySystemName(SnsTopicRegion));
            _logger = logger;
        }

        public async Task Publish(string user, T message)
        {
            try
            {
                var content = new Message(message.GetType().Name, user, JsonConvert.SerializeObject(message));

                _logger.LogInformation($"Iniciando envio de evento - usuario {user} - data {DateTime.UtcNow} - message {content.Detail}");
                await _sqsClient.PublishAsync(new PublishRequest
                {
                    Message = JsonConvert.SerializeObject(content),
                    TopicArn = SnsTopicArn,
                    MessageAttributes = new Dictionary<string, MessageAttributeValue>
                    {
                        {
                            "Name", new MessageAttributeValue()
                            {
                                StringValue = message.GetType().Name,
                                DataType = "String"
                            }
                        }
                    }
                });

                _logger.LogInformation($"Evento enviado com sucesso - usuario {user} - data {DateTime.UtcNow} - message {content}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao publicar evento - {user} - data {DateTime.UtcNow} - Erro: {ex.Message}");
                throw;
            }
        }
    }
}
