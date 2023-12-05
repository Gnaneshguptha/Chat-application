namespace VedioChatApp_Server_.DTO_s
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? ConnectionId { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public int? StatusFlag { get; set; }
        public string? signlaRid { get; set; }
    }
}
