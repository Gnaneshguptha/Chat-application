using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Security.Claims;
using VedioChatApp_Server_.DTO_s;
using VedioChatApp_Server_.Models;

namespace VedioChatApp_Server_.Controllers
{
    public class UserController : Controller
    {
        private readonly chat_AppContext _appContext;


        public UserController(chat_AppContext chat_AppContext)
        {
            _appContext= chat_AppContext;
        }

        [HttpGet("SearchByName")]
        public IActionResult SearchByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name parameter is required.");
            }

            // Perform the search in the database

            var upperName = name.ToUpper();
            var searchResults = _appContext.Users
                .Where(u => EF.Functions.Like(u.Username.ToUpper(), $"%{upperName}%"))
                .ToList();

            return Ok(searchResults);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            var user = await _appContext.Users
                .SingleOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized(); // Invalid credentials
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.SignalRid)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "customauth");

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Ok(user);
        }

        [HttpGet("AllUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _appContext.Users.ToListAsync();
        }

        [HttpGet("GetUser{id:int}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _appContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost("AddUser")]
        public async Task<ActionResult<User>> PostUser([FromBody] RegDto userDto)
        {
            // Assuming User and RegDto have similar properties
            var userEntity = new User
            {
                // Map properties manually
                UserId = userDto.UserId,
                Username=userDto.Username,
                Password=userDto.Password,
                Email=userDto.Email,
                ConnectionId=userDto.ConnectionId,
                StatusFlag=userDto.StatusFlag,
                // ... map other properties
            };

            _appContext.Users.Add(userEntity);
            await _appContext.SaveChangesAsync();

            // Produces status code 201
            return CreatedAtAction(nameof(GetUser), new { id = userEntity.UserId }, userEntity);
        }

        [HttpDelete("Delete{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _appContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _appContext.Users.Remove(user);
            await _appContext.SaveChangesAsync();

            return NoContent();
        }


        [HttpPut("Update/{id:int}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] UserDto userDTO)
        {
            if (id != userDTO.UserId)
            {
                return BadRequest();
            }

            var existingUser = await _appContext.Users.FindAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            // Update properties manually
            existingUser.UserId = userDTO.UserId;
            existingUser.Username = userDTO.Username;
            existingUser.Email = userDTO.Email;
            existingUser.ConnectionId = userDTO.ConnectionId;
            existingUser.StatusFlag = userDTO.StatusFlag;
            // ... (update other properties as needed)

            _appContext.Entry(existingUser).State = EntityState.Modified;
            await _appContext.SaveChangesAsync();

            return NoContent();
        }

    }
}
