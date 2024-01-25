
using System.Text.Json.Serialization;

namespace UserService.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set;}
        public string? Email { get; set;}
        public string? PasswordHash { get; set; }

        public override string ToString()
        {
            return $"Id: ${Id} Name: ${Name} Email : ${Email} Pass: ${PasswordHash}";
        }
    }

    public class UserDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }

    public class UserCreateModel
    {
        public required string Password { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
    }

    public class UserUpdateModel
    {
        public int Id { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
    public class UserLogin
    {
        public required string Name { get; set; }
        public required string Pass { get; set; }
    }

    public class JWTAndUser
    {
        public required string Token { get; set; }
        public required UserDTO User { get; set; }
    }
}
