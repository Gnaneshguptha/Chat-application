using System;
using System.Collections.Generic;

namespace VedioChatApp_Server_.Models
{
    public partial class Friendship
    {
        public int FriendshipId { get; set; }
        public int? User1Id { get; set; }
        public int? User2Id { get; set; }
        public int? Status { get; set; }

        public virtual User? User1 { get; set; }
        public virtual User? User2 { get; set; }
    }
}
