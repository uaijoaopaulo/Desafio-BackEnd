using Amazon.SQS.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Desa.Core.Repositories.Interfaces
{
    public interface ISqsRepository
    {
        void CreateQueue(string queueName);
        Task SendMessageAsync(string queueName, object message, string hashMessage = null);
        Task ReadMessageAsync(string queueName);
        Task<List<Message>> GetMessagesAsync(string queueName, int notificationsCount);
        Task DeleteMessageAsync(string queueName, string receiptHandle);
    }
}
