using Npgsql;

namespace MessageService
{
    public class MessageContext
    {
        private readonly string _connectionString;
        private readonly ILogger<MessageContext> _logger;

        public MessageContext(string connectionString, ILogger<MessageContext> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task SaveMessageAsync(Message message)
        {
            _logger.LogInformation("Saving message to database: {Content}, SequenceNumber: {SequenceNumber}", message.Content, message.SequenceNumber);

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using (var cmd = new NpgsqlCommand("INSERT INTO Messages (Content, Timestamp, SequenceNumber) VALUES (@content, @timestamp, @sequenceNumber)", conn))
            {
                cmd.Parameters.AddWithValue("content", message.Content);
                cmd.Parameters.AddWithValue("timestamp", message.Timestamp);
                cmd.Parameters.AddWithValue("sequenceNumber", message.SequenceNumber);
                await cmd.ExecuteNonQueryAsync();
            }

            _logger.LogInformation("Message saved to database: {Content}, SequenceNumber: {SequenceNumber}", message.Content, message.SequenceNumber);
        }

        public async Task<List<Message>> GetMessagesAsync(DateTime start, DateTime end)
        {
            _logger.LogInformation("Fetching messages from database between {Start} and {End}", start, end);

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand("SELECT * FROM Messages WHERE Timestamp BETWEEN @start AND @end", conn);
            cmd.Parameters.AddWithValue("start", start);
            cmd.Parameters.AddWithValue("end", end);
            await using var reader = await cmd.ExecuteReaderAsync();
            var messages = new List<Message>();
            while (await reader.ReadAsync())
            {
                messages.Add(new Message
                {
                    Id = reader.GetInt32(0),
                    Content = reader.GetString(1),
                    Timestamp = reader.GetDateTime(2),
                    SequenceNumber = reader.GetInt32(3)
                });
            }

            _logger.LogInformation("Retrieved {Count} messages from database", messages.Count);
            return messages;
        }
    }
}
