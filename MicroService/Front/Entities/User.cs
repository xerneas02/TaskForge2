﻿namespace Front.Entities
{
    public class UserDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }

    }

    public class UserCreateModel
    {
        public required string Password { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
    }

    public class JWTAndUser
    {
        public required string Token { get; set; }
        public required UserDTO User { get; set; }
    }
}
