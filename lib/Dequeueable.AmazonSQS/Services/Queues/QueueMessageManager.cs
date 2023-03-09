﻿using Amazon.SQS;
using Amazon.SQS.Model;
using Dequeueable.AmazonSQS.Configurations;
using Dequeueable.AmazonSQS.Factories;

namespace Dequeueable.AmazonSQS.Services.Queues
{
    internal sealed class QueueMessageManager : IQueueMessageManager
    {
        private readonly AmazonSQSClient _client;
        private readonly IHostOptions _hostOptions;

        public QueueMessageManager(IAmazonSQSClientFactory amazonSQSClientFactory,
            IHostOptions hostOptions)
        {
            _client = amazonSQSClientFactory.Create();
            _hostOptions = hostOptions;
        }

        public async Task<Models.Message[]> RetreiveMessagesAsync(CancellationToken cancellationToken = default)
        {
            var request = new ReceiveMessageRequest { QueueUrl = _hostOptions.QueueUrl, MaxNumberOfMessages = _hostOptions.BatchSize, VisibilityTimeout = _hostOptions.VisibilityTimeoutInSeconds, AttributeNames = _hostOptions.AttributeNames };
            var res = await _client.ReceiveMessageAsync(request, cancellationToken);

            var nextVisbileOn = NextVisbileOn();
            return res.Messages.Select(m => new Models.Message(m.MessageId, m.ReceiptHandle, nextVisbileOn, BinaryData.FromString(m.Body), m.Attributes)).ToArray();
        }

        public async Task DeleteMessageAsync(Models.Message message, CancellationToken cancellationToken)
        {
            try
            {
                await _client.DeleteMessageAsync(_hostOptions.QueueUrl, message.ReceiptHandle, cancellationToken);
            }
            catch (AmazonSQSException ex) when (ex.ErrorCode.Contains("NonExistentQueue", StringComparison.InvariantCultureIgnoreCase) || ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
            }
        }

        public async Task<DateTimeOffset> UpdateVisibilityTimeOutAsync(Models.Message message, CancellationToken cancellationToken)
        {
            var request = new ChangeMessageVisibilityRequest(_hostOptions.QueueUrl, message.ReceiptHandle, _hostOptions.VisibilityTimeoutInSeconds);
            await _client.ChangeMessageVisibilityAsync(request, cancellationToken);

            return NextVisbileOn();
        }

        public async Task<DateTimeOffset> UpdateVisibilityTimeOutAsync(Models.Message[] messages, CancellationToken cancellationToken)
        {
            var request = new ChangeMessageVisibilityBatchRequest(_hostOptions.QueueUrl, messages.Select(m => new ChangeMessageVisibilityBatchRequestEntry { Id = m.MessageId, ReceiptHandle = m.MessageId, VisibilityTimeout = _hostOptions.VisibilityTimeoutInSeconds }).ToList());
            await _client.ChangeMessageVisibilityBatchAsync(request, cancellationToken);

            return NextVisbileOn();
        }

        public async Task EnqueueMessageAsync(Models.Message message, CancellationToken cancellationToken)
        {
            var request = new ChangeMessageVisibilityRequest(_hostOptions.QueueUrl, message.ReceiptHandle, 0);
            await _client.ChangeMessageVisibilityAsync(request, cancellationToken);
        }

        public async Task EnqueueMessageAsync(Models.Message[] messages, CancellationToken cancellationToken)
        {
            var request = new ChangeMessageVisibilityBatchRequest(_hostOptions.QueueUrl, messages.Select(m => new ChangeMessageVisibilityBatchRequestEntry { Id = m.MessageId, ReceiptHandle = m.MessageId, VisibilityTimeout = 0 }).ToList());
            await _client.ChangeMessageVisibilityBatchAsync(request, cancellationToken);
        }

        private DateTimeOffset NextVisbileOn()
        {
            return DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(_hostOptions.VisibilityTimeoutInSeconds));
        }
    }
}
