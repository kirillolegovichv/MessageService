using Microsoft.AspNetCore.SignalR;

namespace MessageService
{
    public class MessageHub : Hub
    {
        private readonly ILogger<MessageHub> _logger;

        public MessageHub(ILogger<MessageHub> logger)
        {
            _logger = logger;
        }

        public async Task SendMessage(string content, int sequenceNumber)
        {
            _logger.LogInformation("Sending message via SignalR: {Content}, SequenceNumber: {SequenceNumber}", content, sequenceNumber);

            var message = new Message
            {
                Content = content,
                Timestamp = DateTime.UtcNow,
                SequenceNumber = sequenceNumber
            };

            try
            {
                await Clients.All.SendAsync("ReceiveMessage", message);
                _logger.LogInformation("Message sent successfully via SignalR: {Content}, SequenceNumber: {SequenceNumber}", content, sequenceNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message via SignalR: {Content}, SequenceNumber: {SequenceNumber}", content, sequenceNumber);
            }
        }
    }
}
