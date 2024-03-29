﻿using Front.Entities;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Front.Services
{
    public class LoginService
    {
        private readonly HttpClient _httpClient;

        public LoginService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public UserDTO AuthenticateUser(string username, string password)
        {
            return new UserDTO
            {
                Id = 0,
                Email = "test@test.fr",
                Name = username,
            };
        }

        public Task<HttpResponseMessage> TryLogin(string requestUrl, HttpContent content)
        {
            return this._httpClient.PostAsync(requestUrl, content);
        }

        
    }
}

