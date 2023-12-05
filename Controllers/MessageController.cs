using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VedioChatApp_Server_.DTO_s;
using VedioChatApp_Server_.Models;

namespace VedioChatApp_Server_.Controllers
{
    public class MessageController : Controller
    {
        private readonly chat_AppContext _context;

        public MessageController(chat_AppContext chat_AppContext)
        {
            _context = chat_AppContext;
        }

        [HttpGet("AllMessages")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            return await _context.Messages.ToListAsync();
        }

        [HttpGet("GetByMsgId{id:int}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }

        [HttpGet("GetChatMessages/{userId1}/{userId2}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetChatMessages(int userId1, int userId2)
        {
            var messages = await _context.Messages
                .Where(m =>
                    (m.SenderId == userId1 && m.ReceiverId == userId2) )
                .OrderBy(m => m.SentAt)
                .Select(m => new MessageDto
                {
                    MessageId = m.MessageId,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    SenderUsername = GetUserUsername(m.SenderId,_context),
                    ReceiverUsername = GetUserUsername(m.ReceiverId, _context)
                })
                .ToListAsync();

            return Ok(messages);
        }

        [NonAction]
        public static string GetUserUsername(int? userId, chat_AppContext context)
        {
            return userId.HasValue
                ? context.Users.FirstOrDefault(u => u.UserId == userId)?.Username
                : null;
        }



        [HttpPost("PostMessage")]
        public async Task<ActionResult<Message>> PostMessage([FromBody]MessageDto messageDto)
        {
            // Assuming you have logic to map the MessageDto to your Message entity.
            var message = new Message
            {
                SenderId = messageDto.SenderId,
                ReceiverId = messageDto.ReceiverId,
                Content = messageDto.Content,
                SentAt = DateTime.Now 
            };

            var messageResp = new MessageResp
            {
               MessageId= messageDto.MessageId,
               SenderId = messageDto.SenderId,
               ReceiverId = messageDto.ReceiverId,
               Content = messageDto.Content,
               SentAt = DateTime.Now,
               SenderUsername = GetUserUsername(messageDto.SenderId, _context),
               ReceiverUsername = GetUserUsername(messageDto.ReceiverId, _context)
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Return a 201 Created response with the created message
            return CreatedAtAction(nameof(GetMessage), new { id = message.MessageId }, messageResp);
        }


        [HttpDelete("DeleteMsg{id:int}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
