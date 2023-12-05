namespace VedioChatApp_Server_.DTO_s
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public string? Content { get; set; }
        public DateTime? SentAt { get; set; }
        public string? SenderUsername { get; set; }
        public string? ReceiverUsername { get; set; }
    }
}
