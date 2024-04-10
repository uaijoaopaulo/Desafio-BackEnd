using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Desa.Core.Repositories.Interfaces;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Desa.Core.Repositories
{
    public class SqsRepository : ISqsRepository
    {
        private static string SecretKey;
        private static string AccessKey;
        private static string ServiceUrl;
        private static string AccountId;
        private static string Region;
        private static string MessageGroupId;
        private AmazonSQSClient amazonSqsClient;

        public SqsRepository()
        {
            var configurationManager = (new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)).Build();

            SecretKey = configurationManager.GetSection("AWS.SQS")["SecretKey"];
            AccessKey = configurationManager.GetSection("AWS.SQS")["AccessKey"];
            ServiceUrl = configurationManager.GetSection("AWS.SQS")["ServiceUrl"];
            AccountId = configurationManager.GetSection("AWS.SQS")["AccountId"];
            Region = configurationManager.GetSection("AWS.SQS")["Region"];
            MessageGroupId = configurationManager.GetSection("AWS.SQS")["MessageGroupId"];

            var awsCreds = new BasicAWSCredentials(AccessKey, SecretKey);
            AmazonSQSConfig config;

            if (!string.IsNullOrEmpty(Region))
            {
                config = new AmazonSQSConfig()
                {
                    ServiceURL = ServiceUrl,
                    RegionEndpoint = RegionEndpoint.GetBySystemName(Region)
                };
            }
            else
                config = new AmazonSQSConfig()
                {
                    ServiceURL = ServiceUrl
                };

            if(!string.IsNullOrEmpty(SecretKey) && !string.IsNullOrEmpty(AccessKey))
                amazonSqsClient = new AmazonSQSClient(awsCreds, config);
        }

        public void CreateQueue(string queueName)
        {
            var createQueueRequest = new CreateQueueRequest();

            try
            {
                amazonSqsClient.GetQueueUrlAsync(queueName);
            }
            catch
            {
                createQueueRequest.QueueName = queueName;
                _ = amazonSqsClient.CreateQueueAsync(createQueueRequest);
            }
        }

        public async Task SendMessageAsync(string queueName, object message, string hashMessage = null)
        {
            try
            {
                var queueUrl = string.Format("{0}/{1}/{2}", ServiceUrl, AccountId, queueName);
                var messageBody = JsonConvert.SerializeObject(message);

                var request = new SendMessageRequest
                {
                    QueueUrl = queueUrl,
                    MessageBody = messageBody
                };

                if (!string.IsNullOrWhiteSpace(MessageGroupId))
                {
                    request.MessageGroupId = MessageGroupId;

                    if (hashMessage != null)
                        request.MessageDeduplicationId = hashMessage;
                }

                var response = await amazonSqsClient.SendMessageAsync(request);

                if (response.HttpStatusCode != HttpStatusCode.OK)
                    throw new AmazonSQSException($"Failed to SendMessageAsync for queue {queueName}. Response: {response.HttpStatusCode}");
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ReadMessageAsync(string queueName)
        {
            var queueUrl = string.Format("{0}/{1}/{2}", ServiceUrl, AccountId, queueName);
            var receiveMessageRequest = new ReceiveMessageRequest(queueUrl);

            while (true)
            {
                receiveMessageRequest.WaitTimeSeconds = 1;

                var receiveMessageResponse = await amazonSqsClient.ReceiveMessageAsync(receiveMessageRequest);

                if (receiveMessageResponse.Messages.Count == 0)
                    continue;

                foreach (var message in receiveMessageResponse.Messages)
                {
                    var deleteReq = new DeleteMessageRequest
                    {
                        QueueUrl = queueUrl,
                        ReceiptHandle = message.ReceiptHandle
                    };

                    _ = await amazonSqsClient.DeleteMessageAsync(deleteReq);
                }
            }
        }

        public async Task<List<Message>> GetMessagesAsync(string queueName, int notificationsCount = 10)
        {
            var cancelToken = new CancellationTokenSource();
            var token = cancelToken.Token;
            var queueUrl = string.Format("{0}/{1}/{2}", ServiceUrl, AccountId, queueName);

            try
            {
                var response = await amazonSqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    WaitTimeSeconds = 10,
                    MaxNumberOfMessages = notificationsCount,
                    AttributeNames = new List<string> { "ApproximateReceiveCount" },
                    MessageAttributeNames = new List<string> { "All" },
                }, token);

                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new AmazonSQSException($"Failed to GetMessagesAsync for queue {queueName}. Response: {response.HttpStatusCode}");
                }

                return response.Messages;
            }
            catch (TaskCanceledException)
            {
                return new List<Message>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteMessageAsync(string queueName, string receiptHandle)
        {
            var queueUrl = string.Format("{0}/{1}/{2}", ServiceUrl, AccountId, queueName);
            var receiveMessageRequest = new ReceiveMessageRequest(queueUrl);

            try
            {
                var deleteMessageRequest = new DeleteMessageRequest()
                {
                    QueueUrl = queueUrl,
                    ReceiptHandle = receiptHandle
                };

                var response = await amazonSqsClient.DeleteMessageAsync(deleteMessageRequest);

                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new AmazonSQSException($"Failed to DeleteMessageAsync for queue {queueName}. Response: {response.HttpStatusCode}");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
