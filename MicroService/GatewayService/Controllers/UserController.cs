using Microsoft.AspNetCore.Mvc;
using System.Net;
using UserService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Json;
using System.Text;

namespace GatewayService.Controllers
{

    public class UserServiceContext : DbContext
    {
        public UserServiceContext (DbContextOptions<UserServiceContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; } = default!;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserServiceContext? _context;
        private readonly PasswordHasher<User>? _passwordHasher;
        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

                // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            return await _context!.User
                .Select(u => UserToDTO(u))
                .ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context!.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return UserToDTO(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateModel userUdpate)
        {
            if (id != userUdpate.Id)
            {
                return BadRequest();
            }

            var user = await _context!.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if(userUdpate.Name != null) user.Name = userUdpate.Name;
            if(userUdpate.Email != null) user.Email = userUdpate.Email;

            if(userUdpate.Password != null) {
                user.PasswordHash = _passwordHasher!.HashPassword(user, userUdpate.Password);
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> CreateUser(UserCreateModel userPayload)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string usersApiUrl = "http://localhost:5001/api/Users/register";
                string jsonContent = System.Text.Json.JsonSerializer.Serialize(userPayload);
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage reponse = await httpClient.PostAsync(usersApiUrl, content);

                if (!reponse.IsSuccessStatusCode)
                    throw new Exception($"Failed to register user. Status code: {reponse.StatusCode}");
                else
                {
                    var result = await reponse.Content.ReadFromJsonAsync<UserDTO>();
                    return Ok(result);
                }
            }
        }

        // api/User/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin model)
        {
            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                // Set the base address of the API you want to call
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                // Send a POST request to the login endpoint
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/login", model);

                // Check if the response status code is 200 (OK)
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // You can deserialize the response content here if needed
                    var result = await response.Content.ReadFromJsonAsync<UserDTO>();
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Login failed");
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context!.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context!.User.Any(e => e.Id == id);
        }

        private static UserDTO UserToDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
            };
        }

    }
}
