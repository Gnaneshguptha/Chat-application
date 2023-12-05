using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VedioChatApp_Server_.DTO_s;
using VedioChatApp_Server_.Models;

namespace VedioChatApp_Server_.Controllers
{
    public class FriendsController : Controller
    {
        private readonly chat_AppContext _chat_AppContext;
        public FriendsController(chat_AppContext chat_AppContext)
        {
            _chat_AppContext= chat_AppContext;
        }

        [HttpGet("GetAllFriends")]
        public async Task<ActionResult<IEnumerable<Friendship>>> GetFriends()
        {
            return await _chat_AppContext.Friendships.ToListAsync();
        }

        [HttpGet("GetFriendsByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetFriendsByUserId(int userId)
        {
            var friends = _chat_AppContext.Friendships
                .Where(f => (f.User1Id == userId || f.User2Id == userId) && f.Status == 1)
                .Join(
                    _chat_AppContext.Users,
                    f => f.User1Id,
                    u => u.UserId,
                    (f, u) => new UserDto
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        ConnectionId = u.ConnectionId,
                        Email = u.Email,
                        StatusFlag = u.StatusFlag
                    }
                )
                .Union(
                    _chat_AppContext.Friendships
                    .Where(f => (f.User1Id == userId || f.User2Id == userId) && f.Status == 1)
                    .Join(
                        _chat_AppContext.Users,
                        f => f.User2Id,
                        u => u.UserId,
                        (f, u) => new UserDto
                        {
                            UserId = u.UserId,
                            Username = u.Username,
                            ConnectionId = u.ConnectionId,
                            Email = u.Email,
                            StatusFlag = u.StatusFlag
                        }
                    )
                );

            var result = friends.ToList();
            return result;
        }


        [HttpGet("GetFriendShipById/{id:int}")]
        public async Task<ActionResult<Friendship>> GetFriend(int id)
        {
            var friend = await _chat_AppContext.Friendships.FindAsync(id);

            if (friend == null)
            {
                return NotFound();
            }

            return friend;
        }

        [HttpPost("AddFriends")]
        public async Task<ActionResult<Friendship>> PostFriend(FriendsDto friendDto)
        {
            // Map the FriendDto to the Friend entity
            var friend = new Friendship
            {
                User1Id = friendDto.User1Id,
                User2Id = friendDto.User2Id,
                Status =  friendDto.Status
            };//add

            _chat_AppContext.Friendships.Add(friend);
            await _chat_AppContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFriend), new { id = friend.FriendshipId }, friend);
        }

        [HttpPut("UpdateFriend/{id:int}")]
        public async Task<IActionResult> PutFriend(int id, FriendsDto friendDto)
        {
            var existingFriend = await _chat_AppContext.Friendships.FindAsync(id);

            if (existingFriend == null)
            {
                return NotFound();
            }//accept

            // Update only the properties that are allowed to be modified
            existingFriend.Status = friendDto.Status;

            _chat_AppContext.Entry(existingFriend).State = EntityState.Modified;
            await _chat_AppContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> DeleteFriend(int id)
        {
            var friend = await _chat_AppContext.Friendships.FindAsync(id);

            if (friend == null)
            {
                return NotFound();
            }//reject

            _chat_AppContext.Friendships.Remove(friend);
            await _chat_AppContext.SaveChangesAsync();

            return NoContent();
        }

    }
}
