using System;
using System.Collections.Generic;

namespace VedioChatApp_Server_.Models
{
    public partial class User
    {
        public User()
        {
            FriendshipUser1s = new HashSet<Friendship>();
            FriendshipUser2s = new HashSet<Friendship>();
            MessageReceivers = new HashSet<Message>();
            MessageSenders = new HashSet<Message>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? ConnectionId { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public int? StatusFlag { get; set; }
        public string? SignalRid { get; set; }

        public virtual ICollection<Friendship> FriendshipUser1s { get; set; }
        public virtual ICollection<Friendship> FriendshipUser2s { get; set; }
        public virtual ICollection<Message> MessageReceivers { get; set; }
        public virtual ICollection<Message> MessageSenders { get; set; }
    }
}
