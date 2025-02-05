using Microsoft.AspNetCore.Mvc;

namespace MessageService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly MessageContext _context;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(MessageContext context, ILogger<MessagesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] Message message)
        {
            _logger.LogInformation("Received a new message: {Content}, SequenceNumber: {SequenceNumber}", message.Content, message.SequenceNumber);

            try
            {
                await _context.SaveMessageAsync(message);
                _logger.LogInformation("Message saved successfully: {Content}, SequenceNumber: {SequenceNumber}", message.Content, message.SequenceNumber);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving message: {Content}, SequenceNumber: {SequenceNumber}", message.Content, message.SequenceNumber);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            _logger.LogInformation("Fetching messages from {Start} to {End}", start, end);

            try
            {
                var messages = await _context.GetMessagesAsync(start, end);
                _logger.LogInformation("Retrieved {Count} messages", messages.Count);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching messages from {Start} to {End}", start, end);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
