using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using UserService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;

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

        private string GenerateJwtToken(int userId)
        {
            var claims = new List<Claim>
            {
                // On ajoute un champ UserId dans notre token avec comme valeur userId en string
                new Claim("UserId", userId.ToString()) 
            };

            // On créer la clé de chiffrement
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThomasIsReallySus12345678901234567890"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // On paramètre notre token
            var token = new JwtSecurityToken(
                issuer: "PokemonManager", // Qui a émit le token
                audience: "localhost:5000", // A qui est destiné ce token
                claims: claims, // Les données que l'on veux encoder dans le token
                expires: DateTime.Now.AddDays(7), // Durée de validité
                signingCredentials: creds); // La clé de chiffrement

            // On renvoie le token signé
            return new JwtSecurityTokenHandler().WriteToken(token);
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
                Console.WriteLine("Gateway login");
                // Set the base address of the API you want to call
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                // Send a POST request to the login endpoint
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/login", model);

                // Check if the response status code is 200 (OK)
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // You can deserialize the response content here if needed
                    UserDTO? result = await response.Content.ReadFromJsonAsync<UserDTO>();
                    
                    if(result == null)
                    {
                        return BadRequest("Login failed");
                    }

                    string jwt = GenerateJwtToken(result.Id);
                    var userAndToken = new JWTAndUser() { Token = jwt, User = result };
                    return Ok(userAndToken);
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

        [Authorize]
        [HttpPost("AddRandomPokemon/{trainerId}")]
        public async Task<IActionResult> AddRandomPokemon(int trainerId)
        {
            // Console.WriteLine("GateWay Pokemon" + trainerId);
            using (HttpClient httpClient = new HttpClient())
            {
                string usersApiUrl = $"http://localhost:5228/api/Pokemon/AddRandomPokemon/{trainerId}";

                HttpResponseMessage reponse = await httpClient.PostAsync(usersApiUrl, null);

                if (reponse.IsSuccessStatusCode)
                    
                    return Ok();
                else
                {
                    throw new Exception($"Echec de la requete a la gateway. Status code: {reponse.StatusCode}");
                }
            }
        }

        [Authorize]
        [HttpDelete("pokemon/{pokemonId}")]
        public async Task<IActionResult> ReleasePokemon(int pokemonId)
        {
            Console.WriteLine("GateWay Pokemon" );
            using (HttpClient httpClient = new HttpClient())
            {
                string usersApiUrl = $"http://localhost:5228/api/Pokemon/{pokemonId}";

                HttpResponseMessage reponse = await httpClient.DeleteAsync(usersApiUrl);

                if (reponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Pokemon {pokemonId} supprimé");
                    return Ok();
                }
                else
                {
                    throw new Exception($"Echec de la requete a la gateway. Status code: {reponse.StatusCode}");
                }
            }
        }

        [Authorize]
        [HttpGet("trainer/{trainerId}")]
        public async Task<ActionResult<IEnumerable<Pokemon>>> GetPokemonsOfTrainer(int trainerId)
        {
            // Console.WriteLine("GateWay Pokemon" + trainerId);
            using (HttpClient httpClient = new HttpClient())
            {
                string usersApiUrl = $"http://localhost:5228/api/Pokemon/trainer/{trainerId}";

                HttpResponseMessage reponse = await httpClient.GetAsync(usersApiUrl);

                if (reponse.IsSuccessStatusCode)
                {
                    var result = await reponse.Content.ReadFromJsonAsync<IEnumerable<Pokemon>>();
                    return Ok(result);
                }
                else
                {
                    throw new Exception($"Echec de la requete a la gateway. Status code: {reponse.StatusCode}");
                }
            }
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
